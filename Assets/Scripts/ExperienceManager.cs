using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager Instance;

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
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GainExperience(100);
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
        currentExp -= expToLevel;
        if (level < 20) expToLevel = Mathf.CeilToInt(expToLevel * 1.1f);
        else if (level < 50) expToLevel = Mathf.CeilToInt(expToLevel * 1.07f);
        else expToLevel = Mathf.CeilToInt(expToLevel * 1.04f);
        OnLevelUp?.Invoke(4);
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
