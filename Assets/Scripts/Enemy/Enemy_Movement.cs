using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Movement : MonoBehaviour
{
    public float speed;
    public float attackRange;
    public float attackCooldown;
    public float playerDetectRange;

    public Transform detectionPoint;
    public LayerMask playerLayer;

    protected bool isPlayerDetected;
    public bool isImmuneToKnockback; //넉백 면역

    protected float attackCooldownTimer;
    protected int facingDirection = -1;
    protected EnemyState enemyState;

    public Rigidbody2D rb;
    public Transform player;
    protected Animator anim;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ChangeState(EnemyState.Idle);
    }

    void Update()
    {
        if (enemyState == EnemyState.KnockBack) return;

        CheckForPlayer();
        HandleCombat();

        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;

        }
    }
    protected void Chase()
    {
        if (player == null) return;

        //방향 뒤집기
        if (player.position.x > transform.position.x && facingDirection == -1 || player.position.x < transform.position.x && facingDirection == 1)
        {
            Flip();
        }

        //이동
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * speed;
    }

    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.x);
    }

    protected void CheckForPlayer() //플레이어 감지
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectRange, playerLayer);

        if (hits.Length > 0)
        {
            player = hits[0].transform;
            isPlayerDetected = true;
        }
        else
        {
            isPlayerDetected = false;
            player = null;
        }
    }

    protected virtual void HandleCombat() //공격 처리
    {
        if (!isPlayerDetected)
        {
            rb.velocity = Vector2.zero;
            ChangeState(EnemyState.Idle);
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= attackRange && attackCooldownTimer <= 0)
        {
            attackCooldownTimer = attackCooldown;
            ChangeState(EnemyState.Attacking);
            rb.velocity = Vector2.zero;
        }
        else if (dist > attackRange && enemyState != EnemyState.Attacking)
        {
            ChangeState(EnemyState.Chasing);
            Chase();
        }

        //Debug.Log($"player: {player}, detected: {isPlayerDetected}");
    }

    public void ChangeState(EnemyState newState)
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

        //상태 업데이트
        enemyState = newState;

        //새로운 애니메이션 재생
        if (enemyState == EnemyState.Idle)
            anim.SetBool("IsIdle", true);
        else if (enemyState == EnemyState.Chasing)
            anim.SetBool("IsMoving", true);
        else if (enemyState == EnemyState.Attacking)
            anim.SetBool("IsAttacking", true);
        else if (enemyState == EnemyState.Skill)
            anim.SetBool("IsSkill", true);


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detectionPoint.position, playerDetectRange);
    }

}
public enum EnemyState
{
    Idle,
    Chasing,
    Attacking,
    KnockBack,
    Skill
}

