using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    public static PlayerSkillController Instance;

    //현재 슬롯에 장착된 스킬들 (SkillManager에서 해금 시 여기에 등록)
    public Dictionary<KeyCode, SkillSO> equippedSkills = new Dictionary<KeyCode, SkillSO>();

    //스킬별 스킬 로직 저장하기
    Dictionary<int, ISkillLogic> skillMap;

    //쿨타임용 다음에 사용 가능한 시각
    private Dictionary<int, float> nextReadyTime = new Dictionary<int, float>();

    [SerializeField]
    private SkillSlotUI dash, strongHit, heal, doubleSlash;

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

        skillMap = new Dictionary<int, ISkillLogic>()
        {
            { 1004, new StrongHit() },
            { 1005, new Heal() } ,
            { 1006, new Revive() } ,
            { 1007, new DoubleSlash() }
        };
    }

    void Update()
    {
        if (PlayerMovement.Instance.isDead) return;

        if (Input.GetKeyDown(KeyCode.LeftShift) && PlayerMovement.Instance.inputDir != Vector2.zero)
        {
            AttemptSkill(KeyCode.LeftShift);
        } 
        if (Input.GetKeyDown(KeyCode.Z)) AttemptSkill(KeyCode.Z);
        if (Input.GetKeyDown(KeyCode.X)) AttemptSkill(KeyCode.X);
        if (Input.GetKeyDown(KeyCode.C) && InventoryManager.Instance.IsWeaponEquipped())
        {
            AttemptSkill(KeyCode.C);
        }
    }

    void AttemptSkill(KeyCode key)
    {
        if (PlayerMovement.Instance.isDead) return;

        if (equippedSkills.ContainsKey(key)) //딕셔너리에 현재 스킬이 저장되어있는지 확인(해금여부)
        {
            SkillSO skill = equippedSkills[key];
            int currentLevel = SkillManager.Instance.skillLevels[skill.skillid];
            //Debug.Log(currentLevel);
            //Debug.Log(skill.levelData[currentLevel].cooltime);

            //쿨타임 체크
            if (IsCoolingDown(skill))
            {
                //Debug.Log("현재 쿨타임");
                return;
            }

            if(key != KeyCode.LeftShift) //대쉬는 로직이 따로있으니 빼기
            {
                ExecuteSkill(skill, currentLevel); //스킬 실행
            }
            
            //쿨타임 시작
            StartCooldown(skill, skill.levelData[currentLevel].cooltime);
        }
    }

    public void ExecuteSkill(SkillSO skill, int level)
    {
        int currentLevel = level; // 현재 레벨 가져오기

        if (skillMap.TryGetValue(skill.skillid, out var logic))
        {
            StartCoroutine(logic.Execute(PlayerMovement.Instance, skill, currentLevel));
        }

        //Debug.Log($"{skill.skillName} 시전!");
    }

    public void EquipSkill(KeyCode key, SkillsSlot slot)
    {
        equippedSkills[key] = slot.skillSo;
    }

    private bool IsCoolingDown(SkillSO skill)
    {
        int id = skill.skillid;

        // 저장소에 데이터가 없으면 처음 쓰는 것이므로 쿨타임 아님
        if (!nextReadyTime.ContainsKey(id)) return false;

        // 현재 시간이 '다음에 사용 가능한 시각'보다 작으면 쿨타임 중
        return Time.time < nextReadyTime[id];
    }

    public float GetRemainingCooldown(int skillId) //남은 쿨타임 시간
    {
        if (!nextReadyTime.ContainsKey(skillId)) return 0f;

        float remain = nextReadyTime[skillId] - Time.time;
        return Mathf.Max(0, remain);
    }

    private void StartCooldown(SkillSO skill, float cooltime)
    {
        nextReadyTime[skill.skillid] = Time.time + cooltime;
    }
}
