using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Flash : MonoBehaviour {


    public GameObject theSprite;

    //SpriteRenderer spriteRenderer;
	// Use this for initialization
	public void Start ()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(StartBlinking());
    }
    //https://answers.unity.com/questions/939224/how-to-make-a-sprite-toggle-on-and-off-after-a-few.html
    IEnumerator StartBlinking()
    {
        yield return new WaitForSeconds(1);
        theSprite.GetComponent<SpriteRenderer>().enabled = !theSprite.GetComponent<SpriteRenderer>().enabled;
        StartCoroutine(StartBlinking());
    }

	// Update is called once per frame

	void Update () {
		
	}
}
