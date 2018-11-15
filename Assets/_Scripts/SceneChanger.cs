using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour {

    public void changeMenuScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
