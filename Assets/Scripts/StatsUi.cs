using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsUi : MonoBehaviour
{
    public TMP_Text levelText;
    public TMP_Text damageText;
    public TMP_Text speedText;
    public TMP_Text defenseText;
    public TMP_Text critText;
    public TMP_Text healthText;

    private void Start()
    {
        UpdateAllStats();
    }

    public void UpdateAllStats() //스탯 실시간 업데이트
    {
        levelText.text = ExperienceManager.Instance.level.ToString();
        damageText.text = StatsManager.Instance.damage.ToString();
        speedText.text = StatsManager.Instance.speed.ToString();
        defenseText.text = StatsManager.Instance.defense.ToString();
        critText.text = StatsManager.Instance.crit + "%";
        healthText.text = StatsManager.Instance.currentHealth.ToString() + " / " + StatsManager.Instance.maxHealth.ToString();
    }
}
