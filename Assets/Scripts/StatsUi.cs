using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsUi : MonoBehaviour
{
    public GameObject[] statsSlots;

    private void Start()
    {
        UpdateAllStats();
    }
    public void UpdateDamage()
    {
        statsSlots[0].GetComponentInChildren<TMP_Text>().text = "Damage: " + StatsManager.Instance.damage;
    }
    public void UpdateSpeed()
    {
        statsSlots[1].GetComponentInChildren<TMP_Text>().text = "Speed: " + StatsManager.Instance.speed;
    }

    public void UpdateAllStats()
    {
        UpdateDamage();
        UpdateSpeed();
    }
}
