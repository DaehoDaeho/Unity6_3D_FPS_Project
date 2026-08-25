using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private Rigidbody grenadeRigidbody;
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private float fuseTime = 2.0f;
    [SerializeField] private float explosionRadius = 4.5f;
    [SerializeField] private float explosionForce = 900.0f;

    private int damage;
    private bool hasExploded;

    public void Throw(Vector3 direction, float throwForce, int damageAmount)
    {
        damage = damageAmount;

        if(grenadeRigidbody != null)
        {
            grenadeRigidbody.AddForce(direction * throwForce, ForceMode.Impulse);
        }

        Invoke(nameof(Explode), fuseTime);
    }

    void Explode()
    {
        if(hasExploded == true)
        {
            return;
        }

        hasExploded = true;
        Vector3 center = transform.position;

        Collider[] colliders = Physics.OverlapSphere(center, explosionRadius);
        foreach(Collider hitCollider in colliders)
        {
            EnemyHealth enemyHealth = hitCollider.GetComponent<EnemyHealth>();
            if(enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            Rigidbody hitRigidbody = hitCollider.GetComponent<Rigidbody>();
            if(hitRigidbody != null && hitRigidbody.isKinematic == false)
            {
                hitRigidbody.AddExplosionForce(explosionForce, center, explosionRadius, 0.5f, ForceMode.Impulse);
            }
        }

        if(explosionEffectPrefab != null)
        {
            GameObject effect = Instantiate(explosionEffectPrefab, center, Quaternion.identity);

            Destroy(gameObject, 2.0f);
        }

        Destroy(gameObject);
    }
}
