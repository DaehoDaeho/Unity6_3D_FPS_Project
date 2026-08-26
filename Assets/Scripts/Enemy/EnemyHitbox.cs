using UnityEngine;

public enum EnemyBodyPart
{
    Head,
    Body,
    Arm,
    Leg
}

public class EnemyHitbox : MonoBehaviour
{
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private EnemyBodyPart bodyPart = EnemyBodyPart.Head;
    [SerializeField] private float damageMultiplier = 1.0f;

    public EnemyHealth TargetHealth
    {
        get { return enemyHealth; }
    }

    private void Reset()
    {
        if(enemyHealth == null)
        {
            enemyHealth = GetComponentInParent<EnemyHealth>();
        }
    }

    public void TakeHit(int baseDamage)
    {
        if(enemyHealth == null || enemyHealth.IsDead == true)
        {
            return;
        }

        int finalDamage = Mathf.RoundToInt(baseDamage * damageMultiplier);
        finalDamage = Mathf.Max(finalDamage, 1);

        enemyHealth.TakeDamage(finalDamage);
    }

    public string GetBodyPartName()
    {
        return bodyPart.ToString();
    }
}
