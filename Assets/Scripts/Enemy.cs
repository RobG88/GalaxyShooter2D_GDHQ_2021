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

    [SerializeField] int _scoreValue = 0;
    //[SerializeField] GameObject _enemyInvaderExplosion;
    [SerializeField] GameObject _collisionWithShieldExplosionFX;
    [SerializeField] bool CHEAT_LINE_THEM_UP = false;

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

    void Awake()
    {
        _anim = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _collider2D = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (!CHEAT_LINE_THEM_UP)
            Spawn();

        _fireRate = Random.Range(.5f, 2f);
        _canFire = Time.time + _fireRate;
    }

    void Update()
    {
        CalculateMovement();

        ///
        /// FREEZE/EMP TORPEDO - If enemy is frozen, can shoot, _CanFire is reset upon Thaw
        /// 
        if (Time.time > _canFire && WithinFiringRange() && !isFrozen)
        {
            _fireRate = Random.Range(2f, 7f);

            _canFire = Time.time + _fireRate;

            GameObject EnemyLaserShot = Instantiate(_enemyLaserPrefab, transform.position, Quaternion.identity);
        }
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


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Torpedo") || other.CompareTag("Laser") || other.CompareTag("Player") || other.CompareTag("Shield"))
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
}