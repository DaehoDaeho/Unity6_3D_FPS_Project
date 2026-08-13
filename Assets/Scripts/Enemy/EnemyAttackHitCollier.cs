using UnityEngine;

public class EnemyAttackHitCollier : MonoBehaviour
{
    [SerializeField] private EnemyChaseAgent agent;

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.CompareTag("Player") == true)
        {
            if(agent != null)
            {
                agent.TakeDamage();
            }
        }
    }
}
