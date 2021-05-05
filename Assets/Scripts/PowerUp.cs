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
        EnergyCell,
        Repair,
        FreezeTorpedo,
        Ultimate
    }

    public PowerUpType powerUpType;

    AudioSource _audioSource;

    [SerializeField] float _speed = 4.0f;
    [SerializeField] GameObject _powerUpShieldCollisionFX;
    [SerializeField] AudioClip _powerUpPickUpSFX;
    [SerializeField] bool _rotateOnYAxis;
    float _rotationSpeed = 250f;

    SpriteRenderer _spriteRenderer;
    BoxCollider2D _collider2D;

    [SerializeField] float _destoryYAxisThreshold = -10.0f;

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
        _collider2D = GetComponent<BoxCollider2D>();
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

        if (_rotateOnYAxis) {
            transform.Rotate(0f, _rotationSpeed * Time.deltaTime, 0f);
        }

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
        _spawnPos.y = Random.Range(_respawnYmin, _respawnYmax);
        transform.position = _spawnPos;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(gameObject.name + " just collided with " + other.tag);
        if (other.CompareTag("Player"))
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                _spriteRenderer.enabled = false;
                _collider2D.enabled = false;
                _audioSource.Play();
                Destroy(this.gameObject,.5f);
            }
        }

        if (other.CompareTag("Shield"))
        {
            //Debug.Log("Collided with: " + other.tag);
            _spriteRenderer.enabled = false;
            _collider2D.enabled = false;
            _audioSource.PlayOneShot(_powerUpPickUpSFX);
            _powerUpShieldCollisionFX.SetActive(true);
            Destroy(this.gameObject, 0.25f);
        }
    }

    public PowerUpType PowerType()
    {
        return (powerUpType);
    }
}
