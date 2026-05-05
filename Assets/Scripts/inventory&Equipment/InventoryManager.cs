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
                    QuestManager.Instance.CheckCollectQuests();
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

            if (quantity > 0) DropLoot(itemSO, quantity);
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

                    quantity--;

                    if (quantity <= 0)
                    {
                        QuestManager.Instance.CheckCollectQuests();
                        return;
                    }
                }
            }

            if (quantity > 0)
            {
                for (int i = 0; i < quantity; i++)
                {
                    DropLoot(itemSO, 1);
                }
            }
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
        QuestManager.Instance.CheckCollectQuests();
    }

    private void DropLoot(ItemSO itemSO, int quantity) //아이템이 들어갈 공간이없을시 다시 내려놓기
    {
        Loot loot = Instantiate(lootPrefab, player.position, Quaternion.identity).GetComponent<Loot>();
        loot.Initialize(itemSO, quantity, false);
        QuestManager.Instance.CheckCollectQuests();
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
    public void UseItem(ItemSlot slot) //아이템 사용
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
        Debug.Log("UseItem 발동!");

        QuestManager.Instance.CheckCollectQuests();
    }

    public void EquipGear(ItemSlot slot, ItemType itemtype)  //장비 장착
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
        QuestManager.Instance.CheckCollectQuests();
    }
    public void DeselectItem()
    {
        if (selectedSlot != null)
        {
            selectedSlot.Deselect();
            selectedSlot = null;
        }
    }

    public bool IsWeaponEquipped() //무기가 장착되었는지 확인하기
    {
        return weaponSlot.slotuse && weaponSlot.equippedItem != null && weaponSlot.equippedItem.itemType == ItemType.weapon;
    }

    public int GetItemCount(string itemId) //아이템 수량 가져오기
    {
        int count = 0;

        foreach (var item in itemSlots)
        {
            if (item.itemSO == null)
                continue;

            if (item.itemSO.itemName == itemId)
            {
                count += item.quantity;
            }
        }
        return count;
    }

    public void RemoveItem(string itemId, int amount)
    {
        int remaining = amount;

        foreach (var slot in itemSlots)
        {
            if (slot.itemSO == null)
                continue;

            if (slot.itemSO.itemName != itemId)
                continue;

            int removeAmount = Mathf.Min(slot.quantity, remaining); //현재 슬롯에서 제거가능한 양 구하기
            slot.quantity -= removeAmount;
            remaining -= removeAmount;

            slot.UpdateUI();

            if (remaining <= 0) //전부 제거 완료
                break;
        }
    }


    public InventoryData GetSaveItemData() //아이템 슬롯 저장
    {
        InventoryData data = new InventoryData();

        /*foreach (var slot in itemSlots)
        {
            Debug.Log($"슬롯: {slot.itemSO}, 개수: {slot.quantity}");
        }*/

        data.items = new List<InventoryItemData>();
        foreach (var slot in itemSlots)
        {
            data.items.Add(new InventoryItemData
            {
                itemId = slot.itemSO != null ? slot.itemSO.itemName : null,
                count = slot.quantity
            });
        }

        data.gold = gold;

        return data;
    }

    public void GetLoadItemData(InventoryData data) //아이템 슬롯 데이터 로드
    {
        //기존 슬롯 초기화
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].Clear();
        }

        for (int i = 0; i < data.items.Count; i++)
        {
            var itemData = data.items[i];

            if (itemData.itemId == null)
                continue;

            ItemSO item = ItemDatabase.Instance.Get(itemData.itemId); //아이템 데이터 베이스에서 id가져오기

            itemSlots[i].Set(item, itemData.count);
        }

        gold = data.gold;
        goldText.text = gold.ToString();
    }

    public EquipmentData GetSaveEquipItemData() //장비 슬롯 데이터 저장
    {
        EquipmentData data = new EquipmentData();

        /*foreach (var slot in equipmentSlots)
        {
            Debug.Log($"슬롯: {slot.itemSO}");
        }*/

        // 인벤토리
        data.equips = new List<EquipmentItemData>();
        foreach (var slot in equipmentSlots)
        {
            data.equips.Add(new EquipmentItemData
            {
                itemId = slot.itemSO != null ? slot.itemSO.itemName : null,
                count = slot.quantity
            }); 
        }
        return data;
    }

    public void GetLoadEquipItemData(EquipmentData data) //장비 슬롯 데이터 로드
    {
        // 기존 슬롯 초기화
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            equipmentSlots[i].Clear();
        }

        // 아이템 복구
        for (int i = 0; i < data.equips.Count; i++)
        {
            var itemData = data.equips[i];

            if (itemData.itemId == null)
                continue;

            ItemSO item = ItemDatabase.Instance.Get(itemData.itemId);

            equipmentSlots[i].Set(item, itemData.count);
        }
    }

    public EquipSaveData GetEquipSaveData() // 장착중인 장비 데이터 저장
    {
        EquipSaveData equipdata = new EquipSaveData();
        
        equipdata = new EquipSaveData
        {
            helmetId = helmetSlot.equippedItem?.itemName,
            armorId = ArmorSlot.equippedItem?.itemName,
            bottomId = BottomSlot.equippedItem?.itemName,
            weaponId = weaponSlot.equippedItem?.itemName
        };
        return equipdata;
    }

    public void GetLoadEquipData(EquipSaveData data) //장착중인 장비 데이터 로드
    {
        helmetSlot.LoadSet(ItemDatabase.Instance.Get(data.helmetId));
        ArmorSlot.LoadSet(ItemDatabase.Instance.Get(data.armorId));
        BottomSlot.LoadSet(ItemDatabase.Instance.Get(data.bottomId));
        weaponSlot.LoadSet(ItemDatabase.Instance.Get(data.weaponId));
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
