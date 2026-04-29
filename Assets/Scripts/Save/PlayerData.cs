using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string name;

    public float posx;
    public float posy;
    public float cooltime;

    public int currenthp;
    public int maxhp;
    public int level;
    public int exp;
    public int maxexp;
    public int damage;
    public int crit;
    public float speed;
    public int defense;
    public int skillPoint;

    public bool canDash;
}
