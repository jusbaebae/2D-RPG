using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventProxy : MonoBehaviour
{
    private SlimeMovement parentMovement;
    private Enemy_Combat parentCombat;

    void Start()
    {
        //부모에 있는 스크립트를 찾아오기.
        parentMovement = GetComponentInParent<SlimeMovement>();
        parentCombat = GetComponentInParent<Enemy_Combat>();
    }

    //애니메이션 이벤트에서 설정한 함수 이름과 똑같이
    public void Jump()
    {
        if (parentMovement != null)
        {
            parentMovement.Jump(); //부모의 Jump()
        }
    }
    public void Attack()
    {
        if (parentCombat != null)
        {
            parentCombat.Attack(); //부모의 Attack()
            Debug.Log("공격 이벤트 실행");
        }
    }
    public void ChangeState(EnemyState newState)
    {
        if (parentMovement != null)
        {
            parentMovement.ChangeState(newState); //부모의 ChangeState()
        }
    }
}
