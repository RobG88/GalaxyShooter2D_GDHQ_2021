using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierFollowPath : MonoBehaviour
{
    [SerializeField] Transform[] routes;

    int routeToGo;

    float tParam;

    Vector2 enemyPosition;

    [SerializeField] float enemySpeed = 2f;
    [SerializeField] float enemyRotationSpeed = 2f;

    bool coroutineAllowed;

    void Start()
    {
        routeToGo = 0;
        tParam = 0f;
        coroutineAllowed = true;
    }

    void Update()
    {
        if (coroutineAllowed)
        {
            StartCoroutine(GoByTheRoute(routeToGo));
        }
    }

    IEnumerator GoByTheRoute(int routeNumber)
    {
        coroutineAllowed = false;

        Vector2 p0 = routes[routeNumber].GetChild(0).position;
        Vector2 p1 = routes[routeNumber].GetChild(1).position;
        Vector2 p2 = routes[routeNumber].GetChild(2).position;
        Vector2 p3 = routes[routeNumber].GetChild(3).position;

        while (tParam < 1)
        {

            tParam += Time.deltaTime * enemySpeed;

            enemyPosition = Mathf.Pow(1 - tParam, 3) * p0 +
                3 * Mathf.Pow(1 - tParam, 2) * tParam * p1 +
                3 * (1 - tParam) * Mathf.Pow(tParam, 2) * p2 +
                Mathf.Pow(tParam, 3) * p3;

            transform.position = enemyPosition;
            //yield return new WaitForEndOfFrame();
            yield return new WaitForFixedUpdate();
        }

        tParam = 0f;

        routeToGo += 1;

        if (routeToGo > routes.Length - 1)
        {
            routeToGo = 0;
        }

        coroutineAllowed = true;
    }
}
