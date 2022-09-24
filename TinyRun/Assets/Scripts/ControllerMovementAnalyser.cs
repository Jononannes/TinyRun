using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerMovementAnalyser : MonoBehaviour {

    public ControllerTracker rightController;
    public ControllerTracker leftController;
    public Transform environment;

    // configurable settings
    [Range(0f, 5f)]
    public float minimumArmMovementForRun = 1f;
    [Range(0f, 5f)]
    public float minimumEnvironmentMovement = 0f;
    [Range(0f, 5f)]
    public float armToEnvironmentMovementScaling = 0.25f;
    

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (rightController.GetVelocity().z > 0 && leftController.GetVelocity().z < 0 ||
            rightController.GetVelocity().z < 0 && leftController.GetVelocity().z > 0) {
            float totalZMovement = Mathf.Abs(rightController.GetVelocity().z) + Mathf.Abs(leftController.GetVelocity().z);
            if (totalZMovement > minimumArmMovementForRun) {
                float environmentMovement = (totalZMovement - minimumArmMovementForRun) * armToEnvironmentMovementScaling + minimumEnvironmentMovement;
                environment.transform.position -=  new Vector3(0f, 0f, environmentMovement * Time.deltaTime);
            }
        }
    }
}
