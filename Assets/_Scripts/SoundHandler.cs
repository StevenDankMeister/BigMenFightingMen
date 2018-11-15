using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHandler : MonoBehaviour {

    AudioSource audioSource;
    public static SoundHandler soundHandler;

	// Use this for initialization
	void Start () {
        soundHandler = GetComponent<SoundHandler>();
        audioSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public AudioClip RandomSound(AudioClip[] array)
    {
        float length = array.Length;
        int i = Mathf.RoundToInt(Random.Range(0, length - 1));

        return array[i];
    }
}
