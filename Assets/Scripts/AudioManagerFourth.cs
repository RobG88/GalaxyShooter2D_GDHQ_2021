using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class AudioManagerFourth : MonoBehaviour
{
    public AudioMixer mixer;

    public Slider sliderMaster;
    public Toggle toggleMaster;

    public Slider sliderMusic;
    public Toggle toggleMusic;

    public Slider sliderSFX;
    public Toggle toggleSFX;

    public float sliderMasterValue;
    [SerializeField] float _preMutedMasterSliderValue = 0f;

    public float sliderMusicValue;
    [SerializeField] float _preMutedMusicSliderValue = 0f;

    public float sliderSFXValue;
    [SerializeField] float _preMutedSFXSliderValue = 0f;

    public enum MixerExposedParam { MasterVolume, MusicVolume, SFXVolume }
    public MixerExposedParam _mixerExposedParam;

    float _multiplier = 20f;
    bool _disableMasterToggleEvent, _disableMusicToggleEvent, _disableSFXToggleEvent;

    void Awake()
    {
        sliderMaster.onValueChanged.AddListener(HandleMasterSliderValueChanged);
        toggleMaster.onValueChanged.AddListener(HandleMasterToggleValueChange);

        sliderMusic.onValueChanged.AddListener(HandleMusicSliderValueChanged);
        toggleMusic.onValueChanged.AddListener(HandleMusicToggleValueChange);

        sliderSFX.onValueChanged.AddListener(HandleSFXSliderValueChanged);
        toggleSFX.onValueChanged.AddListener(HandleSFXToggleValueChange);
    }
    void Start()
    {
        sliderMaster.value = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        sliderMasterValue = sliderMaster.value;

        sliderMusic.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        sliderMusicValue = sliderMusic.value;

        sliderSFX.value = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
        sliderSFXValue = sliderSFX.value;
    }
    public void SetLevel(float sliderValue)
    {
        mixer.SetFloat("MasterVolume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("MasterVolume", sliderValue);
        sliderValue = sliderMaster.value;
    }

    public void SetMusicLevel(float sliderValue)
    {
        mixer.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("MusicVolume", sliderValue);
        sliderValue = sliderMusic.value;
    }

    public void SetSFXLevel(float sliderValue)
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("SFXVolume", sliderValue);
        sliderValue = sliderSFX.value;
    }

    void HandleMasterToggleValueChange(bool enableSound)
    {
        Debug.Log("EnableSound = " + enableSound);
        if (_disableMasterToggleEvent) return;

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

    void HandleMasterSliderValueChanged(float value)
    {
        mixer.SetFloat("MasterVolume", Mathf.Log10(sliderMaster.value) * _multiplier);
        sliderMasterValue = sliderMaster.value;
        Debug.Log("New Master Volume = " + sliderMasterValue);
        //if (value == slider.minValue) AudioListener.pause = true;
        _disableMasterToggleEvent = true;
        toggleMaster.isOn = sliderMaster.value > sliderMaster.minValue;
        _disableMasterToggleEvent = false;
    }

    void HandleMusicToggleValueChange(bool enableSound)
    {
        Debug.Log("EnableSound = " + enableSound);
        if (_disableMusicToggleEvent) return;

        if (enableSound)
        {
            //slider.value = slider.maxValue;
            sliderMusic.value = _preMutedMusicSliderValue;
            Debug.Log("Setting Value after RE-enabling toggle");
        }
        else
        {
            _preMutedMusicSliderValue = sliderMusic.value;
            sliderMusic.value = sliderMusic.minValue;
        }
    }

    void HandleMusicSliderValueChanged(float value)
    {
        mixer.SetFloat("MusicVolume", Mathf.Log10(sliderMusic.value) * _multiplier);
        sliderMusicValue = sliderMusic.value;
        Debug.Log("New Music Volume = " + sliderMusicValue);
        //if (value == slider.minValue) AudioListener.pause = true;
        _disableMusicToggleEvent = true;
        toggleMusic.isOn = sliderMusic.value > sliderMusic.minValue;
        _disableMusicToggleEvent = false;
    }

    void HandleSFXToggleValueChange(bool enableSound)
    {
        Debug.Log("EnableSound = " + enableSound);
        if (_disableSFXToggleEvent) return;

        if (enableSound)
        {
            //slider.value = slider.maxValue;
            sliderSFX.value = _preMutedSFXSliderValue;
            Debug.Log("Setting Value after RE-enabling toggle");
        }
        else
        {
            _preMutedSFXSliderValue = sliderSFX.value;
            sliderSFX.value = sliderSFX.minValue;
        }
    }

    void HandleSFXSliderValueChanged(float value)
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(sliderSFX.value) * _multiplier);
        sliderSFXValue = sliderSFX.value;
        Debug.Log("New SFX Volume = " + sliderSFXValue);
        //if (value == slider.minValue) AudioListener.pause = true;
        _disableSFXToggleEvent = true;
        toggleSFX.isOn = sliderSFX.value > sliderSFX.minValue;
        _disableSFXToggleEvent = false;
    }
}
