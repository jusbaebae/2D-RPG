using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkillTreeManager : MonoBehaviour
{
    public SkillsSlot[] skillSlots;
    public TMP_Text pointsText;
    public int availablePoints;

    private void OnEnable()
    {
        SkillsSlot.OnAbilityPointSpent += HandleAbilityPointSpent;
        SkillsSlot.OnSkillMaxed += HandleSkillMaxed;
        ExperienceManager.OnLevelUp += UpdateAbilityPoints;
    }
    private void OnDisable()
    {
        SkillsSlot.OnAbilityPointSpent -= HandleAbilityPointSpent;
        SkillsSlot.OnSkillMaxed -= HandleSkillMaxed;
        ExperienceManager.OnLevelUp -= UpdateAbilityPoints;
    }

    private void Start()
    {
        foreach(SkillsSlot slot in skillSlots)
        {
            slot.skillButton.onClick.AddListener(() => CheckAvailablePoints(slot));
        }
        UpdateAbilityPoints(0);
    }

    private void CheckAvailablePoints(SkillsSlot slot)
    {
        if(availablePoints > 0)
        {
            slot.TryUpgradeSkill();
        }
    }

    private void HandleAbilityPointSpent(SkillsSlot skillSlot)
    {
        if(availablePoints > 0)
        {
            UpdateAbilityPoints(-1);
        }
    }

    private void HandleSkillMaxed(SkillsSlot skillsSlot)
    {
        foreach(SkillsSlot slot in skillSlots)
        {
            if(!slot.isUnlocked && slot.CanUnlockSkill())
                slot.Unlock();
        }
    }

    public void UpdateAbilityPoints(int amount)
    {
        availablePoints += amount;
        pointsText.text = "Points: " + availablePoints;
    }
}
