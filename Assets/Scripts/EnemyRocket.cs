using UnityEngine;

public class EnemyRocket : MonoBehaviour
{
    [SerializeField] float _speed = 1.0f;
    [SerializeField] float _rotateSpeed = 200.0f;
    [SerializeField] GameObject _sparkles;
    [SerializeField] Transform target;

    [SerializeField] AudioClip _torpedoExplosionSFX;
    private Rigidbody2D rb;
    private AudioSource _sound;
    [SerializeField]private Renderer rend;
    [SerializeField] private MeshRenderer mr;

    [SerializeField] Transform player;
    [SerializeField] GameObject _explosion;
    [SerializeField] GameObject _energyExplosion;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        _sound = GetComponent<AudioSource>();
        rend = gameObject.GetComponentInChildren<Renderer>();
        mr = gameObject.GetComponentInChildren<MeshRenderer>();
        //_sound.PlayOneShot(_sound.clip);

        target = player;
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            Vector2 direction = (Vector2)target.position - rb.position;
            direction.Normalize();
            float rotateAmount = Vector3.Cross(direction, transform.up).z;
            rb.angularVelocity = -rotateAmount * _rotateSpeed;
            rb.velocity = transform.up * _speed;
        }
        else
        {
            Debug.Log("NO TARGET!!!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Instantiate(_explosion, transform.position, transform.rotation);
            //_sound.clip = _torpedoExplosionSFX;
            //_sound.PlayOneShot(_sound.clip);
            //Instantiate(_freezeExplosion, transform.position, transform.rotation);
            mr.enabled = false;
            gameObject.GetComponent<Collider2D>().enabled = false;
            _sparkles.SetActive(false);
            Destroy(this.gameObject, 2f);
        }

        if (other.tag == "Enemy")
        {
            // Destory enemy
        }
    }
}