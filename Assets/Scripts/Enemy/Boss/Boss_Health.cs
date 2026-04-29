using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Health : Enemy_Health
{
    private Boss_Movement boss_Movement;
    void Awake()
    {
        boss_Movement = GetComponent<Boss_Movement>();
    }

    public override void ChangeHealth(int amount)
    {
        hpBar.SetActive(true); //맞으면 표시
        currentHealth += amount;
        ShowDamage(Mathf.Abs(amount));
        targetHP = currentHealth;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if (currentHealth <= 0)
        {
            //사망
            boss_Movement.isDead = true;
        }
    }
}
