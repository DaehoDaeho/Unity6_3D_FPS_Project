using UnityEngine;

public class EnemyPerception : MonoBehaviour
{
    [SerializeField] private float detectRange = 15.0f;
    [SerializeField] private float viewAngle = 90.0f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private Transform eyePoint;

    private Transform player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTarget(Transform target)
    {
        player = target;
    }

    public bool CanSeePlayer()
    {
        if(player == null)
        {
            return false;
        }

        Vector3 targetPosition = player.position + Vector3.up * 1.0f;

        Vector3 toPlayer = targetPosition - eyePoint.position;
        float distanceToPlayer = toPlayer.magnitude;

        if(distanceToPlayer > detectRange)
        {
            return false;
        }

        // 플레이어가 적의 시야각 내에 있는지 여부를 반환.
        return IsInsideViewAngle(toPlayer) && IsViewClear(toPlayer);
    }

    bool IsInsideViewAngle(Vector3 toPlayer)
    {
        Vector3 directionToPlayer = toPlayer.normalized;

        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        return angle <= viewAngle * 0.5f;
    }

    bool IsViewClear(Vector3 toPlayer)
    {
        Vector3 directionToPlayer = toPlayer.normalized;
        float distanceToPlayer = toPlayer.magnitude;

        bool blocked = Physics.Raycast(eyePoint.position, directionToPlayer, distanceToPlayer, obstacleLayer);

        return blocked == false;
    }
}
