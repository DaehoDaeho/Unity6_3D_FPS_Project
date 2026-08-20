using System.Collections;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Collider myCollider;
    [SerializeField] private float enabledTime = 5.0f;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") == false)
        {
            return;
        }

        if(ItemManager.Instance != null)
        {
            ItemManager.Instance.ApplyItem(id);
            meshRenderer.enabled = false;
            myCollider.enabled = false;
            StartCoroutine(WaitForEnableItem());
        }
    }

    IEnumerator WaitForEnableItem()
    {
        yield return new WaitForSeconds(enabledTime);

        meshRenderer.enabled = true;
        myCollider.enabled = true;
    }
}
