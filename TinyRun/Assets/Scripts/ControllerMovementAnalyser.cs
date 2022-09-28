using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerMovementAnalyser : MonoBehaviour {

    public ControllerTracker rightController;
    public ControllerTracker leftController;
    public Transform player;

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
    private bool isJumping = false;
    private float timeSinceJumpStart = 0f;
    public bool canJump = false;


    // Start is called before the first frame update
    private void Start() {
        audioSource = GetComponent<AudioSource>();

        // canJump starts as false in order to behave nicely on the first frame,
        // but is set to true the next frame
        StartCoroutine(SetCanJumpNextFrame(true));
    }



    // Update is called once per frame
    void Update() {

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
                player.position +=  new Vector3(0f, 0f, environmentMovement * Time.deltaTime);

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
    private IEnumerator JumpCoroutine() {
        timeSinceJumpStart = 0f;
        isJumping = true;
        float jumpLength = jumpCurve.keys[jumpCurve.keys.Length - 1].time;

        while (timeSinceJumpStart < jumpTime) {
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
        float oY = jumpCurve.Evaluate(timeSinceJumpStart * jumpLength / jumpTime) -
                    jumpCurve.Evaluate((timeSinceJumpStart - Time.deltaTime) * jumpLength / jumpTime);
        player.position += new Vector3(0f, oY, 0f);

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
        player.position = new Vector3(0f, 0f, 0f);
        StartCoroutine(SetCanJumpNextFrame(couldJump));
    }

    // Sets the canJump variable on the next frame
    private IEnumerator SetCanJumpNextFrame(bool newCanJump) {
        yield return null;
        canJump = newCanJump;
    }
}
