using UnityEngine;

public class EnemyFOVRear : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            this.transform.parent.GetComponent<Enemy>().FireRocketAtPlayer();
        }
    }
}