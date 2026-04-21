using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UiDrag : MonoBehaviour, IDragHandler
{
    public RectTransform targetPanel;
    public Canvas canvas;


    private Vector2 offset;

    void Start()
    {
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        targetPanel.anchoredPosition += eventData.delta / canvas.scaleFactor;

        ClampToScreen();
    }

    private void ClampToScreen() //UI창 밖으로 못나가게
    {
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        Vector2 size = targetPanel.rect.size;
        Vector2 pos = targetPanel.anchoredPosition;

        pos.x = Mathf.Clamp(pos.x, -canvasRect.sizeDelta.x / 2 + size.x / 2, canvasRect.sizeDelta.x / 2 - size.x / 2);
        pos.y = Mathf.Clamp(pos.y, -canvasRect.sizeDelta.y / 2 + size.y / 2, canvasRect.sizeDelta.y / 2 - size.y / 2);

        targetPanel.anchoredPosition = pos;
    }
}
