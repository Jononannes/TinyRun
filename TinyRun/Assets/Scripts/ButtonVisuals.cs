using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ButtonVisuals : MonoBehaviour {

    [SerializeField]
    private Color32 hoverColor;
    private Color32 nonHoverColor;
    private TextMeshProUGUI text;

    private void Start() {
        text = GetComponent<TextMeshProUGUI>();
        nonHoverColor = text.color;
    }


    public void SetHover(bool hover) {
        text.color = hover ? hoverColor : nonHoverColor;
    }
}
