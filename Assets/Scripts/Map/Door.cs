using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;

public class Door : MonoBehaviour
{
    public DoorDirection doorDirection;
    public Room connectedRoom; //연결된 방
    public Tilemap tilemap;
    private bool isOpen = false;
    public Collider2D block; //문 Collider(다음방 이동)

    void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        block = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isOpen) return;
        if (!other.CompareTag("Player")) return;

        StartCoroutine(EnterRoomRoutine());

    }

    private IEnumerator EnterRoomRoutine()
    {
       
        yield return SceneTransition.Instance.FadeOut(0.5f).WaitForCompletion();
        PlayerMovement.Instance.isinteract = true;

        DungeonManager.Instance.EnterRoom(connectedRoom, doorDirection);

        yield return new WaitForSeconds(0.3f);

        yield return SceneTransition.Instance.FadeIn(0.5f).WaitForCompletion();
        PlayerMovement.Instance.isinteract = false;
    }

    public void Close()
    {
        isOpen = false;
        block.isTrigger = false;
        SetAlpha(1f);    
    }

    public void Open()
    {
        isOpen = true;
        block.isTrigger = true;
        SetAlpha(0f);
    }

    void SetAlpha(float alpha)
    {
        Color c = tilemap.color;
        c.a = alpha;
        tilemap.color = c;
    }
}


public enum DoorDirection
{
    Up, Down, Left, Right
}
