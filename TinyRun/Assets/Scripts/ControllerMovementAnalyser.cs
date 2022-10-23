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
    public float collisionStunTime = 0.5f;
    public float collisionShakeIntensity = 0.1f;
    public float segmentTransitionDistance = 0.5f;
    public float transitionTime = 2f;

    private AudioSource audioSource;
    private bool lastRightArmSwingForward = false;
    [HideInInspector] public bool isJumping = false;
    private float timeSinceJumpStart = 0f;
    [HideInInspector] public bool canJump = false;
    public CameraShake cameraShake;

    private bool rightGripDown;
    private bool leftGripDown;
    private bool rightIsGripping;
    private bool isInCollisionStun = false;



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
    }
    

    // Update is called once per frame
    void Update() {
        if (!isInCollisionStun) {
            // easier transition between climbing and running segments
            if (path.GetCurrentSegment().type == PathSegment.Type.Running
                && path.GetNextSegment().type == PathSegment.Type.Climbing
                && Mathf.Abs(path.GetNextSegment().startPosition.position.z - player.position.z) < segmentTransitionDistance
                && PlayerIsDoingClimbingMotion()) {

                path.MoveToNextSegment();
                StartCoroutine(SmoothSegmentTransition());

            } else if (path.GetCurrentSegment().type == PathSegment.Type.Climbing
                && path.GetNextSegment().type == PathSegment.Type.Running
                && Mathf.Abs(path.GetNextSegment().startPosition.position.y - player.position.y) < segmentTransitionDistance
                && PlayerIsDoingRunMotion()) {

                path.MoveToNextSegment();
                StartCoroutine(SmoothSegmentTransition());
            }

            switch (path.GetCurrentSegment().type) {
                case PathSegment.Type.Running:
                    HandleRunningSegment();
                    break;
                case PathSegment.Type.Climbing:
                    HandleClimbingSegment();
                    break;
            }
        }
    }


    private IEnumerator SmoothSegmentTransition() {
        PathSegment.Type type = path.GetCurrentSegment().type;
        float diff;
        if (type == PathSegment.Type.Running) {
            diff = path.GetCurrentSegment().startPosition.position.y - player.position.y;
        } else if (type == PathSegment.Type.Climbing) {
            diff = path.GetCurrentSegment().startPosition.position.z - player.position.z;
        } else {
            yield break;
        }

        canJump = false;

        float time = 0f;
        while (time < transitionTime) {
            
            
            if (type == PathSegment.Type.Running) {
                player.position += new Vector3(0f, diff / transitionTime * Time.deltaTime, 0f);
            } else if (type == PathSegment.Type.Climbing) {
                player.position += new Vector3(0f, 0f, diff / transitionTime * Time.deltaTime);
            }

            time += Time.deltaTime;
            yield return null;
        }

        canJump = true;
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
        if (PlayerIsDoingJumpMotion()) {
            StartCoroutine(JumpCoroutine());
        }

        // Running behaviour
        if (PlayerIsDoingRunMotion()) {

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


    private bool PlayerIsDoingJumpMotion() {
        return !isJumping
            && canJump
            && (rightController.GetVelocity().y > minimumArmMovementForJump
                && leftController.GetVelocity().y > minimumArmMovementForJump);
    }

    private bool PlayerIsDoingRunMotion() {
        return !isJumping
            && (rightController.GetVelocity().z > minimumArmMovementForRun && leftController.GetVelocity().z < -minimumArmMovementForRun
                || rightController.GetVelocity().z < -minimumArmMovementForRun && leftController.GetVelocity().z > minimumArmMovementForRun);
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
        SetCanJumpNextFrame(couldJump);
        player.position = path.segments[0].startPosition.position;
        path.Reset();
    }

    public void SetCanJumpNextFrame(bool newCanJump) {
        StartCoroutine(CanJumpNextFrameRoutine(newCanJump));
    }

    // Sets the canJump variable on the next frame
    public IEnumerator CanJumpNextFrameRoutine(bool newCanJump) {
        yield return null;
        canJump = newCanJump;
    }


    public void SetCanJump(bool newCanJump) {
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

    private bool PlayerIsDoingClimbingMotion() {
        return rightGripDown || leftGripDown;
    }



    public void Stun() {
        StartCoroutine(StunCoroutine());
    }

    private IEnumerator StunCoroutine() {
        isInCollisionStun = true;
        cameraShake.StartShake(collisionShakeIntensity);
        yield return new WaitForSeconds(collisionStunTime);
        cameraShake.StopShake();
        isInCollisionStun = false;
    }
}
