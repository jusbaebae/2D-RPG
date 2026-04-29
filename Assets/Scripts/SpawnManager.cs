using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SpawnManager : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject player;
    public CinemachineConfiner2D confiner;
    public PolygonCollider2D confinerboundary;

    void Start()
    {
        confiner = FindFirstObjectByType<CinemachineConfiner2D>();
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        player = GameManager.Instance.player;

        if (player != null && spawnPoint != null)
        {
            player.transform.position = spawnPoint.position;
        }
        if (confiner != null)
        {
            confiner.m_BoundingShape2D = confinerboundary;
            confiner.InvalidateCache();
        }
    }
}
