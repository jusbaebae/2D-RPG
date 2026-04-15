using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;

    public bool isinteract;
    private bool isKnockBack;
    public bool isShooting;

    public SPUM_Prefabs spum;
    public PlayerCombat playerCombat;
    public Ghost ghost;

    public Vector2 inputDir;
    bool dashInput; //대쉬 입력
    bool isDash; //대쉬 시간
    bool canDash = true;
    PlayerState currentState; //상태 전환하기

    private void Start()
    {
        if (spum != null)
        {
            spum.PopulateAnimationLists();
            spum.OverrideControllerInit();
        }
    }
    private void Update()
    {
        if (isinteract) //대화중일때 이동 무시
        {
            inputDir = Vector2.zero;
            ChangeState(PlayerState.IDLE);
            return;
        }

        if (isKnockBack) return;

        inputDir.x = Input.GetAxisRaw("Horizontal");
        inputDir.y = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Slash") && playerCombat.enabled == true && Time.timeScale != 0)
        {
            playerCombat.Attack();
        }

        if (Input.GetButtonDown("Dash") && canDash)
        {
            dashInput = true;
        }
    }

    void FixedUpdate()
    {
        if (currentState == PlayerState.ATTACK) return;

        if (dashInput) //대쉬
        {
            Dash(inputDir, 30f, 0.1f);
            dashInput = false;
            return;
        }

        if (isKnockBack || isDash) return;

        if (inputDir != Vector2.zero)
        {
            ChangeState(PlayerState.MOVE); 
        }
        else
        {
            ChangeState(PlayerState.IDLE);
        }

        if (isShooting == true)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if ((inputDir.x > 0 && transform.localScale.x < 0) || (inputDir.x < 0 && transform.localScale.x > 0))
        {
            Flip();
        }

        rb.velocity = inputDir.normalized * StatsManager.Instance.speed; //일반적인 이동
    }

    void Flip()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    public void Knockback(Transform enemy, float force, float stunTime)
    {
        isKnockBack = true;
        //ChangeState(PlayerState.DAMAGED);
        Vector2 direction = (transform.position - enemy.position).normalized;
        rb.AddForce(direction * force, ForceMode2D.Impulse);
        StartCoroutine(KnockbackCounter(stunTime));
    }

    public void Dash(Vector2 dir, float force, float time)
    {
        if (dir == Vector2.zero) return;

        ghost.makeGhost = true; //잔상제어
        isDash = true;
        canDash = false;

        Vector2 dashDir = dir.normalized;

        rb.velocity = Vector2.zero; //기존 속도 제거
        rb.velocity = dashDir * force;

        StartCoroutine(DashCoroutine(time));
        StartCoroutine(DashCooldown());
    }

    public void ChangeState(PlayerState newState) //애니메이션 상태 전환
    {
        if (currentState == newState) return;

        currentState = newState;
        spum.PlayAnimation(newState, 0);
    }

    IEnumerator KnockbackCounter(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        rb.velocity = Vector2.zero;
        isKnockBack = false;
    }

    IEnumerator DashCoroutine(float time) //대쉬 시간
    {
        yield return new WaitForSeconds(time);

        rb.velocity = Vector2.zero;
        isDash = false;
        ghost.makeGhost = false;
    }

    IEnumerator DashCooldown() //대쉬 쿨타임
    {
        yield return new WaitForSeconds(1);
        canDash = true;
    }
}
