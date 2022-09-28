using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerMovementAnalyser : MonoBehaviour {

    public ControllerTracker rightController;
    public ControllerTracker leftController;
    public Transform player;

    // configurable settings
    [Range(0f, 5f)]
    public float minimumArmMovementForRun = 0.5f;
    [Range(0f, 5f)]
    public float armToEnvironmentMovementScaling = 0.25f;


    private AudioSource audioSource;
    private bool lastRightArmSwingForward = false;


    private void Start() {
        audioSource = GetComponent<AudioSource>();
    }



    // Update is called once per frame
    void Update() {
        if (rightController.GetVelocity().z > minimumArmMovementForRun && leftController.GetVelocity().z < -minimumArmMovementForRun ||
            rightController.GetVelocity().z < -minimumArmMovementForRun && leftController.GetVelocity().z > minimumArmMovementForRun) {

            float totalZMovement = Mathf.Abs(rightController.GetVelocity().z) + Mathf.Abs(leftController.GetVelocity().z);
            if (totalZMovement > minimumArmMovementForRun) {
                float environmentMovement = (totalZMovement - minimumArmMovementForRun) * armToEnvironmentMovementScaling;
                player.position +=  new Vector3(0f, 0f, environmentMovement * Time.deltaTime);

                bool rightArmSwingForward = rightController.GetVelocity().z > 0;

                if ((rightArmSwingForward && !lastRightArmSwingForward) ||
                    (!rightArmSwingForward && lastRightArmSwingForward)) {
                    audioSource.pitch = Random.Range(0.8f, 1.25f);
                    audioSource.Play();
                }

                lastRightArmSwingForward = rightArmSwingForward;
            }
        }
    }
}
