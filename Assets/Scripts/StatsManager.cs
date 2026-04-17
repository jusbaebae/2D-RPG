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

    [Header("Combat Stats")]
    public int damage;
    public int crit;
    public bool isCrit;
    public float weaponRange;
    public float knockbackForce;
    public float knockbackTime;
    public float stunTime;

    [Header("Movement Stats")]
    public int speed;

    [Header("Health Stats")]
    public int maxHealth;
    public int currentHealth;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void UpdateMaxHealth(int amount)
    {
        maxHealth += amount;
        healthtext.text = currentHealth + "/ " + maxHealth;
        hpbar.SetMaxHealth(maxHealth);
    }

    public void UpdateHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth >= maxHealth)
            currentHealth = maxHealth;

        hpbar.SetHealth(currentHealth);
        healthtext.text = currentHealth + " / " + maxHealth;
    }

    public void UpdateSpeed(int amount)
    {
        speed += amount;
        statsUI.UpdateAllStats();
    }

    public void CritCheck()
    {
        int critcheck = Mathf.Min(StatsManager.Instance.crit, 100);
        isCrit = Random.Range(0, 100) < critcheck;
    }

    public void FillData(PlayerData data)
    {
        data.currenthp = currentHealth;
        data.maxhp = maxHealth;
        data.damage = damage;
        data.crit = crit;
        data.speed = speed;
    }
}
