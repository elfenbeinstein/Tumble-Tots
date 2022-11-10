using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sliders : MonoBehaviour
{
    [SerializeField] private Slider sliderSFX;
    [SerializeField] private Slider sliderMusic;

    void Start()
    {
        sliderSFX.value = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
        sliderMusic.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
    }

    public void SetLevelSFX(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        EventSystem.Instance.Fire("AUDIO", "sfx", value);
    }
    public void SetLevelMusic(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        EventSystem.Instance.Fire("AUDIO", "music", value);
    }
}
