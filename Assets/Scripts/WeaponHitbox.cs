using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    private bool hasHit = false;

    public void ResetHit()
    {
        hasHit = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;

        if (!collision.CompareTag("Enemy"))
            return;

        Enemy_Health hp = collision.GetComponentInParent<Enemy_Health>();
        Enemy_KnockBack kb = collision.GetComponentInParent<Enemy_KnockBack>();

        if (hp == null) return;

        hasHit = true;

        int minDamage = Mathf.RoundToInt(StatsManager.Instance.damage * 0.7f);

        if (minDamage <= 0) minDamage = 1;

        int maxDamage = Mathf.RoundToInt(StatsManager.Instance.damage * 1.3f);

        int finalDamage;

        StatsManager.Instance.CritCheck();

        if (StatsManager.Instance.isCrit)
        {
            finalDamage = StatsManager.Instance.damage * 3;
        }
        else
        {
            finalDamage = Random.Range(minDamage, maxDamage + 1);
        }

        hp.ChangeHealth(-finalDamage);

        if (kb != null)
        {
            kb.Knockback( PlayerCombat.Instance.transform,StatsManager.Instance.knockbackForce, StatsManager.Instance.knockbackTime,StatsManager.Instance.stunTime);
        }

        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Hit);
    }
}
