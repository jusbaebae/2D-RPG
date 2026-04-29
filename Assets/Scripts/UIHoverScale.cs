using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UIHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public float scaleUp = 1.1f;
    public float duration = 0.2f;

    [Header("Click Bounce")]
    public float pressScale = 0.9f;   // 눌릴 때
    public float bounceScale = 1.15f; // 튀는 크기

    private Vector3 originalScale;
    private Tween currentTween;

    private bool isHovering;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        currentTween?.Kill();
        currentTween = transform.DOScale(originalScale * scaleUp, duration).SetEase(Ease.OutBack).SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        currentTween?.Kill();
        currentTween = transform.DOScale(originalScale, duration).SetEase(Ease.OutQuad).SetUpdate(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        currentTween?.Kill();
        currentTween = transform.DOScale(originalScale * pressScale, 0.08f).SetEase(Ease.OutQuad).SetUpdate(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        currentTween?.Kill();

        Sequence seq = DOTween.Sequence().SetUpdate(true);

        //살짝 크게 튀기
        seq.Append(transform.DOScale(originalScale * bounceScale, 0.1f).SetEase(Ease.OutQuad));

        //호버 상태면 hoverScale, 아니면 원래 크기로
        float target = isHovering ? scaleUp : 1f;
        seq.Append(transform.DOScale(originalScale * target, 0.15f).SetEase(Ease.OutBack));
    }
}
