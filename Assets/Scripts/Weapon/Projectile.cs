using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Rigidbody projectileRigidbody;
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private float lifeTime = 4.0f;

    private int damage;
    private float explosionRadius;
    private bool hasProcessdHit;

    public void Launch(Vector3 direction, float speed, int damageAmount, float radius)
    {
        damage = damageAmount;
        explosionRadius = Mathf.Max(0.0f, radius);

        if(projectileRigidbody != null)
        {
            projectileRigidbody.linearVelocity = direction.normalized * speed;
        }

        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(hasProcessdHit == true)
        {
            return;
        }

        if (other.CompareTag("Player") == true)
        {
            return;
        }

        hasProcessdHit = true;
        Vector3 hitPoint = other.ClosestPoint(transform.position);

        if(explosionRadius > 0.0f)
        {
            // 광역 데미지 적용.
            ApplyExplosionDamage(hitPoint);
        }
        else
        {
            // 맞은 놈한테만 데미지 적용.
            ApplyDirectDamage(other);
        }

        // 피격 이펙트 출력.
        CreateHitEffect(hitPoint);
        Destroy(gameObject);
    }

    void ApplyDirectDamage(Collider other)
    {
        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
        if(enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }
    }

    void ApplyExplosionDamage(Vector3 center)
    {
        Collider[] colliders = Physics.OverlapSphere(center, explosionRadius);
        foreach(Collider hitCollider in colliders)
        {
            //EnemyHealth enemyHealth = hitCollider.GetComponent<EnemyHealth>();
            //if(enemyHealth != null)
            //{
            //    enemyHealth.TakeDamage(damage);
            //}

            ApplyDirectDamage(hitCollider);
        }
    }

    void CreateHitEffect(Vector3 hitPoint)
    {
        if(hitEffectPrefab == null)
        {
            return;
        }

        GameObject effect = Instantiate(hitEffectPrefab, hitPoint, Quaternion.identity);

        Destroy(effect, 1.5f);
    }
}
