using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro : MonoBehaviour
{
    public GameObject noSaveUI;
    public GameObject hasSaveUI;
    public GameObject settingsUI;
    public GameObject quitUI;

    public SettingUI setui;
    public UIAnim uiAnim;

    private bool isTransitioning;

    public void OnClickStart()
    {
        if (isTransitioning) return;

        if (SaveManager.Instance.HasSaveData())
        {
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.Click);
            uiAnim.Show(hasSaveUI);
        }
        else
        {
            OnStart();
        }
    }

    // 이어하기
    public void OnClickContinue()
    {
        if (isTransitioning) return;

        if (SaveManager.Instance.HasSaveData())
        {
            Debug.Log("세이브파일 있음!");
            OnLoad();
        }
        else
        {
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.Click);
            Debug.Log("세이브파일 없음!");
            uiAnim.Show(noSaveUI);
        }
    }

    // 설정
    public void OnClickSettings()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Click);
        uiAnim.Show(settingsUI);
    }

    // 종료
    public void OnClickQuit()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Click);
        uiAnim.Show(quitUI);
    }

    public void OnClose()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Cancel);
        PlayerPrefs.SetFloat("BGM", setui.bgmSlider.value);
        PlayerPrefs.SetFloat("SFX", setui.sfxSlider.value);
        PlayerPrefs.Save();

        uiAnim.Hide(settingsUI);
        uiAnim.Hide(noSaveUI);
        uiAnim.Hide(hasSaveUI);
        uiAnim.Hide(quitUI);
    }

    public void OnStart()
    {
        isTransitioning = true;
        SceneTransition.Instance.StartTransition("map1");
    }

    public void OnLoad()
    {
        isTransitioning = true;
        SceneTransition.Instance.LoadTransition("map1");
    }

    public void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
        Debug.Log("게임 종료"); // 에디터 확인용
    }

}
