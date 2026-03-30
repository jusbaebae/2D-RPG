using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    Vector3 lastPos;
    public float distanceThreshold;
    public bool makeGhost;


    void Update()
    {
        if (!makeGhost) return;
        
        float dist = Vector3.Distance(transform.position, lastPos);

        if (dist > distanceThreshold)
        {
            CreateGhost();
            lastPos = transform.position;
        }
    }

    void CreateGhost()
    {
        //부모 오브젝트 (잔상 묶음)
        GameObject ghostRoot = new GameObject("Ghost");

        ghostRoot.AddComponent<GhostFade>();

        //모든 SpriteRenderer 가져오기
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();

        foreach (var sr in renderers)
        {
            GameObject part = new GameObject("Part");

            //위치/회전/스케일 복사
            part.transform.position = sr.transform.position;
            part.transform.rotation = sr.transform.rotation;
            part.transform.localScale = sr.transform.lossyScale;

            //SpriteRenderer 복사
            SpriteRenderer ghostSR = part.AddComponent<SpriteRenderer>();
            ghostSR.sprite = sr.sprite;
            ghostSR.material = new Material(sr.material);
            ghostSR.sortingLayerName = "Ghost";
            ghostSR.sortingOrder = sr.sortingOrder - 1;

            Color c = sr.color;
            ghostSR.color = new Color(c.r, c.g, c.b, 0.7f);
            part.transform.SetParent(ghostRoot.transform, true);
        }

        Destroy(ghostRoot, 0.3f);
    }
}
