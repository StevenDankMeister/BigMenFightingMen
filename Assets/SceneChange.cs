using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class SceneChange : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    public void changeMenuScene(string sceneName)
    {
        if (Input.anyKey)
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    // Update is called once per frame
    void Update () {
        if (Input.anyKey)
        {
            SceneManager.LoadScene("Arena");
        }


	}
}
