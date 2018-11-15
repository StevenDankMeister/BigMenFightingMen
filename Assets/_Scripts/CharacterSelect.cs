using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour {

    public Sprite[] sprites;
    public GameObject child;
    public GameObject textHolder;
    private Text text;

    SpriteRenderer spriteHolder;

	void Start () {
        spriteHolder = child.GetComponent<SpriteRenderer>();
        text = textHolder.GetComponent<Text>();
	}
		
	void Update ()
    {
        Left();
        Right();
    }

    private void Right()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            spriteHolder.sprite = sprites[1];
            text.text = "Preset:\nSlot locked! Unlock for $4.99.";
        }
    }

    private void Left()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            spriteHolder.sprite = sprites[0];
            text.text = "Preset:\nBig Brown Man";
        }
    }
}
