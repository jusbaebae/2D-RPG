using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public static PlayerCombat Instance;

    public PlayerMovement playerMovement;
    public CircleCollider2D fistHitbox;
    public BoxCollider2D weaponHitbox;
    public LayerMask enemyLayer;
    public Transform skillPoint;

    public float cooldown;
    private float timer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); //이 객체를 유지
        }
        else
        {
            Destroy(gameObject); //이미 있으면 새로 생긴 건 삭제
            return;
        }
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
            playerMovement.isSlash = true;
            StopAllCoroutines();
            StartCoroutine(AttackCall());
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.Sword);
        }
    }

    public void DealSkillDamage(float radius, float damageMultiplier, GameObject hitEffect = null) //스킬 공격용(범위)
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(skillPoint.position, radius,enemyLayer);

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
                AudioManager.Instance.PlaySfx(AudioManager.Sfx.Hit);
                GameObject obj = Instantiate(hitEffect, enemyCol.transform.position, Quaternion.identity);
                Destroy(obj, 1f);
            }
        }
    }
    public void EnableAttackHitbox() //공격 히트박스 띄우기
    {
        //Debug.Log("EnableAttackHitbox호출");
        if (InventoryManager.Instance.IsWeaponEquipped())
        {
            weaponHitbox.enabled = true;
        }
        else
        {
            fistHitbox.enabled = true;
        }
    }
    public void DisableAttackHitbox()
    {
        weaponHitbox.enabled = false;
        fistHitbox.enabled = false;
    }

    public void FinishAttacking() //공격 끝나면 상태바꾸기
    {
        playerMovement.isSlash = false;
        playerMovement.ChangeState(PlayerState.IDLE, 0);
    }

    public void SetWeaponHitbox(Vector2 size, Vector2 offset)
    {
        weaponHitbox.size = size;
        weaponHitbox.offset = offset;
    }

    IEnumerator AttackCall()
    {
        yield return new WaitForSeconds(0.2f);
        if (InventoryManager.Instance.IsWeaponEquipped())
        {
            weaponHitbox.GetComponent<WeaponHitbox>().ResetHit();
            weaponHitbox.enabled = true;
        }
        else
        {
            fistHitbox.GetComponent<WeaponHitbox>().ResetHit();
            fistHitbox.enabled = true;
        }
        yield return new WaitForSeconds(0.1f);
        DisableAttackHitbox();
        FinishAttacking();
    }

    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(skillPoint.position, 0.5f);
    }
}
