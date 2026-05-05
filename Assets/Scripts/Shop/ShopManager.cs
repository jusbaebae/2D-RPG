using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [SerializeField] private ShopSlot[] shopSlots;

    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private ShopInventoryUI shopUI;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); //이 객체를 유지
        }
        else
        {
            Destroy(gameObject); //이미 있으면 새로 생긴 건 삭제
            return;
        }
    }

    public void PopulateShopItems(List<ShopItems> shopItems)
    {
        for (int i = 0; i < shopItems.Count && i < shopSlots.Length; i++)
        {
            ShopItems shopItem = shopItems[i];
            shopSlots[i].Initialized(shopItem.itemSO, shopItem.price);
            shopSlots[i].gameObject.SetActive(true);
        }

        for (int i = shopItems.Count; i < shopSlots.Length; i++)
        {
            shopSlots[i].gameObject.SetActive(false);
        }
    }

    public void TryBuyItem(ItemSO itemSO, int price, int amount)
    {
        if (itemSO != null && inventoryManager.gold >= price)
        {
            if (HasSpaceForItem(itemSO))
            {
                inventoryManager.gold -= price * amount;
                inventoryManager.goldText.text = inventoryManager.gold.ToString();
                inventoryManager.AddItem(itemSO, amount);
                if (shopUI != null)
                {
                    // 현재 활성화된 탭에 맞춰 리프레시
                    shopUI.RefreshCurrentTab();
                }
            }
        }
    }

    private bool HasSpaceForItem(ItemSO itemSO)
    {
        foreach (var slot in inventoryManager.itemSlots)
        {
            if (slot.itemSO == itemSO && slot.quantity < itemSO.stackSize)
                return true;
            else if (slot.itemSO == null)
                return true;
        }
        return false;
    }

    public void SellItem(ItemSO itemSO, int count)
    {
        if (itemSO == null) return;

        foreach(var slot in inventoryManager.itemSlots)
        {
            if(slot.itemSO == itemSO)
            {
                ProcessSale(slot, count, itemSO);
                return;
            }
        }

        foreach (var eqSlot in inventoryManager.equipmentSlots)
        {
            if (eqSlot.itemSO == itemSO)
            {
                ProcessSale(eqSlot, count, itemSO);
                return;
            }
        }
    }

    private void ProcessSale(ItemSlot slot, int count, ItemSO itemSO) //아이템 팔기
    {
        slot.quantity -= count;
        inventoryManager.gold += itemSO.saleprice * count;
        inventoryManager.goldText.text = inventoryManager.gold.ToString();
        slot.UpdateUI();
        if (shopUI != null)
        {
            // 현재 활성화된 탭에 맞춰 리프레시
            shopUI.RefreshCurrentTab();
        }
        QuestManager.Instance.CheckCollectQuests();
    }
}

    [System.Serializable]
    public class ShopItems
{
    public ItemSO itemSO;
    public int price;
}


