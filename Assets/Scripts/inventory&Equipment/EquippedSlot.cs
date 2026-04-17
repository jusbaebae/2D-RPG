using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquippedSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private InventoryManager inventoryManager;

    //슬롯 외형//
    [SerializeField]
    private Image slotImage;
    [SerializeField]
    private Sprite DefaultslotImage; //기존 슬롯 이미지

    public ItemSO equippedItem; //기존 아이템 정보

    [SerializeField]
    private SpriteRenderer[] playerDisplayImage; //Preview 이미지 변경
    [SerializeField]
    private SpriteRenderer[] playerImage; //Player 이미지 변경

    private bool slotuse = false;
    private void Start()
    {
        DefaultslotImage = slotImage.sprite; //기존 슬롯 이미지 저장
    }

    public void EquipGearImage(ItemSO item)
    {
        if (item == null) return;
        if (slotuse) //장비 교체
        {
            inventoryManager.AddItem(equippedItem, 1);
        }

        equippedItem = item; //현재 장착된 장비 기억하기
        slotImage.sprite = item.icon;

        switch (item.itemType)
        {
            case ItemType.armor:
                playerDisplayImage[0].sprite = item.armorSprites.body;
                playerDisplayImage[1].sprite = item.armorSprites.leftArm;
                playerDisplayImage[2].sprite = item.armorSprites.rightArm;
                playerImage[0].sprite = item.armorSprites.body;
                playerImage[1].sprite = item.armorSprites.leftArm;
                playerImage[2].sprite = item.armorSprites.rightArm;
                break;

            case ItemType.bottom:
                playerDisplayImage[0].sprite = item.bottomSprites.leftLeg;
                playerDisplayImage[1].sprite = item.bottomSprites.rightLeg;
                playerImage[0].sprite = item.bottomSprites.leftLeg;
                playerImage[1].sprite = item.bottomSprites.rightLeg;
                break;

            default:
                playerDisplayImage[0].sprite = item.icon;
                playerImage[0].sprite = item.icon;
                break;
        }
        slotuse = true;
    }
    public void UnEquipGearImage(ItemSO item)
    {
        if (item == null) return;

        slotImage.sprite = DefaultslotImage;
        inventoryManager.AddItem(equippedItem, 1);

        switch (item.itemType)
        {
            case ItemType.armor:
                playerDisplayImage[0].sprite = null;
                playerDisplayImage[1].sprite = null;
                playerDisplayImage[2].sprite = null;
                playerImage[0].sprite = null;
                playerImage[1].sprite = null;
                playerImage[2].sprite = null;
                break;

            case ItemType.bottom:
                playerDisplayImage[0].sprite = null;
                playerDisplayImage[1].sprite = null;
                playerImage[0].sprite = null;
                playerImage[1].sprite = null;
                break;

            default:
                playerDisplayImage[0].sprite = null;
                playerImage[0].sprite = null;
                break;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!slotuse || equippedItem == null)
            return;

        if (eventData.clickCount == 2)
        {
            UnEquipGearImage(equippedItem);

            equippedItem = null;
            slotuse = false;
        }
    }
}
