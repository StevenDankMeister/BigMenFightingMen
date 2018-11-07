using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameHandler : MonoBehaviour {

    public static FrameHandler frameHandler;

    private float frame = 0;

	// Use this for initialization
	void Start () {
        frameHandler = GetComponent<FrameHandler>();
	}
	
	// Update is called once per frame
	void Update () {
        frame += 1;
	}

    public IEnumerator waitForFrames(float framesToWait)
    {
        float startFrame = frame;
        //print("Startd at " + startFrame);
        //print("Waiting until " + startFrame + "+" + framesToWait);
        yield return new WaitWhile(() => startFrame > frame - framesToWait);
        //print("Finished at" + frame);
    }

    public void gay(float framesToWait)
    {
        for(int i = 0; i < framesToWait; i++)
        {
            print(frame);
            continue;
        }
    }
}
