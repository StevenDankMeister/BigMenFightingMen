using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour {

    private double frame = 0;
    private double startFrame = 0;

	void Start () {
        StartCoroutine(autoDestroy());
	}

    void Update()
    {
        frame += 1;
    }

    public IEnumerator autoDestroy()
    {
        //print("created at " + frame);
        yield return new WaitWhile(() => startFrame > frame - 10);
        Destroy(this.gameObject);
        //print("destroyed at " + frame);
    }
}
