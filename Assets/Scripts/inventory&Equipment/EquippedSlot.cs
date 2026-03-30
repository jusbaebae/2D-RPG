using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquippedSlot : MonoBehaviour
{
    //슬롯 외형//
    [SerializeField]
    private Image slotImage;

    [SerializeField]
    private SpriteRenderer[] playerDisplayImage; //Preview 이미지 변경
    [SerializeField]
    private SpriteRenderer[] playerImage; //Player 이미지 변경

    //슬롯 데이터//
    [SerializeField]
    private ItemType itemType = new ItemType();

    //다른 데이터//
    private bool slotInUse;

    public void EquipGear(ItemSO item)
    {
        if (item == null) return;

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

        slotInUse = true;
    }
}
