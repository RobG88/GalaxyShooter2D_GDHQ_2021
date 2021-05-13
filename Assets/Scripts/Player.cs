using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    [SerializeField] int _playerLives = 3;
    [SerializeField] float _speed; // player/ship movement speed
    [SerializeField] float _spaceshipSpeed = 15.0f;  // player/ship BASE CONSTANT speed
                                                     // speed lost for damage

    int _score = 0;

    float horizontalInput, verticalInput;
    Vector3 direction;

    ///
    /// MAIN THRUSTER VARIABLES: Main Thruster, SHIFT enabled speed boost 
    ///
    [SerializeField] GameObject _mainThruster;
    [SerializeField] float _maxThrusters, _currentThruster;
    float _thrusterBurnRate = 1.5f;
    [SerializeField] bool _enableMainThruster;
    [SerializeField] bool _regeneratingThrusters;
    float _thrustersInitialRegenDelay = 2.0f; // wait 2.0f seconds before THRUSTERS begin to regenerate
    float _thrustersRegenTick = 0.1f;
    WaitForSeconds _thrustersRegenDelay;
    WaitForSeconds _thrustersRegenTickDelay;
    [SerializeField] float _regeneratingThrusterRate;
    ///
    /// MAIN THRUSTER VARIABLES - END
    ///
    float thruster_y; // Variable to amplify the ship's thrusters during forward/up movement
    float _thrustersAnimation; // Variable to amplify the thrusters animation 'flicker'
    [SerializeField] bool _thrusters_always_on; //TODO: Screen capturing ONLY --- REMOVE

    [SerializeField] GameObject _thruster_left;
    [SerializeField] GameObject _thruster_right;
    [SerializeField] GameObject _thrusters;

    Vector3 _originalThrustersLocalScale;
    Vector3 _leftThrusterOriginalPos;
    Vector3 _rightThrusterOriginalPos;
    Vector3 ThrusterOffset = new Vector3(0, -0.5f, 0);
    Vector3 _newLeft;
    Vector3 _newRight;
    ///
    /// THRUSTERS ALL VARIABLES - END
    ///

    [SerializeField] GameObject _shipDamageLeft;
    [SerializeField] GameObject _shipDamageRight;
    bool _damagedLeft = false;
    bool _damagedRight = false;
    Animator _animShipDamageLeft;
    Animator _animShipDamageRight;

    bool isGameOver = false;

    [SerializeField] float _fireRate = 0.15f;  // delay (in Seconds) how quickly the laser will fire
    float _nextFire = -1.0f;  // game time value, tracking when player/ship can fire next laser

    [SerializeField] GameObject _laserPrefab;
    [SerializeField] Transform _gunLeft, _gunRight, _gunCenter;

    Animator _anim;
    SpriteRenderer _spriteRenderer;
    BoxCollider2D _boxCollider2D;

    bool _wrapShip = false; // Q = toggle wrap
    float _xScreenClampRight = 18.75f;
    float _xScreenClampLeft = -18.75f;
    float _yScreenClampUpper = 0;
    float _yScreenClampLower = -7f; // offical game = -3.8f;


    ///////////////////////////////
    /// AUDIO VARIABLES
    /// 
    //[SerializeField] AudioClip _laserSFX;
    [SerializeField] AudioClip _PowerUpSFX;
    [SerializeField] AudioClip _explosionSFX;
    [SerializeField] AudioClip _explosionFinaleSFX;
    [SerializeField] GameObject _playerFinalExplosionFX;
    AudioSource _audioSource; // Audio source for laser, player damage & powerups

    ///
    /// AMMO (ENERGY) VARIABLES
    ///
    [SerializeField] int _maxAmmo, _currentAmmo;
    [SerializeField] AudioClip _out_of_ammo_sfx;
    ///
    /// AMMO (ENERGY) VARIABLES - END
    ///

    /// 
    /// POWERUP VARIABLES
    /// 
    /// TRIPLESHOT VARIABLES - BEGIN
    /// 
    [SerializeField] bool _powerUp_Tripleshot;
    [SerializeField] GameObject _tripleshotPrefab;
    /// 
    /// TRIPLESHOT VARIABLES - END
    /// 

    /// 
    /// SPEED VARIABLES
    /// 
    bool _speedActive = false;
    /// 
    /// SPEED VARIABLES - END
    /// 

    /// 
    /// SHIELD VARIABLES
    /// 
    [SerializeField] bool _shieldActive = false;
    [SerializeField] GameObject _shield;
    [SerializeField] int _shieldPower;
    [SerializeField] int _shieldBonus;
    Vector3 _shieldOriginalSize;
    PlayerShields _playerShield;
    /// 
    /// SHIELD VARIABLES - END
    /// 
    [SerializeField] GameObject _bonusLifeShield;
    [SerializeField] bool _bonusLife;
    [SerializeField] AudioClip _bonusLifeSFX;
    bool _bonusLifeOncePerLevel;
    ///
    /// REPAIR 'Health' VARIABLES
    ///
    private bool _repairBotsActive;
    [SerializeField] GameObject _repairBotsPE;
    ///
    /// REPAIR 'Health' VARIABLES - END
    ///
    ///
    /// FREEZE/EMP TORPEDO second fire VARIABLES
    ///
    [SerializeField] GameObject _freezeTorpedo;
    private bool _freezeTorpedoLoaded;
    [SerializeField] GameObject _freezeTorpedoSprite;
    ///
    /// FREEZE/EMP TORPEDO second fire VARIABLES - END
    ///

    /// ULTIMATE POWER-UP, NEGATIVE PICK-UP :: VARIABLES
    /// 
    bool _ultimate;
    [SerializeField] bool _ultimateReset;
    bool _ultimateSpinning;
    Quaternion _originalRotation;
    float _rotationSpriteSpeed;
    /// ULTIMATE POWER-UP, NEGATIVE PICK-UP :: VARIABLES END


    // CHEAT KEYS
    //
    // G = GOD mode
    bool _cheat_GODMODE = false;
    // T = TRIPLESHOT
    bool _cheat_TRIPLE = false;

    void Awake()
    {
        instance = this;
        _anim = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        _playerLives = 3;
        _bonusLifeOncePerLevel = false;
        isGameOver = false;
        transform.position = new Vector3(0, -5, 0); // offical game (0, -3.5f, 0);

        _originalThrustersLocalScale = _thruster_left.transform.localScale;
        ////transform.position = new Vector3(0.3834784f, -5, 0); // exactly fire two lasers into one enemy
        ///
        //
        // Initialize Player/Ship variables & set GUI
        // Ship speed
        // At the start of the game the ship has taken damage to both 
        // port & starboard sides so speed will reduced by Damage(), Damage()
        // Disable _repairBotsActive, shields, torpedo & laser cannons
        //
        ///
        /// REPAIR 'Health' VARIABLES INIT
        /// Repair Bots are a Particle Effect when Repairs Powerup picked up
        ///
        _repairBotsActive = false;
        ///
        /// REPAIR 'Health' VARIABLES INIT - END
        /// 

        _speed = _spaceshipSpeed; // initialize Ship/Player speed

        UIManager.instance.DisplayLives(_playerLives);
        UIManager.instance.DisplayShipWrapStatus();

        ///
        /// AMMO VARIABLES INITIALIZE
        ///
        // Laser Cannon Ammo
        _currentAmmo = 15;
        _maxAmmo = 15;
        UIManager.instance.SetMaxAmmo(_maxAmmo);
        UIManager.instance.SetAmmo(_currentAmmo);
        ///
        /// AMMO VARIABLES INITIALIZE - END
        ///

        /// Thrusters Left & Right
        /// Thrusters Damage
        _leftThrusterOriginalPos = _thruster_left.transform.localPosition;
        _rightThrusterOriginalPos = _thruster_right.transform.localPosition;
        _newLeft = _leftThrusterOriginalPos + ThrusterOffset;
        _newRight = _rightThrusterOriginalPos + ThrusterOffset;
        _animShipDamageLeft = _shipDamageLeft.GetComponent<Animator>();
        _animShipDamageRight = _shipDamageRight.GetComponent<Animator>();
        ///
        /// THRUSTERS VARIABLES INITIALIZE
        ///
        _maxThrusters = 10.0f;
        _currentThruster = 10.0f; // TODO: disable for beginning of game
        _thrustersRegenDelay = new WaitForSeconds(_thrustersInitialRegenDelay);
        _thrustersRegenTickDelay = new WaitForSeconds(_thrustersRegenTick);
        //UIManager.instance.SetMaxThrusters(_maxThrusters);
        //UIManager.instance.SetThrusters(_currentThrusters);
        ///
        /// THRUSTERS VARIABLES INIT - END
        ///

        ///
        /// SHIELDS VARIABLES INITIALIZE
        ///
        //_shieldOriginalSize = _shield.transform.localScale;
        ///
        /// SHIELDS VARIABLES INITIALIZE - END
        ///
    }

    IEnumerator RepeatLerp(Vector3 a, Vector3 b, float time)
    {

        yield return null;
    }

    void Update()
    {
        if (GameManager.PlayerIsAlive)
        {
            if (_ultimateReset)
            {
                float rotationSpeed = 10f;
                transform.rotation = Quaternion.Slerp(transform.rotation, _originalRotation, rotationSpeed * Time.deltaTime);

                if (transform.rotation == _originalRotation)
                {
                    _ultimateReset = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.Space) && Time.time > _nextFire && Ammo() && !_freezeTorpedoLoaded)
            //if (Input.GetKeyDown(KeyCode.Space) && Time.time > _nextFire)
            {
                FireLaser();
            }
            ///
            /// FREEZE/EMP TORPEDO second fire User Input
            ///
            /// TORPEDO
            else if (Input.GetKeyDown(KeyCode.Space) && Time.time > _nextFire && _freezeTorpedoLoaded)
            {
                FireFreezeTorpedo();
            }
            ///
            /// FREEZE/EMP TORPEDO second fire User Input
            ///

            ///
            /// AMMO - Out of Ammo SFX
            /// 
            else if (Input.GetKeyDown(KeyCode.Space) && !Ammo() && !_freezeTorpedoLoaded)
            {
                _audioSource.clip = _out_of_ammo_sfx;
                _audioSource.PlayOneShot(_audioSource.clip);
            }
            ///
            /// AMMO - Out of Ammo SFX - END
            /// 

            // Cheat Keys:
            // 
            // Q = Enable ship wrapping left/right
            // Below is for testing purposes only
            //
            // G = Enable GOD mode
            //
            //if (Input.GetKeyDown(KeyCode.Q)) { _wrapShip = !_wrapShip; }
            if (Input.GetKeyDown(KeyCode.Q)) { _wrapShip = !_wrapShip; UIManager.instance.SetCheatKey(_wrapShip); UIManager.instance.DisplayShipWrapStatus(); }
            if (Input.GetKeyDown(KeyCode.G)) { _cheat_GODMODE = !_cheat_GODMODE; }
            if (Input.GetKeyDown(KeyCode.T)) { _cheat_TRIPLE = !_cheat_TRIPLE; _powerUp_Tripleshot = _cheat_TRIPLE; }

            if (Input.GetKeyDown(KeyCode.X) && _shieldActive) { DropShield(); }
            ///
            /// MAIN THRUSTER
            /// Regenerate, Burn & Disable
            if (RegenThruster() && !_enableMainThruster)
            {
                StartCoroutine(RegeneratorThruster());
            }
            if (Input.GetKey(KeyCode.LeftShift) && Thruster())
            {
                EnableMainThruster();
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                DisableMainThruster();
            }
            ///
            ///  MAIN THRUSTER - END
            ///

            CalculateMovement();
        }
    }

    void CalculateMovement()
    {
        _speed = CalculateShipSpeed();

        //UIManager.instance.CurrentSpeed(_speed); // TODO: Remove for Medium article on Speed/Thrusters

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // Animate Ship Tilting/Banking
        _anim.SetFloat("Tilt", horizontalInput); // * 2.0f);

        if (_ultimate) // Out of Control horizontalInput verticalInput
        {
            direction = new Vector3(verticalInput, horizontalInput, 0);
            //direction = new Vector3((verticalInput * Random.Range(-1, 2)), (horizontalInput * Random.Range(-1, 2)), 0);
            //transform.Translate((direction * Random.Range(-1, 2)) * (_speed * Random.Range(-1f, 3f)) * Time.deltaTime);
            transform.Translate(direction * (_speed * Random.Range(-1f, 3f)) * Time.deltaTime);
            transform.Rotate(0f, 0f, _rotationSpriteSpeed * Time.deltaTime);
        }
        else
        {
            //direction = new Vector3(horizontalInput, verticalInput, 0);
            //transform.Translate(direction * _speed * Time.deltaTime);
            direction = new Vector3(horizontalInput, verticalInput, 0);
            transform.Translate(direction * _speed * Time.deltaTime);
        }





        /////////////////////////////////////////////////////////////////////////////////
        /// Thrusters Left & Right (not MAIN THRUSTER)
        // Use the verticalInput 'W' or UpArrow * 1.75 as Thruster localScale multiplier

        // TODO: For Scene capturing/recording only delete, along with _thrusters_always_on bool
        /*
        if (_thrusters_always_on)
            thruster_y = 1 * .6f;
        else
            thruster_y = verticalInput * .6f; */

        thruster_y = verticalInput * .6f;

        //if (_thrusters_always_on || verticalInput > 0.20f)
        if (verticalInput > 0.20f)
        {
            // Reset Thrusters/Afterburners to originalLocalScale
            _thruster_left.transform.localScale = _originalThrustersLocalScale;
            _thruster_right.transform.localScale = _originalThrustersLocalScale;

            Vector3 thrusters = new Vector3(_thruster_left.transform.localScale.x,
                                    thruster_y,
                                    _thruster_left.transform.localScale.z);

            _thrustersAnimation = Random.Range(1.25f, 1.50f);
            _thruster_left.transform.localScale = thrusters * _thrustersAnimation;
            _thruster_right.transform.localScale = thrusters * _thrustersAnimation;
        }
        else if (verticalInput < 0.20f) // if the verticalInput < .20 then 'flicker' thrusters
        {
            _thrustersAnimation = Random.Range(1.25f, 1.50f);
            _thruster_left.transform.localScale = _originalThrustersLocalScale * _thrustersAnimation;
            _thruster_right.transform.localScale = _originalThrustersLocalScale * _thrustersAnimation;
        }

        // Player Boundaries 
        // Clamp Ship's Y pos
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, _yScreenClampLower, _yScreenClampUpper), 0);

        // Clamp xPos
        if (_wrapShip && transform.position.x > _xScreenClampRight) // Wrap ship
        {
            transform.position = new Vector3(_xScreenClampLeft, transform.position.y, 0);
        }
        else if (!_wrapShip && transform.position.x > _xScreenClampRight) // Lock pos
        {
            transform.position = new Vector3(_xScreenClampRight, transform.position.y, 0);
        }

        // or Wrap Ship's X pos
        if (_wrapShip && transform.position.x < _xScreenClampLeft) // Wrap ship
        {
            transform.position = new Vector3(_xScreenClampRight, transform.position.y, 0);
        }
        else if (!_wrapShip && transform.position.x < _xScreenClampLeft) // Lock pos 
        {
            transform.position = new Vector3(_xScreenClampLeft, transform.position.y, 0);
        }
    }

    void FireLaser()
    {
        _nextFire = Time.time + _fireRate; // delay (in Seconds) how quickly the laser will fire

        ///
        /// AMMO TRACKING & UI UPDATE
        ///
        _currentAmmo--;
        UIManager.instance.SetAmmo(_currentAmmo);
        ///
        /// AMMO TRACKING & UI UPDATE - END
        ///

        if (!_powerUp_Tripleshot)
        {
            GameObject laser1 = Instantiate(_laserPrefab, _gunLeft.position, Quaternion.identity);
            GameObject laser2 = Instantiate(_laserPrefab, _gunRight.position, Quaternion.identity);
            //_audioSource.pitch = Random.Range(2.5f, 3.0f);
        }
        else
        {
            GameObject tripleshot = Instantiate(_tripleshotPrefab, _gunCenter.position, Quaternion.identity);
            //_audioSource.pitch = Random.Range(0.85f, 1.15f);
        }

        //_audioSource.PlayOneShot(_laserSFX, 0.50f);
    }

    public void FireLaserCanon()
    {
        _anim.SetBool("fire", false);
        FireLaser();
    }

    ///
    /// AMMO BOOL FUNCTION
    ///
    private bool Ammo() // True, if ship has Laser Cannon Energy
    {
        return (_currentAmmo > 0);
    }
    ///
    /// AMMO BOOL FUNCTION
    ///

    ///
    /// FREEZE/EMP TORPEDO second fire User Input
    ///
    private void FireFreezeTorpedo()
    {
        // Remove 'Torpedo' Sprite once player launches
        _freezeTorpedoLoaded = false;
        _freezeTorpedoSprite.SetActive(false);
        var _torpedoLaunch = new Vector3(0, 1.85f, 0);
        Instantiate(_freezeTorpedo, _freezeTorpedoSprite.transform.position, Quaternion.identity);
    }
    ///
    /// FREEZE/EMP TORPEDO second fire User Input - END
    ///

    ///
    /// SHIELDS - SHIP DAMAGE ROUTINE
    /// 
    public void Damage() // Ship & Shield damage
    {
        if (_cheat_GODMODE) return;

        if (_shieldBonus == 3 && !_bonusLifeOncePerLevel) // Enable 3x Shield Bonus 'hit'
        {
            _bonusLife = true;
            _shieldBonus = 0;
            _bonusLifeOncePerLevel = true;
        }

        if (_shieldActive)
        {
            _playerShield.Damage();
        }
        else
        {
            if (_bonusLife)
            {
                //Debug.Log("Bonus Life used");
                //UIManager.Instance.UpdateShieldBonusUI(_shieldBonus);
                _bonusLife = false;
                _bonusLifeShield.SetActive(false);
                _audioSource.PlayOneShot(_bonusLifeSFX, 5f);
                UIManager.instance.UpdateShieldBonusUI(_shieldBonus);
            }
            else
            {
                _playerLives--;
                if (_playerLives < 0) _playerLives = 0;
                UIManager.instance.DisplayLives(_playerLives);
                _bonusLifeOncePerLevel = true;
                _shieldBonus = 0;
                //UIManager.Instance.UpdateShieldBonusUI(_shieldBonus);

                if (_playerLives == 0)
                {
                    //isGameOver = true;  Set in PlayerDeath() 
                    //PlayerDeathSequence();
                    ///
                    /// CAMERA SHAKE done via CINEMACHINE
                    /// 
                    CinemachineShake.Instance.ShakeCamera(16f, 4f);
                    PlayerDeathSequence();
                    return;
                }
                ///
                /// CAMERA SHAKE done via CINEMACHINE
                /// 
                CinemachineShake.Instance.ShakeCamera(5f, 1f);
                SpaceshipDamaged();
            }
        }
    }
    ///
    /// SHIELDS - SHIP DAMAGE ROUTINE - END
    /// 

    void SpaceshipDamaged() // if player ship is hit, damage port or starboard
    {
        if (!_damagedLeft && !_damagedRight)
        {
            int RND_Damage = Random.Range(0, 2);
            if (RND_Damage == 0)
            {
                SpaceshipDamagedLeft();
            }
            else if (RND_Damage == 1)
            {
                SpaceshipDamagedRight();
            }
        }
        else if (_damagedLeft && !_damagedRight)
        {
            SpaceshipDamagedRight();
        }
        else if (!_damagedLeft && _damagedRight)
        {
            SpaceshipDamagedLeft();
        }
        //_sound.clip = _explosionSFX;
        //_sound.PlayOneShot(_sound.clip);
    }

    void SpaceshipDamagedLeft() // ship port side damage
    {
        _damagedLeft = true;
        _shipDamageLeft.SetActive(true);
        _audioSource.PlayOneShot(_explosionSFX, 5f);
        //_animShipDamageLeft.SetTrigger("PlayerDamageLeft");
    }

    void SpaceshipDamagedRight() // ship starboard side damage
    {
        _damagedRight = true;
        _shipDamageRight.SetActive(true);
        _audioSource.PlayOneShot(_explosionSFX, 5f);
        //_animShipDamageRight.SetTrigger("PlayerDamageRight");
    }


    void PlayerDeathSequence()
    {
        _spriteRenderer.enabled = false;
        _boxCollider2D.enabled = false;

        //_audioSource.PlayOneShot(_explosionSFX);
        _playerFinalExplosionFX.SetActive(true);
        _audioSource.PlayOneShot(_explosionFinaleSFX, 4f);
        ///
        /// THRUSTER - DISABLE PLAYER DEATH
        /// 
        _mainThruster.SetActive(false);
        ///
        /// THRUSTER - DISABLE PLAYER DEATH - END
        ///
        _thruster_left.SetActive(false);
        _thruster_right.SetActive(false);
        _shipDamageLeft.SetActive(false);
        _shipDamageRight.SetActive(false);
        _freezeTorpedoSprite.SetActive(false);
        GameManager.instance.OnPlayerDeath();
        isGameOver = true;
        UIManager.instance.GameOver(isGameOver);
        WaveSpawner.instance.OnPlayerDeath();
        Destroy(this.gameObject, 5f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            bool FrozenEnemy = other.GetComponent<Enemy>().Frozen();
            if (!FrozenEnemy)
            {
                Damage();
            }
        }

        if (other.CompareTag("EnemyLaser")) Damage();
        if (other.CompareTag("Rocket")) Damage();

        if (other.CompareTag("PowerUp"))
        {
            if (_shieldActive || _ultimateSpinning) return;

            string PowerUpToActivate;
            PowerUpToActivate = other.transform.GetComponent<PowerUp>().PowerType().ToString();

            // TODO: Need to Handle Multiple PowerUps?

            ActivatePowerUp(PowerUpToActivate);
        }
    }

    public void Activate_PowerUp_Tripleshot()
    {
        StartCoroutine(ActivatePowerupTripleshot());
    }

    IEnumerator ActivatePowerupTripleshot()
    {
        UIManager.instance.ActiveTripleShotUI();
        _powerUp_Tripleshot = true;
        yield return new WaitForSeconds(5f);
        _powerUp_Tripleshot = false;
    }

    public void Activate_PowerUp_SpeedBoost()
    {
        StartCoroutine(ActivatePowerupSpeedBoost());
    }

    IEnumerator ActivatePowerupSpeedBoost()
    {
        UIManager.instance.ActiveSpeedBoostUI();
        _speedActive = true;
        yield return new WaitForSeconds(5f);
        _speedActive = false;
    }

    public void Activate_PowerUp_Shields()
    {
        _shieldActive = true;
        _shield.SetActive(_shieldActive);
        _playerShield = gameObject.GetComponentInChildren<PlayerShields>();
    }

    public void ShieldsDestroyed()
    {
        _shieldActive = false;
        _shield.SetActive(_shieldActive);
    }

    IEnumerator ActivatePowerupShields()
    {
        UIManager.instance.ActiveShieldsUI();
        _shieldActive = true;
        yield return new WaitForSeconds(5f);
        _shieldActive = false;
        _shield.SetActive(false);
    }

    IEnumerator ActivePowerUp(int timer)
    {
        yield return new WaitForSeconds(timer);
    }

    void ActivatePowerUp(string _powerUpType) // PowerUp activations
    {
        //_timesUpText.text = _powerUpType;

        switch (_powerUpType)
        {
            case "TripleShot":
                LaserCannonsRefill(15);
                Activate_PowerUp_Tripleshot();
                /*
                _tripleShotActive = true;

                //_timesUpText.text = _powerUpType;
                // Enable PowerUpCountDownBar
                _powerUpCountDownBar.SetActive(true);
                StartCoroutine(PowerUpCoolDownRoutine(_tripleShotCoolDown));
                */
                break;
            case "Speed":
                Activate_PowerUp_SpeedBoost();
                break;
            /// 
            /// SHIELD POWERUP
            /// 
            case "Shield":
                Activate_PowerUp_Shields();
                break;
            /// 
            /// SHIELD POWERUP - END
            /// 


            ///
            /// REPAIR 'Health' POWERUP
            /// 
            case "Repair":
                RepairShip();
                break;
            ///
            /// REPAIR 'Health' POWERUP - END
            /// 

            ///
            /// FREEZE/EMP TORPEDO POWERUP
            /// 
            case "FreezeTorpedo":
                _freezeTorpedoLoaded = true;
                _freezeTorpedoSprite.SetActive(true);
                break;
            ///
            /// FREEZE/EMP TORPEDO POWERUP - END
            ///

            case "EnergyCell":
                LaserCannonsRefill(10);
                break;

            case "Ultimate":
                //Debug.Log("PowerUp Pickup = " + _powerUpType);
                _originalRotation = transform.rotation;
                _rotationSpriteSpeed = Random.Range(-360, 360);
                _ultimate = true;
                _ultimateSpinning = true;
                //float _UltimateTimer = Time.time + Random.Range(2f, 4f);
                float _UltimateTimer = Time.time + Random.Range(9f, 15f);
                DropShield();
                if (_playerLives > 1) Damage();
                if (_playerLives > 1) Damage();
                StartCoroutine(UltimateCoolDown(_UltimateTimer));
                break;
        }
        _audioSource.pitch = 1.0f;
        //_audioSource.PlayOneShot(_PowerUpSFX);
    }


    ///
    /// AMMO FILL FUNCTION
    ///

    private void LaserCannonsRefill(int ammo)
    {
        _currentAmmo = _currentAmmo + ammo;
        if (_currentAmmo > 15)
            _currentAmmo = 15;
        UIManager.instance.SetAmmo(_currentAmmo);
    }
    ///
    /// AMMO FILL FUNCTION - END
    ///



    /// 
    /// CalculateShipSpeed
    /// 
    /// Speed will be reduced on damage
    /// This calculated base can also be
    /// modified by Speed Boost power-up
    /// and Afterburner thruster
    /// 
    float CalculateShipSpeed()
    {
        var _newSpeed = _spaceshipSpeed;

        if (_playerLives == 2)
        {
            _newSpeed = _spaceshipSpeed - 2;
        }

        if (_playerLives == 1)
        {
            _newSpeed = _spaceshipSpeed - 4;
        }

        if (_speedActive) // PowerUp = speed * 175%
        {
            _newSpeed = _spaceshipSpeed * 1.75f;
        }
        ///
        /// THRUSTERS - SPEED CALC
        /// 
        if (_enableMainThruster) // Thrusters = speed * 250%
        {
            _newSpeed = _spaceshipSpeed * 2.50f;
        }
        ///
        /// THRUSTERS - SPEED CALC - END
        /// 
        if (_ultimate) _newSpeed = 3f;
        return _newSpeed;
    }

    ///
    /// MAIN THRUSTER ROUTINES
    ///
    bool Thruster() // True, if ship has Main Thruster power
    {
        return (_currentThruster > 0);
    }
    bool RegenThruster() // True, if ship's Main Thurster power < Max Thrusters
    {
        return (_currentThruster < _maxThrusters);
    }
    void EnableMainThruster()
    {
        _regeneratingThrusters = false;
        _enableMainThruster = true;
        _mainThruster.SetActive(_enableMainThruster);

        Vector3 _mainThrusterDelta = new Vector3(_thruster_left.transform.localScale.x,
                        -0.55f,
                        _thruster_left.transform.localScale.z);

        float _afterburnerAnimation = Random.Range(1.25f, 1.50f);

        _mainThruster.transform.localScale = _mainThrusterDelta * _afterburnerAnimation;

        if (_currentThruster >= 10)
        {
            _regeneratingThrusterRate = 10000f;
        }

        if (_currentThruster > 0)
        {
            _currentThruster -= _thrusterBurnRate * Time.deltaTime;

            if (_currentThruster <= 0)
            {
                _currentThruster = 0;
                _enableMainThruster = false;
                _regeneratingThrusterRate = 100000f;
                _mainThruster.SetActive(_enableMainThruster);
                _speed = CalculateShipSpeed();
            }

            UIManager.instance.SetThrusters(_currentThruster);
        }
    }

    void DisableMainThruster()
    {
        _enableMainThruster = false;
        _mainThruster.SetActive(_enableMainThruster);
    }

    IEnumerator RegeneratorThruster()
    {
        _regeneratingThrusters = true;

        yield return _thrustersRegenDelay; // cahced WaitForSeconds(_thrustersInitialRegenDelay)

        while (_currentThruster < _maxThrusters && _regeneratingThrusters)
        {
            //_currentThruster += _maxThrusters / 100000;
            _currentThruster += _maxThrusters / _regeneratingThrusterRate;
            UIManager.instance.SetThrusters(_currentThruster);
            yield return _thrustersRegenTickDelay;
        }

        _regeneratingThrusters = false;
    }
    ///
    /// MAIN THRUSTER ROUTINES - END
    ///


    public void AddScore(int scoreAmount) // Update score, send to UI
    {
        _score += scoreAmount;
        UIManager.instance.UpdateScore(_score);
    }

    void DropShield()
    {
        _shieldActive = false;
        _shield.SetActive(false);
    }

    public void PlayExplosion()
    {
        _audioSource.PlayOneShot(_explosionSFX);
    }

    ///
    /// REPAIR 'Health' Functions
    /// 
    private void RepairShip()
    {
        if (_playerLives < 3)
        {
            _playerLives++;
            UIManager.instance.UpdatePlayerLives(_playerLives);

            if (_damagedLeft && _damagedRight)
            {
                int RND_Damage = Random.Range(0, 2);
                if (RND_Damage == 0)
                {
                    SpaceshipRepairLeft();
                }
                else if (RND_Damage == 1)
                {
                    SpaceshipRepairRight();
                }
            }
            else if (_damagedLeft && !_damagedRight)
            {
                SpaceshipRepairLeft();
            }
            else if (!_damagedLeft && _damagedRight)
            {
                SpaceshipRepairRight();
            }
            RepairBotsDeployed();
        }
        else
        {
            if (!_bonusLifeOncePerLevel)
            {
                ///
                /// REPAIR POWERUP when No Shields
                ///
                _shieldBonus++;
                UIManager.instance.UpdateShieldBonusUI(_shieldBonus);
                if (_shieldBonus == 3)
                {
                    _bonusLifeShield.SetActive(true);
                }
                ///
                /// REPAIR POWERUP when No Shields - END
                ///
            }
        }
    }

    private void RepairBotsDeployed()
    {
        _repairBotsActive = !_repairBotsActive;
        _repairBotsPE.SetActive(_repairBotsActive);
        _repairBotsActive = !_repairBotsActive;
    }

    private void RepairBotsEnabled()
    {
        // Paladin announcement 'RepairBots restored'
        // Play RepairBots burst
        _repairBotsActive = true;
        RepairBotsDeployed();
    }

    private void SpaceshipRepairLeft() // ship port side damage
    {
        _damagedLeft = false;
        _shipDamageLeft.SetActive(false);
    }

    private void SpaceshipRepairRight() // ship starboard side damage
    {
        _damagedRight = false;
        _shipDamageRight.SetActive(false);
    }
    ///
    /// REPAIR 'Health' Functions - END
    /// 

    IEnumerator UltimateCoolDown(float timer)
    {
        while (Time.time < timer)
        {
            yield return new WaitForEndOfFrame();
        }
        _ultimate = false;
        _ultimateReset = true;
        _ultimateSpinning = false;
        LaserCannonsRefill(15);
        Activate_PowerUp_Tripleshot();
        RepairShip();
        RepairShip();
        Activate_PowerUp_Shields();
    }
}