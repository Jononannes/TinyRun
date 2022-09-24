using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerTracker : MonoBehaviour {
    
    public Text text;
    public bool debug = false;

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

        if (debug) {
            FindObjectOfType<DebuggerText>().Log(transform.position);
        }
    }


    public Vector3 GetPosition() {
        return transform.position;
    }

    public Vector3 GetVelocity() {
        return currentVelocity;
    }

    public float GetSpeed() {
        return GetVelocity().magnitude;
    }
}
