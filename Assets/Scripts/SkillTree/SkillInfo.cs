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
    public TMP_Text RequirePointText;

    [Header("현재 레벨 정보")]
    public TMP_Text levelText;

    [Header("스탯")]
    public TMP_Text[] statTexts;

    private RectTransform infoPanelRect;

    private void Awake()
    {
        infoPanelRect = GetComponent<RectTransform>();
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

        // 요구 포인트 표시
        if(currentLevel == skill.maxLevel)
        {
            RequirePointText.text = "";
        }
        else
        {
            RequirePointText.text = "필요 포인트 : " + skill.levelData[currentLevel].requirePoint.ToString();
        }
        

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
        Vector2 mousePosition = Input.mousePosition;
        Vector2 offset = new Vector2(50, 150);

        Vector2 panelSize = Vector2.Scale(infoPanelRect.rect.size, infoPanelRect.lossyScale);
        Vector2 targetPos = mousePosition + new Vector2(offset.x, offset.y);

        // 오른쪽 화면 밖 체크
        if (targetPos.x + panelSize.x > Screen.width)
        {
            targetPos.x = mousePosition.x - panelSize.x - offset.x;
        }

        // 아래쪽 화면 밖 체크
        if (targetPos.y - panelSize.y < 0)
        {
            targetPos.y = panelSize.y;
        }

        infoPanelRect.position = targetPos;
    }
}
