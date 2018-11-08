using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHandler : MonoBehaviour {

    public static UIHandler uiHandler;

	// Use this for initialization
	void Start () {
        uiHandler = GetComponent<UIHandler>();
	}
	
	public void updateHealthBar(float currentHealth, float maxHealth, GameObject ch)
    {
        
    }
}
