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
    public VRButton difficultyButton;
    public VRButton restartButton;
    public bool isInStartMenu = true;
    public bool isInVictoryMenu = false;


    void Update() {
        if (isInStartMenu) {
            if (Input.GetKeyDown(KeyCode.S)) {
                isInStartMenu = false;
                difficultyButton.onPressed.Invoke();
            }
        } else if (isInVictoryMenu) {
            if (Input.GetKeyDown(KeyCode.R)) {
                restartButton.onPressed.Invoke();
            }
        }

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
        if (analyser.canJump && Input.GetKeyDown(KeyCode.Space)) {
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



    public void SetIsInStartMenu(bool newBool) {
        isInStartMenu = newBool;
    }

    public void SetIsInVictoryMenu(bool newBool) {
        isInVictoryMenu = newBool;
    }
}
