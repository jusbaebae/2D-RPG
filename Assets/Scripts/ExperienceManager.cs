using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ExperienceManager : MonoBehaviour
{
    public int level;
    public int currentExp;
    private int expToLevel = 10;
    public float expGrowthMultiplier = 1.2f; //레벨오를때마다 최대경험치가 20%씩 상승
    public Slider expSlider;
    public TMP_Text currentLevelText;

    public static event Action<int> OnLevelUp;

    private void Start()
    {
        UpdateUi();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GainExperience(2);
        }
        expSlider.value = Mathf.Lerp(expSlider.value, currentExp, Time.deltaTime * 5f);
    }

    public void GainExperience(int amount)
    {
        currentExp += amount;
        if(currentExp >= expToLevel)
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
        expToLevel = Mathf.RoundToInt(expToLevel * expGrowthMultiplier);
        OnLevelUp?.Invoke(1);
    }

    public void UpdateUi()
    {
        expSlider.maxValue = expToLevel;
        currentLevelText.text = "Level: " + level;
    }
}
