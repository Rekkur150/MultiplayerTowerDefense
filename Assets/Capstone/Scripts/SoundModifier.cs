using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundModifier : MonoBehaviour
{

    public AudioMixer masterMixer;
    public Slider audioSlider;

    private void Start()
    {
        audioSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
    }

    public void SetMasterVolume(float newLevel)
    {
        masterMixer.SetFloat("MasterVolume", Mathf.Log10(newLevel) * 20);
        PlayerPrefs.SetFloat("MasterVolume", newLevel);
    }
}
