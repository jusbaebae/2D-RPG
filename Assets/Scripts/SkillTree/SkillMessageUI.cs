using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class SkillMessageUI : MonoBehaviour
{
    public static SkillMessageUI Instance;

    public bool isOpen = false;

    [Header("UI")]
    public GameObject panel;
    public TMP_Text messageText;
    public Button OkButton;
    public Button NoButton;
    public Button CancelButton;

    private SkillsSlot targetSlot;

    private void Awake()
    {
        Instance = this;

        panel.SetActive(false);

        OkButton.onClick.AddListener(Confirm);
        NoButton.onClick.AddListener(Close);
        CancelButton.onClick.AddListener(Close);
    }

    private bool CanUpgrade(SkillsSlot slot)
    {
        return slot.isUnlocked && slot.currentLevel < slot.skillSo.maxLevel;
    }

    public void Show(SkillsSlot slot)
    {
        isOpen = true;
        targetSlot = slot;

        panel.SetActive(true);

        if (CanUpgrade(slot))
        {
            ShowConfirmUI(slot);
        }
        else
        {
            ShowFailUI(slot);
        }
    }

    private void ShowConfirmUI(SkillsSlot slot)
    {
        messageText.text = $"{slot.skillSo.skillName} 스킬을\n레벨 업 하시겠습니까?";

        OkButton.gameObject.SetActive(true);
        NoButton.gameObject.SetActive(true);
        CancelButton.gameObject.SetActive(false);
    }

    private void ShowFailUI(SkillsSlot slot)
    {
        if (!slot.isUnlocked)
        {
            messageText.text = "아직 해금되지 않은 스킬입니다.";
        }
        else
        {
            messageText.text = "이미 최대 레벨입니다.";
        }

        OkButton.gameObject.SetActive(false);
        NoButton.gameObject.SetActive(false);
        CancelButton.gameObject.SetActive(true);
    }

    public void FailPointUI()
    {
        
        messageText.text = "포인트가 부족합니다.";

        OkButton.gameObject.SetActive(false);
        NoButton.gameObject.SetActive(false);
        CancelButton.gameObject.SetActive(true);
    }

    private void Confirm()
    {
        targetSlot?.TryUpgradeSkill();
        Close();
    }

    private void Close()
    {
        isOpen = false;
        targetSlot = null;
        panel.SetActive(false);
    }
}
