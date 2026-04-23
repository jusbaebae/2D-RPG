using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Movement : Enemy_Movement
{
    public Boss_Health boss_health;

    public List<BossSkill> skills;

    public bool isDead;
    public bool isUsingSkill;
    void Awake()
    {
        isUsingSkill = false;
    }
    void Update()
    {
        if (enemyState == EnemyState.KnockBack) return;
        if (isDead) 
        {
            //현재 애니메이션 중지
            if (enemyState == EnemyState.Idle)
                anim.SetBool("IsIdle", false);
            else if (enemyState == EnemyState.Chasing)
                anim.SetBool("IsMoving", false);
            else if (enemyState == EnemyState.Attacking)
                anim.SetBool("IsAttacking", false);
            else if (enemyState == EnemyState.Skill)
                anim.SetBool("IsSkill", false);

            rb.velocity = Vector2.zero;
            anim.SetBool("IsDead", true);
            return;
        }
        
        if (isUsingSkill) return;

        CheckForPlayer();
        HandleCombat();

        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
        }
    }

    protected override void HandleCombat()
    {
        if (!isPlayerDetected)
        {
            ChangeState(EnemyState.Idle);
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);

        if (isUsingSkill) return;

        if (dist <= attackRange && attackCooldownTimer <= 0)
        {
            attackCooldownTimer = attackCooldown;
            ChoosePattern(); //공격 or 스킬
        }
        else
        {
            ChangeState(EnemyState.Chasing);
            Chase();
        }
    }

    private void ChoosePattern()
    {
        float rand = Random.value;

        foreach (var skill in skills)
        {
            if (skill is HealSkill heal)
            {
                if (heal.CanUse() && boss_health.currentHealth < boss_health.maxHealth * 0.5f)
                {
                    //힐 스킬 보스체력이 50%이하로 내려가면 사용
                    StartCoroutine(heal.UseSkill(this));
                    return;
                }
            }
        }

        if (rand < 0.6f) 
        {
            // 60% 일반공격
            isImmuneToKnockback = true;
            isUsingSkill = true;
            ChangeState(EnemyState.Attacking);
            rb.velocity = Vector2.zero;
        }
        else
        {
            // 40% 스킬
            List<BossSkill> usableSkills = new List<BossSkill>();

            foreach (var skill in skills)
            {
                if (skill is HealSkill) continue;
                usableSkills.Add(skill);
            }

            int idx = Random.Range(0, usableSkills.Count);
            StartCoroutine(usableSkills[idx].UseSkill(this));
        }
    }

    private void FinishAttack() //애니메이션 키 이벤트
    {
        isUsingSkill = false;
        isImmuneToKnockback = false;
    }
}
