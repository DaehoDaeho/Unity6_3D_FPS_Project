using UnityEngine;
using UnityEngine.AI;

public class EnemyChaseAgent : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private PlayerHealth targetHealth;
    [SerializeField] private float chaseDistance = 12.0f;
    [SerializeField] private float stopDistance = 2.2f;
    [SerializeField] private float updateInterval = 0.15f;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    [SerializeField] private float attackDistance = 2.4f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1.2f;

    private float updateTimer;
    private bool isStopped = true;
    private float lastAttackTime = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(agent != null)
        {
            agent.stoppingDistance = stopDistance;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(agent == null || target == null)
        {
            return;
        }

        if (targetHealth != null && targetHealth.IsDead == true)
        {
            StopAgent();
            return;
        }

        updateTimer -= Time.deltaTime;
        if(updateTimer > 0.0f)
        {
            return;
        }

        updateTimer = updateInterval;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if(distanceToTarget <= attackDistance)
        {
            StopForAttack();
            TryAttack();
        }
        else if(distanceToTarget <= chaseDistance)
        {
            ChaseTarget();
        }
        else
        {
            StopAgent();
        }

        UpdateAnimation();
    }

    void ChaseTarget()
    {
        agent.isStopped = false;
        isStopped = agent.isStopped;
        agent.SetDestination(target.position);
    }

    void StopAgent()
    {
        if(agent == null)
        {
            return;
        }

        agent.isStopped = true;
        isStopped = agent.isStopped;
        agent.ResetPath();
    }

    void StopForAttack()
    {
        agent.isStopped = true;
        isStopped = agent.isStopped;
    }

    void TryAttack()
    {
        if(targetHealth == null)
        {
            return;
        }

        if(Time.time < lastAttackTime + attackCooldown)
        {
            return;
        }

        lastAttackTime = Time.time;
        animator.SetTrigger("IsAttacking");
    }

    public void OnAttackEvent()
    {
        targetHealth.TakeDamage(attackDamage);
    }

    void UpdateAnimation()
    {
        if (animator == null || agent == null)
        {
            return;
        }

        animator.SetBool("IsChasing", !isStopped);
    }

    public void OnFootstep()
    {

    }
}
