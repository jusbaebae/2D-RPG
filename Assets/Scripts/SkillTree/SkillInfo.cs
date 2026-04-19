using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkillInfo : MonoBehaviour
{
    public CanvasGroup infoPanel;

    public TMP_Text skillNameText;
    public TMP_Text skillDescriptionText;
    public TMP_Text skillTypeText;

    [Header("현재 레벨 정보")]
    public TMP_Text levelText;

    [Header("스탯")]
    public TMP_Text[] statTexts;

    private RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void ShowSkillInfo(SkillSO skill, int currentLevel, bool isunlocked)
    {
        infoPanel.alpha = 1;

        // 기본 정보
        skillNameText.text = skill.skillName;
        skillDescriptionText.text = skill.skilldescription;
        if (skill.skillType == SkillType.Active)
        {
            skillTypeText.text = "액티브";
            skillTypeText.color = Color.red;
        }
        else
        {
            skillTypeText.text = "패시브";
            skillTypeText.color = Color.blue;
        }
        

        // 레벨 표시
        if (!isunlocked)
        {
            levelText.text = "UNLOCK";
        }
        else
        {
            if(currentLevel == skill.maxLevel)
            {
                levelText.text = $"LV.MAX";
            }
            else 
            {
                levelText.text = $"Lv.{currentLevel} / {skill.maxLevel}";
            }
            
        }

        // 현재 레벨 데이터 가져오기
        int index = Mathf.Clamp(currentLevel - 1, 0, skill.levelData.Length - 1);
        SkillLevelData data = skill.levelData[index];

        List<string> stats = new List<string>();

        if (data.cooltime > 0)
            stats.Add($"Cooldown: {data.cooltime}");

        if (data.value > 0)
            stats.Add($"Value: {data.value}");

        // UI 적용
        for (int i = 0; i < statTexts.Length; i++)
        {
            if (i < stats.Count)
            {
                statTexts[i].text = stats[i];
                statTexts[i].gameObject.SetActive(true);
            }
            else
            {
                statTexts[i].gameObject.SetActive(false);
            }
        }
        
    }

    public void Hide()
    {
        infoPanel.alpha = 0;
    }

    public void FollowMouse()
    {
        Vector3 offset = new Vector3(50, 150, 0);
        rect.position = Input.mousePosition + offset;
    }
}
