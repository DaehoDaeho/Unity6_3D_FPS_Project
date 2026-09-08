using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private EnemyChaseAgent agent;
    [SerializeField] private RangedEnemy rangedEnemy;
    [SerializeField] private RagdollDeathController ragdollController;
    [SerializeField] private bool useRagdoll = true;

    private int currentHealth = 0;
    private bool isDead;

    public bool IsDead
    {
        get { return isDead; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        isDead = false;
    }

    public void TakeDamage(int damage)
    {
        if(isDead == true)
        {
            return;
        }

        currentHealth -= damage;

        if(currentHealth < 0)
        {
            currentHealth = 0;
        }

        Debug.Log("데미지를 입었습니다. " + currentHealth + " / " + maxHealth);

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if(isDead == true)
        {
            return;
        }

        isDead = true;

        Debug.Log("사망했씁니다.");

        if(useRagdoll == true && ragdollController != null)
        {
            ragdollController.Die();
        }
        else
        {
            if (agent != null)
            {
                agent.ChangeState(EnemyChaseAgent.EnemyState.Dead);
            }

            if (rangedEnemy != null)
            {
                rangedEnemy.PlayDeadAnimation();
            }
        }

        //gameObject.SetActive(false);
    }
}
