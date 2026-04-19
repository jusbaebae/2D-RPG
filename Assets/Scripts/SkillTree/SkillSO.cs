using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName ="SkillTree/Skill")]
public class SkillSO : ScriptableObject
{
    [Header("스킬 정보")]
    public int skillid;
    public string skillName;
    [TextArea]public string skilldescription;
    public Sprite skillIcon;
    public SkillType skillType;
    public int maxLevel;

    [Header("스킬 능력치")]
    public SkillLevelData[] levelData;

    [Header("스킬 효과음")]
    public AudioClip skillSFX;
}



public enum SkillType
{
    Passive,
    Active
}

