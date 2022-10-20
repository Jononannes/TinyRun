using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {
	public Transform[] transforms;

	private bool isShaking = false;
	private float shakeAmount;
	private Vector3[] originalPositions;

	void OnEnable() {
		originalPositions = new Vector3[transforms.Length];
		for (int i = 0; i < transforms.Length; i++) {
			originalPositions[i] = transforms[i].localPosition;
		}
	}

	void Update() {
		if (isShaking) {
			Vector3 offset = Random.insideUnitSphere * shakeAmount;
			for (int i = 0; i < transforms.Length; i++) {
				transforms[i].localPosition = originalPositions[i] + offset;
			}
		}
	}

	public void StartShake(float intensity) {
		shakeAmount = intensity;
		isShaking = true;
    }

	public void StopShake() {
		for (int i = 0; i < transforms.Length; i++) {
			transforms[i].localPosition = originalPositions[i];
		}
		isShaking = false;
	}
}