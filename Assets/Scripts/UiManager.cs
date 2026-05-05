using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UiManager : MonoBehaviour
{
    public static UiManager Instance;

    public CanvasGroup inventoryUI; 
    public CanvasGroup shopUI;
    public CanvasGroup skillUI; 
    public CanvasGroup equipmentUI;
    public CanvasGroup questUI;

    public GameObject SettingPanel;
    public SettingPanel PanelBtnCS;
    public StatsUi statsUi;
    public ShopInventoryUI shopInventoryUI;


    public UIType currentOpenUI = UIType.None;
    public bool isInteract; //대화창 용도

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

    private void Update()
    {
        if (isInteract) return;

        if (Input.GetButtonDown("ToggleInventory"))
            ToggleUI(UIType.Inventory);
        if (Input.GetButtonDown("ToggleSkillTree"))
            ToggleUI(UIType.Skill);
        if (Input.GetButtonDown("ToggleEquipment"))
        {
            ToggleUI(UIType.Equipment);
            statsUi.UpdateAllStats();
        }
        if (Input.GetButtonDown("ToggleQuest"))
        {
            QuestUIManager.Instance.RefreshUI();
            ToggleUI(UIType.Quest);
        }
        if (Input.GetButtonDown("ToggleSetting"))
        {
            ToggleUI(UIType.Setting);
        }
        if (Input.GetKeyDown(KeyCode.Escape)) //ESC로 UI닫기
        {
            if (currentOpenUI != UIType.None) CloseAll();
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
            case UIType.Skill: 
                SetUI(skillUI, true); 
                break; 
            case UIType.Shop: 
                SetUI(shopUI, true);
                shopInventoryUI.ShowItemTab();
                break; 
            case UIType.Equipment: 
                SetUI(equipmentUI, true);
                break;
            case UIType.Quest:
                QuestManager.Instance.CheckCollectQuests();
                SetUI(questUI, true);
                break;
            case UIType.Setting:
                SettingPanel.SetActive(true);
                PanelBtnCS.SetBtn();
                break;
        }
        if (type != UIType.Inventory && type != UIType.Equipment)
        {
            PlayerMovement.Instance.isinteract = true;
        }
        currentOpenUI = type;
    }

    public void CloseAll()
    {
        SetUI(inventoryUI, false); 
        SetUI(shopUI, false); 
        SetUI(skillUI, false);
        SetUI(equipmentUI, false); 
        SetUI(questUI, false);
        SettingPanel.SetActive(false);
        SkillMessageUI.Instance.Close(false);

        Time.timeScale = 1;
        if (currentOpenUI != UIType.Inventory && currentOpenUI != UIType.Equipment && currentOpenUI != UIType.None)
        {
            PlayerMovement.Instance.isinteract = false;
        }
        currentOpenUI = UIType.None;
    }

    void SetUI(CanvasGroup ui, bool state)
    {
        if (!ui) return;

        ui.alpha = state ? 1 : 0;
        ui.blocksRaycasts = state;
        ui.interactable = state;
    }
}

public enum UIType
{
    None,
    Inventory,
    Skill,
    Shop,
    Equipment,
    Quest,
    Setting
}
