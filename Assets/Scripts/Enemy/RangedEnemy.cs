using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class RangedEnemy : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Animator animator;

    [SerializeField] private float detectRange = 18.0f;
    [SerializeField] private float attackRange = 12.0f;

    [SerializeField] private float stopRange = 7.0f;

    [SerializeField] private float fireInterval = 1.2f;
    [SerializeField] private int damage = 8;

    private float fireTimer;

    private bool isDead;
    private bool isAttacking = false;

    public void SetTarget(Transform newTarget, PlayerHealth newTargetHealth)
    {
        player = newTarget;
        playerHealth = newTargetHealth;
    }

    private void Reset()
    {
        if(agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead == true || player == null)
        {
            return;
        }

        // 발사 쿨타임 체크.
        UpdateFireTimer();
        // 적 행동.
        UpdateEnemyAction();
        UpdateAnimation();
    }

    void UpdateFireTimer()
    {
        if (fireTimer > 0.0f)
        {
            fireTimer -= Time.deltaTime;
        }
    }

    void UpdateEnemyAction()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if(distance > detectRange)
        {
            isAttacking = false;
            // 행동 중단.
            StopMoving();
            return;
        }
        else if(distance > attackRange)
        {
            isAttacking = false;
            // 플레이어 추적.
            ChasePlayer();
            return;
        }

        //이동을 멈추고 사격.
        StopAndShoot(distance);
    }

    void ChasePlayer()
    {
        if(agent == null)
        {
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    void StopMoving()
    {
        if(agent != null)
        {
            agent.isStopped = true;
        }
    }

    void StopAndShoot(float distance)
    {
        StopMoving();
        // 플레이어를 바라보게 만든다.
        LookAtPlayer();

        if (fireTimer <= 0.0f)
        {
            isAttacking = true;
            // 사격.
            Shoot();
            fireTimer = fireInterval;
        }
    }

    void LookAtPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0.0f;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // lerp -> 선형 보간.
        // Slert -> 구면 보간.
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 8.0f);
    }

    void Shoot()
    {
        if(firePoint == null || playerHealth == null || playerHealth.IsDead == true)
        {
            return;
        }

        animator.SetTrigger("IsShooting");

        Vector3 target = player.position + Vector3.up * 1.2f;
        Vector3 direction = (target - firePoint.position).normalized;

        bool isHit = Physics.Raycast(firePoint.position, direction, out RaycastHit hitInfo, attackRange);

        // Debug.DrawRay

        if(isHit == true)
        {
            if(hitInfo.collider.CompareTag("Player") == true)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }

    void UpdateAnimation()
    {
        if(animator == null)
        {
            return;
        }

        bool move = agent.isStopped == false && isAttacking == false;
        animator.SetBool("IsChasing", move);
    }

    public void PlayDeadAnimation()
    {
        StopMoving();
        isDead = true;
        if (animator == null)
        {
            return;
        }

        animator.SetTrigger("IsDead");

        StartCoroutine(DisableEnemy());
    }

    IEnumerator DisableEnemy()
    {
        yield return new WaitForSeconds(3.0f);

        gameObject.SetActive(false);
    }

    void OnFootstep()
    {

    }
}
