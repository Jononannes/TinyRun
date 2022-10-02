using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishRun : MonoBehaviour {

    public GameObject path;
    public GameObject onObject;
    public GameObject offObject;

    public void ShowTutorial() {
        onObject.SetActive(true);
        offObject.SetActive(false);
    }

    public void HideTutorial() {
        onObject.SetActive(false);
        offObject.SetActive(true);
    }

}
