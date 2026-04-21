using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleSlash : ISkillLogic
{
    public IEnumerator Execute(PlayerMovement player, SkillSO skill, int level)
    {
        Animator anim = player.anim;
        float skillvalue = skill.levelData[level].value;

        player.ChangeState(PlayerState.ATTACK, 0);

        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        PlayerCombat.Instance.DealSkillDamage(2f, skillvalue, skill.skillPrefab);

        player.ChangeState(PlayerState.IDLE, 0);
        yield return null;
        player.ChangeState(PlayerState.ATTACK, 1);

        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        PlayerCombat.Instance.DealSkillDamage(4f, skillvalue * 2, skill.skillPrefab);

        player.ChangeState(PlayerState.IDLE, 0);
    }
}

