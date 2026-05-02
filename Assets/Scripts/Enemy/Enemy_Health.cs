using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class Enemy_Health : MonoBehaviour
{
    public Room room;

    public string Enemy_name;
    public int ExpReward;

    public static event Action<int>  OnMonsterDefeated;
    public event Action Ondeath;

    public GameObject damageTextPrefab;
    public GameObject deathEffect;

    public int currentHealth;
    public int maxHealth;

    public Slider hpSlider;
    public GameObject hpBar;
    protected float targetHP;

    public List<LootItem> lootTable;
    public GameObject lootPrefab; //아이템 정보

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

    private void OnDestroy()
    {
        QuestManager.Instance.AddProgress(QuestType.KillMonster, Enemy_name, 1);
        //Debug.Log(Enemy_name + "잡았다");
    }
    public void Init(Room room)
    {
        this.room = room;
    }

    public virtual void ChangeHealth(int amount)
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
            InvokeDeath();
        }
    }
    protected void ShowDamage(int damage)
    {
        GameObject dmg = Instantiate(damageTextPrefab, transform.position, Quaternion.identity);
        dmg.GetComponent<DamageText>().SetDamage(damage,StatsManager.Instance.isCrit);
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
                obj.GetComponent<ItemPop>().Pop();
            }
        }
    }

    public void InvokeDeath() //죽었는지 확인하기
    {
        Ondeath?.Invoke();
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        OnMonsterDefeated(ExpReward);
        DropLoot();
        room.OnMonsterDead();
        room.spawnedMonsters.Remove(this);
        Destroy(gameObject);
    }
}

[System.Serializable]
public class LootItem //아이템 드랍 정보 클래스
{
    public ItemSO itemSO; //아이템 정보
    public Vector2Int quantityRange; //아이템 수량
    [Range(0f, 1f)] public float dropChance; //획득확률
}


