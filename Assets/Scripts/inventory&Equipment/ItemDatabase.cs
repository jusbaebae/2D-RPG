using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;
    private Dictionary<string, ItemSO> dict;
    public List<ItemSO> equipments;
    public List<ItemSO> consumables;
    public List<ItemSO> CollectItems;

    private void Awake()
    {
        Instance = this;

        dict = new Dictionary<string, ItemSO>();

        foreach (var item in equipments)
            dict[item.itemName] = item;

        foreach (var item in consumables)
            dict[item.itemName] = item;

        foreach (var item in CollectItems)
            dict[item.itemName] = item;
    }

    public ItemSO Get(string id)
    {
        if (id == null) return null;

        if (dict.ContainsKey(id))
            return dict[id];

        Debug.LogWarning($"아이템 못찾음: {id}");
        return null;
    }
}
