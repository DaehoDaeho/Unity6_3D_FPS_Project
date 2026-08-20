using System;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Potion,
    AmmoBox
}

[Serializable]
public class ItemData
{
    public int id;
    public ItemType itemType;
    public string name;
    public GameObject itemPrefab;
    public int value;
}

[CreateAssetMenu(fileName = "ItemDataSO", menuName = "Data/Item")]
public class ItemDataSO : ScriptableObject
{
    public List<ItemData> listItemDatas = new List<ItemData>();
    private Dictionary<int, ItemData> dicItemDatas = new Dictionary<int, ItemData>();

    public void InitDicItemDatas()
    {
        foreach(ItemData data in listItemDatas)
        {
            dicItemDatas.Add(data.id, data);
        }
    }

    public ItemData GetItemData(int id)
    {
        if(dicItemDatas.ContainsKey(id) == false)
        {
            return null;
        }

        return dicItemDatas[id];
    }
}
