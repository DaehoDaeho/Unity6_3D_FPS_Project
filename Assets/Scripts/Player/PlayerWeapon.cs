using UnityEngine;
using TMPro;
using System;

public enum HitType
{
    Raycast,
    Projectile
}

[Serializable]
public class WeaponSlot
{
    public string displayName = "Pistol";
    public HitType hitType;
    public GameObject weaponObject;
    public GameObject projectilePrefab;
    public float projectileSpeed = 5.0f;
    public float projectileExplosionRadius = 0.0f;
    public int damage = 10;
    public float fireDistance = 100.0f;
    public int magazineSize = 12;
    public int currentAmmo = 12;
    public int reserveAmmo = 36;
    public float reloadTime = 1.5f;

    public Transform firePoint;
    public ParticleSystem[] muzzleFlash;

    public GameObject hitEffectPrefab;
    public AudioSource weaponAudioSource;
    public AudioClip fireSound;
    public AudioClip hitSound;
    public float hitEffectDestroyTime = 1.5f;
}

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private WeaponSlot[] weaponSlots;
    [SerializeField] private int startWeaponIndex = 0;

    [SerializeField] private Camera playerCamera;
    [SerializeField] private Color debugRayColor = Color.red;
    [SerializeField] private float debugDuration = 0.2f;    

    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private TMP_Text reloadText;
    [SerializeField] private TMP_Text weaponNameText;

    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float grenadeThrowForce = 10.0f;
    [SerializeField] private int grenadeDamage = 500;

    private int currentWeaponIndex;
    private bool isReloading;
    private float reloadTimer;

    private WeaponSlot CurrentWeapon
    {
        get
        {
            if(weaponSlots == null || weaponSlots.Length == 0)
            {
                return null;
            }

            if(currentWeaponIndex < 0 || currentWeaponIndex >= weaponSlots.Length)
            {
                return null;
            }

            return weaponSlots[currentWeaponIndex];
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeWeaponSlot();
        SelectStartWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        HandleWeaponSwitchInput();
        HandleFireInput();
        HandleReloadInput();
        HandleThrowGrenadeInput();
        UpdateReload();
    }

    void InitializeWeaponSlot()
    {
        if(weaponSlots == null)
        {
            return;
        }

        foreach(WeaponSlot slot in weaponSlots)
        {
            slot.magazineSize = Mathf.Max(1, slot.magazineSize);
            slot.currentAmmo = Mathf.Clamp(slot.currentAmmo, 0, slot.magazineSize);
            slot.reserveAmmo = Mathf.Max(0, slot.reserveAmmo);
        }
    }

    void SelectStartWeapon()
    {
        if (weaponSlots == null)
        {
            return;
        }

        currentWeaponIndex = Mathf.Clamp(startWeaponIndex, 0, weaponSlots.Length - 1);
        // 무기 오브젝트 교체 (선택한 무기만 활성화)
        ApplyWeaponVisuals();
        UpdateWeaponUI();
    }

    void HandleWeaponSwitchInput()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1) == true)
        {
            TrySwitchWeapon(0);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2) == true)
        {
            TrySwitchWeapon(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) == true)
        {
            TrySwitchWeapon(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) == true)
        {
            TrySwitchWeapon(3);
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

    void HandleThrowGrenadeInput()
    {
        if(Input.GetKeyDown(KeyCode.G) == true)
        {
            // 수류탄 투척.
            ThrowGrenadeWeapon();
        }
    }

    void ThrowGrenadeWeapon()
    {
        GameObject grenadeObject = Instantiate(grenadePrefab, firePoint.position, Quaternion.identity);

        Grenade grenade = grenadeObject.GetComponent<Grenade>();
        if (grenade != null)
        {
            Vector3 throwDirection = playerCamera.transform.forward + Vector3.up * 0.25f;
            grenade.Throw(throwDirection.normalized, grenadeThrowForce, grenadeDamage);
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
        WeaponSlot weapon = CurrentWeapon;
        if(weapon == null)
        {
            return false;
        }

        if (playerCamera == null)
        {
            return false;
        }

        if (isReloading == true)
        {
            return false;
        }

        return weapon.currentAmmo > 0;
    }

    void Fire()
    {
        WeaponSlot weapon = CurrentWeapon;

        if (CanFire() == false || weapon == null)
        {
            Debug.Log("Cannot fire.", this);
            return;
        }

        if (playerCamera == null)
        {
            return;
        }

        weapon.currentAmmo--;
        Debug.Log($"Ammo: {weapon.currentAmmo} / {weapon.reserveAmmo}", this);
        UpdateAmmoText();

        PlayFireFeedback();

        if(weapon.hitType == HitType.Projectile)
        {
            FireProjectileWeapon();
            return;
        }

        Vector3 rayStartPosition = playerCamera.transform.position;
        Vector3 rayDirection = playerCamera.transform.forward;

        Debug.DrawRay(rayStartPosition, rayDirection * weapon.fireDistance, debugRayColor, debugDuration);

        bool isHit = Physics.Raycast(rayStartPosition, rayDirection, out RaycastHit hitInfo, weapon.fireDistance);

        if(isHit == true)
        {
            Debug.Log(hitInfo.collider.name + " / " + hitInfo.distance);
            Debug.Log("Hit Point: " + hitInfo.point);

            PlayHitFeedback(hitInfo);

            EnemyHitbox enemyHitbox = hitInfo.collider.GetComponent<EnemyHitbox>();
            if(enemyHitbox != null)
            {
                enemyHitbox.TakeHit(weapon.damage);
                return;
            }    

            EnemyHealth enemyHealth = hitInfo.collider.GetComponent<EnemyHealth>();
            if(enemyHealth != null)
            {
                enemyHealth.TakeDamage(weapon.damage);
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

    void FireProjectileWeapon()
    {
        WeaponSlot weapon = CurrentWeapon;
        if (weapon == null || weapon.projectilePrefab == null)
        {
            return;
        }

        GameObject projectileObject = Instantiate(weapon.projectilePrefab, weapon.firePoint.position, Quaternion.identity);

        if(projectileObject != null)
        {
            Projectile projectile = projectileObject.GetComponent<Projectile>();
            if(projectile != null)
            {
                projectile.Launch(playerCamera.transform.forward, weapon.projectileSpeed, weapon.damage, weapon.projectileExplosionRadius);
            }
        }
    }

    void PlayFireFeedback()
    {
        WeaponSlot weapon = CurrentWeapon;
        if(weapon == null)
        {
            return;
        }

        if(weapon.muzzleFlash != null)
        {
            foreach (ParticleSystem particle in weapon.muzzleFlash)
            {
                if(particle != null)
                {
                    particle.Play();
                }
            }
        }

        if (weapon.weaponAudioSource != null && weapon.fireSound != null)
        {
            weapon.weaponAudioSource.PlayOneShot(weapon.fireSound);
        }
    }

    void PlayHitFeedback(RaycastHit hitInfo)
    {
        WeaponSlot weapon = CurrentWeapon;
        if (weapon == null)
        {
            return;
        }

        if (weapon.hitEffectPrefab != null)
        {
            GameObject hitEffect = Instantiate(weapon.hitEffectPrefab, hitInfo.point, Quaternion.identity);

            Destroy(hitEffect, weapon.hitEffectDestroyTime);
        }

        if (weapon.weaponAudioSource != null && weapon.hitSound != null)
        {
            weapon.weaponAudioSource.PlayOneShot(weapon.hitSound);
        }
    }

    private void TryBeginReload()
    {
        WeaponSlot weapon = CurrentWeapon;
        if(weapon == null)
        {
            return;
        }

        if (isReloading == true)
        {
            return;
        }

        if (weapon.currentAmmo >= weapon.magazineSize)
        {
            return;
        }

        if (weapon.reserveAmmo <= 0)
        {
            return;
        }

        BeginReload();
    }

    private void BeginReload()
    {
        WeaponSlot weapon = CurrentWeapon;

        isReloading = true;
        reloadTimer = weapon.reloadTime;

        UpdateReloadText();
        Debug.Log("Reload started.", this);
    }

    private void FinishReload()
    {
        WeaponSlot weapon = CurrentWeapon;
        if(weapon == null)
        {
            return;
        }

        int neededAmmo = weapon.magazineSize - weapon.currentAmmo;
        int ammoToLoad = Mathf.Min(neededAmmo, weapon.reserveAmmo);

        weapon.currentAmmo += ammoToLoad;
        weapon.reserveAmmo -= ammoToLoad;

        isReloading = false;
        reloadTimer = 0f;

        UpdateAmmoText();
        UpdateReloadText();

        Debug.Log($"Reload finished. Ammo: {weapon.currentAmmo} / {weapon.reserveAmmo}", this);
    }

    void UpdateAmmoText()
    {
        WeaponSlot weapon = CurrentWeapon;
        if(weapon == null)
        {
            return;
        }

        if(ammoText == null)
        {
            return;
        }

        ammoText.text = $"{weapon.currentAmmo} / {weapon.reserveAmmo}";
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

    public void AddReserveAmmo(int ammoAmount)
    {
        WeaponSlot weapon = CurrentWeapon;
        if(weapon == null)
        {
            return;
        }

        weapon.reserveAmmo += ammoAmount;
        UpdateAmmoText();
    }

    void TrySwitchWeapon(int targetIndex)
    {
        if(isReloading == true)
        {
            return;
        }

        if(weaponSlots == null || targetIndex < 0 || targetIndex >= weaponSlots.Length)
        {
            return;
        }

        if(currentWeaponIndex == targetIndex)
        {
            return;
        }

        currentWeaponIndex = targetIndex;
        // 무기 오브젝트 교체.
        ApplyWeaponVisuals();
        UpdateWeaponUI();
    }

    void ApplyWeaponVisuals()
    {
        if(weaponSlots == null)
        {
            return;
        }

        for(int i=0; i<weaponSlots.Length; i++)
        {
            if (weaponSlots[i] == null || weaponSlots[i].weaponObject == null)
            {
                continue;
            }

            weaponSlots[i].weaponObject.SetActive(i == currentWeaponIndex);
        }
    }

    void UpdateWeaponUI()
    {
        UpdateAmmoText();
        UpdateReloadText();
    }
}
