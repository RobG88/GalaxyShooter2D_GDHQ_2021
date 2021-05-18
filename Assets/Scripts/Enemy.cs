using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    ////////////////////////////////
    /// Enemy (Basic)
    /// 
    /// Enemy gameObject travels DOWN @ _speed
    /// 
    /// Enemy triggers/collides with object &
    /// is destroyed or is re-deployed when
    /// position.y > _yThreshold (8)
    /// 
    /// Audio: pitch is random & hit for Triple Shot
    /// 

    [SerializeField] float _speed = 4.0f;
    float _tempSpeed;
    [SerializeField] GameObject _enemyLaserPrefab;

    [SerializeField] Camera cam;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Transform firePoint;
    [SerializeField] Vector2 mousePositionToWorld;

    [SerializeField] Transform player;
    [SerializeField] Transform _target;
    public Transform target { get; private set; }
    [SerializeField] float rangeOfWeapon;
    [SerializeField] float rotationSpeed = 3f;
    Vector3 laserMissVector3;


    [SerializeField] int _scoreValue = 0;
    //[SerializeField] GameObject _enemyInvaderExplosion;
    [SerializeField] GameObject _collisionWithShieldExplosionFX;
    [SerializeField] bool CHEAT_LINE_THEM_UP = false;

    [SerializeField] GameObject _cloakingObject;

    Animator _anim;
    AudioSource _audioSource;
    BoxCollider2D _collider2D;
    SpriteRenderer _spriteRenderer;

    /// <summary>
    /// Enemy Laser Attack
    //[SerializeField] float _enemyLaser = 6.0f;  // speed of enemy's laser
    float _fireRate = 3.0f;
    [SerializeField] float _canFire = -1.0f;
    float _upperFireRange = 11.5f;
    float _lowerFireRange = -4.0f;
    /// </summary>

    bool _isDestroyed = false; // if enemy is hit by player/ship Laser then isDestroyed = true, enemy is put back into pool

    float _enemyReSpawnThreshold = -10.0f; // Game Screen threshold, once enemy is beyond this point and has been destroyed it will respawn 
    Vector3 _enemyPos = new Vector3(0, 0, 0); // Random position of enemy once re-spawned X(-8,8) Y(12,9)

    float _respawnXmin = -9.0f;
    float _respawnXmax = 9.0f;
    float _respawnYmin = 14.0f;
    float _respawnYmax = 19.0f;

    ///
    /// FREEZE/EMP TORPEDO Secondary Fire Variable
    ///
    bool isFrozen = false;
    ///
    /// FREEZE/EMP TORPEDO Secondary Fire Variable - END
    ///

    /// 
    /// SHIELD VARIABLES
    /// 
    [SerializeField] bool _shieldActive = false;
    [SerializeField] GameObject _shield;
    [SerializeField] int _shieldPower;
    EnemyShield _enemyShield;
    /// 
    /// SHIELD VARIABLES - END
    /// 

    [SerializeField] bool _canCloak;
    [SerializeField] bool _isAggressive;
    [SerializeField] bool _canDestroyPowerUps;
    [SerializeField] bool _hasLongRangeCannon;

    float _playerYThreshold = -9f; // set game 'Player' Y Threshold
    [SerializeField] bool _enemySmartBackFire;
    [SerializeField] GameObject _enemySmartFOV;
    [SerializeField] GameObject _enemyRocket;

    void Awake()
    {
        _anim = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _collider2D = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _canCloak = (Random.value > 0.5f);
        _canCloak = false;
        _cloakingObject.SetActive(_canCloak);

        _isAggressive = (Random.value > 0.5f);
        _isAggressive = false;
        _cloakingObject.SetActive(_isAggressive);

        _canDestroyPowerUps = (Random.value > 0.5f);
        _canDestroyPowerUps = false;
        _cloakingObject.SetActive(_canDestroyPowerUps);

        _hasLongRangeCannon = (Random.value > 0.5f);
        _hasLongRangeCannon = false;
        _cloakingObject.SetActive(_hasLongRangeCannon);

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Start()
    {
        if (!CHEAT_LINE_THEM_UP)
            Spawn();

        _fireRate = Random.Range(.5f, 2f);
        _canFire = Time.time + _fireRate;

        DisableLaser();
    }

    void Update()
    {
        if (_enemySmartBackFire && transform.position.y <= _playerYThreshold)
        {
            PlayerInSights(true);
        }
        else if (_enemySmartBackFire && transform.position.y > _playerYThreshold)
        {
            PlayerInSights(false);
        }

        if (Input.GetKeyDown(KeyCode.U)) { Activate_Shields(); }

        CalculateMovement();

        ///
        /// FREEZE/EMP TORPEDO - If enemy is frozen, can shoot, _CanFire is reset upon Thaw
        /// 
        if (Time.time > _canFire && WithinFiringRange() && !isFrozen)
        {
            _fireRate = Random.Range(2f, 4f);

            _canFire = Time.time + _fireRate;

            if (_hasLongRangeCannon)
            {
                //TargetPlayer(); // Only for RAMMING
                StartCoroutine(Shoot());
            }
            else
            {
                GameObject EnemyLaserShot = Instantiate(_enemyLaserPrefab, transform.position, Quaternion.identity);
            }
        }

        /*
        if (Input.GetButtonDown("Fire1"))
        {
            if (target != null)
                EnableLaser();
        }

        if (Input.GetButton("Fire1") && target != null)
        {
            UpdateLaser();
        }

        if (Input.GetButtonUp("Fire1"))
        {
            if (target != null)
                DisableLaser();
        }
        */
    }

    bool WithinFiringRange()
    {
        if (transform.position.y > _upperFireRange || transform.position.y < _lowerFireRange)
            return false;
        return true;
    }

    private void CalculateMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < _enemyReSpawnThreshold && !_isDestroyed)
        {
            transform.position = SpawnEnmeyAtRandomLocation();
        }
    }

    Vector3 SpawnEnmeyAtRandomLocation()
    {
        return (_enemyPos = new Vector3(Random.Range(_respawnXmin, _respawnXmax), Random.Range(_respawnYmin, _respawnYmax), 0));
    }

    void Spawn()
    {
        float respawnX = Random.Range(_respawnXmin, _respawnXmax);

        _enemyPos.x = respawnX;

        if (CHEAT_LINE_THEM_UP)
        {
            _enemyPos.x = 0;  // testing, line enemy for two lasers intersecting collider}
        }

        _enemyPos.y = Random.Range(_respawnYmin, _respawnYmax);

        transform.position = _enemyPos;
    }

    bool CheckSpawnPosition(Vector3 pos)
    {
        return (Physics.CheckSphere(pos, 0.8f));
    }

    void EnableLaser()
    {
        lineRenderer.enabled = true;
    }

    void UpdateLaser()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, -firePoint.up);

        if (hitInfo)
        {
            //Debug.Log("Enemy Laser hit = " + hitInfo.transform.tag);
        }
        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, target.position);
    }

    void DisableLaser()
    {
        lineRenderer.enabled = false;
    }


    public void DestroyPowerUp(GameObject powerUpPosition)
    {
        StartCoroutine(ShootPowerUp(powerUpPosition.transform));
        powerUpPosition.GetComponent<PowerUp>().DestroyPowerUp();
    }

    IEnumerator ShootPowerUp(Transform _powerUpTarget) // WHY??? is Raycast being used?? Is it???
    {
        lineRenderer.SetPosition(0, firePoint.position);

        lineRenderer.SetPosition(1, _powerUpTarget.position);

        lineRenderer.enabled = true;

        yield return new WaitForSeconds(0.2f);

        lineRenderer.enabled = false;
    }

    public void RamPlayer()
    {
        _speed = 12f;
        // Enable Thrusters

        // If the enemy ship re-spawns then reset the speed because 
        // the enemy ship has MISSed the PLAYER ship
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser") && _shieldActive) { _enemyShield.Damage(); return; }

        if (!_canCloak)
        {
            if (other.CompareTag("Torpedo") || other.CompareTag("Laser") || other.CompareTag("Player") || other.CompareTag("Shield"))
            //if (other.CompareTag("Player") || other.CompareTag("Shield"))
            {
                _speed = 0;

                _collider2D.enabled = false; // disable collider so two lasers can not collider at the same time

                if (other.CompareTag("Shield"))
                {
                    //Debug.Log("ENERGY EXPLOSION");
                    _collisionWithShieldExplosionFX.SetActive(true);
                    _spriteRenderer.enabled = false;
                    /// Need to replace with ZAPPER
                    _audioSource.Play();
                    UpdateEnemyDeath();
                }
                else
                {
                    //Debug.Log("Enemy Regular explosion");
                    //Debug.Log("Enemy destoryed by: " + other.tag);
                    //Instantiate(_enemyInvaderExplosion, transform.position, Quaternion.identity);
                    _anim.SetTrigger("OnEnemyDeath");
                    _audioSource.Play();
                }
            }
        }
        else
        {
            if (other.CompareTag("Player") || other.CompareTag("Shield"))
            {
                _speed = 0;

                _collider2D.enabled = false; // disable collider so two lasers can not collider at the same time

                if (other.CompareTag("Shield"))
                {
                    //Debug.Log("ENERGY EXPLOSION");
                    _collisionWithShieldExplosionFX.SetActive(true);
                    _spriteRenderer.enabled = false;
                    /// Need to replace with ZAPPER
                    _audioSource.Play();
                    UpdateEnemyDeath();
                }
                else
                {
                    //Debug.Log("Enemy Regular explosion");
                    //Debug.Log("Enemy destoryed by: " + other.tag);
                    //Instantiate(_enemyInvaderExplosion, transform.position, Quaternion.identity);
                    _anim.SetTrigger("OnEnemyDeath");
                    _audioSource.Play();
                }
            }
        }

    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Enemy COLLIDED with :: " + other.gameObject.tag);
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Shield"))
        {
            _speed = 0;

            _collider2D.enabled = false; // disable collider so two lasers can not collider at the same time

            if (other.gameObject.CompareTag("Shield"))
            {
                //Debug.Log("ENERGY EXPLOSION");
                _collisionWithShieldExplosionFX.SetActive(true);
                _spriteRenderer.enabled = false;
                /// Need to replace with ZAPPER
                _audioSource.Play();
                UpdateEnemyDeath();
            }
            else
            {
                //Debug.Log("Enemy Regular explosion");
                //Debug.Log("Enemy destoryed by: " + other.tag);
                //Instantiate(_enemyInvaderExplosion, transform.position, Quaternion.identity);
                _anim.SetTrigger("OnEnemyDeath");
                _audioSource.Play();
            }
        }
    }

    void OnDestroy()
    {
        Destroy(this.gameObject);
    }

    void UpdateEnemyDeath()
    {
        Player.instance.AddScore(_scoreValue);
        WaveSpawner.instance.EnemyDeath();
        Destroy(this.gameObject, .5f);
    }

    ///
    /// FREEZE/EMP TORPEDO Secondary Fire Functions
    ///
    public void FreezeEnemyShip(float speed)
    {
        _tempSpeed = _speed;
        _speed = speed;
        isFrozen = true;
    }

    public void ThawedEnemyShip()
    {
        _speed = _tempSpeed;
        isFrozen = false;
        _canFire = Time.time + _fireRate; // reset CanFire time after thaw
    }

    public bool Frozen() { return isFrozen; }
    ///
    /// FREEZE/EMP TORPEDO Secondary Fire Functions - END
    ///

    public void Activate_Shields()
    {
        _shieldActive = true;
        _shield.SetActive(_shieldActive);
        _enemyShield = gameObject.GetComponentInChildren<EnemyShield>();
    }

    public void ShieldsDestroyed()
    {
        _shieldActive = false;
        _shield.SetActive(_shieldActive);
    }

    public void FireRocketAtPlayer()
    {
        Debug.Log("Fire homing ROCKET at Player!");
        GameObject EnemyRocket = Instantiate(_enemyRocket, transform.position, Quaternion.identity);
    }

    void PlayerInSights(bool activateFOV)
    {
        _enemySmartFOV.SetActive(activateFOV);
    }

    void TargetPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= rangeOfWeapon)
        {
            target = player;

            Vector3 relativeTarget = (target.transform.position - transform.position).normalized;
            Quaternion toQuaternion = Quaternion.FromToRotation(-Vector3.up, relativeTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, toQuaternion, rotationSpeed * Time.deltaTime);
        }
        else
        {
            target = null;
            /* // This ELSE portion of the function is fighting with the EnemyBehavior follow script.
             *    If the enemy does not have a TARGET then let the ENEMY BEHAVIOR script move and rotate the ship.
            if (transform.rotation.z != 0)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, rotationSpeed * Time.deltaTime);
            }
            */
        }
        _target = target;
    }


    IEnumerator Shoot()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, firePoint.up);

        if (hitInfo)
        {
            Debug.Log(hitInfo.transform.name);

            lineRenderer.SetPosition(0, firePoint.position);

            var laserHit = Random.Range(0, 4); // Cound have an enemy 'accuracy' attribute
            laserHit = 0;

            if (laserHit != 0) // laserHit > 0, laser missed, adjust target.position
            {
                float x = Random.Range(-2f, 2f);
                float y = Random.Range(-2f, 2f);
                float z = Random.Range(-2f, 2f);
                laserMissVector3 = new Vector3(x, y, z);
                Debug.Log("Laser Missed!");
            }
            else
            {
                Debug.Log("Laser Hit!");
                laserMissVector3 = new Vector3(0, 0, 0);
                player.GetComponent<Player>().Damage();
            }

            //lineRenderer.SetPosition(1, target.position + laserMissVector3);
            lineRenderer.SetPosition(1, player.position + laserMissVector3);
        }
        else
        {
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, firePoint.position + firePoint.up * 100);
        }

        lineRenderer.enabled = true;

        yield return new WaitForSeconds(0.2f);

        lineRenderer.enabled = false;

        //lineRenderer.SetPosition(0, firePoint.position);

        //lineRenderer.SetPosition(1, _powerUpTarget.position);

        //lineRenderer.enabled = true;

        //yield return new WaitForSeconds(0.2f);

        //lineRenderer.enabled = false;
    }
}