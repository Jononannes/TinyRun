using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : MonoBehaviour {
    public IK[] iks;
    public Transform[] offsets;
    public float moveThreshold;
    public float animationSpeed;
    //public float distanceToGround;
    //public float bodyAnimationSpeed;
    public Transform rayOrigin;

    private Vector3[] points;
    private Vector3[] oldPoints;
    private float[] legAnimationLerps;
    private float[] prevLegAnimationLerps;
    private Vector3 prevPosition;
    private AudioSource audioSource;


    void Start() {
        audioSource = GetComponent<AudioSource>();
        points = new Vector3[iks.Length];
        oldPoints = new Vector3[iks.Length];
        legAnimationLerps = new float[iks.Length];
        prevLegAnimationLerps = new float[iks.Length];
        prevPosition = transform.position;

        for (int i = 0; i < iks.Length; i++) {
            Vector3 dir = (offsets[i].position - rayOrigin.position).normalized;
            RaycastHit hit;
            if (Physics.Raycast(rayOrigin.position, dir, out hit, 100, LayerMask.GetMask("Environment"))) {
                points[i] = hit.point;
                oldPoints[i] = hit.point;
            } else {
                points[i] = Vector3.zero;
                oldPoints[i] = Vector3.zero;
            }
        }
    }


    void Update() {

        // for every leg
        for (int i = 0; i < iks.Length; i++) {
            Vector3 dir = (offsets[i].position - rayOrigin.position).normalized;
            RaycastHit hit;

            // find potential position where we can place foot
            if (Physics.Raycast(rayOrigin.position, dir, out hit, 100, LayerMask.GetMask("Environment"))) {

                // if the foot currently is far behind where we want to place the foot, we want to move it
                if ((hit.point - points[i]).magnitude > moveThreshold) {
                    bool update = true;

                    // make sure we don't move the foot if the foot on the other
                    // side is not on the ground
                    if (i % 4 == 0 || i % 4 == 3) {
                        for (int j = 1; j < iks.Length; j += 4) {
                            if (legAnimationLerps[j] != 1f) {
                                update = false;
                            }
                        }
                        for (int j = 2; j < iks.Length; j += 4) {
                            if (legAnimationLerps[j] != 1f) {
                                update = false;
                            }
                        }
                    } else {
                        for (int j = 0; j < iks.Length; j += 4) {
                            if (legAnimationLerps[j] != 1f) {
                                update = false;
                            }
                        }
                        for (int j = 3; j < iks.Length; j += 4) {
                            if (legAnimationLerps[j] != 1f) {
                                update = false;
                            }
                        }
                    }

                    // move the preferred placement of the foot to the new location smoothly
                    if (update) {
                        oldPoints[i] = points[i];
                        points[i] = hit.point + (transform.position - prevPosition).normalized * moveThreshold;
                        legAnimationLerps[i] = 0;
                    }
                } else {
                    legAnimationLerps[i] = Mathf.Clamp(legAnimationLerps[i] + animationSpeed * Time.deltaTime, 0, 1);

                    if (legAnimationLerps[i] == 1 && prevLegAnimationLerps[i] < 1) {
                        audioSource.pitch = Random.Range(0.8f, 1.25f);
                        audioSource.Play();
                    }
                }

                prevLegAnimationLerps[i] = legAnimationLerps[i];

                // make the leg follow the position of where the foot should be
                iks[i].follow(legAnimationLerps[i] * points[i] + (1 - legAnimationLerps[i]) * oldPoints[i]);
            }
        }



        //RaycastHit groundHit;
        //if (Physics.Raycast(transform.position, -transform.up, out groundHit, iks[0].Length(), LayerMask.GetMask("Ground"))) {
        //    transform.position += (new Vector3(0, distanceToGround - (transform.position - groundHit.point).magnitude, 0)) * bodyAnimationSpeed * Time.deltaTime;
        //}

        prevPosition = transform.position;
    }

    // draw debug things
    //private void OnDrawGizmosSelected() {
    //    Gizmos.color = Color.red;
    //    foreach (IK ik in iks) {
    //        Gizmos.DrawSphere(ik.transform.position, 0.05f);
    //    }

    //    Gizmos.color = Color.blue;
    //    for (int i = 0; i < iks.Length; i++) {
    //        Gizmos.DrawSphere(offsets[i].position, 0.05f);
    //    }

    //    for (int i = 0; i < iks.Length; i++) {
    //        Vector3 dir = offsets[i].position - rayOrigin.position;
    //        Gizmos.DrawRay(rayOrigin.position, dir);
    //    }
    //}
}
