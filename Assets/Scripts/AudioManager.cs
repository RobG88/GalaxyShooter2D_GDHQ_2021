using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    /*
    public class AudioManager : MonoSingleton<AudioManager>

    public override void Init()
    {
        base.Init();
    }

    */
    public static AudioManager instance;



    // Set to lazy Instance from Jon's MonoSingleton
    /// <summary>
    /// ///////////////////////////////////////////////////////
    /// </summary>

    public AudioMixer _MasterMixer;

    [SerializeField] Slider _Master_Volume_Slider;
    [SerializeField] Slider _SFX_Volume_Slider;
    [SerializeField] Slider _Music_Volume_Slider;

    [SerializeField] string _Master_Exposed_parameter;
    [SerializeField] string _SFX_Exposed_parameter;
    [SerializeField] string _Music_Exposed_parameter;

    [SerializeField] float _playerPrefMaster_Volume;
    [SerializeField] float _playerPrefMusic_Volume;
    [SerializeField] float _playerPrefSFX_Volume;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // The getting & setting AudioMixer needs to happen in Start(), can not be Awake
        _playerPrefMaster_Volume = PlayerPrefs.GetFloat(_Master_Exposed_parameter);
        _playerPrefSFX_Volume = PlayerPrefs.GetFloat(_SFX_Exposed_parameter);
        _playerPrefMusic_Volume = PlayerPrefs.GetFloat(_Music_Exposed_parameter);
        _Master_Volume_Slider.value = _playerPrefMaster_Volume;
        //_Master_Volume_Slider.value = PlayerPrefs.GetFloat(_Master_Exposed_parameter); //, _Master_Volume_Slider.value);
        _SFX_Volume_Slider.value = PlayerPrefs.GetFloat(_SFX_Exposed_parameter);
        _Music_Volume_Slider.value = PlayerPrefs.GetFloat(_Music_Exposed_parameter);

        ////SetMasterVolume(_playerPrefMaster_Volume);
        //SetFXVol(_playerPrefSFX_Volume);
        //SetBGMusicVol(_playerPrefMusic_Volume);
    }

    public void SetMasterVolume(Slider volume)
    {
        _MasterMixer.SetFloat(_Master_Exposed_parameter, volume.value);
    }

    public void SetFXVol(float sfxLevel)
    {
        //UIManager.instance.EnableDisableSFXIcon(sfxLevel <= _SFX_Volume_Slider.minValue);
        _MasterMixer.SetFloat(_SFX_Exposed_parameter, sfxLevel);
        //PlayerPrefs.SetFloat(_SFX_Exposed_parameter, sfxLevel);
    }

    public void SetBGMusicVol(float BGMusicLevel)
    {
        //UIManager.instance.EnableDisableBGMIcon(BGMusicLevel <= _Music_Volume_Slider.minValue);
        //PlayerPrefs.SetFloat(_Music_Exposed_parameter, BGMusicLevel);
        _MasterMixer.SetFloat(_Music_Exposed_parameter, Mathf.Log10(BGMusicLevel) * 20);
    }
}
/*
[System.Serializable]
public class AudioSetting
{
    public Slider slider;
    public GameObject redX;
    public string exposedParam;

    public void Initialize()
    {
        slider.value = PlayerPrefs.GetFloat(exposedParam);
    }

    public void SetExposedParam(float value) // 1
    {
        redX.SetActive(value <= slider.minValue); // 2
        AudioManager.Instance._MasterMixer.SetFloat(exposedParam, value); // 3
        PlayerPrefs.SetFloat(exposedParam, value); // 4
    }
}
*/