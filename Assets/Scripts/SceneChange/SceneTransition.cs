using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;

    public RectTransform panel;
    public float duration = 0.2f;
    public float holdTime = 0.5f;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
        panel.localScale = Vector3.zero;
    }

    public void StartTransition(string sceneName)
    {
        StartCoroutine(TransitionRoutine(sceneName, false));
    }

    public void LoadTransition(string sceneName)
    {
        StartCoroutine(TransitionRoutine(sceneName, true));
    }

    public IEnumerator TransitionRoutine(string sceneName, bool isload)
    {
        panel.localScale = Vector3.zero;

        yield return panel.DOScale(1.1f, duration).SetEase(Ease.InOutExpo) .WaitForCompletion();

        //씬이동
        GameManager.Instance.ActiveAll();
        if (isload)
        {
            SaveManager.Instance.LoadGame();
        }
        else
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
        

        yield return new WaitForSeconds(holdTime);

        yield return panel.DOScale(0f, duration).SetEase(Ease.InExpo).WaitForCompletion();
    }
}
