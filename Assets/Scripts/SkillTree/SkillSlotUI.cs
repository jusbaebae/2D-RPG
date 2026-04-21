using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillSlotUI : MonoBehaviour
{
    public SkillSO skill;
    public Image icon;
    public Image cooldownOverlay;
    public TMP_Text cooldownText;
    public Image lockOverlay;
    bool wasCoolingDown = false;

    void Update()
    {
        if (skill == null) return;

        int level = SkillManager.Instance.skillLevels.ContainsKey(skill.skillid) ? SkillManager.Instance.skillLevels[skill.skillid] : -1;

        //해금 안됨
        if (level < 0)
        {
            lockOverlay.gameObject.SetActive(true);
            icon.color = Color.gray;

            cooldownOverlay.gameObject.SetActive(false);
            cooldownText.gameObject.SetActive(false);
            return;
        }
        else
        {
            lockOverlay.gameObject.SetActive(false);
            icon.color = Color.white;
        }

        //쿨타임 표시
        float remain = PlayerSkillController.Instance.GetRemainingCooldown(skill.skillid);

        float total = skill.levelData[level].cooltime;

        bool isCoolingDown = remain > 0;

        if (wasCoolingDown && !isCoolingDown)
        {
            StartCoroutine(PlayReadyEffect());
        }
        if (remain > 0)
        {
            cooldownOverlay.fillAmount = remain / total;
            cooldownText.text = remain.ToString("F1");

            cooldownOverlay.gameObject.SetActive(true);
            cooldownText.gameObject.SetActive(true);
        }
        else
        {
            cooldownOverlay.fillAmount = 0;
            cooldownOverlay.gameObject.SetActive(false);
            cooldownText.gameObject.SetActive(false);
        }

        wasCoolingDown = isCoolingDown;
    }

    IEnumerator PlayReadyEffect()
    {
        float t = 0;
        float duration = 0.2f;

        while (t < duration)
        {
            float scale = 1 + Mathf.Sin(t / duration * Mathf.PI) * 0.2f;
            transform.localScale = Vector3.one * scale;

            t += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.one;
    }
}
