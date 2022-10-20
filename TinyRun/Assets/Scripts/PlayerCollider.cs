using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CapsuleCollider)), RequireComponent(typeof(Rigidbody))]
public class PlayerCollider : MonoBehaviour {

    public UnityEvent onCollision;

    private CapsuleCollider playerCollider;

    void Start() {
        playerCollider = GetComponent<CapsuleCollider>();
    }


    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Obstacle")) {
            onCollision.Invoke();
        }
    }


    public void CalculateHeight() {
        float realHeight = transform.position.y;
        float adjustedHeight = realHeight - 0.25f;
        playerCollider.height = adjustedHeight;
    }
}
