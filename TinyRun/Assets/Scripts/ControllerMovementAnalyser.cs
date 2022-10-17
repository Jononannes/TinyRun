using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerMovementAnalyser : MonoBehaviour {

    public ControllerTracker rightController;
    public ControllerTracker leftController;
    public Transform player;
    public Path path;
    public InputActionReference rightGripPressed;
    public InputActionReference leftGripPressed;


    // configurable settings
    [Range(0f, 5f)]
    public float minimumArmMovementForRun = 0.5f;
    [Range(0f, 5f)]
    public float armToEnvironmentMovementScaling = 0.25f;
    public AnimationCurve jumpCurve;
    public float jumpTime = 1f;
    public float minimumArmMovementForJump = 1f;

    private AudioSource audioSource;
    private bool lastRightArmSwingForward = false;
    public bool isJumping = false;
    private float timeSinceJumpStart = 0f;
    public bool canJump = false;

    private bool rightGripDown;
    private bool leftGripDown;
    private bool rightIsGripping;



    private void Awake() {
        rightGripPressed.action.performed += HandleRightGripPressed;
        rightGripPressed.action.canceled += HandleRightGripReleased;
        leftGripPressed.action.performed += HandleLeftGripPressed;
        leftGripPressed.action.canceled += HandleLeftGripReleased;

    }

    private void OnDestroy() {
        rightGripPressed.action.performed -= HandleRightGripPressed;
        rightGripPressed.action.canceled -= HandleRightGripReleased;
        leftGripPressed.action.performed -= HandleLeftGripPressed;
        leftGripPressed.action.canceled -= HandleLeftGripReleased;
    }

    // Start is called before the first frame update
    private void Start() {
        audioSource = GetComponent<AudioSource>();

        // canJump starts as false in order to behave nicely on the first frame,
        // but is set to true the next frame
        StartCoroutine(SetCanJumpNextFrame(true));
    }
    

    // Update is called once per frame
    void Update() {
        switch (path.GetCurrentSegment().type) {
            case PathSegment.Type.Running:
                HandleRunningSegment();
                break;
            case PathSegment.Type.Climbing:
                HandleClimbingSegment();
                break;
        }
    }


    private void HandleRightGripPressed(InputAction.CallbackContext context) {
        rightGripDown = true;
        rightIsGripping = true;
    }

    private void HandleRightGripReleased(InputAction.CallbackContext context) {
        rightGripDown = false;
        if (leftGripDown && rightIsGripping) {
            rightIsGripping = false;
        }
    }

    private void HandleLeftGripPressed(InputAction.CallbackContext context) {
        leftGripDown = true;
        rightIsGripping = false;
    }

    private void HandleLeftGripReleased(InputAction.CallbackContext context) {
        leftGripDown = false;
        if (rightIsGripping && !rightIsGripping) {
            rightIsGripping = true;
        }
    }



    private void HandleRunningSegment() {
        // Jumping behaviour
        if (!isJumping && canJump &&
            (rightController.GetVelocity().y > minimumArmMovementForJump && leftController.GetVelocity().y > minimumArmMovementForJump)) {
            StartCoroutine(JumpCoroutine());
        }

        // Running behaviour
        if (!isJumping &&
            (rightController.GetVelocity().z > minimumArmMovementForRun && leftController.GetVelocity().z < -minimumArmMovementForRun ||
            rightController.GetVelocity().z < -minimumArmMovementForRun && leftController.GetVelocity().z > minimumArmMovementForRun)) {

            // Calculate how much the player moved their arms
            float totalZMovement = Mathf.Abs(rightController.GetVelocity().z) + Mathf.Abs(leftController.GetVelocity().z);

            if (totalZMovement > minimumArmMovementForRun) {
                // Calculate how much the player should move in-game
                float environmentMovement = (totalZMovement - minimumArmMovementForRun) * armToEnvironmentMovementScaling;

                // Move the player
                player.position += new Vector3(0f, 0f, environmentMovement * Time.deltaTime);

                bool rightArmSwingForward = rightController.GetVelocity().z > 0;

                // Play footstep sound if we just changed arm directions (correlates to steps)
                if ((rightArmSwingForward && !lastRightArmSwingForward) ||
                    (!rightArmSwingForward && lastRightArmSwingForward)) {
                    audioSource.pitch = Random.Range(0.8f, 1.25f);
                    audioSource.Play();
                }

                lastRightArmSwingForward = rightArmSwingForward;
            }
        }
    }



    // Performs a jump
    public IEnumerator JumpCoroutine() {
        timeSinceJumpStart = 0f;
        isJumping = true;
        float jumpLength = jumpCurve.keys[jumpCurve.keys.Length - 1].time;

        while (timeSinceJumpStart < jumpTime) {
            // If we jumped into a segment that is not a running segment,
            // we don't want to continue the jump becuase that would lead
            // to going through the environment
            if (path.GetCurrentSegment().type != PathSegment.Type.Running) {
                isJumping = false;
                yield break;
            }

            // Calculate how much the player is moving upwards (or downwards) in the jump on this frame
            float offsetY = jumpCurve.Evaluate(timeSinceJumpStart * jumpLength / jumpTime) -
                            jumpCurve.Evaluate((timeSinceJumpStart - Time.deltaTime) * jumpLength / jumpTime);

            // Calculate how much the player is moving forwards in the jump on this frame
            float offsetZ = Time.deltaTime * jumpLength / jumpTime;

            // Move the player
            player.position += new Vector3(0f, offsetY, offsetZ);

            timeSinceJumpStart += Time.deltaTime;

            // Waits until next frame to continue the loop
            yield return null;
        }

        
        timeSinceJumpStart = jumpTime;
        player.position += new Vector3(0f, path.GetCurrentSegment().startPosition.position.y - player.position.y, 0f);

        isJumping = false;
    }

    // Resets the player's position to (0,0,0)
    public void ResetPlayerPosition() {
        // When the player is teleported, the controllers "move" in-game,
        // which sometimes makes the player jump even when they weren't moving
        // their arms. Setting the canJump variable to false this frame and resetting
        // it next frame prevents that
        bool couldJump = canJump;
        canJump = false;
        StartCoroutine(SetCanJumpNextFrame(couldJump));
        player.position = path.segments[0].startPosition.position;
        path.Reset();
    }

    // Sets the canJump variable on the next frame
    public IEnumerator SetCanJumpNextFrame(bool newCanJump) {
        yield return null;
        canJump = newCanJump;
    }



    private void HandleClimbingSegment() {
        if (rightGripDown || leftGripDown) {
            ControllerTracker tracker = null;

            if (rightIsGripping && rightGripDown) {
                tracker = rightController;
            } else if (!rightIsGripping && leftGripDown) {
                tracker = leftController;
            }

            float vel = tracker.GetVelocity().y;
            if (vel < 0) {
                player.position += new Vector3(0f, Mathf.Abs(vel), 0f) * Time.deltaTime * 2f;
            }
        }
    }
}
