using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowTableTutorial : MonoBehaviour {

    public GameObject tutorial;

    public void ShowTutorial() {
        tutorial.SetActive(true);
    }

    public void HideTutorial() {
        tutorial.SetActive(false);
    }

}
