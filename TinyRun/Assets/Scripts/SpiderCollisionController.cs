using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SpiderCollisionController : MonoBehaviour {

    public Transform player;
    public Transform spider;

    [Range(0f, 5f)]
    public float collisionDistance = 1.5f;
    [Range(0f, 50f)]
    public float warningSoundDistance = 10f;
    [Min(0f)]
    public float warningSoundCooldownTime = 4f;

    private AudioSource audioSource;
    private float timeSinceWarningSound = 0f;


    private void Start() {
        audioSource = GetComponent<AudioSource>();
    }


    void Update() {
        if ((player.position - spider.position).magnitude < collisionDistance) {
            // game over
        }

        if (timeSinceWarningSound > warningSoundCooldownTime) {
            if ((player.position - spider.position).magnitude < warningSoundDistance) {
                audioSource.pitch = Random.Range(0.8f, 1.25f);
                audioSource.Play();
                timeSinceWarningSound = 0f;
            }
        }

        timeSinceWarningSound += Time.deltaTime;
    }


    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(player.position, 0.5f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(spider.position, warningSoundDistance);
    }
}
