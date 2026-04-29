using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Combat : Enemy_Combat
{
    public void IncreaseDamage()
    {
        damage *= 1.5f;
    }
}
