using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro : MonoBehaviour
{
    public GameObject noSaveUI;
    public GameObject hasSaveUI;
    public GameObject settingsUI;

    public UIAnim uiAnim;

    public void OnClickStart()
    {
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
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
        Debug.Log("게임 종료"); // 에디터 확인용
    }

    public void OnClose()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Cancel);
        uiAnim.Hide(settingsUI);
        uiAnim.Hide(noSaveUI);
        uiAnim.Hide(hasSaveUI);
    }

    public void OnStart()
    {
        SceneTransition.Instance.StartTransition("map1");
    }

    public void OnLoad()
    {
        SceneTransition.Instance.LoadTransition("map1");
    }
}
