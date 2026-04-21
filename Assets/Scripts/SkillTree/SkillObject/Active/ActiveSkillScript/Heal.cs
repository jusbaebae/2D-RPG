using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : ISkillLogic
{
    public IEnumerator Execute(PlayerMovement player, SkillSO skill, int level)
    {
        player.ChangeState(PlayerState.OTHER, 1);

        yield return new WaitForSeconds(0.2f);

        Vector3 spawnPos = player.transform.position + new Vector3(0, 1f, 0);
        GameObject obj = GameObject.Instantiate(skill.skillPrefab, spawnPos, Quaternion.identity);

        int healthup = (int)(StatsManager.Instance.maxHealth / 100 * skill.levelData[level].value);
        StatsManager.Instance.UpdateHealth(healthup);
        StatsManager.Instance.ShowHealText((int)(StatsManager.Instance.maxHealth / 100 * skill.levelData[level].value));

        yield return new WaitForSeconds(0.2f);

        player.ChangeState(PlayerState.IDLE, 0);

        GameObject.Destroy(obj, 1f);
    }
}
