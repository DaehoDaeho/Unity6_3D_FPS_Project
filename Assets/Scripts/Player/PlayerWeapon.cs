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

    [SerializeField] private ParticleSystem[] muzzleFlash;
    [SerializeField] private GameObject hitEffectPrefab;

    [SerializeField] private AudioSource weaponAudioSource;

    [SerializeField] private AudioClip fireSound;
    [SerializeField] private AudioClip hitSound;

    [SerializeField] private float hitEffectDestroyTime = 1.5f;

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

        PlayFireFeedback();

        Vector3 rayStartPosition = playerCamera.transform.position;
        Vector3 rayDirection = playerCamera.transform.forward;

        Debug.DrawRay(rayStartPosition, rayDirection * fireDistance, debugRayColor, debugDuration);

        bool isHit = Physics.Raycast(rayStartPosition, rayDirection, out RaycastHit hitInfo, fireDistance);

        if(isHit == true)
        {
            Debug.Log(hitInfo.collider.name + " / " + hitInfo.distance);
            Debug.Log("Hit Point: " + hitInfo.point);

            PlayHitFeedback(hitInfo);

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

    void PlayFireFeedback()
    {
        if(muzzleFlash != null)
        {
            foreach (ParticleSystem particle in muzzleFlash)
            {
                if(particle != null)
                {
                    particle.Play();
                }
            }
        }

        if (weaponAudioSource != null && fireSound != null)
        {
            weaponAudioSource.PlayOneShot(fireSound);
        }
    }

    void PlayHitFeedback(RaycastHit hitInfo)
    {
        if(hitEffectPrefab != null)
        {
            GameObject hitEffect = Instantiate(hitEffectPrefab, hitInfo.point, Quaternion.identity);

            Destroy(hitEffect, hitEffectDestroyTime);
        }

        if (weaponAudioSource != null && hitSound != null)
        {
            weaponAudioSource.PlayOneShot(hitSound);
        }
    }
}
