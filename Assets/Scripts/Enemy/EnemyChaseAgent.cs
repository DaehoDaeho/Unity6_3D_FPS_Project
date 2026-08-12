using UnityEngine;
using UnityEngine.AI;

public class EnemyChaseAgent : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Chase,
        Attack,
        Dead
    }

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

    private EnemyState currentState = EnemyState.Idle;

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
            ChangeState(EnemyState.Idle);
            RunCurrentState();
            return;
        }

        if (targetHealth != null && targetHealth.IsDead == true)
        {
            ChangeState(EnemyState.Idle);
            RunCurrentState();
            return;
        }

        updateTimer -= Time.deltaTime;
        if(updateTimer <= 0.0f)
        {
            updateTimer = updateInterval;
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            DecideState(distanceToTarget);
        }

        RunCurrentState();

        UpdateAnimation();
    }

    void DecideState(float distanceToTarget)
    {
        if(distanceToTarget <= attackDistance)
        {
            // 공격 상태로 전환.
            ChangeState(EnemyState.Attack);
        }
        else if(distanceToTarget <= chaseDistance)
        {
            // 추적 상태로 전환.
            ChangeState(EnemyState.Chase);
        }
        else
        {
            // 대기 상태로 전환.
            ChangeState(EnemyState.Idle);
        }
    }

    public void ChangeState(EnemyState nextState)
    {
        if(currentState == nextState)
        {
            return;
        }

        currentState = nextState;
        Debug.Log("Enemy State: " + currentState);
    }

    void RunCurrentState()
    {
        // switch ~ case 문.
        switch(currentState)
        {
            case EnemyState.Idle:
                {
                    RunIdleState();
                }
                break;

            case EnemyState.Chase:
                {
                    RunChaseState();
                }
                break;

            case EnemyState.Attack:
                {
                    RunAttackState();
                }
                break;

            case EnemyState.Dead:
                {
                    RunDeadState();
                }
                break;
        }
    }

    void RunIdleState()
    {
        StopAgent();
    }

    void RunChaseState()
    {
        ChaseTarget();
    }

    void RunAttackState()
    {
        StopForAttack();
        TryAttack();
    }

    void RunDeadState()
    {
        StopAgent();
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
