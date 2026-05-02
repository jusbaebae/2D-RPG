using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Movement : MonoBehaviour
{
    [Header("Wander")]
    public float wanderRadius = 3f;
    public float wanderMoveTime = 3f;
    public float wanderIdleTime = 2f;

    protected Vector2 spawnPoint; 
    protected Vector2 wanderDirection;
    protected float wanderTimer;

    public float WanderSpeed;
    public float speed;
    public float attackRange;
    public float attackCooldown;
    public float playerDetectRange;
    public float originPlayerDetechRange;

    public Transform detectionPoint;
    public LayerMask playerLayer;
    public LayerMask enemyLayer;

    protected bool isPlayerDetected;
    public bool isImmuneToKnockback; //넉백 면역

    protected float attackCooldownTimer;
    protected int facingDirection = -1;
    protected EnemyState enemyState;

    public Rigidbody2D rb;
    public Transform player;
    [SerializeField] protected Animator anim;


    protected void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        spawnPoint = transform.position;
        originPlayerDetechRange = playerDetectRange;

        StartWander();
    }

    protected void Update()
    {
        if (PlayerMovement.Instance.isInvincible)
        {
            playerDetectRange = 0;
        }
        else
        {
            playerDetectRange = originPlayerDetechRange;
        }

        if (enemyState == EnemyState.KnockBack) return;

        CheckForPlayer();
        HandleCombat();

        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;

        }
    }

    protected void FixedUpdate()
    {
        if (enemyState == EnemyState.Attacking || enemyState == EnemyState.KnockBack || enemyState == EnemyState.Skill)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        //배회
        if (enemyState == EnemyState.Wander)
        {
            rb.velocity = wanderDirection * (WanderSpeed);
            ControlFlip(wanderDirection.x);

            wanderTimer -= Time.fixedDeltaTime;
            if (wanderTimer <= 0)
            {
                rb.velocity = Vector2.zero;
                ChangeState(EnemyState.Idle);

                wanderTimer = Random.Range(1, wanderIdleTime); //이때 wanderTimer는 대기시간
            }
        }
        else if (enemyState == EnemyState.Idle && !isPlayerDetected)
        {
            rb.velocity = Vector2.zero;

            wanderTimer -= Time.fixedDeltaTime;
            if (wanderTimer <= 0)
            {
                StartWander();
            }
        }

        //추적
        //Debug.Log("속도: " + rb.velocity + " / 크기: " + rb.velocity.magnitude);
        if (enemyState == EnemyState.Chasing)
        {
            float distToPlayer = Vector2.Distance(transform.position, player.position);
            float stoppingDistance = 1.2f; //플레이어와의 최소거리

            if (distToPlayer > stoppingDistance)
            {
                Vector2 dir = (player.position - transform.position).normalized;
                Vector2 sep = GetSeparationForce() * 1.5f;

                rb.velocity = (dir + sep) * speed;
                ControlFlip(dir.x);
            }
            else
            {
                rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, Time.fixedDeltaTime * 5f);

                float faceDir = player.position.x - transform.position.x;
                ControlFlip(faceDir);
            }

            //Debug.Log("dir: " + dir.magnitude);
        }
    }
    protected void StartWander() //추적중이 아닐때는 주변배회
    {
        if (isPlayerDetected) return;

        ChangeState(EnemyState.Wander);
        Vector2 currentPos = transform.position;
        float distFromSpawn = Vector2.Distance(spawnPoint, currentPos);
        if (distFromSpawn > wanderRadius)
        {
            //스폰 구역을 벗어났다면 스폰 지점 방향으로 강제 이동
            wanderDirection = (spawnPoint - currentPos).normalized;
        }
        else
        {
            //구역 내라면 랜덤 방향
            wanderDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        }

        wanderTimer = Random.Range(1, wanderMoveTime); //배회시간은 1~3초
    }

    protected void Chase()
    {
        if (player == null) return;

        float dirX = player.position.x - transform.position.x;
        ControlFlip(dirX);
        //Debug.Log(rb.velocity.magnitude);
    }


    //X값을 기준으로 방향바꾸기
    protected void ControlFlip(float horizontalDir)
    {
        if (horizontalDir > 0 && facingDirection == -1)
        {
            Flip();
        }
        else if (horizontalDir < 0 && facingDirection == 1)
        {
            Flip();
        }
    }

    protected void Flip()
    {
        facingDirection *= -1;
        Vector3 scale = transform.localScale;
        scale.x *= -1; //X만 뒤집기
        transform.localScale = scale;
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
        if (enemyState == EnemyState.Attacking)
            return;

        if (!isPlayerDetected)
        {
            if (enemyState == EnemyState.Idle && wanderTimer > 0)
            {
                return;
            }

            if (enemyState != EnemyState.Wander && enemyState != EnemyState.KnockBack)
            {
                StartWander();
            }
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= attackRange && attackCooldownTimer <= 0)
        {
            attackCooldownTimer = attackCooldown;
            ChangeState(EnemyState.Attacking);
            rb.velocity = Vector2.zero;
        }
        else
        {
            ChangeState(EnemyState.Chasing);
            Chase();
        }

        //Debug.Log($"player: {player}, detected: {isPlayerDetected}");
    }

    Vector2 GetSeparationForce() //적 겹치는거 방지
    {
        Collider2D[] others = Physics2D.OverlapCircleAll(transform.position, 0.8f, enemyLayer);

        Vector2 force = Vector2.zero;
        float radius = 0.8f;

        foreach (var other in others)
        {
            if (other.gameObject == gameObject) continue;

            Vector2 dir = transform.position - other.transform.position;
            float dist = dir.magnitude;

            if (dist > 0 && dist < radius)
            {
                float strength = (radius - dist) / radius;

                force += dir.normalized * strength;
            }
        }

        return force;
    }


    public void ChangeState(EnemyState newState)
    {
        if (enemyState == newState) return;

        //현재 애니메이션 중지
        if (enemyState == EnemyState.Idle)
            anim.SetBool("IsIdle", false);
        else if (enemyState == EnemyState.Chasing)
            anim.SetBool("IsMoving", false);
        else if (enemyState == EnemyState.Attacking)
            anim.SetBool("IsAttacking", false);
        else if (enemyState == EnemyState.Skill)
            anim.SetBool("IsSkill", false);
        else if (enemyState == EnemyState.Wander)
            anim.SetBool("IsMoving", false);

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
        else if (enemyState == EnemyState.Wander)
            anim.SetBool("IsMoving", true);
    }

    private void OnDrawGizmosSelected()
    {
        // 배회 가능 구역 표시 (푸른색)
        Gizmos.color = Color.blue;
        if (Application.isPlaying) Gizmos.DrawWireSphere(spawnPoint, wanderRadius);
        else Gizmos.DrawWireSphere(transform.position, wanderRadius);

        // 플레이어 감지 범위 표시 (붉은색)
        Gizmos.color = Color.red;
        if (detectionPoint != null) Gizmos.DrawWireSphere(detectionPoint.position, playerDetectRange);
    }

}
public enum EnemyState
{
    Idle,
    Wander,
    Chasing,
    Attacking,
    KnockBack,
    Skill
}

