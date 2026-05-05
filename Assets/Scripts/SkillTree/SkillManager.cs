using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;

    public Dictionary<int, int> skillLevels = new Dictionary<int, int>();

    public bool hasRevived = false;

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
        SkillsSlot.OnAbilityPointSpent += HandleSkill;
    }
    private void OnDisable()
    {
        SkillsSlot.OnAbilityPointSpent -= HandleSkill;
    }

    private void HandleSkill(SkillsSlot slot)
    {
        if (slot.skillSo.skillType == SkillType.Passive)
        {
            HandlePassive(slot);
        }
        else if (slot.skillSo.skillType == SkillType.Active)
        {
            HandleActive(slot);
        }
    }

    private void HandlePassive(SkillsSlot slot) //패시브 스킬 효과
    {
        int id = slot.skillSo.skillid;

        switch (id)
        {
            case 1000: //근력 단련
                StatsManager.Instance.damage += (int)slot.skillSo.levelData[slot.currentLevel].value;
                break;

            case 1001: //체력 증진
                StatsManager.Instance.UpdateMaxHealth((int)slot.skillSo.levelData[slot.currentLevel].value);
                StatsManager.Instance.UpdateHealth((int)slot.skillSo.levelData[slot.currentLevel].value);
                break;

            case 1002: //이속 훈련
                StatsManager.Instance.UpdateSpeed((int)slot.skillSo.levelData[slot.currentLevel].value);
                break;

            default:
                Debug.LogWarning("Unknown skill" + slot.skillSo.skillName);
                break;
        }
    }

    private void HandleActive(SkillsSlot slot) //액티브 스킬 효과
    {
        int id = slot.skillSo.skillid;

        skillLevels[id] = slot.currentLevel;// 스킬 레벨 저장

        if (slot.currentLevel == 0) // 처음 배웠을 때 
        {
            KeyCode key = GetKeyForSkill(id);

            if(key != KeyCode.None)
            {
                PlayerSkillController.Instance.EquipSkill(key, slot);
            }
        }

        switch (id)
        {
            case 1003: //대쉬 -> 이미 있음
                PlayerMovement.Instance.canDash = true;
                PlayerMovement.Instance.cooltime = slot.skillSo.levelData[slot.currentLevel].cooltime;
                //Debug.Log((int)slot.skillSo.levelData[slot.currentLevel].cooltime);
                break;

            default:
                //Debug.LogWarning("Unknown skill" + slot.skillSo.skillName);
                break;
        }
    }

    public void GetLoadSkillUIData(List<SkillSaveData> data) //저장한 스킬데이터 로드하기
    {
        if (data == null) return;
        if (PlayerSkillController.Instance == null) return;
        if (SkillDatabase.Instance == null) return;

        foreach (var save in data)
        {
            if (!save.isUnlocked) continue;

            skillLevels[save.skillid] = save.currentLevel - 1;

            KeyCode key = GetKeyForSkill(save.skillid);
            if (key != KeyCode.None)
            {
                var skill = SkillDatabase.Instance.GetSkill(save.skillid);
                if (skill != null)
                {
                    PlayerSkillController.Instance.equippedSkills[key] = skill;
                }
            }

            //Debug.Log(save.skillid + " currentlevel ->"+ skillLevels[save.skillid]);
        }
    }

    public bool TryRevive() //부활 가능여부 판단하기
    {
        int id = 1006; //부활 스킬id

        if (hasRevived)
            return false;

        if (!skillLevels.TryGetValue(id, out int level))
            return false;

        if (level == 0)
        {
            hasRevived = true;
            SkillSO skillSO = SkillDatabase.Instance.GetSkill(id);
            PlayerSkillController.Instance.ExecuteSkill(skillSO, level);
            return true;
        }

        return false;
    }

    KeyCode GetKeyForSkill(int skillId)
    {
        switch (skillId)
        {
            case 1003: return KeyCode.LeftShift;
            case 1004: return KeyCode.Z;
            case 1005: return KeyCode.X;
            case 1007: return KeyCode.C;
            default: return KeyCode.None;
        }
    }
}
