using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DoorSlot
{
    public DoorDirection direction;

    public GameObject wall; // 연결 안된 경우
    public Door door;       // 연결된 경우

    public bool isConnected = false;

    // 연결 설정 (맵 생성 시 사용)
    public void SetConnection(bool connected)
    {
        isConnected = connected;

        if (connected)
        {
            wall.SetActive(false);
            door.gameObject.SetActive(true);
        }
        else
        {
            wall.SetActive(true);
            door.gameObject.SetActive(false);
        }
    }

    public void Close()
    {
        if (isConnected)
            door.Close();
    }

    public void Open()
    {
        if (isConnected)
            door.Open();
    }
}
