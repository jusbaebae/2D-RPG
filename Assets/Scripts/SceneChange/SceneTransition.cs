using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;

    public CanvasGroup panel;
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
    }

    public Tween FadeOut(float time) => panel.DOFade(1f, time);
    public Tween FadeIn(float time) => panel.DOFade(0f, time);

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
        panel.alpha = 0f;
        panel.gameObject.SetActive(true);

        //씬이동
        GameManager.Instance.ActiveAll();
        GameManager.Instance.previousScene = SceneManager.GetActiveScene().name;
        yield return panel.DOFade(1f, duration).SetEase(Ease.InOutExpo).WaitForCompletion();
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

        yield return panel.DOFade(0f, duration).SetEase(Ease.InExpo).WaitForCompletion();
    }
}
