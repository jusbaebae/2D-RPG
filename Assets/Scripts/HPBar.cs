using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public Slider slider;

    float targetHealth;

    void Update()
    {
        slider.value = Mathf.Lerp(slider.value, targetHealth, Time.deltaTime * 5f);
    }

    public void SetMaxHealth(int maxHealth)
    {
        slider.maxValue = maxHealth;
    }

    public void SetHealth(int health)
    {
        targetHealth = health;
    }
}
