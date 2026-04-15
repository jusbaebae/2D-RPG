using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UiManager : MonoBehaviour
{
    public static UiManager Instance;

    public StatsUi statsUi;

    public CanvasGroup inventoryUI;
    public CanvasGroup shopUI;
    public CanvasGroup statUI;
    public CanvasGroup skillUI;
    public CanvasGroup equipmentUI;

    private UIType currentOpenUI = UIType.None;
    public bool isInteract;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (isInteract) return;

        if (Input.GetButtonDown("ToggleStats"))
        {
            ToggleUI(UIType.Stat);
            statsUi.UpdateAllStats();
        }
        if (Input.GetButtonDown("ToggleInventory"))
            ToggleUI(UIType.Inventory);
        if (Input.GetButtonDown("ToggleSkillTree"))
            ToggleUI(UIType.Skill);
        if (Input.GetButtonDown("ToggleEquipment"))
            ToggleUI(UIType.Equipment);
        if (Input.GetKeyDown(KeyCode.Escape)) //ESC로 UI닫기
        {
            CloseAll();
        }
    }

    public void ToggleUI(UIType type)
    {
        if(currentOpenUI == type) //이미 열려있는UI면 닫기
        {
            CloseAll();
            return;
        }
        if (currentOpenUI != UIType.None) //다른 UI가 열려있으면 무시
            return;

        OpenUI(type);
    }

    private void OpenUI(UIType type)
    {
        CloseAll();

        switch (type)
        {
            case UIType.Inventory:
                SetUI(inventoryUI, true);
                break;
            case UIType.Stat:
                SetUI(statUI, true);
                break;
            case UIType.Skill:
                SetUI(skillUI, true);
                break;
            case UIType.Shop:
                SetUI(shopUI, true);
                break;
            case UIType.Equipment:
                SetUI(equipmentUI, true);
                break;
        }

        Time.timeScale = 0;
        currentOpenUI = type;
    }

    public void CloseAll()
    {
        SetUI(inventoryUI, false);
        SetUI(shopUI, false);
        SetUI(statUI, false);
        SetUI(skillUI, false);
        SetUI(equipmentUI, false);

        Time.timeScale = 1;
        currentOpenUI = UIType.None;
    }

    void SetUI(CanvasGroup ui, bool state)
    {
        ui.alpha = state ? 1 : 0;
        ui.blocksRaycasts = state;
        ui.interactable = state;
    }
}

public enum UIType
{
    None,
    Inventory,
    Stat,
    Skill,
    Shop,
    Equipment
}
