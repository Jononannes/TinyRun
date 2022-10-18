using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour {

    public TextMeshProUGUI text;
    float time;

    private GameStateController gameStateController;

    void Start() {
        gameStateController = FindObjectOfType<GameStateController>();
    }


    void Update() {
        if (gameStateController.state == GameStateController.State.PLAYING) {
            time += Time.deltaTime;
            text.text = SecondsToTime(time);
        }
    }


    private string SecondsToTime(float seconds) {
        int minutes = (int) (seconds / 60f);
        float secondsLeft = seconds - 60 * minutes;
        return minutes.ToString() + ":" + secondsLeft.ToString("F2");
    }

    public void ResetTime() {
        time = 0;
    }
}
