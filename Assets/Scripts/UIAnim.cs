using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIAnim : MonoBehaviour
{
    public float duration = 0.25f;

    public void Show(GameObject ui)
    {
        ui.SetActive(true);

        RectTransform rt = ui.GetComponent<RectTransform>();
        rt.localScale = Vector3.zero;

        rt.DOScale(Vector3.one, duration).SetEase(Ease.OutBack);
    }

    public void Hide(GameObject ui)
    {
        RectTransform rt = ui.GetComponent<RectTransform>();

        rt.DOScale(Vector3.zero, duration).SetEase(Ease.InBack).OnComplete(() => {ui.SetActive(false);});
    }
}
