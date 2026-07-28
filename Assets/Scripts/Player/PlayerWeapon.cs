using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private string weaponName = "Training Rifle";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CheckRequiredReferences();
        Debug.Log(weaponName + " is ready.");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CheckRequiredReferences()
    {
        if(firePoint == null)
        {
            Debug.LogWarning("FirePoint 가 연결되지 않았습니다.", this);
        }
    }
}
