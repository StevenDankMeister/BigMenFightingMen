using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHandler : MonoBehaviour {

    AudioSource audioSource;
    public static SoundHandler soundHandler;

    private static bool created = false;

	// Use this for initialization
	void Awake () {
        if (!created)
        {
            created = true;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (created)
        {
            Destroy(this.gameObject);
        }        
    }

    void Start()
    {
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
