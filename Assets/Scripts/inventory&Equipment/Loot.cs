using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    public ItemSO itemSO;
    public SpriteRenderer sr;
    public Animator anim;

    public bool canBePickedUp;
    private bool isPicked = false;
    public int quantity;
    public static event Action<ItemSO, int> OnItemLooted;

    private void OnValidate()
    {
        if (itemSO == null)
            return;

        sr.sprite = itemSO.icon;
        this.name = itemSO.itemName;
    }

    public void Initialize(ItemSO itemSO, int quantity, bool canBePickedUp) 
    {
        this.itemSO = itemSO; //아이템 스크립터블 객체 저장
        this.quantity = quantity; //수량 조절
        this.canBePickedUp = canBePickedUp; //줍기 설정
        UpdateAppearance();
    }

    private void UpdateAppearance()
    {
        sr.sprite = itemSO.icon;
        this.name = itemSO.itemName;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPicked) return;

        if (collision.CompareTag("Player") && canBePickedUp == true)
        {
            isPicked = true;
            GetComponent<Collider2D>().enabled = false; //중복 습득 방지

            anim.Play("LootPickup");
            OnItemLooted?.Invoke(itemSO, quantity);
            Destroy(gameObject, .5f);
        }
    }
    private void OnTriggerExit2D(Collider2D collision) 
    {
        if (collision.CompareTag("Player"))
        {
            canBePickedUp = true;
        }
    }
}
