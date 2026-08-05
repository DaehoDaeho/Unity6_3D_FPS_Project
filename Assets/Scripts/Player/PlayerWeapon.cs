using UnityEngine;
using TMPro;

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

    [SerializeField] private int magazineSize = 12;
    [SerializeField] private int currentAmmo = 12;
    [SerializeField] private int reserveAmmo = 36;
    [SerializeField] private float reloadTime = 1.5f;

    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private TMP_Text reloadText;

    private bool isReloading;
    private float reloadTimer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CheckRequiredReferences();
        Debug.Log(weaponName + " is ready.");

        UpdateAmmoText();
        UpdateReloadText();
    }

    // Update is called once per frame
    void Update()
    {
        HandleFireInput();
        HandleReloadInput();
        UpdateReload();
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

    private void HandleReloadInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            TryBeginReload();
        }
    }

    private void UpdateReload()
    {
        if (isReloading == false)
        {
            return;
        }

        reloadTimer -= Time.deltaTime;
        UpdateReloadText();

        if (reloadTimer <= 0f)
        {
            FinishReload();
        }
    }

    private bool CanFire()
    {
        if (playerCamera == null)
        {
            return false;
        }

        if (isReloading == true)
        {
            return false;
        }

        return currentAmmo > 0;
    }

    void Fire()
    {
        if (CanFire() == false)
        {
            Debug.Log("Cannot fire.", this);
            return;
        }

        if (playerCamera == null)
        {
            return;
        }

        currentAmmo--;
        Debug.Log($"Ammo: {currentAmmo} / {reserveAmmo}", this);
        UpdateAmmoText();

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

    private void TryBeginReload()
    {
        if (isReloading == true)
        {
            return;
        }

        if (currentAmmo >= magazineSize)
        {
            return;
        }

        if (reserveAmmo <= 0)
        {
            return;
        }

        BeginReload();
    }

    private void BeginReload()
    {
        isReloading = true;
        reloadTimer = reloadTime;

        UpdateReloadText();
        Debug.Log("Reload started.", this);
    }

    private void FinishReload()
    {
        int neededAmmo = magazineSize - currentAmmo;
        int ammoToLoad = Mathf.Min(neededAmmo, reserveAmmo);

        currentAmmo += ammoToLoad;
        reserveAmmo -= ammoToLoad;

        isReloading = false;
        reloadTimer = 0f;

        UpdateAmmoText();
        UpdateReloadText();

        Debug.Log($"Reload finished. Ammo: {currentAmmo} / {reserveAmmo}", this);
    }

    void UpdateAmmoText()
    {
        if(ammoText == null)
        {
            return;
        }

        ammoText.text = $"{currentAmmo} / {reserveAmmo}";
    }

    void UpdateReloadText()
    {
        if(reloadText == null)
        {
            return;
        }

        if(isReloading == true)
        {
            reloadText.text = $"RELOADING {reloadTimer:0.0}s";
        }
        else
        {
            reloadText.text = string.Empty;
        }
    }
}
