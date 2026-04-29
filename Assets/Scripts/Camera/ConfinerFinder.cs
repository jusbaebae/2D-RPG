using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class ConfinerFinder : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        CinemachineConfiner2D confiner = GetComponent<CinemachineConfiner2D>();

        // 1. 태그로 오브젝트 찾기
        GameObject confinerObj = GameObject.FindWithTag("Confiner");
        if (confinerObj != null)
        {
            PolygonCollider2D collider = confinerObj.GetComponent<PolygonCollider2D>();
            if (collider != null)
            {
                confiner.m_BoundingShape2D = collider;
                confiner.InvalidateCache();
                Debug.Log("Confiner 연결 성공!");
            }
        }
    }
}

