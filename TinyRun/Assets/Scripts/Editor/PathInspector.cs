using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Path))]
public class PathInspector : Editor {

    public override void OnInspectorGUI() {
        Path path = (Path)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Update segment positions")) {
            for (int i = 1; i < path.segments.Length; i++) {
                path.segments[i].transform.position = path.segments[i - 1].GetEndPosition();
            }
        }
    }
}
