using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    [TextArea]public string itemDescription;
    public Sprite icon;
    public ItemCategory itemCategory;
    public ItemType itemType;

    public bool isGold;
    public int stackSize;

    [Header("Stats")]
    public int currentHealth;
    public int maxHealth;
    public int speed;
    public int damage;

    [Header("For Temporary Items")]
    public float duration;

    [Header("Equipment Sprites")] //스프라이트 형태가 다르기때문에 각각조정
    public ArmorSprites armorSprites;
    [Header("Equipment Sprites")]
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
