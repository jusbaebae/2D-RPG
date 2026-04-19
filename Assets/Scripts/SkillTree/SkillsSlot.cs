using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

using UnityEngine.EventSystems;

public class SkillsSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    public List<SkillsSlot> prerequisiteSkillSlots;
    public SkillSO skillSo;
    [SerializeField] private SkillInfo skillInfo;

    public int currentLevel;
    public bool isUnlocked;

    public Image ButtonImage;
    public Image skillIcon;
    public Button skillButton;
    public TMP_Text skillLevelText;

    public static event Action<SkillsSlot> OnAbilityPointSpent;
    public static event Action OnSkillMaxed;

    private void OnValidate()
    {
        if(skillSo != null && skillLevelText != null)
        {
            UpdateUI();
        }
    }

    public void TryUpgradeSkill() //스킬 레벨 업그레이드
    {
        if(isUnlocked && currentLevel < skillSo.maxLevel)
        {
            currentLevel++;
            OnAbilityPointSpent?.Invoke(this);

            if(currentLevel >= skillSo.maxLevel)
            {
                OnSkillMaxed?.Invoke();
            }
            UpdateUI();
        }
    }

    public bool CanUnlockSkill() //선행 스킬 검사
    {
        foreach(SkillsSlot slot in prerequisiteSkillSlots)
        {
            if (!slot.isUnlocked || slot.currentLevel < slot.skillSo.maxLevel)
            {
                return false;
            }
        }
        return true;
    }

    public void Unlock() //스킬 해금
    {
        isUnlocked = true;
        UpdateUI();
    }

    public int GetRequiredPoint() //레벨 별 요구 포인트
    {
        if (currentLevel >= skillSo.levelData.Length)
            return 0;

        return skillSo.levelData[currentLevel].requirePoint;
    }

    public void UpdateUI() //화면상 UI업데이트
    {
        skillIcon.sprite = skillSo.skillIcon;
        if (isUnlocked)
        {
            skillButton.interactable = true;
            if(currentLevel == skillSo.maxLevel)
            {
                skillLevelText.text = "LV.MAX";
            }
            else
            {
                skillLevelText.text = "LV " + currentLevel.ToString();
            }
            ButtonImage.color = Color.white;
            skillIcon.color = Color.white;
        }
        else
        {
            skillButton.interactable = false;
            skillLevelText.text = "Locked";
            ButtonImage.color = Color.grey;
            skillIcon.color = Color.grey;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (SkillMessageUI.Instance.isOpen) return;

        if (skillSo != null)
        {
            Cursor.visible = false;
            skillInfo.ShowSkillInfo(skillSo, currentLevel, isUnlocked);
        }
           
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.visible = true;
        skillInfo.Hide();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (skillInfo != null) skillInfo.FollowMouse();
    }

    
}
