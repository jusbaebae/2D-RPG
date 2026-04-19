using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDatabase : MonoBehaviour
{
    public static SkillDatabase Instance;

    public List<SkillSO> allSkills;

    private Dictionary<int, SkillSO> skillDict;

    private void Awake()
    {
        Instance = this;

        skillDict = new Dictionary<int, SkillSO>();

        foreach (var skill in allSkills)
        {
            skillDict.Add(skill.skillid, skill);
        }
    }

    public SkillSO GetSkill(int id)
    {
        skillDict.TryGetValue(id, out var skill);
        return skill;
    }
}
