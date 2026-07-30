using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private string weaponName = "Training Rifle";

    [SerializeField] private Camera playerCamera;
    [SerializeField] private float fireDistance = 100.0f;
    [SerializeField] private Color debugRayColor = Color.red;
    [SerializeField] private float debugDuration = 0.2f;

    [SerializeField] private int damage = 10;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CheckRequiredReferences();
        Debug.Log(weaponName + " is ready.");
    }

    // Update is called once per frame
    void Update()
    {
        HandleFireInput();
    }

    private void CheckRequiredReferences()
    {
        if(firePoint == null)
        {
            Debug.LogWarning("FirePoint 가 연결되지 않았습니다.", this);
        }
    }

    void HandleFireInput()
    {
        if(Input.GetMouseButtonDown(0) == true)
        {
            Fire();
        }
    }

    void Fire()
    {
        if(playerCamera == null)
        {
            return;
        }

        Vector3 rayStartPosition = playerCamera.transform.position;
        Vector3 rayDirection = playerCamera.transform.forward;

        Debug.DrawRay(rayStartPosition, rayDirection * fireDistance, debugRayColor, debugDuration);

        bool isHit = Physics.Raycast(rayStartPosition, rayDirection, out RaycastHit hitInfo, fireDistance);

        if(isHit == true)
        {
            Debug.Log(hitInfo.collider.name + " / " + hitInfo.distance);
            Debug.Log("Hit Point: " + hitInfo.point);

            EnemyHealth enemyHealth = hitInfo.collider.GetComponent<EnemyHealth>();
            if(enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            else
            {
                Debug.Log("적이 아닌 오브젝트에 맞았습니다.");
            }
        }
        else
        {
            Debug.Log("빗나감");
        }
    }
}
