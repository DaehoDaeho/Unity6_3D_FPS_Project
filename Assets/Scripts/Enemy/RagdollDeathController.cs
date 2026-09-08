using UnityEngine;

public class RagdollDeathController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private Rigidbody[] ragdollBodies;
    private Collider[] ragdollColliders;

    private bool isDead;

    private void Awake()
    {
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        SetRagdoll(false);

    }

    void SetRagdoll(bool active)
    {
        for(int i=0; i<ragdollBodies.Length; ++i)
        {
            ragdollBodies[i].isKinematic = !active;
        }

        for(int i=0; i<ragdollColliders.Length; ++i)
        {
            ragdollColliders[i].enabled = active;
        }
    }

    public void Die()
    {
        if(isDead == true)
        {
            return;
        }

        isDead = true;
        animator.enabled = false;
        SetRagdoll(true);
    }
}
