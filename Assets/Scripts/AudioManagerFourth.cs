using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class AudioManagerFourth : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider sliderMaster;
    public Toggle toggleMaster;

    public float sliderMasterValue;
    [SerializeField] float _preMutedMasterSliderValue = 0f;

    public enum MixerExposedParam { MasterVolume, MusicVolume, SFXVolume }
    public MixerExposedParam _mixerExposedParam;

    float _multiplier = 20f;
    bool _disableToggleEvent;

    void Awake()
    {
        sliderMaster.onValueChanged.AddListener(HandleSliderValueChanged);
        toggleMaster.onValueChanged.AddListener(HandleToggleValueChange);
    }
    void Start()
    {
        sliderMaster.value = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        sliderMasterValue = sliderMaster.value;
    }
    public void SetLevel(float sliderValue)
    {
        mixer.SetFloat("MasterVolume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("MasterVolume", sliderValue);
        sliderValue = sliderMaster.value;
    }

    void HandleToggleValueChange(bool enableSound)
    {
        Debug.Log("EnableSound = " + enableSound);
        if (_disableToggleEvent) return;

        if (enableSound)
        {
            //slider.value = slider.maxValue;
            sliderMaster.value = _preMutedMasterSliderValue;
            Debug.Log("Setting Value after RE-enabling toggle");
        }
        else
        {
            _preMutedMasterSliderValue = sliderMaster.value;
            sliderMaster.value = sliderMaster.minValue;
        }
    }

    void HandleSliderValueChanged(float value)
    {
        mixer.SetFloat("MasterVolume", Mathf.Log10(sliderMaster.value) * _multiplier);
        sliderMasterValue = sliderMaster.value;
        Debug.Log("New Master Volume = " + sliderMasterValue);
        //if (value == slider.minValue) AudioListener.pause = true;
        _disableToggleEvent = true;
        toggleMaster.isOn = sliderMaster.value > sliderMaster.minValue;
        _disableToggleEvent = false;
    }
}
