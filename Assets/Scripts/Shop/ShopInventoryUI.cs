using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopInventoryUI : MonoBehaviour
{
    [Header("아이템,장비 패널")]
    public GameObject itemPanel; // 24칸짜리 아이템 패널
    public GameObject equipPanel; // 16칸짜리 장비 패널

    [Header("아이템,장비 탭")]
    public CanvasGroup itemTabCG; // 아이템 탭 버튼의 CanvasGroup
    public CanvasGroup equipTabCG; // 장비 탭 버튼의 CanvasGroup

    public TextMeshProUGUI goldTXT;

    void Update()
    {
        goldTXT.text = InventoryManager.Instance.gold.ToString();
    }

    public void ShowItemTab()
    {
        itemPanel.SetActive(true);
        equipPanel.SetActive(false);

        // 시각적 효과 (선택된 탭은 진하게, 아닌 건 반투명하게)
        itemTabCG.alpha = 1f;
        equipTabCG.alpha = 0.5f;

        // 데이터 갱신 로직 (여기서 인벤토리 아이템을 슬롯에 뿌려줌)
        UpdateItemSlots(itemPanel, InventoryManager.Instance.itemSlots);
    }

    public void ShowEquipTab()
    {
        itemPanel.SetActive(false);
        equipPanel.SetActive(true);

        itemTabCG.alpha = 0.5f;
        equipTabCG.alpha = 1f;

        UpdateEquipSlots(equipPanel, InventoryManager.Instance.equipmentSlots);
    }

    public void UpdateItemSlots(GameObject panel, InventorySlot[] originalSlots)
    {
        //해당 패널(ItemPanel 또는 EqPanel)에 있는 상점용 슬롯들을 다 가져옴
        InventorySlot[] shopSlots = panel.GetComponentsInChildren<InventorySlot>();

        //상점 슬롯들을 돌면서 원본 데이터와 동기화
        for (int i = 0; i < shopSlots.Length; i++)
        {
            //원본 인벤토리/장비창에 해당 인덱스의 아이템이 있다면
            if (i < originalSlots.Length && originalSlots[i].itemSO != null)
            {
                //상점 슬롯에 아이템 정보 복사
                shopSlots[i].itemSO = originalSlots[i].itemSO;
                shopSlots[i].quantity = originalSlots[i].quantity;
                shopSlots[i].UpdateUI(); // UI 새로고침
            }
            else
            {
                //아이템이 없는 빈 슬롯 처리
                shopSlots[i].itemSO = null;
                shopSlots[i].quantity = 0;
                shopSlots[i].UpdateUI();
            }
        }
    }

    public void UpdateEquipSlots(GameObject panel, EquipmentSlot[] originalSlots)
    {
        //해당 패널(ItemPanel 또는 EqPanel)에 있는 상점용 슬롯들을 다 가져옴
        EquipmentSlot[] shopSlots = panel.GetComponentsInChildren<EquipmentSlot>();

        //상점 슬롯들을 돌면서 원본 데이터와 동기화
        for (int i = 0; i < shopSlots.Length; i++)
        {
            //원본 인벤토리/장비창에 해당 인덱스의 아이템이 있다면
            if (i < originalSlots.Length && originalSlots[i].itemSO != null)
            {
                //상점 슬롯에 아이템 정보 복사
                shopSlots[i].itemSO = originalSlots[i].itemSO;
                shopSlots[i].quantity = originalSlots[i].quantity;
                shopSlots[i].UpdateUI(); // UI 새로고침
            }
            else
            {
                //아이템이 없는 빈 슬롯 처리
                shopSlots[i].itemSO = null;
                shopSlots[i].quantity = 0;
                shopSlots[i].UpdateUI();
            }
        }
    }

    public void RefreshCurrentTab()
    {
        if (itemPanel.activeSelf)
            UpdateItemSlots(itemPanel, InventoryManager.Instance.itemSlots);
        else
            UpdateEquipSlots(equipPanel, InventoryManager.Instance.equipmentSlots);
    }
}

