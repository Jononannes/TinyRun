using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class VRButton : MonoBehaviour {

    public ButtonVisuals buttonVisuals;

    public UnityEvent onPressed;

    public void OnStartHover() {
        if (buttonVisuals != null) {
            buttonVisuals.SetHover(true);
        }
    }

    public void OnStopHover() {
        if (buttonVisuals != null) {
            buttonVisuals.SetHover(false);
        }
    }

}
