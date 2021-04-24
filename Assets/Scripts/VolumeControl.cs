using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] string _volumeParameter = "MasterVolume";
    [SerializeField] AudioMixer _mixer;
    [SerializeField] Slider _slider;
    [SerializeField] float _multiplier = 20f;
    [SerializeField] GameObject _Icon;
    [SerializeField] GameObject _redIcon;

    void Awake()
    {
        _slider.onValueChanged.AddListener(HandleSliderValueChanged);
    }

    void Start()
    {
        _slider.value = PlayerPrefs.GetFloat(_volumeParameter, _slider.value);
        if (_slider.value <= _slider.minValue)
        {
            EnableDisablesIcon(true);
        }
        else
        {
            EnableDisablesIcon(false);
        }
    }

    void HandleSliderValueChanged(float value)
    {
        _mixer.SetFloat(_volumeParameter, Mathf.Log10(value) * _multiplier);
        if (_slider.value <= _slider.minValue)
        {
            EnableDisablesIcon(true);
        }
        else
        {
            EnableDisablesIcon(false);
        }
    }

    void EnableDisablesIcon(bool isDisable)
    {
        _redIcon.GetComponent<Image>().enabled = isDisable;
        _Icon.GetComponent<Image>().enabled = !isDisable;
    }

    void OnDisable()
    {
        PlayerPrefs.SetFloat(_volumeParameter, _slider.value);
    }
}
