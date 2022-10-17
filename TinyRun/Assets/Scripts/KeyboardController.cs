using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardController : MonoBehaviour {

    public Transform player;
    public Path path;
    public float movementSpeed = 2f;
    public AnimationCurve jumpCurve;
    public float jumpTime = 1f;
    public ControllerMovementAnalyser analyser;


    void Update() {
        switch (path.GetCurrentSegment().type) {
            case PathSegment.Type.Running:
                HandleRunningSegment();
                break;
            case PathSegment.Type.Climbing:
                HandleClimbingSegment();
                break;
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            FindObjectOfType<GameStateController>().PlayPause(new UnityEngine.InputSystem.InputAction.CallbackContext());
        }
    }



    private void HandleRunningSegment() {
        if (!analyser.isJumping && Input.GetKeyDown(KeyCode.Space)) {
            StartCoroutine(analyser.JumpCoroutine());
        } else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            player.position += new Vector3(0f, 0f, movementSpeed * Time.deltaTime);
        }
    }


    private void HandleClimbingSegment() {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            player.position += new Vector3(0f, movementSpeed * Time.deltaTime, 0f);
        }
    }
}
