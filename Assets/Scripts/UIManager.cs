using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] Text _scoreText;
    [SerializeField] Image _livesRemainingImage;
    [SerializeField] Sprite[] _livesSprites;

    [SerializeField] Text _shipWrap;

    [SerializeField] Text _playerLives;

    [SerializeField] GameObject[] _shieldBonus;
    [SerializeField] Text _bonusLife_text;

    [SerializeField] Text _gameOverText;
    [SerializeField] float BlinkTime;

    [SerializeField] Text _enemiesRemaing;

    [SerializeField] Text _currentLevel;

    [SerializeField] Text _waveName;
    [SerializeField] Text _waveIncomingText;
    [SerializeField] Text _waveIncomingSecondsText;

    [SerializeField] Text _restartGameText;

    [SerializeField] GameObject _PowerUp_Tripleshot;
    [SerializeField] GameObject _PowerUp_SpeedBoost;
    [SerializeField] GameObject _PowerUp_Shields;

    [SerializeField] Timer Timer_PowerUp_Tripleshot;
    [SerializeField] Timer Timer_PowerUp_Speed;
    [SerializeField] Timer Timer_PowerUp_Shields;

    [SerializeField] GameObject _optionsMenu;
    [SerializeField] GameObject _pausePanel;

    ///
    /// AMMO Variables
    ///
    [SerializeField] Slider _ammoSlider;
    [SerializeField] Text _ammoCount;
    //[SerializeField] Gradient _ammoBarGradient;
    //[SerializeField] Image _ammoBarFill;
    ///
    /// AMMO Variables - END
    ///

    ///
    /// MAIN THRUSTERS Variable
    ///
    [SerializeField] Slider _thrustersSlider;
    ///
    /// MAIN THRUSTERS Variable - END
    ///

    bool ShieldBonusActivated;
    WaitForSeconds BonusLifePause = new WaitForSeconds(.25f);

    string textToBlink;

    bool CheatKeyEnabled;

    public override void Init()
    {
        base.Init();
    }

    void Start()
    {
        _scoreText.text = "---";
        _gameOverText.gameObject.SetActive(false);
        _restartGameText.gameObject.SetActive(false);
    }

    public void UpdateScore(int playerScore)
    {
        if (_scoreText != null)
        {
            _scoreText.text = playerScore.ToString("#,#");
        }
    }


    public void UpdatePlayerLives(int livesRemaining)
    {
        _livesRemainingImage.sprite = _livesSprites[livesRemaining];
        //_livesRemainingText.text = "LIVES = " + livesRemaining;
        _playerLives.text = livesRemaining.ToString();

        if (livesRemaining == 0)
        {
            DisplayGameOver();
        }
    }

    public void UpdateShieldBonusUI(int shieldBonus)
    {
        if (shieldBonus > 0 && shieldBonus < 4)
        {
            _shieldBonus[shieldBonus - 1].SetActive(true);
            if (shieldBonus == 3)
            {
                ShieldBonusActivated = true;
                _bonusLife_text.gameObject.SetActive(true);
                StartCoroutine(BonusLifeMessage());
            }
        }
        else if (shieldBonus == 0)
        {
            ShieldBonusActivated = false;
            _bonusLife_text.gameObject.SetActive(false);
            foreach (var shield in _shieldBonus)
            {
                shield.SetActive(false);
            }
        }
        else if (shieldBonus > 3)
        {
            // TODO:
            // Display MAX SHIELD PROTECTION
            // Blink
            // Paladin "Max Shield Protection reached"
        }
    }

    public void DisplayLives(int _playerLivesRemaining)
    {
        _livesRemainingImage.sprite = _livesSprites[_playerLivesRemaining];
        _playerLives.text = _playerLivesRemaining.ToString();
    }

    public void DisplayEnemies(int _remainingEnemies, int _totalEnemies)
    {
        if (_enemiesRemaing != null)
        {
            _enemiesRemaing.text = _remainingEnemies + "/" + _totalEnemies;
        }
    }

    public void DisplayLevel(int _level)
    {
        _currentLevel.text = _level.ToString();
    }

    public void DisplayShipWrapStatus()
    {
        if (CheatKeyEnabled)
        {
            textToBlink = "Ship Wrap: Enabled";
            _shipWrap.text = textToBlink;
            StartCoroutine(Blink());
        }
        else
        {
            textToBlink = "Ship Wrap: Disabled";
            _shipWrap.text = textToBlink;
        }
    }

    IEnumerator Blink()
    {
        while (CheatKeyEnabled)
        {
            _shipWrap.text = textToBlink;
            yield return new WaitForSeconds(BlinkTime);
            _shipWrap.text = string.Empty;
            yield return new WaitForSeconds(BlinkTime);
        }

        _shipWrap.text = "Ship Wrap: Disabled";
    }

    public void SetCheatKey(bool status)
    {
        CheatKeyEnabled = status;
    }

    public void WaveCountdown(int Timer)
    {
        Timer++;
        _waveIncomingSecondsText.text = Timer.ToString();
    }

    public void WaveCountdownEnableUI(bool isEnabled, int countdownTime, string waveName)
    {
        _waveName.text = waveName;
        _waveName.gameObject.SetActive(isEnabled);
        _waveIncomingText.gameObject.SetActive(isEnabled);
        _waveIncomingSecondsText.gameObject.SetActive(isEnabled);
        _waveIncomingSecondsText.text = countdownTime.ToString();
    }

    public void ActiveTripleShotUI()
    {
        _PowerUp_Tripleshot.SetActive(true);
        Activate_Tripleshot(5);
    }

    public void ActiveSpeedBoostUI()
    {
        _PowerUp_SpeedBoost.SetActive(true);
        Activate_SpeedBoost(5);
    }

    public void ActiveShieldsUI()
    {
        _PowerUp_Shields.SetActive(true);
        Activate_Shields(5);
    }

    void Activate_Tripleshot(int duration)
    {
        Timer_PowerUp_Tripleshot
        .SetDuration(duration)
        .OnEnd(() => _PowerUp_Tripleshot.SetActive(false))
        .Begin();
        Timer_PowerUp_Tripleshot.enabled = false;
    }

    public void Activate_SpeedBoost(int duration)
    {
        Timer_PowerUp_Speed
        .SetDuration(duration)
        .OnEnd(() => _PowerUp_SpeedBoost.SetActive(false))
        .Begin();
    }

    public void Activate_Shields(int duration)
    {
        Timer_PowerUp_Shields
        .SetDuration(duration)
        .OnEnd(() => _PowerUp_Shields.SetActive(false))
        .Begin();
    }

    public void ActivatePowerUp()
    {
        /// Similar to Player PowerUp Case & PowerUp String
        /// Activating the PowerUp Counter should work the same
    }

    ///
    /// AMMO UI Functions
    ///
    /*
    public void SetMaxAmmo(int ammo)
    {
        _ammoSlider.maxValue = ammo;
        _ammoSlider.value = ammo;

        _ammoBarFill.color = _ammoBarGradient.Evaluate(1f);
    }
    public void SetAmmo(int ammo)
    {
        _ammoSlider.value = ammo;
        _ammoBarFill.color = _ammoBarGradient.Evaluate(_ammoSlider.normalizedValue);
    }*/
    public void SetMaxAmmo(int ammo) {
        _ammoSlider.maxValue = ammo;
    }
    public void SetAmmo(int ammo)
    {
        _ammoSlider.value = ammo;
        _ammoCount.text = ammo + "/" + _ammoSlider.maxValue;
    }
    ///
    /// AMMO UI Functions - END
    ///

    ///
    /// MAIN THRUSTERS UI Functions
    ///
    public void SetMaxThrusters(float thrusters)
    {
        _thrustersSlider.maxValue = thrusters;
        _thrustersSlider.value = thrusters;
    }
    public void SetThrusters(float thrusters)
    {
        _thrustersSlider.value = thrusters;
        //Debug.Log("Thrusters Level = " + thrusters);
    }
    ///
    /// MAIN THRUSTERS UI Functions - END
    ///

    public void GameOver(bool isGameOVer)
    {
        _gameOverText.gameObject.SetActive(isGameOVer);
        //_gameOverText.gameObject.SetActive(true);
        _restartGameText.gameObject.SetActive(isGameOVer);
        StartCoroutine(GameOverColorChange());
    }
    public void DisplayGameOver()
    {
        _gameOverText.gameObject.SetActive(true);
        _restartGameText.gameObject.SetActive(true);
        StartCoroutine(GameOverColorChange());
    }

    IEnumerator GameOverColorChange()
    {
        var gameoverText = _gameOverText.GetComponent<Text>();

        for (int i = 60; i >= 0; i--)
        {
            gameoverText.color = Color.red;
            if (i % 2 == 0)
            {
                gameoverText.color = Color.blue;
            }
            if (i % 3 == 0)
            {
                gameoverText.color = Color.white;
            }
            yield return new WaitForSeconds(.1f);
        }
        gameoverText.color = Color.red;
    }


    /////////////////////////////////////////////////////////////////
    /// Menu Methods
    /// 
    public void OptionsMenu()
    {
        _pausePanel.SetActive(false);
        _optionsMenu.SetActive(true);
    }

    public void OptionsBackButton()
    {
        _optionsMenu.SetActive(false);
        _pausePanel.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        //SceneManager.LoadScene("MainMenu"); // Scene 0 = 'MainMenu'
        //SceneManager.LoadSceneAsync(2);
        GameManager.instance.PauseGame();
        SceneManager.LoadScene(2);
    }

    IEnumerator BonusLifeMessage()
    {
        Color colorBlue;
        Color colorSilver;

        ColorUtility.TryParseHtmlString("#0d00ff", out colorBlue);
        ColorUtility.TryParseHtmlString("#a6a6a6", out colorSilver);

        while (ShieldBonusActivated)
        {
            _bonusLife_text.color = colorBlue;
            yield return BonusLifePause;
            _bonusLife_text.color = colorSilver;
            yield return BonusLifePause;
        }
    }
}
