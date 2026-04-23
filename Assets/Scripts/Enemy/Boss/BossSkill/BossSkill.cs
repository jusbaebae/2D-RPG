using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossSkill : MonoBehaviour
{
    public abstract IEnumerator UseSkill(Boss_Movement boss);
}
