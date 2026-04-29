using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public static PlayerCombat Instance;

    public PlayerMovement playerMovement;
    public Transform attackPoint;
    public LayerMask enemyLayer;

    public float cooldown;
    private float timer;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }

    public void Attack()
    {
        if(timer <= 0)
        {
            timer = cooldown;
            playerMovement.ChangeState(PlayerState.ATTACK, 0);
            StartCoroutine(AttackCall());
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.Sword);
            FinishAttacking();
        }
    }
    public void DealDamage() //일반 공격용(단일)
    {
        StatsManager.Instance.isCrit = false;
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, StatsManager.Instance.weaponRange, enemyLayer);
        int minDamage = Mathf.RoundToInt(StatsManager.Instance.damage * 0.7f); //최소 데미지
        if(minDamage <= 0)
        {
            minDamage = 1; //최소 데미지는 무조건 1이상
        }
        int maxDamage = Mathf.RoundToInt(StatsManager.Instance.damage * 1.3f); //최대 데미지

        if (enemies.Length > 0)
        {
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.Hit);
            StatsManager.Instance.CritCheck();
            if (StatsManager.Instance.isCrit)
            {
                enemies[0].GetComponent<Enemy_Health>().ChangeHealth((int)(-StatsManager.Instance.damage * 3));
                enemies[0].GetComponent<Enemy_KnockBack>().Knockback(transform, StatsManager.Instance.knockbackForce, StatsManager.Instance.knockbackTime, StatsManager.Instance.stunTime);
            }
            else
            {
                enemies[0].GetComponent<Enemy_Health>().ChangeHealth(-Random.Range(minDamage, maxDamage + 1));
                enemies[0].GetComponent<Enemy_KnockBack>().Knockback(transform, StatsManager.Instance.knockbackForce, StatsManager.Instance.knockbackTime, StatsManager.Instance.stunTime);
            }
        }
    }

    public void DealSkillDamage(float radius, float damageMultiplier, GameObject hitEffect = null) //스킬 공격용(범위)
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, radius,enemyLayer);

        foreach (var enemyCol in enemies)
        {
            var hp = enemyCol.GetComponent<Enemy_Health>();
            var kb = enemyCol.GetComponent<Enemy_KnockBack>();

            if (hp == null) continue;

            int baseDamage = StatsManager.Instance.damage;

            int minDamage = Mathf.RoundToInt(baseDamage * 0.7f * damageMultiplier);
            int maxDamage = Mathf.RoundToInt(baseDamage * 1.3f * damageMultiplier);

            int finalDamage = Random.Range(minDamage, maxDamage + 1);

            //치명타 체크
            StatsManager.Instance.CritCheck();
            if (StatsManager.Instance.isCrit)
            {
                finalDamage *= 3;
            }

            hp.ChangeHealth(-finalDamage);

            if (kb != null)
            {
                kb.Knockback( transform, StatsManager.Instance.knockbackForce, StatsManager.Instance.knockbackTime, StatsManager.Instance.stunTime);
            }

            //적 위치에 이펙트 생성
            if (hitEffect != null)
            {
                GameObject obj = Instantiate(hitEffect, enemyCol.transform.position, Quaternion.identity);
                Destroy(obj, 1f);
            }
        }
    }

    public void FinishAttacking() //공격 끝나면 상태바꾸기
    {
        playerMovement.ChangeState(PlayerState.IDLE, 0);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        if (StatsManager.Instance == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, StatsManager.Instance.weaponRange);
    }
    IEnumerator AttackCall()
    {
        yield return new WaitForSeconds(0.15f);
        DealDamage();
    }
}
