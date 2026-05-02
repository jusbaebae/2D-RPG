using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;

    public SceneType CurrentSceneType;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CurrentSceneType = FindFirstObjectByType<SceneInfo>().sceneType;
        if(CurrentSceneType == SceneType.Town)
        {
            SaveManager.Instance.OnEnterTown();
            AudioManager.Instance.PlayBgm(AudioManager.BgmType.Town);
        }
        if (CurrentSceneType == SceneType.Dungeon)
        {
            AudioManager.Instance.PlayBgm(AudioManager.BgmType.Dungeon);
        }
    }
}

public enum SceneType
{
    Town,
    Dungeon
}
