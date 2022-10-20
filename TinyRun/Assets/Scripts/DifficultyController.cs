using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyController : MonoBehaviour {

    [HideInInspector]
    public float difficultyMultiplier = 1f;

    public void SetDifficultyMultiplier(float multiplier) {
        difficultyMultiplier = multiplier;
    }
}
