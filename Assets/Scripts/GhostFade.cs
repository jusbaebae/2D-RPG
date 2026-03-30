using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostFade : MonoBehaviour
{
    public float lifeTime = 0.3f;
    private float time;

    private SpriteRenderer[] renderers;
    void Start()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();
    }

    void Update()
    {
        time += Time.deltaTime;
        float alpha = Mathf.Lerp(0.7f, 0f, time / lifeTime);

        foreach(var sr in renderers)
        {
            Color c = sr.color;
            sr.color = new Color(c.r, c.g, c.b, alpha);
        }

        if(time >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
}
