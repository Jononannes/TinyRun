using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderController : MonoBehaviour {

    public Spider spider;
    public Transform player;
    public Path path;

    private int spiderSegmentIndex = 0;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        PathSegment playerSegment = PlayerSegment();
        //PathSegment spiderSegment = SpiderSegment();

        if ((spider.transform.position - SpiderSegment().startPosition.position).magnitude > SpiderSegment().length) {
            // change to the next segment
            spiderSegmentIndex += 1;

            // set player to the correct position
            spider.transform.position = SpiderSegment().startPosition.position;

            if (SpiderSegment().type == PathSegment.Type.Running) {
                spider.transform.position += new Vector3(0f, 0.3f, 0f);
                //spider.transform.rotation = new Quaternion(0f, 0f, 0f, 1f);
                spider.transform.forward = Vector3.forward;
                spider.transform.up = Vector3.up;
            } else if (SpiderSegment().type == PathSegment.Type.Climbing) {
                spider.transform.position += new Vector3(0f, 0f, -0.3f);
                //spider.transform.rotation = new Quaternion(-90f, 0f, 0f, 1f);
                spider.transform.forward = Vector3.up;
                spider.transform.up = -Vector3.forward;
            }
        }

        // the difficulty of the path segment that the player is currently on
        float difficulty = playerSegment.difficulty;

        if (SpiderSegment().type == PathSegment.Type.Running) {
            spider.transform.position += new Vector3(0f, 0f, difficulty) * Time.deltaTime;
        } else if (SpiderSegment().type == PathSegment.Type.Climbing) {
            spider.transform.position += new Vector3(0f, difficulty, 0f) * Time.deltaTime;
        }
    }


    private PathSegment PlayerSegment() {
        return path.GetCurrentSegment();
    }

    private PathSegment SpiderSegment() {
        return path.segments[spiderSegmentIndex];
    }
}
