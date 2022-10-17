using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer))]
public class Pointer : MonoBehaviour {

    public InputActionReference triggerPressed;

    private LineRenderer lineRenderer;
    private VRButton currentButton = null;


    private void Awake() {
        triggerPressed.action.performed += OnTrigger;
    }

    private void OnDestroy() {
        triggerPressed.action.performed -= OnTrigger;
    }

    void Start() {
        lineRenderer = GetComponent<LineRenderer>();
    }


    void Update() {
        // Raycast from controller and see if we hit anything
        float maxDist = 2f;
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxDist, LayerMask.GetMask("VR Button"))) {
            VRButton button = hit.collider.gameObject.GetComponent<VRButton>();
            if (button != currentButton) {
                OnNewHover(button);
            }
            currentButton = button;
            lineRenderer.SetPosition(1, hit.point);
        } else {
            if (currentButton != null) {
                OnNewHover(null);
                currentButton = null;
            }
            lineRenderer.SetPosition(1, transform.position + transform.forward * maxDist);
        }
        lineRenderer.SetPosition(0, transform.position);
    }


    private void OnNewHover(VRButton newButton) {
        if (currentButton != null) {
            currentButton.OnStopHover();
        }
        if (newButton != null) {
            newButton.OnStartHover();
        }
    }


    private void OnTrigger(InputAction.CallbackContext context) {
        if (currentButton != null) {
            currentButton.onPressed.Invoke();
        }
    }
}
