using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour {

    public Transform player;
    public PathSegment[] segments;

    [Min(0)]
    public int startAtSegment = 0;


    private int currentSegment = 0;
    
    
    void Start() {
        for (int i = 1; i < segments.Length; i++) {
            segments[i].transform.position = segments[i - 1].GetEndPosition();
        }

        SetCurrentSegment(startAtSegment);
        player.position = GetCurrentSegment().startPosition.position;
    }


    void Update() {
        // if we have traveled far enough on the current segment (further than its length)
        if ((player.position - GetCurrentSegment().startPosition.position).magnitude > GetCurrentSegment().length) {
            // change to the next segment
            SetCurrentSegment(currentSegment + 1);

            // set player to the correct position
            if (segments[currentSegment - 1].type != segments[currentSegment].type) {
                player.position = GetCurrentSegment().startPosition.position;
            }
        }
    }


    public PathSegment GetCurrentSegment() {
        return segments[currentSegment];
    }


    public void Reset() {
        for (int i = currentSegment; i >= 0; i--) {
            segments[i].resetEvent.Invoke();
        }
        SetCurrentSegment(0);
    }


    private void OnDrawGizmosSelected() {
        foreach (PathSegment segment in segments) {
            segment.OnDrawGizmosSelected();
        }
    }


    private void SetCurrentSegment(int segmentIndex) {
        currentSegment = segmentIndex;
        GetCurrentSegment().onReached.Invoke();
    }
}
