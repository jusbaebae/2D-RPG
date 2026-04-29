using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager Instance;

    public Room currentRoom;
    public GameObject player;
    public CinemachineConfiner2D confiner;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        player = GameManager.Instance.player;
        confiner = FindFirstObjectByType<CinemachineConfiner2D>();

        if (currentRoom != null)
        {
            ApplyRoom(currentRoom);
        }
    }

    public void EnterRoom(Room room)
    {
        currentRoom = room;
        ApplyRoom(room);
        MinimapManager.Instance.VisitRoom(room);
        MinimapManager.Instance.UpdatePlayerPosition(room);
    }

    void ApplyRoom(Room room)
    {
        // 플레이어 위치
        if (player != null)
            player.transform.position = room.spawnPoint.position;

        // 카메라 경계
        if (confiner != null)
        {
            confiner.m_BoundingShape2D = room.confiner;
            confiner.InvalidateCache();
        }
    }
}
