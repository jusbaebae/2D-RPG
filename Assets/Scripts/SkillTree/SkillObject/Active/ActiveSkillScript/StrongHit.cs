using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongHit : ISkillLogic
{
    public IEnumerator Execute(PlayerMovement player, SkillSO skill, int level)
    {
        player.ChangeState(PlayerState.ATTACK, 1);
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.StrongHit);
        yield return new WaitForSeconds(0.3f);

        float skillvalue = skill.levelData[level].value;
        PlayerCombat.Instance.DealSkillDamage(2f, skillvalue, skill.skillPrefab);
        yield return new WaitForSeconds(0.3f);

        player.ChangeState(PlayerState.IDLE, 0);
    }
}
