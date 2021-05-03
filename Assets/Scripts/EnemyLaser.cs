using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaser : MonoBehaviour
{
    [SerializeField] float _speed = 13.0f;
    [SerializeField] float _yThreshold = 12.5f;
    [SerializeField] GameObject _playerShieldCollisionFX;
    [SerializeField] AudioClip _playerShieldCollisionSFX;
    [SerializeField] AudioClip _collidingWithPlayerLaserSFX;

    CapsuleCollider2D _collider2D;
    SpriteRenderer _spriteRenderer;

    AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _collider2D = GetComponent<CapsuleCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _audioSource.pitch = Random.Range(0.5f, 1.55f);
        _audioSource.PlayOneShot(_audioSource.clip);

        _speed += Random.Range(0.3f, 1f);
    }
    void Update()
    {
        MoveDown();

        if (transform.position.y < _yThreshold)
            Destroy(transform.gameObject);
    }

    void MoveDown()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser") || other.CompareTag("Player") || other.CompareTag("Shield"))
        {
            _spriteRenderer.enabled = false;
            _collider2D.enabled = false;

            if (other.CompareTag("Shield"))
            {
                _audioSource.PlayOneShot(_playerShieldCollisionSFX);
                _playerShieldCollisionFX.SetActive(true);
            }

            if (other.CompareTag("Laser"))
            {
                _audioSource.PlayOneShot(_collidingWithPlayerLaserSFX);
                _playerShieldCollisionFX.transform.localScale = new Vector3(.25f, .25f, .25f);
                _playerShieldCollisionFX.SetActive(true);
            }

            Destroy(gameObject, .3f);
        }
    }
}
