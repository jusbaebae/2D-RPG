using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_KnockBack : MonoBehaviour
{
    private Rigidbody2D rb;
    private Enemy_Movement enemy_Movement;
    SpriteRenderer sprite;
    Color originalColor;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemy_Movement = GetComponent<Enemy_Movement>();
        sprite = GetComponent<SpriteRenderer>();
        originalColor = sprite.color;
    }

    public void Knockback(Transform forceTransform, float knockbackForce, float knockbackTime, float stunTime)
    {
        enemy_Movement.ChangeState(EnemyState.KnockBack);
        StartCoroutine(stunTimer(knockbackTime,stunTime));
        StartCoroutine(HitFlash());
        Vector2 direction = (transform.position - forceTransform.position).normalized;
        rb.velocity = direction * knockbackForce;
    }

    IEnumerator stunTimer(float knockbackTime, float stunTime)
    {
        yield return new WaitForSeconds(knockbackTime);
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(stunTime);
        enemy_Movement.ChangeState(EnemyState.Idle);
    }

    IEnumerator HitFlash()
    {
        sprite.color = new Color(1f, 0.4f, 0.4f);
        yield return new WaitForSeconds(0.15f);
        sprite.color = originalColor;
    }
}
