using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public Transform attackPoint;
    public LayerMask enemyLayer;

    public float cooldown;
    private float timer;


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
            playerMovement.ChangeState(PlayerState.ATTACK);
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Sword);
            FinishAttacking();
        }
    }
    public void DealDamage()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, StatsManager.Instance.weaponRange, enemyLayer);
        int minDamage = Mathf.RoundToInt(StatsManager.Instance.damage * 0.7f); //최소 데미지
        if(minDamage <= 0)
        {
            minDamage = 1; //최소 데미지는 무조건 1이상
        }
        int maxDamage = Mathf.RoundToInt(StatsManager.Instance.damage * 1.3f); //최대 데미지

        if (enemies.Length > 0)
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
            StatsManager.Instance.CritCheck();
            if (StatsManager.Instance.isCrit)
            {
                enemies[0].GetComponent<Enemy_Health>().ChangeHealth(-StatsManager.Instance.damage * 3);
                enemies[0].GetComponent<Enemy_KnockBack>().Knockback(transform, StatsManager.Instance.knockbackForce, StatsManager.Instance.knockbackTime, StatsManager.Instance.stunTime);
            }
            else
            {
                enemies[0].GetComponent<Enemy_Health>().ChangeHealth(-Random.Range(minDamage, maxDamage + 1));
                enemies[0].GetComponent<Enemy_KnockBack>().Knockback(transform, StatsManager.Instance.knockbackForce, StatsManager.Instance.knockbackTime, StatsManager.Instance.stunTime);
            }
        }
    }

    public void FinishAttacking()
    {
        playerMovement.ChangeState(PlayerState.IDLE);
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
}
