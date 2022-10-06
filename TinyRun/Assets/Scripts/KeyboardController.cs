using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardController : MonoBehaviour {

    public Transform player;
    public Path path;
    public float movementSpeed = 2f;
    public AnimationCurve jumpCurve;
    public float jumpTime = 1f;

    private bool isJumping = false;
    private float timeSinceJumpStart = 0f;



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



    private void HandleRunningSegment() {
        if (!isJumping && Input.GetKeyDown(KeyCode.Space)) {
            StartCoroutine(JumpCoroutine());
        } else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            player.position += new Vector3(0f, 0f, movementSpeed * Time.deltaTime);
        }
    }


    private void HandleClimbingSegment() {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            player.position += new Vector3(0f, movementSpeed * Time.deltaTime, 0f);
        }
    }




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


}
