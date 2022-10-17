using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameStateController : MonoBehaviour {

    public State state {
        get;
        private set;
    }
    public InputActionReference leftPausePressed;
    public InputActionReference rightPausePressed;


    private void Awake() {
        leftPausePressed.action.performed += PlayPause;
        rightPausePressed.action.performed += PlayPause;
    }

    private void OnDestroy() {
        leftPausePressed.action.performed -= PlayPause;
        rightPausePressed.action.performed -= PlayPause;
    }

    void Start() {
        Pause();
    }




    public void PlayPause(InputAction.CallbackContext context) {
        if (state == State.PLAYING) {
            Pause();
        } else if (state == State.PAUSED) {
            Play();
        }
    }

    public void Pause() {
        print("pausing");
        state = State.PAUSED;
        Time.timeScale = 0.0001f;
    }

    public void Play() {
        print("playing");
        state = State.PLAYING;
        Time.timeScale = 1f;
    }



    public enum State {
        PLAYING,
        PAUSED
    }
}
