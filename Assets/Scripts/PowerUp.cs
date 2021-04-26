using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType
    {
        TripleShot,
        Shield,
        Speed,
        LaserEnergy,
        Repair,
        Ultimate
    }

    public PowerUpType powerUpType;

    AudioSource _audioSource;

    [SerializeField] float _speed = 4.0f;
    [SerializeField] GameObject _powerUpShieldCollisionFX;
    [SerializeField] AudioClip _powerUpPickUpSFX;

    SpriteRenderer _spriteRenderer;
    BoxCollider2D _boxCollider2D;

    float _destoryYAxisThreshold = -10.0f;

    Vector3 _spawnPos = new Vector3(0, 0, 0); // Random position of enemy once re-spawned X(-8,8) Y(12,9)

    float _respawnXmin = -15.0f;
    float _respawnXmax = 15.0f;
    float _respawnYmin = 12.0f;
    float _respawnYmax = 15.0f;

    /// <summary>
    /// Testing: force Power-Up to drop where placed
    /// </summary>
    [SerializeField] bool CHEAT_LINE_THEM_UP = false;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
    }
    void Start()
    {
        if (!CHEAT_LINE_THEM_UP)
            transform.position = RandomSpawnLocation();
    }

    void Update()
    {
        CalculateMovement();
    }

    private void CalculateMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y < _destoryYAxisThreshold)
        {
            Destroy(this.gameObject);
        }
    }

    Vector3 RandomSpawnLocation()
    {
        return _spawnPos = new Vector3(Random.Range(_respawnXmin, _respawnXmax), Random.Range(_respawnYmin, _respawnYmax), 0);
    }

    void Spawn()
    {
        float respawnX = Random.Range(_respawnXmin, _respawnXmax);
        _spawnPos.x = respawnX;
        // CHEAT KEYS SHOULD BE GAMEMANAGER GLOBAL VARS
        /*
        if (CHEAT_LINE_THEM_UP)
        {
            _enemyPos.x = 0;  // testing, line enemy for two lasers intersecting collider}
        }
        */
        _spawnPos.y = Random.Range(_respawnYmin, _respawnYmax);
        transform.position = _spawnPos;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                _spriteRenderer.enabled = false;
                _boxCollider2D.enabled = false;
                _audioSource.Play();
                Destroy(this.gameObject,.5f);
            }
        }

        if (other.CompareTag("Shield"))
        {
            _powerUpShieldCollisionFX.SetActive(true);
            //Instantiate(_sfx, transform.position, Quaternion.identity);
            Destroy(this.gameObject, .15f);
        }
    }

    public PowerUpType PowerType()
    {
        return (powerUpType);
    }
}
