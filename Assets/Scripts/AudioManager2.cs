using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager2 : MonoBehaviour
{
    [SerializeField] string _volumeParameterMaster = "MasterVolume";
    [SerializeField] string _volumeParameterMusic = "MusicVolume";
    [SerializeField] string _volumeParameterSFX = "SFXVolume";
    [SerializeField] AudioMixer _mixer;
    [SerializeField] Slider _sliderMaster;
    [SerializeField] Slider _sliderMusic;
    [SerializeField] Slider _sliderSFX;
    [SerializeField] float _multiplier = 20f;
    [SerializeField] GameObject _Icon;
    [SerializeField] GameObject _redIcon;

    void Start()
    {
        _sliderMaster.value = PlayerPrefs.GetFloat("MasterVolume", 0.80f);
        _mixer.SetFloat("MasterVolume", Mathf.Log10(_sliderMaster.value) * 20);
        Debug.Log("AudioManager2 :: MasterVolume.Value = " + _sliderMaster.value);

        _sliderMusic.value = PlayerPrefs.GetFloat(_volumeParameterMusic, 0.60f);
        _mixer.SetFloat(_volumeParameterMusic, Mathf.Log10(_sliderMusic.value) * 20);
        Debug.Log("AudioManager2 :: MusicVolume.Value = " + _sliderMusic.value);

        _sliderSFX.value = PlayerPrefs.GetFloat(_volumeParameterSFX, 0.75f);
        _mixer.SetFloat(_volumeParameterSFX, Mathf.Log10(_sliderSFX.value) * 20);
        Debug.Log("AudioManager2 :: SFXVolume.Value = " + _sliderSFX.value);
    }
}
