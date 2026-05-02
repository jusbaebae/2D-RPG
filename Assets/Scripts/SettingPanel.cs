using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingPanel : MonoBehaviour
{
    public GameObject optionBtn;
    public GameObject villageBtn;
    public GameObject quitBtn;
    public GameObject SettingMenu;
    public GameObject villageMenu;
    public GameObject QuitMenu;

    public SettingUI setui;
    public UIAnim uiAnim;
    private SubMenuType currentType;

    public void SetBtn()
    {
        if(SceneController.Instance.CurrentSceneType == SceneType.Town)//마을이면 마을귀환은 빼기
        {
            optionBtn.SetActive(true);
            villageBtn.SetActive(false);
            quitBtn.SetActive(true);
        }
        else
        {
            optionBtn.SetActive(true);
            villageBtn.SetActive(true);
            quitBtn.SetActive(true);
        }
    }

    public void OnClick(SubMenuType type)
    {
        if (currentType != SubMenuType.None) return;
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Click);
        switch (type)
        {
            case SubMenuType.Setting:
                uiAnim.Show(SettingMenu);
                currentType = SubMenuType.Setting;
                break;
            case SubMenuType.Village:
                uiAnim.Show(villageMenu);
                currentType = SubMenuType.Village;
                break;
            case SubMenuType.Quit:
                uiAnim.Show(QuitMenu);
                currentType = SubMenuType.Quit;
                break;
        }
    }

    public void OnClose(SubMenuType type)
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Cancel);
        switch (type)
        {
            case SubMenuType.Setting:
                uiAnim.Hide(SettingMenu);
                break;
            case SubMenuType.Village:
                uiAnim.Hide(villageMenu);
                break;
            case SubMenuType.Quit:
                uiAnim.Hide(QuitMenu);
                break;
        }

        PlayerPrefs.SetFloat("BGM", setui.bgmSlider.value);
        PlayerPrefs.SetFloat("SFX", setui.sfxSlider.value);
        PlayerPrefs.Save();
        currentType = SubMenuType.None;
    }

    public void OnClickSetting() => OnClick(SubMenuType.Setting);
    public void OnClickVillage() => OnClick(SubMenuType.Village);
    public void OnClickQuit() => OnClick(SubMenuType.Quit);

    public void OnCloseSetting() => OnClose(SubMenuType.Setting);
    public void OnCloseVillage() => OnClose(SubMenuType.Village);
    public void OnCloseQuit() => OnClose(SubMenuType.Quit);

    public void OnComfirmVillage()
    {
        OnClose(SubMenuType.Village);
        UiManager.Instance.CloseAll();
        SceneTransition.Instance.StartTransition(GameManager.Instance.previousScene);
    }
    public void OnComfirmQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
public enum SubMenuType 
{ 
    None,
    Setting,
    Village, 
    Quit 
}
