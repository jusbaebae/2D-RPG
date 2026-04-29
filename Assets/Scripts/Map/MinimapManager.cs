using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MinimapManager : MonoBehaviour
{
    public static MinimapManager Instance;

    public RectTransform playerIcon;
    public GameObject BossIcon;
    public GameObject TreasureIcon;
    public GameObject nodePrefab;
    public Transform container;
    public RectTransform containerRect;
    public TextMeshProUGUI mapNameText;

    private Vector2Int min;
    private Vector2Int max;
    private bool boundsReady = false;
    private float spacing;
    private Vector2 offset;

    private Dictionary<Vector2Int, MinimapNode> nodes = new();

    private void Awake()
    {
        Instance = this;
    }

    public void CreateNode(Room room)
    {
        if (!boundsReady) return; //먼저 맵 전체 범위를 구한후 노드 생성

        GameObject obj = Instantiate(nodePrefab, container);
        MinimapNode node = obj.GetComponent<MinimapNode>();

        Vector2 pos = ((Vector2)room.gridPos - min) * spacing - offset;

        obj.GetComponent<RectTransform>().anchoredPosition = pos;

        if (room.isCleared) node.SetVisited();
        else node.SetUnvisited();

        nodes[room.gridPos] = node;
    }

    public void VisitRoom(Room room)
    {
        if (nodes.TryGetValue(room.gridPos, out var node))
        {
            GameObject icon = null;

            if (room.type == RoomType.Boss)
            {
                icon = Instantiate(BossIcon, node.transform);
                icon.SetActive(true);
            }
            else if (room.type == RoomType.Treasure)
            {
                icon = Instantiate(TreasureIcon, node.transform);
                icon.SetActive(true);
            }

            if (icon != null)
            {
                RectTransform rect = icon.GetComponent<RectTransform>();
                rect.anchoredPosition = Vector2.zero;
                rect.localScale = Vector3.one;
            }

            node.SetVisited();
        }
    }

    //맵의 전체 범위 구하기
    public void CalculateBounds(Dictionary<Vector2Int,Room>.ValueCollection rooms)
    {
        bool first = true;

        foreach (var r in rooms)
        {
            if (first)
            {
                min = r.gridPos;
                max = r.gridPos;
                first = false;
                continue;
            }

            min = Vector2Int.Min(min, r.gridPos);
            max = Vector2Int.Max(max, r.gridPos);
        }

        int width = max.x - min.x + 1;
        int height = max.y - min.y + 1;

        Rect rect = containerRect.rect;

        float spacingX = rect.width / width;
        float spacingY = rect.height / height;

        spacing = Mathf.Min(spacingX, spacingY);

        offset = new Vector2((width - 1) * spacing / 2f, (height - 1) * spacing / 2f);

        boundsReady = true;
    }

    public void UpdatePlayerPosition(Room room) //플레이어 아이콘 현재 방에 표시
    {
        if (!boundsReady) return;

        Vector2 pos = ((Vector2)room.gridPos - min) * spacing - offset;

        playerIcon.anchoredPosition = pos;
    }
}
