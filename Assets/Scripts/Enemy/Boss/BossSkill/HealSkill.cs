using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealSkill : BossSkill
{
    bool isHeal;

    public Boss_Health boss_health;
    public GameObject HealEffect;
    public float cooldown;
    private float lastUseTime = -999f;

    public Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public override IEnumerator UseSkill(Boss_Movement boss)
    {
        isHeal = true;
        boss.isImmuneToKnockback = true;
        boss.isUsingSkill = true;
        boss.rb.velocity = Vector2.zero;

        lastUseTime = Time.time; //쿨타임 시작

        // 힐
        boss.ChangeState(EnemyState.Skill);
        yield return new WaitForSeconds(0.4f); //힐 애니메이션 시간

        boss.isUsingSkill = false;
        boss.isImmuneToKnockback = false;
        isHeal = false;

        Debug.Log("보스현재 체력: " + boss_health.currentHealth + "보스현재 체력: " + boss_health.maxHealth);
        Debug.Log("현재 쿨타임 : " + lastUseTime);
    }

    public bool CanUse()
    {
        return Time.time >= lastUseTime + cooldown;
    }

    public void ShowEffect() //애니메이션 키 프레임 함수
    {
        if (!isHeal) return;
        int healAmount = Mathf.RoundToInt(boss_health.maxHealth * 0.1f);
        boss_health.ChangeHealth(healAmount);
        GameObject obj = Instantiate(HealEffect, transform.position, Quaternion.identity);
        Destroy(obj, 2f);
    }
}
