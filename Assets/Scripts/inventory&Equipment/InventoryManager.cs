using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    //아이템 슬롯//
    public InventorySlot[] itemSlots;

    //장비 아이템슬롯//
    public EquipmentSlot[] equipmentSlots;

    //장비 슬롯//
    [SerializeField]
    private EquippedSlot helmetSlot, ArmorSlot, BottomSlot, weaponSlot;

    public UseItem useItem;
    public int gold;
    public TMP_Text goldText;
    public GameObject lootPrefab;
    public Transform player;
    public Image dragIcon;

    ItemSlot selectedSlot;

    private void Start()
    {
        foreach(var slot in itemSlots)
        {
            slot.UpdateUI();
        }
        foreach (var slot in equipmentSlots)
        {
            slot.UpdateUI();
        }
    }
    private void OnEnable()
    {
        Loot.OnItemLooted += AddItem;
        DialogueManager.OnRewardGold += AddGold;

    }

    private void OnDisable()
    {
        Loot.OnItemLooted -= AddItem;
        DialogueManager.OnRewardGold -= AddGold;
    }

    public void AddItem(ItemSO itemSO, int quantity)
    {
        if (itemSO.isGold) //아이템이 돈일경우(나중에 분리하기)
        {
            gold += quantity;
            goldText.text = gold.ToString();
            return;
        }

        if (itemSO.itemCategory == ItemCategory.Consumable || itemSO.itemCategory == ItemCategory.Collectable) //소비아이템이거나 기타아이템일때
        {
            foreach (var slot in itemSlots)
            {
                if (slot.itemSO == itemSO && slot.quantity < itemSO.stackSize) //같은아이템일시 먼저 스택채우기
                {
                    int availableSpace = itemSO.stackSize - slot.quantity; //남은 공간
                    int amountToAdd = Mathf.Min(availableSpace, quantity); //최대개수 초과 방지

                    slot.quantity += amountToAdd;
                    quantity -= amountToAdd;

                    slot.UpdateUI();
                    if (quantity <= 0)
                        return;
                }
            }

            foreach (var slot in itemSlots)
            {
                if (slot.itemSO == null) //아이템이 남았거나 다른아이템일시 다음 슬롯으로 채우기
                {
                    int amountToAdd = Mathf.Min(itemSO.stackSize, quantity);
                    slot.itemSO = itemSO;
                    slot.quantity = amountToAdd;
                    slot.UpdateUI();
                    quantity -= amountToAdd;

                    if (quantity <= 0)
                        return;
                }
            }

            if (quantity > 0)
                DropLoot(itemSO, quantity);
        }
        else //장비아이템일때
        {
            foreach (var slot in equipmentSlots)
            {
                if (slot.itemSO == null)
                {
                    slot.itemSO = itemSO;
                    slot.quantity = 1;
                    slot.UpdateUI();
                    return;
                }
            }

            DropLoot(itemSO, 1);
        }
    }

    public void AddGold(int quantity)
    {
        gold += quantity;
        goldText.text = gold.ToString();
    }

    public void DropItem(ItemSlot slot) 
    {
        DropLoot(slot.itemSO, 1);
        slot.quantity--;
        if(slot.quantity <= 0)
        {
            slot.itemSO = null;
            slot.quantity = 0;
        }
        slot.UpdateUI();
    }

    private void DropLoot(ItemSO itemSO, int quantity) //아이템이 들어갈 공간이없을시 다시 내려놓기
    {
        Loot loot = Instantiate(lootPrefab, player.position, Quaternion.identity).GetComponent<Loot>();
        loot.Initialize(itemSO, quantity, false);
    }

    public void SwapItems(ItemSlot a, ItemSlot b) //슬롯끼리 아이템 교환하기
    {
        ItemSO tempItem = a.itemSO;
        int tempQuantity = a.quantity;

        a.itemSO = b.itemSO;
        a.quantity = b.quantity;

        b.itemSO = tempItem;
        b.quantity = tempQuantity;

        a.UpdateUI();
        b.UpdateUI();

        DeselectItem();
        selectedSlot = b;
        b.Select();
    }

    public void OnSlotClicked(ItemSlot slot, int clickCount)
    {
        if (slot.itemSO == null)
        {
            return;
        }

        if (clickCount == 2) //더블클릭
        {
            if(slot.itemSO.itemCategory == ItemCategory.Consumable) //아이템이 소모품이면 UseItem()
            {
                if (slot.itemSO.currentHealth > 0 && StatsManager.Instance.currentHealth >= StatsManager.Instance.maxHealth)
                    return;

                UseItem(slot);
                slot.Deselect();
                selectedSlot = null;
                return;
            }

            if (slot.itemSO.itemCategory == ItemCategory.Equipment) //아이템이 장비템이면 EquipGear()
            {
                EquipGear(slot, slot.itemSO.itemType);
                slot.Deselect();
                selectedSlot = null;
                return;
            }
        }

        if (selectedSlot != null && selectedSlot != slot) //다른곳 아이템 체크하면 해당아이템으로 체크
            selectedSlot.Deselect();

        selectedSlot = slot;
        selectedSlot.Select();
    }
    public void UseItem(ItemSlot slot)
    {
        if (slot.itemSO != null && slot.quantity > 0)
        {
            useItem.ApplyItemEffects(slot.itemSO);

            slot.quantity--;
            if (slot.quantity <= 0)
            {
                slot.itemSO = null;
            }
            slot.UpdateUI();
        }
    }

    public void EquipGear(ItemSlot slot, ItemType itemtype) 
    {
        if (itemtype == ItemType.helmet)
            helmetSlot.EquipGearImage(slot.itemSO);
        if (itemtype == ItemType.armor)
            ArmorSlot.EquipGearImage(slot.itemSO);
        if (itemtype == ItemType.bottom)
            BottomSlot.EquipGearImage(slot.itemSO);
        if (itemtype == ItemType.weapon)
            weaponSlot.EquipGearImage(slot.itemSO);

        slot.quantity--;
        if (slot.quantity <= 0)
        {
            slot.itemSO = null;
        }
        slot.UpdateUI();
    }
    public void DeselectItem()
    {
        if (selectedSlot != null)
        {
            selectedSlot.Deselect();
            selectedSlot = null;
        }
    }

    public InventoryData GetSaveData()
    {
        InventoryData data = new InventoryData();

        // 인벤토리
        data.items = new List<InventoryItemData>();
        foreach (var slot in itemSlots)
        {
            data.items.Add(new InventoryItemData
            {
                itemId = slot.itemSO != null ? slot.itemSO.itemName : null,
                count = slot.quantity
            });
        }

        // 골드
        data.gold = gold;

        return data;
    }

    public EquipSaveData GetEquipSaveData()
    {
        EquipSaveData equipdata = new EquipSaveData();
        // 장비
        equipdata = new EquipSaveData
        {
            helmetId = helmetSlot.equippedItem?.itemName,
            armorId = ArmorSlot.equippedItem?.itemName,
            bottomId = BottomSlot.equippedItem?.itemName,
            weaponId = weaponSlot.equippedItem?.itemName
        };

        return equipdata;
    }
}

public enum ItemCategory //아이템 카테고리
{
    Consumable,
    Equipment,
    Collectable
}
public enum ItemType //아이템 타입
{
    helmet,
    armor,
    bottom,
    weapon,
    potion,
    none
};
