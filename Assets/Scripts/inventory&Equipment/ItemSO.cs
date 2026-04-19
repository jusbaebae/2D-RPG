using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item")]
public class ItemSO : ScriptableObject
{
    [Header("아이템 정보")]
    public string itemName;
    [TextArea]public string itemDescription;
    public Sprite icon;
    public ItemCategory itemCategory;
    public ItemType itemType;

    public bool isGold;
    public int stackSize;

    [Header("아이템 능력치")]
    public int currentHealth;
    public int maxHealth;
    public int speed;
    public int damage;

    [Header("지속 시간")]
    public float duration;

    [Header("장비 스프라이트_갑옷")] //스프라이트 형태가 다르기때문에 각각조정
    public ArmorSprites armorSprites;
    [Header("장비 스프라이트_신발")]
    public BottomSprites bottomSprites;
}


[System.Serializable]
public class ArmorSprites //갑옷 스프라이트
{
    public Sprite body;
    public Sprite leftArm;
    public Sprite rightArm;
}

[System.Serializable]
public class BottomSprites //신발 스프라이트
{
    public Sprite leftLeg;
    public Sprite rightLeg;
}
