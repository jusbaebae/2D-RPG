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
        if (itemSO.crit > 0) stats.Add("치명타확률\n+" + itemSO.crit.ToString());

        for (int i = 0; i < statTexts.Length; i++)
        {
            if (i < stats.Count)
            {
                statTexts[i].text = stats[i];
                statTexts[i].gameObject.SetActive(true);
            }
            else
            {
                statTexts[i].text = "";
                statTexts[i].gameObject.SetActive(false);
            }
        }
    }

    public void HideItemInfo()
    {
        infoPanel.alpha = 0;

        itemNameText.text = "";
        itemDescriptionText.text = "";

        foreach (TMP_Text stat in statTexts)
        {
            stat.text = "";
            stat.gameObject.SetActive(false);
        }
    }

    public void FollowMouse()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 offset = new Vector2(50, 50);

        Vector2 panelSize = Vector2.Scale(infoPanelRect.rect.size, infoPanelRect.lossyScale);
        Vector2 targetPos = mousePosition + new Vector2(offset.x, offset.y);

        // 오른쪽 화면 밖 체크
        if (targetPos.x + panelSize.x > Screen.width)
        {
            targetPos.x = mousePosition.x - panelSize.x - offset.x;
        }

        // 아래쪽 화면 밖 체크
        if (targetPos.y - panelSize.y < 0)
        {
            targetPos.y = panelSize.y;
        }

        infoPanelRect.position = targetPos;
    }
}
