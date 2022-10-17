using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class ControllerTracker : MonoBehaviour {
    
    public Text text;

    private Vector3 currentVelocity;
    private Vector3 prevPosition;

    // Start is called before the first frame update
    void Start() {
        currentVelocity = new(0f, 0f, 0f);
        prevPosition = transform.position;
    }

    // Update is called once per frame
    void Update() {
        Vector3 vel = (transform.position - prevPosition) / Time.deltaTime;
        currentVelocity = vel;
        prevPosition = transform.position;
        text.text = vel.ToString() + "\n" + vel.magnitude.ToString();
    }

    // The in-game world space of the controller
    public Vector3 GetPosition() {
        return transform.position;
    }

    // The velocity that the controller is moving at in in-game world space
    public Vector3 GetVelocity() {
        return currentVelocity;
    }

    // How fast the controller is moving regardless of direction
    public float GetSpeed() {
        return GetVelocity().magnitude;
    }
}
