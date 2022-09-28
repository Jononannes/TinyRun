using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ControllerTracker : MonoBehaviour {

    public string controllerName;
    public Text text;
    public bool debug = false;

    public InputActionReference triggerPressed;
    public InputActionReference gripPressed;
    public InputActionReference startPressed;

    private Vector3 currentVelocity;
    private Vector3 prevPosition;

    private void Awake() {
        triggerPressed.action.performed += TestTrigger;
        gripPressed.action.performed += TestGrip;

        if (startPressed != null) {
            startPressed.action.performed += TestStart;
        }
    }

    private void OnDestroy() {
        triggerPressed.action.performed -= TestTrigger;
        gripPressed.action.performed -= TestGrip;

        if (startPressed != null) {
            startPressed.action.performed -= TestStart;
        }
    }

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


    private void TestTrigger(InputAction.CallbackContext context) {
        //FindObjectOfType<DebuggerText>().Log(controllerName + " trigger event");
        FindObjectOfType<ControllerMovementAnalyser>().ResetPlayerPosition();
    }

    private void TestGrip(InputAction.CallbackContext context) {
        //FindObjectOfType<DebuggerText>().Log(controllerName + " grip event");
        //ControllerMovementAnalyser analyser = FindObjectOfType<ControllerMovementAnalyser>();
        //analyser.canJump = !analyser.canJump;
        //FindObjectOfType<DebuggerText>().Log("Setting canJump to " + analyser.canJump);
    }

    private void TestStart(InputAction.CallbackContext context) {
        //FindObjectOfType<DebuggerText>().Log(controllerName + " pause event");
    }
}
