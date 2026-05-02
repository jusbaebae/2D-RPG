using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SlimeMovement : Enemy_Movement
{
    [Header("Slime Jump")]
    public float jumpForce;

    [SerializeField] private Transform shadow;
    [SerializeField] private Transform bodyVisual;
    [SerializeField] private float jumptimer;
    public float time; //슬라임 velocity조정용

    private Coroutine stopMoveCoroutine;

    private new void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = bodyVisual.GetComponent<Animator>();

        spawnPoint = transform.position;
        originPlayerDetechRange = playerDetectRange;

        StartCoroutine(JumpRoutine());
    }
    private new void FixedUpdate()
    {
        if (enemyState == EnemyState.KnockBack)
            return;

        rb.velocity = Vector2.Lerp(rb.velocity,Vector2.zero,Time.fixedDeltaTime * time);
    }

    IEnumerator JumpRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1f, jumptimer));
            if (enemyState == EnemyState.KnockBack ||enemyState == EnemyState.Attacking ||enemyState == EnemyState.Skill)
                continue;

            //플레이어 있으면 추적 점프
            if (isPlayerDetected)
            {
                ChangeState(EnemyState.Chasing);
                anim.SetTrigger("Jump");
            }
            else //없으면 배회상태 점프
            {
                bool shouldJump = Random.value > 0.5f;
                if (shouldJump)
                {
                    ChangeState(EnemyState.Wander);

                    anim.SetTrigger("Jump");
                }
                else
                {
                    ChangeState(EnemyState.Idle);
                }
            }
        }
    }

    public void Jump()
    {
        Vector2 dir;

        if (isPlayerDetected && player != null) 
        {
            dir = (player.position - transform.position).normalized;
            ControlFlip(dir.x);
        }
        else
        {
            dir = Random.insideUnitCircle.normalized; //반지름 1인 원안에서 랜덤한 방향 구하기
            if (dir == Vector2.zero) dir = Vector2.right;
        }
        if (dir.x != 0) ControlFlip(dir.x);

        //물리 이동
        rb.velocity = Vector2.zero;
        rb.AddForce(dir * jumpForce, ForceMode2D.Impulse);

        //슬라임 위로 띄우기
        if (bodyVisual != null) bodyVisual.DOKill();
        bodyVisual.DOLocalMoveY(1.5f, 0.15f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.OutQuad);

        //그림자 축소
        if(shadow != null) shadow.DOKill();
        shadow.DOLocalMoveY(-1f, 0.15f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.OutQuad);
        shadow.DOScale(new Vector3(0.5f, 0.2f, 1), 0.15f).SetLoops(2, LoopType.Yoyo);

        if (stopMoveCoroutine != null) StopCoroutine(stopMoveCoroutine);
        stopMoveCoroutine = StartCoroutine(StopMoveAnim());
    }

    IEnumerator StopMoveAnim()
    {
        yield return new WaitForSeconds(1f);

        // 추적 중이 아니라면 Wander 상태로 변경하여 다음 배회를 준비
        if (!isPlayerDetected)
        {
            ChangeState(EnemyState.Wander);
        }
    }

    protected override void HandleCombat()
    {
        if (enemyState == EnemyState.Attacking)
            return;

        if (!isPlayerDetected)
        {
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);

        //공격 범위
        if (dist <= attackRange && attackCooldownTimer <= 0)
        {
            attackCooldownTimer = attackCooldown;
            ChangeState(EnemyState.Attacking);
        }
    }
}
