using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;


public class DamageText : MonoBehaviour
{
    public float moveDistance = 1.5f; //이동 거리
    public float duration = 0.8f;     //전체 시간

    TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetDamage(int damage, bool isCrit)
    {
        text.text = damage.ToString();

        if (isCrit) text.color = Color.magenta; //크리티컬이면 보라색텍스트

        //랜덤 방향
        float randomX = Random.Range(-0.5f, 0.5f);
        Vector3 moveDir = new Vector3(randomX, 1f, 0f).normalized;

        //시작 상태
        transform.localScale = Vector3.zero;
        float scale = isCrit ? 0.15f : 0.1f;

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(scale, 0.3f).SetEase(Ease.OutBack));
        seq.Append(transform.DOScale(0.05f, 0.2f));
        seq.Join(transform.DOMove(transform.position + moveDir * moveDistance, duration).SetEase(Ease.OutCubic));
        seq.Join(text.DOFade(0f, duration).SetEase(Ease.InQuad));

        seq.SetLink(gameObject);
        seq.OnComplete(() => Destroy(gameObject));
    }
}
