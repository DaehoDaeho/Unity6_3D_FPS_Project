using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    [SerializeField] private ItemDataSO itemDataSO;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerWeapon playerWeapon;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(itemDataSO != null)
        {
            itemDataSO.InitDicItemDatas();
        }
    }

    public void ApplyItem(int id)
    {
        ItemData itemData = itemDataSO.GetItemData(id);
        if(itemData != null)
        {
            if(itemData.itemType == ItemType.Potion)
            {
                // 체력 회복.
                playerHealth.Heal(itemData.value);
            }
            else if(itemData.itemType == ItemType.AmmoBox)
            {
                // 예비 탄약의 개수 증가.
                playerWeapon.AddReserveAmmo(itemData.value);
            }
        }
    }
}
