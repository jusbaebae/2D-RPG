using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public static GameManager Instance;

    public string previousScene;

    [Header("Persitent Objects")]
    public GameObject[] persistentObjects;

    private void Awake()
    {
        if(Instance != null)
        {
            CleanUpAndDestroy();
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            MarkPersistentObject();
        }
    }

    void Start()
    {
        StartCoroutine(AutoSaveRoutine());
    }

    private void MarkPersistentObject()
    {
        foreach (GameObject obj in persistentObjects)
        {
            if(obj != null)
            {
                DontDestroyOnLoad(obj);
            }
        }
    }

    void OnApplicationQuit() //게임 끌때 저장
    {
        if (SceneManager.GetActiveScene().name == "StartScene") return;

            SaveManager.Instance.SaveGame();
    }

    private void CleanUpAndDestroy()
    {
        foreach(GameObject obj in persistentObjects)
        {
            Destroy(obj);
        }
        Destroy(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        previousScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator AutoSaveRoutine() //자동저장 60초마다
    {
        while (true)
        {
            yield return new WaitForSeconds(60f);
            if (!SceneManager.GetActiveScene().isLoaded) continue;
            SaveManager.Instance.SaveGame();
        }
    }

    public void ActiveAll()
    {
        foreach(GameObject obj in persistentObjects)
        {
            obj.SetActive(true);
        }
    }
}
