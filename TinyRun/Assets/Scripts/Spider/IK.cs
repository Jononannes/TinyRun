using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK : MonoBehaviour {

    public Segment[] segments;
    public Transform pole;

    public void follow(Vector3 target) {
        Segment s = segments[segments.Length - 1];
        s.Follow(target);
        s.SetStart(target + (s.transform.position - target).normalized * s.length);

        for (int i = segments.Length - 2; i >= 0; i--) {
            s = segments[i];
            Segment s2 = segments[i + 1];
            s.Follow(s2.start.position);
            s.SetStart(s2.transform.position + (s.transform.position - s2.transform.position).normalized * s.length);
        }

        segments[0].SetStart(transform.position);
        for (int i = 1; i < segments.Length; i++) {
            segments[i].SetStart(segments[i - 1].end.position);
        }


        if (pole != null) {
            for (int i = 1; i < segments.Length; i++) {
                Plane plane = new Plane(segments[i - 1].transform.position - segments[i].end.position, segments[i].end.position);
                Vector3 projectedPole = plane.ClosestPointOnPlane(pole.position);
                Vector3 projectedJoint = plane.ClosestPointOnPlane(segments[i].transform.position);
                float angle = Vector3.SignedAngle(projectedJoint - segments[i].end.position, projectedPole - segments[i].end.position, plane.normal);
                segments[i].SetStart(Quaternion.AngleAxis(angle, plane.normal) * (segments[i].transform.position - segments[i].end.position) + segments[i].end.position);
            }
        }
    }

    public float Length() {
        float length = 0;
        foreach (Segment s in segments) {
            length += s.length;
        }
        return length;
    }

    public Vector3 GetEndPoint() {
        return segments[segments.Length - 1].end.position;
    }
}