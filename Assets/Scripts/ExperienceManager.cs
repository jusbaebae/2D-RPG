using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager Instance;

    private Coroutine levelUpCoroutine;
    public GameObject LevelUpEffect;
    public TextMeshProUGUI levelUpText;

    public int level;
    public int currentExp;
    private int expToLevel = 10;
    public float expGrowthMultiplier = 1.2f; //레벨오를때마다 최대경험치가 20%씩 상승
    public Slider expSlider;
    public TMP_Text currentLevelText;

    public static event Action<int> OnLevelUp;

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

    private void Start()
    {
        UpdateUi();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11))
        {
            GainExperience(1000);
            InventoryManager.Instance.gold += 9999;
        }
        expSlider.value = Mathf.Lerp(expSlider.value, currentExp, Time.deltaTime * 5f);
    }

    public void GainExperience(int amount)
    {
        currentExp += amount;
        while (currentExp >= expToLevel)
        {
            LevelUp();
        }
        UpdateUi();
    }

    private void OnEnable()
    {
        Enemy_Health.OnMonsterDefeated += GainExperience;
        DialogueManager.OnRewardexp += GainExperience;
    }
    private void OnDisable()
    {
        Enemy_Health.OnMonsterDefeated -= GainExperience;
        DialogueManager.OnRewardexp -= GainExperience;
    }

    private void LevelUp()
    {
        level++;
        if (levelUpCoroutine == null)
        {
            levelUpCoroutine = StartCoroutine(Levelupeffect());
        }
        currentExp -= expToLevel;
        if (level < 20) expToLevel = Mathf.CeilToInt(expToLevel * 1.1f);
        else if (level < 50) expToLevel = Mathf.CeilToInt(expToLevel * 1.07f);
        else expToLevel = Mathf.CeilToInt(expToLevel * 1.04f);
        OnLevelUp?.Invoke(4);
    }

    IEnumerator Levelupeffect()
    {
        if(LevelUpEffect != null)
        {
            LevelUpEffect.SetActive(true);
            PlayWorldSpaceLevelUp(PlayerMovement.Instance.transform);
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.Levelup);

            yield return new WaitForSeconds(1f);

            LevelUpEffect.SetActive(false);
        }
        levelUpCoroutine = null;
    }

    public void PlayWorldSpaceLevelUp(Transform playerTransform)
    {
        // 텍스트를 플레이어 머리 위 위치로 초기화
        levelUpText.transform.position = playerTransform.position + Vector3.up * 2.0f;
        levelUpText.gameObject.SetActive(true);
        levelUpText.color = new Color(1, 1, 1, 0); // 투명하게 시작

        // DOTween 연출
        Sequence seq = DOTween.Sequence();

        // 1. 나타나면서 살짝 커짐
        seq.Append(levelUpText.DOFade(1f, 0.2f));
        seq.Join(levelUpText.transform.DOPunchScale(Vector3.one * 1.2f, 0.3f));

        // 2. 위로 서서히 올라가며 사라짐
        seq.Append(levelUpText.transform.DOMoveY(levelUpText.transform.position.y + 1.5f, 0.8f));
        seq.Join(levelUpText.DOFade(0f, 0.8f));

        seq.OnComplete(() => levelUpText.gameObject.SetActive(false));
    }

    public void UpdateUi()
    {
        expSlider.maxValue = expToLevel;
        currentLevelText.text = "Level: " + level;
    }

    public void FillData(PlayerData data)
    {
        data.level = level;
        data.exp = currentExp;
        data.maxexp = expToLevel;
    }

    public void LoadFromData(PlayerData data)
    {
        level = data.level;
        currentExp = data.exp;
        expToLevel = data.maxexp;

        UpdateUi();
    }
}
