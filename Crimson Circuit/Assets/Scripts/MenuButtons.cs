using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{
    public GameObject options;
    public GameObject start;

    public Slider sensitivitySlider;
    public Slider volumeSlider;

    public GameObject backButton;
    public GameObject startButton;

    private void Start()
    {
        PlayerPrefs.SetFloat("Volume", 1.0f);
        PlayerPrefs.SetFloat("CamSensitivity", 5.0f);
        sensitivitySlider.value = 0.5f;
        volumeSlider.value = 1.0f;

        sensitivitySlider.onValueChanged.AddListener(UpdateSensitivity);
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float sliderValue)
    {
        PlayerPrefs.SetFloat("Volume", sliderValue);
    }

    public void UpdateSensitivity(float value)
    {
        float sense = value * 10f;
        PlayerPrefs.SetFloat("CamSensitivity", sense);
        Debug.Log(value);
    }

    public void OptionsActivator()
    {
        start.SetActive(false);
        options.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(backButton);
    }

    public void StartActivator()
    {
        start.SetActive(true);
        options.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(startButton);
    }

}
