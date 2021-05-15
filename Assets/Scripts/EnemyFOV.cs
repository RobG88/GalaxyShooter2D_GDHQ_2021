using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFOV : MonoBehaviour
{
    Enemy enemy;
    Transform powerUpVector3;
    [SerializeField] bool _canCloak;
    [SerializeField] PolygonCollider2D _coneCollider;

    private void OnEnable()
    {
        _coneCollider.enabled = true;
    }

    private void Awake()
    {
        //enemy = GetComponent<EnemyFireLaser>();

        //enemy = transform.parent.GetComponent<EnemyFireLaser>().DestroyPowerUp(powerUpVector3);
    }
    private void Start()
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Enemy FOV triggered with: " + other.name);
        Debug.Log("Enemy has spotted: " + other.tag);

        //var powerUpTrans = other.gameObject;
        //var powerUpVector3 = other.gameObject.transform;

        //Debug.Log("PowerUp Vector = " + powerUpVector3);

        //Debug.Log("PowerUpTrans = " + powerUpTrans.name);

        if (other.CompareTag("PowerUp"))
        {
            this.transform.parent.GetComponent<Enemy>().DestroyPowerUp(other.gameObject);
        }

        /*
        if (other.tag == "Player")
        {
            this.transform.parent.GetComponent<EnemyFireLaser>().FireRocketAtPlayer();
        }
        */

        if ((other.CompareTag("Torpedo") || other.CompareTag("Laser")) && _canCloak)
        {
            //Debug.Log("Enemy FOV has triggers with " + other.tag);
            this.transform.parent.GetComponent<Cloak>().Cloaking();
        }
    }
}
