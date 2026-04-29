using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class itemInfo : MonoBehaviour
{
    public CanvasGroup infoPanel;

    public TMP_Text itemNameText;
    public TMP_Text itemTypeText;
    public TMP_Text itemDescriptionText;
    public TMP_Text sellgoldText;

    [Header("Stats Fields")]
    public TMP_Text[] statTexts;

    private RectTransform infoPanelRect;

    private void Awake()
    {
        infoPanelRect = GetComponent<RectTransform>();
    }

    public void ShowItemInfo(ItemSO itemSO)
    {
        infoPanel.alpha = 1;

        itemNameText.text = itemSO.itemName;
        if(itemSO.itemCategory == ItemCategory.Equipment)
        {
            itemTypeText.text = "장비";
        }
        else if(itemSO.itemCategory == ItemCategory.Consumable)
        {
            itemTypeText.text = "소모품";
        }
        else if (itemSO.itemCategory == ItemCategory.Collectable)
        {
            itemTypeText.text = "기타";
        }
        itemDescriptionText.text = itemSO.itemDescription;
        sellgoldText.text = itemSO.saleprice.ToString();

        List<string> stats = new List<string>();
        if (itemSO.maxHealth > 0) stats.Add("최대체력\n+" + itemSO.maxHealth.ToString());
        if (itemSO.currentHealth > 0) stats.Add("체력\n+" + itemSO.currentHealth.ToString());
        if (itemSO.damage > 0) stats.Add("공격력\n+" + itemSO.damage.ToString());
        if (itemSO.speed > 0) stats.Add("이동속도\n+" + itemSO.speed.ToString());
        if (itemSO.defense > 0) stats.Add("방어력\n+" + itemSO.defense.ToString());
        if (itemSO.crit > 0) stats.Add("치명타 확률\n+" + itemSO.crit.ToString());

        if (stats.Count <= 0)
            return;

        for (int i = 0; i < statTexts.Length; i++)
        {
            if (i < stats.Count)
            {
                statTexts[i].text = stats[i];
                statTexts[i].gameObject.SetActive(true);
            }
            else
            {
                statTexts[i].gameObject.SetActive(false);
            }
        }
    }

    public void HideItemInfo()
    {
        infoPanel.alpha = 0;

        itemNameText.text = "";
        itemDescriptionText.text = "";
    }

    public void FollowMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 offset = new Vector3(50, 50, 0);

        infoPanelRect.position = mousePosition + offset;
    }
}
