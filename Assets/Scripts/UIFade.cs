using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFade : MonoBehaviour
{
    public Transform player;
    public CanvasGroup canvasGroup;

    [Range(0f, 1f)]
    public float fadeAlpha = 0.3f;
    public float fadeSpeed = 10f;

    private RectTransform rectTransform;
    private Camera cam;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        cam = Camera.main;
    }

    private void Update()
    {
        Vector2 screenPos = cam.WorldToScreenPoint(player.position);

        bool contains = RectTransformUtility.RectangleContainsScreenPoint(rectTransform,screenPos,null);
        float targetAlpha = contains ? fadeAlpha : 1f;

        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
    }
}
