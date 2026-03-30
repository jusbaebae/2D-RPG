using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public TMP_Text healthText;
    public HPBar hpbar;

    void Start()
    {
        StatsManager.Instance.currentHealth = StatsManager.Instance.maxHealth;
        healthText.text = StatsManager.Instance.currentHealth + " / " + StatsManager.Instance.maxHealth;
        hpbar.SetMaxHealth(StatsManager.Instance.maxHealth);
        hpbar.SetHealth(StatsManager.Instance.currentHealth);
    }
    public void ChangeHealth(int amount)
    {
        StatsManager.Instance.currentHealth += amount;
        healthText.text = StatsManager.Instance.currentHealth + " / " + StatsManager.Instance.maxHealth;
        hpbar.SetHealth(StatsManager.Instance.currentHealth);

        if (StatsManager.Instance.currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        gameObject.SetActive(false);
    }
}
