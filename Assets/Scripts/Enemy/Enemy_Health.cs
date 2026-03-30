using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class Enemy_Health : MonoBehaviour
{
    public int ExpReward = 3;

    public delegate void MonsterDefeated(int exp);
    public static event MonsterDefeated OnMonsterDefeated;

    public GameObject damageTextPrefab;
    public GameObject deathEffect;

    public int currentHealth;
    public int maxHealth;

    public Slider hpSlider;
    public GameObject hpBar;
    float targetHP;

    public List<LootItem> lootTable;
    public GameObject lootPrefab; //아이템 정보

    public event Action Ondeath;

    private void Start()
    {
        currentHealth = maxHealth;
        hpSlider.maxValue = maxHealth;
        hpSlider.value = maxHealth;
        targetHP = maxHealth;

        hpBar.SetActive(false); //처음에는 숨김
    }
    void Update()
    {
        hpSlider.value = Mathf.Lerp(hpSlider.value, targetHP, Time.deltaTime * 10f);
    }

    public void ChangeHealth(int amount)
    {
        hpBar.SetActive(true); //맞으면 표시
        currentHealth += amount;
        ShowDamage(Mathf.Abs(amount));
        targetHP = currentHealth;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if(currentHealth <= 0)
        {
            Ondeath?.Invoke();
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            OnMonsterDefeated(ExpReward);
            DropLoot();
            Destroy(gameObject);
        }
    }
    void ShowDamage(int damage)
    {
        GameObject dmg = Instantiate(damageTextPrefab, transform.position, Quaternion.identity);
        dmg.GetComponent<DamageText>().SetDamage(damage);
    }

    public void DropLoot()
    {
        foreach (var loot in lootTable)
        {
            if (Random.value <= loot.dropChance)
            {
                int amount = Random.Range(loot.quantityRange.x, loot.quantityRange.y+1);
                Vector3 offset = Random.insideUnitCircle * 1f;
                GameObject obj = Instantiate(lootPrefab, transform.position + offset, Quaternion.identity);
                obj.GetComponent<Loot>().Initialize(loot.itemSO, amount, true); 
            }
        }
    }
}

[System.Serializable]
public class LootItem //아이템 드랍 정보 클래스
{
    public ItemSO itemSO; //아이템 정보
    public Vector2Int quantityRange; //아이템 수량
    [Range(0f, 1f)] public float dropChance; //획득확률
}


