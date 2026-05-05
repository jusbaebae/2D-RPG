using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager Instance;

    public UIAnim uianim;
    public Room currentRoom;
    public GameObject player;
    public CinemachineConfiner2D confiner;
    public GameObject ReturnUI;

    public RectTransform dungeonNameUI;
    public TextMeshProUGUI dungeonNameText;
    public string dungeonName;

    private void Awake()
    {
        Instance = this;
        PlayDungeonIntro();
    }

    void Start()
    {
        player = GameManager.Instance.player;
        confiner = FindFirstObjectByType<CinemachineConfiner2D>();

        if (currentRoom != null)
        {
            StartRoom(currentRoom);
        }
    }
    private void SetupRoomEnvironment(Room room)
    {
        //카메라 경계
        if (confiner != null)
        {
            confiner.m_BoundingShape2D = room.confiner;
            confiner.InvalidateCache();
        }

        if (MinimapManager.Instance != null)
        {
            MinimapManager.Instance.VisitRoom(room);
            MinimapManager.Instance.UpdatePlayerPosition(room);
        }
    }

    public void EnterRoom(Room room, DoorDirection dir) //다음 방으로 이동처리
    {
        Vector3 nextPos = room.GetArrivalPoint(dir).position;
        
        // 플레이어 위치
        if (player != null)
        {
            player.transform.position = nextPos;
            room.playerEnterPos = nextPos;
        }

        SetupRoomEnvironment(room);
    }

    public void StartRoom(Room room) //시작방
    {
        // 플레이어 위치
        if (player != null)
        {
            player.transform.position = room.spawnPoint.position;
        }

        SetupRoomEnvironment(room);
    }

    public void Onreturn()
    {
        SceneTransition.Instance.StartTransition(GameManager.Instance.previousScene);
        StatsManager.Instance.UpdateHealth(9999);
    }

    public void Onclose()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Cancel);
        uianim.Hide(ReturnUI);
    }

    public void PlayDungeonIntro()
    {
        StartCoroutine(DungeonIntroRoutine());
    }

    IEnumerator DungeonIntroRoutine()
    {
        yield return new WaitForSeconds(1.5f); //화면전환 시간 고려
        if (dungeonNameUI == null) yield break;

        dungeonNameText.text = dungeonName;

        float offsetX = 2000f; // 화면 밖 느낌 (Canvas 스케일 기준으로 조절)

        Vector2 startPos = new Vector2(-offsetX, dungeonNameUI.anchoredPosition.y);
        Vector2 midPos = new Vector2(0f, dungeonNameUI.anchoredPosition.y);
        Vector2 endPos = new Vector2(offsetX, dungeonNameUI.anchoredPosition.y);

        dungeonNameUI.anchoredPosition = startPos;

        //왼쪽 → 중앙 (빠르게)
        yield return dungeonNameUI.DOAnchorPos(midPos, 0.5f).SetEase(Ease.OutBack).WaitForCompletion();
        Debug.Log(dungeonNameUI.anchoredPosition);
        //중앙에서 1초 정지
        yield return new WaitForSeconds(1f);

        //중앙 → 오른쪽 (빠르게 퇴장)
        yield return dungeonNameUI.DOAnchorPos(endPos, 0.5f).SetEase(Ease.InBack).WaitForCompletion();
        //Debug.Log(dungeonNameUI.anchoredPosition);
        //Debug.Log("애니메이션 실행완료!");
    }
}
