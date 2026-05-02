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
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); //이 객체를 유지
        }
        else
        {
            Destroy(gameObject); //이미 있으면 새로 생긴 건 삭제
            return;
        }
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
            slot.skillButton.onClick.AddListener(() => Checkslot(slot));
        }

        UpdateAbilityPoints(0);
    }

    private void Checkslot(SkillsSlot slot)
    {
        if (SkillMessageUI.Instance.isOpen) return;

        if (!slot.isUnlocked && !slot.isUnlockable)
        {
            SkillMessageUI.Instance.ShowFailUI(slot); //완전 잠금
        }
        else if (availablePoints < slot.GetRequiredPoint())
        {
            SkillMessageUI.Instance.FailPointUI(); //포인트 부족 
        }
        else if (!slot.isUnlocked && slot.isUnlockable)
        {
            SkillMessageUI.Instance.ShowUnlockUi(slot); // 해금 가능
        }
        else if (slot.currentLevel >= slot.skillSo.maxLevel)
        {
            SkillMessageUI.Instance.ShowFailUI(slot); //이미 만렙
        }
        else
        {
            SkillMessageUI.Instance.ShowConfirmUI(slot); //업그레이드
        }

        //Debug.Log("Checkslot 호출");
    }

    private void HandleAbilityPointSpent(SkillsSlot slot) 
    {
        UpdateAbilityPoints(-slot.GetRequiredPoint());

        //Debug.Log("HandleAbilityPointSpent 호출");
    }

    private void HandleSkillMaxed()
    {
        foreach(SkillsSlot slot in skillSlots)
        {
            if(!slot.isUnlocked && slot.CanUnlockSkill())
                slot.Unlockable();
        }
    }

    public void UpdateAbilityPoints(int amount) //스킬포인트 나타내기
    {
        availablePoints += amount;
        pointsText.text = "Points : " + availablePoints;
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
                isUnlocked = slot.isUnlocked,
                isUnlockable = slot.isUnlockable
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
                    slot.isUnlockable = save.isUnlockable;
                    slot.UpdateUI(); //UI업데이트 필수

                    if(slot.skillSo.skillid == 1003 && slot.isUnlocked)
                    {
                        PlayerMovement.Instance.canDash = true;
                    }
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
