using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    public ItemSO itemSO;
    public SpriteRenderer sr;
    public Animator anim;
    private CircleCollider2D col;

    public bool canBePickedUp;
    private bool isPicked = false;
    public int quantity;
    public static event Action<ItemSO, int> OnItemLooted;

    private void OnValidate()
    {
        if (itemSO == null)
            return;

        sr.sprite = itemSO.icon;
        name = itemSO.itemName;


        ApplyCollider();
    }

    public void Initialize(ItemSO itemSO, int quantity, bool canBePickedUp) 
    {
        this.itemSO = itemSO; //아이템 스크립터블 객체 저장
        this.quantity = quantity; //수량 조절
        this.canBePickedUp = canBePickedUp; //줍기 설정
        UpdateAppearance();
        ApplyCollider();
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
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.Drop);
            OnItemLooted?.Invoke(itemSO, quantity);
            Destroy(gameObject, .5f);
        }

        //Debug.Log($"{collision.name}과 충돌함!");
    }
    private void OnTriggerExit2D(Collider2D collision) 
    {
        if (collision.CompareTag("Player"))
        {
            canBePickedUp = true;
        }
    }

    void ApplyCollider() //아이템 스프라이트이미지에 맞게 범위 설정
    {
        if (col == null) col = GetComponent<CircleCollider2D>();
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();
        if (col == null || sr == null || sr.sprite == null) return;

        float width = sr.sprite.rect.width / sr.sprite.pixelsPerUnit;
        float height = sr.sprite.rect.height / sr.sprite.pixelsPerUnit;

        float maxDimension = Mathf.Max(width, height);
        col.radius = maxDimension / 2f;

        //중심점 맞추기
        Vector2 spritePivot = sr.sprite.pivot / sr.sprite.pixelsPerUnit;
        Vector2 spriteCenter = new Vector2(width / 2f, height / 2f);
        col.offset = spriteCenter - spritePivot;

        //Debug.Log("콜라이더 생성 : " + col.radius);
    }
}
