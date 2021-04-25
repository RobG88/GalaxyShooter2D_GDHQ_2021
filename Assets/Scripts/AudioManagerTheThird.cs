using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManagerTheThird : MonoBehaviour
{
    public static AudioManagerTheThird instance;
    public enum MixerExposedParam { MasterVolume, MusicVolume, SFXVolume }
    public MixerExposedParam _mixerExposedParam;

    public AudioMixer mixer;
    public Slider slider;
    public float sliderValue;

    [SerializeField] GameObject _Icon;
    [SerializeField] GameObject _redIcon;

    bool _isPaused = false;

    void Awake()
    {
        instance = this;
        slider.onValueChanged.AddListener(HandleSliderValueChanged);
    }

    void Start()
    {
        slider.value = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        sliderValue = slider.value;
        if (sliderValue == slider.minValue) AudioListener.pause = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            _isPaused = !_isPaused;
            AudioListener.pause = _isPaused;
        }
    }
    public void SetLevel(float sliderValue)
    {
        mixer.SetFloat("MasterVolume", Mathf.Log10(sliderValue) * 20);
        if (sliderValue == slider.minValue) sliderValue = 0.005f;
        PlayerPrefs.SetFloat("MasterVolume", sliderValue);

    }

    void HandleSliderValueChanged(float value)
    {
        sliderValue = slider.value;
        Debug.Log("New Master Volume = " + sliderValue);
        if (value == slider.minValue) AudioListener.pause = true;
    }

    /*
    public void SetLevel(float sliderValue)
    {
        //mixer.SetFloat(_mixerExposedParam.ToString(), Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat(_mixerExposedParam.ToString(), sliderValue);
    }
    */
}
