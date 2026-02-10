using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterVolume : MonoBehaviour
{
    public AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        float volume = PlayerPrefs.GetFloat("Volume", 0f);
        SetVolume(volume);
    }

    private void Update()
    {
        float volume = PlayerPrefs.GetFloat("Volume", 0f);
        SetVolume(volume);
    }

    public void SetVolume(float value)
    {
        audioSource.volume = value;
    }
}
