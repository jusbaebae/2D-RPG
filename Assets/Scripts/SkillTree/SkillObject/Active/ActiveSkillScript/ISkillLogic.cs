using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillLogic
{
    IEnumerator Execute(PlayerMovement player, SkillSO skill, int level);
}
