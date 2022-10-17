using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class GameStateController : MonoBehaviour {

    public State state {
        get;
        private set;
    }
    public InputActionReference leftPausePressed;
    public InputActionReference rightPausePressed;
    public UnityEvent onPaused;
    public UnityEvent onPlayed;


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
        state = State.PAUSED;
        Time.timeScale = 0f;
        onPaused.Invoke();
    }

    public void Play() {
        state = State.PLAYING;
        Time.timeScale = 1f;
        onPlayed.Invoke();
    }



    public enum State {
        PLAYING,
        PAUSED
    }
}
