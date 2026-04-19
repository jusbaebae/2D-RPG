using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkillTreeManager : MonoBehaviour
{
    public static SkillTreeManager Instance;

    public SkillsSlot[] skillSlots;
    public TMP_Text pointsText;
    public int availablePoints;

    private void Awake()
    {
        Instance = this;
    }

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
        if (SkillMessageUI.Instance.isOpen) return;

        if (availablePoints >= slot.GetRequiredPoint())
        {
            SkillMessageUI.Instance.Show(slot);
        }
        else
        {
            SkillMessageUI.Instance.FailPointUI();
        }
    }

    private void HandleAbilityPointSpent(SkillsSlot slot)
    {
        if(availablePoints > 0)
        {
            UpdateAbilityPoints(-slot.GetRequiredPoint());
        }
    }

    private void HandleSkillMaxed()
    {
        foreach(SkillsSlot slot in skillSlots)
        {
            if(!slot.isUnlocked && slot.CanUnlockSkill())
                slot.Unlock();
        }
    }

    public void UpdateAbilityPoints(int amount) //스킬포인트 나타내기
    {
        availablePoints += amount;
        pointsText.text = "Points: " + availablePoints;
    }

    public List<SkillSaveData> GetSaveSkillData()
    {
        List<SkillSaveData> list = new List<SkillSaveData>();

        foreach (var slot in skillSlots)
        {
            list.Add(new SkillSaveData
            {
                skillid = slot.skillSo.skillid,
                currentLevel = slot.currentLevel,
                isUnlocked = slot.isUnlocked
            });
        }

        return list;
    }

    public void GetLoadSkillData(List<SkillSaveData> data)
    {
        foreach (var save in data)
        {
            foreach (var slot in skillSlots)
            {
                if (slot.skillSo.skillid == save.skillid)
                {
                    slot.currentLevel = save.currentLevel;
                    slot.isUnlocked = save.isUnlocked;
                    slot.UpdateUI(); //UI업데이트 필수
                
                }
            }
        }
    }

    public void FillData(PlayerData data) //스킬 포인트는 따로 플레이어 데이터에 저장
    {
        data.skillPoint = availablePoints;
    }

    public void LoadFromData(PlayerData data) //로드도 따로
    {
        availablePoints = data.skillPoint;
        UpdateAbilityPoints(0);
    }
}
