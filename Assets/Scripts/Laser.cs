using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    ////////////////////////////////
    /// Laser (Player)
    /// 
    /// Laser gameObject travels UP @ _speed
    /// 
    /// Laser triggers/collides with object &
    /// is destroyed or self-desrtucts when
    /// position.y > _yThreshold (8)
    /// 
    /// Audio: pitch is random & hit for Triple Shot

    AudioSource _audioSource;
    CapsuleCollider2D _collider2D;
    SpriteRenderer _spriteRenderer;

    [SerializeField] AudioClip _laserSFX;
    [SerializeField] float _speed = 13.0f;
    [SerializeField] float _yThreshold = 12.5f;
    [SerializeField] GameObject _playerShieldCollisionFX;
    [SerializeField] AudioClip _playerShieldCollisionSFX;

    void Awake()
    {
        _collider2D = GetComponent<CapsuleCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (transform.parent != null)
        {
            _audioSource.pitch = Random.Range(0.85f, 1.15f);
        }
        else
        {
            _audioSource.pitch = Random.Range(2.0f, 4.0f);
        }
        _audioSource.PlayOneShot(_laserSFX);
    }

    void Update()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        if (transform.position.y > _yThreshold)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("EnemyShield"))
        {
            Debug.Log("Collided with " + other.tag);
            _spriteRenderer.enabled = false;
            _collider2D.enabled = false;
            _speed = 0;

            if (other.CompareTag("Enemy")) // || other.CompareTag("Asteroid"))
            {/*
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            else { Destroy(gameObject); }
            */
            }

            if (other.CompareTag("EnemyShield"))
            {
                _audioSource.PlayOneShot(_playerShieldCollisionSFX);
                //_playerShieldCollisionFX.transform.localScale -= new Vector3(1f, 1f, 1f);
                _playerShieldCollisionFX.SetActive(true);
            }

            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject, 2f);
            }
            else { Destroy(gameObject, 2f); }
        }


    }
}
