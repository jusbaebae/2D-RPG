using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    SpriteRenderer[] renderers;

    public GameObject damageTextPrefab;
    public GameObject HealTextPrefab;
    public GameObject ReviveEffect;

    public TMP_Text healthText;
    public HPBar hpbar;

    Color[] originalColors; //원래 스프라이트 색 저장하기

    void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            originalColors[i] = renderers[i].color;
        }
    }
    void Start()
    {
        StatsManager.Instance.currentHealth = StatsManager.Instance.maxHealth;
        healthText.text = StatsManager.Instance.currentHealth + " / " + StatsManager.Instance.maxHealth;
        hpbar.SetMaxHealth(StatsManager.Instance.maxHealth);
        hpbar.SetHealth(StatsManager.Instance.currentHealth);
    }
    public void ChangeHealth(int amount)
    {
        if (PlayerMovement.Instance.isInvincible) return;

        StatsManager.Instance.currentHealth += amount;
        StatsManager.Instance.currentHealth = Mathf.Clamp(StatsManager.Instance.currentHealth,0,StatsManager.Instance.maxHealth);
        healthText.text = StatsManager.Instance.currentHealth + " / " + StatsManager.Instance.maxHealth;
        ShowDamage(Mathf.Abs(amount));
        hpbar.SetHealth(StatsManager.Instance.currentHealth);

        StartCoroutine(HitFlash());

        if (StatsManager.Instance.currentHealth <= 0)
        {
            //부활 체크
            if (SkillManager.Instance.TryRevive())
                return;

            PlayerMovement.Instance.isDead = true;
            Die();
            StartCoroutine(ReSpawn());
            //Debug.Log("부활스킬없어서 그냥 죽었다!");
        }
    }

    public void Die()
    {
        PlayerMovement.Instance.isInvincible = true;
        PlayerMovement.Instance.ChangeState(PlayerState.DEATH, 0);
    }

    void ShowDamage(int damage) //데미지 효과
    {
        GameObject dmg = Instantiate(damageTextPrefab, transform.position, Quaternion.identity);
        dmg.GetComponent<DamageText>().SetDamage(damage,false);
    }

    public void ShowHeal(int Heal) //힐 텍스트 효과
    {
        GameObject dmg = Instantiate(HealTextPrefab, transform.position, Quaternion.identity);
        dmg.GetComponent<DamageText>().SetDamage(Heal,false);
    }

    IEnumerator HitFlash() //맞으면 깜빡이는 효과
    {
        float duration = 0.2f;
        float interval = 0.05f;

        float time = 0;

        

        while (time < duration) //맞으면 깜빡깜빡하기
        {
            SetColor(Color.red);
            yield return new WaitForSeconds(interval);

            RestoreColor(originalColors);
            yield return new WaitForSeconds(interval);

            time += interval * 2;
        }

        //저장했던 색깔 복구
        RestoreColor(originalColors);
    }

    void SetColor(Color color)
    {
        foreach (var r in renderers)
        {
            r.color = color;
        }
    }

    void RestoreColor(Color[] colors)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].color = colors[i];
        }
    }

    IEnumerator ReSpawn()
    {
        yield return new WaitForSeconds(0.5f);
        SceneTransition.Instance.StartTransition(SaveManager.Instance.respawnScene);
        yield return new WaitForSeconds(1f);
        StatsManager.Instance.UpdateHealth(9999);
        PlayerMovement.Instance.isDead = false;
        PlayerMovement.Instance.ChangeState(PlayerState.IDLE, 0);
        PlayerMovement.Instance.isInvincible = false;
        SaveManager.Instance.SaveGame();
    }
}
