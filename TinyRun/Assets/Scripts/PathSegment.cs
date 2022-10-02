using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PathSegment : MonoBehaviour {

    public Transform startPosition;
    [Min(0f)]
    public float length;
    public Type type;
    [Range(0f, 10f)]
    public float difficulty;
    public UnityEvent onReached;
    public UnityEvent resetEvent;


    void Start() {

    }

    
    void Update() {

    }


    public Vector3 GetEndPosition() {
        switch (type) {
            case Type.Running: return startPosition.position + new Vector3(0f, 0f, length);
            case Type.Climbing: return startPosition.position + new Vector3(0f, length, 0f);
            default: return startPosition.position;
        }
    }


    public void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(startPosition.position, 0.5f);
        Gizmos.DrawWireSphere(GetEndPosition(), 0.5f);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(startPosition.position, GetEndPosition());
    }


    public enum Type {
        Running,
        Climbing,
        Finish
    }
}
