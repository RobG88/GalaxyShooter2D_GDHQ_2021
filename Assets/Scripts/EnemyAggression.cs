using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAggression : MonoBehaviour
{
    // if PLAYER is with +/- 1 on Y and RANGE X
    // enemy will fire Thrusters and RAM PLAYER

    public Transform target { get; private set; }
    [SerializeField] Transform player;
    [SerializeField] Transform _target;
    [SerializeField] float rangeToRam = 5f;
    [SerializeField] bool _canRamPlayer = false;
    [SerializeField] float rotationSpeed = 7f;
    //Quaternion originalRotation;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        //originalRotation = transform.rotation;
    }

    void Update()
    {
        if (target == null)
        {
            TargetPlayer();
        }
    }

    void TargetPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= rangeToRam)
        {
            target = player;

            Vector3 relativeTarget = (target.transform.position - transform.position).normalized;
            Quaternion toQuaternion = Quaternion.FromToRotation(-Vector3.up, relativeTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, toQuaternion, rotationSpeed * Time.deltaTime);
            Kamakaze();
        }
        else
        {
            target = null;

        }
        _target = target;
    }

    void Kamakaze()
    {
        // Enable Enemy Thrusters
        // gameObject.GetComponent<ScriptName>().variable
        gameObject.GetComponent<Enemy>().RamPlayer();
    }
}
