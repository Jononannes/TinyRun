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


    private void Start() {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(SetCanJumpNextFrame(true));
    }



    // Update is called once per frame
    void Update() {
        if (!isJumping &&
            (rightController.GetVelocity().y > minimumArmMovementForJump && leftController.GetVelocity().y > minimumArmMovementForJump)) {
            Jump();
        }

        if (!isJumping &&
            (rightController.GetVelocity().z > minimumArmMovementForRun && leftController.GetVelocity().z < -minimumArmMovementForRun ||
            rightController.GetVelocity().z < -minimumArmMovementForRun && leftController.GetVelocity().z > minimumArmMovementForRun)) {

            float totalZMovement = Mathf.Abs(rightController.GetVelocity().z) + Mathf.Abs(leftController.GetVelocity().z);
            if (totalZMovement > minimumArmMovementForRun) {
                float environmentMovement = (totalZMovement - minimumArmMovementForRun) * armToEnvironmentMovementScaling;
                player.position +=  new Vector3(0f, 0f, environmentMovement * Time.deltaTime);

                bool rightArmSwingForward = rightController.GetVelocity().z > 0;

                if ((rightArmSwingForward && !lastRightArmSwingForward) ||
                    (!rightArmSwingForward && lastRightArmSwingForward)) {
                    audioSource.pitch = Random.Range(0.8f, 1.25f);
                    audioSource.Play();
                }

                lastRightArmSwingForward = rightArmSwingForward;
            }
        }
    }


    private void Jump() {
        StartCoroutine(JumpCoroutine());
    }


    private IEnumerator JumpCoroutine() {
        timeSinceJumpStart = 0f;
        isJumping = true;

        float jumpLength = jumpCurve.keys[jumpCurve.keys.Length - 1].time;

        //FindObjectOfType<DebuggerText>().Log("Jumping, length: " + jumpLength.ToString() + ", time: " + jumpTime.ToString());

        if (canJump) {
            while (timeSinceJumpStart < jumpTime) {
                float offsetY = jumpCurve.Evaluate(timeSinceJumpStart * jumpLength / jumpTime) -
                                jumpCurve.Evaluate((timeSinceJumpStart - Time.deltaTime) * jumpLength / jumpTime);
                float offsetZ = Time.deltaTime * jumpLength / jumpTime;
                player.position += new Vector3(0f, offsetY, offsetZ);

                timeSinceJumpStart += Time.deltaTime;
                yield return null;
            }

            timeSinceJumpStart = jumpTime;
            float oY = jumpCurve.Evaluate(timeSinceJumpStart * jumpLength / jumpTime) -
                       jumpCurve.Evaluate((timeSinceJumpStart - Time.deltaTime) * jumpLength / jumpTime);
            player.position += new Vector3(0f, oY, 0f);
        }

        isJumping = false;
    }


    public void ResetPlayerPosition() {
        bool couldJump = canJump;
        canJump = false;
        player.position = new Vector3(0f, 0f, 0f);
        StartCoroutine(SetCanJumpNextFrame(couldJump));
    }

    private IEnumerator SetCanJumpNextFrame(bool newCanJump) {
        yield return null;
        canJump = newCanJump;
    }
}
