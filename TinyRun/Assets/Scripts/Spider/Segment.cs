using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment : MonoBehaviour {
    public Transform start, end;
    [HideInInspector] public float length;


    void Start() {
        length = (start.position - end.position).magnitude;
    }

    public void SetStart(Vector3 pos) {
        transform.position = pos;
    }

    public void Follow(Vector3 target) {
        transform.LookAt(target);
    }


    //private void OnDrawGizmosSelected() {
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawSphere(transform.position, 0.05f);
    //}
}
