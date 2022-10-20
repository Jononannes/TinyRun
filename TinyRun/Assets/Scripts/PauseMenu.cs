using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {


    public VRButton[] buttons;
    
    
    void Update() {
        foreach (VRButton button in buttons) {
            button.gameObject.SetActive(false);
            button.gameObject.SetActive(true);
        }
    }
}
