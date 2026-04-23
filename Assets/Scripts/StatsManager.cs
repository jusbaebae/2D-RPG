using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;

    public StatsUi statsUI;
    public TMP_Text healthtext;
    public HPBar hpbar;

    public PlayerHealth playerHealth;

    [Header("공격력 스탯")]
    public int damage;
    public int crit;
    public int defense;
    public bool isCrit;
    public float weaponRange;
    public float knockbackForce;
    public float knockbackTime;
    public float stunTime;

    [Header("이동 스탯")]
    public int speed;

    [Header("체력 스탯")]
    public int maxHealth;
    public int currentHealth;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void UpdateMaxHealth(int amount) //최대 체력 업데이트
    {
        maxHealth += amount;
        healthtext.text = currentHealth + "/ " + maxHealth;
        hpbar.SetMaxHealth(maxHealth);
        statsUI.UpdateAllStats();
    }

    public void UpdateHealth(int amount) //체력 업데이트
    {
        currentHealth += amount;
        if (currentHealth >= maxHealth)
            currentHealth = maxHealth;

        hpbar.SetHealth(currentHealth);
        healthtext.text = currentHealth + " / " + maxHealth;
        statsUI.UpdateAllStats();
    }

    public void UpdateSpeed(int amount) //속도 업데이트
    {
        speed += amount;
        statsUI.UpdateAllStats();
    }

    public void CritCheck() //치명타 체크
    {
        int critcheck = Mathf.Min(crit, 100);
        isCrit = Random.Range(0, 100) < critcheck;
    }

    public void ShowHealText(int amount)
    {
        playerHealth.ShowHeal(amount);
    }

    public void FillData(PlayerData data) //세이브 데이터
    {
        data.currenthp = currentHealth;
        data.maxhp = maxHealth;
        data.damage = damage;
        data.crit = crit;
        data.speed = speed;
        data.defense = defense;
    }

    public void LoadFromData(PlayerData data) //로드 데이터
    {
        currentHealth = data.currenthp;
        maxHealth = data.maxhp;
        damage = data.damage;
        crit = data.crit;
        speed = data.speed;
        defense = data.defense;

        statsUI.UpdateAllStats();
    }
}
