using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revive : ISkillLogic
{
    public IEnumerator Execute(PlayerMovement player, SkillSO skill, int level)
    {
        PlayerMovement.Instance.isInvincible = true;
        PlayerMovement.Instance.isDead = true;

        player.ChangeState(PlayerState.DEATH, 0);

        yield return new WaitForSeconds(0.3f);

        Vector3 spawnPos = player.transform.position + new Vector3(0, 1f, 0);
        GameObject obj = GameObject.Instantiate(skill.skillPrefab, spawnPos, Quaternion.identity);

        yield return new WaitForSeconds(2f);

        StatsManager.Instance.UpdateHealth(999);
        StatsManager.Instance.ShowHealText(StatsManager.Instance.maxHealth);
        PlayerMovement.Instance.isDead = false;

        player.ChangeState(PlayerState.IDLE, 0);
        PlayerMovement.Instance.isInvincible = false;
        GameObject.Destroy(obj, 1f);
    }
}
