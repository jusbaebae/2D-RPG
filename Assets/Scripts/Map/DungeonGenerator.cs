using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject roomPrefab;

    public int roomCount; 

    private Dictionary<Vector2Int, Room> rooms = new Dictionary<Vector2Int, Room>();

    void Start()
    {
        GenerateDungeon(); //맵 랜덤 생성
        AssignRoomTypes(); //방 타입 정하기

        var allRooms = rooms.Values;
        MinimapManager.Instance.CalculateBounds(allRooms); //먼저 전체 크기 계산

        foreach (var room in allRooms)
        {
            MinimapManager.Instance.CreateNode(room);      //그 다음 배치
            if(room.type == RoomType.Start)
            {
                MinimapManager.Instance.VisitRoom(room);
                MinimapManager.Instance.UpdatePlayerPosition(room);
            }
        }
        MinimapManager.Instance.playerIcon.SetAsLastSibling(); //플레이어 현위치아이콘 맨아래로 설정
    }
    Vector2Int GetMin()
    {
        Vector2Int min = Vector2Int.zero;
        foreach (var pos in rooms.Keys)
        {
            min = Vector2Int.Min(min, pos);
        }
        return min;
    }

    Vector2Int GetMax()
    {
        Vector2Int max = Vector2Int.zero;
        foreach (var pos in rooms.Keys)
        {
            max = Vector2Int.Max(max, pos);
        }
        return max;
    }


    void GenerateDungeon()
    {
        Vector2Int startPos = Vector2Int.zero;
        CreateRoom(startPos);

        List<Vector2Int> directions = new List<Vector2Int>{ Vector2Int.up, Vector2Int.down,Vector2Int.left,Vector2Int.right};

        for (int i = 0; i < roomCount; i++)
        {
            Vector2Int randomRoom = GetRandomRoom();

            Vector2Int min = GetMin();
            Vector2Int max = GetMax();

            List<Vector2Int> validDirs = new List<Vector2Int>();
            foreach (var dir in directions)
            {
                Vector2Int next = randomRoom + dir;

                int width = Mathf.Max(max.x, next.x) - Mathf.Min(min.x, next.x) + 1;
                int height = Mathf.Max(max.y, next.y) - Mathf.Min(min.y, next.y) + 1;

                //5x5 제한
                if (width <= 5 && height <= 5)
                {
                    validDirs.Add(dir);
                }
            }

            //갈 수 있는 방향 없으면 스킵
            if (validDirs.Count == 0) continue;

            Vector2Int dirs = validDirs[Random.Range(0, validDirs.Count)];
            Vector2Int newPos = randomRoom + dirs;

            if (rooms.ContainsKey(newPos)) continue;

            CreateRoom(newPos);
            ConnectAdjacentRooms(newPos);
        }
    }

    Vector2Int GetRandomRoom()
    {
        List<Vector2Int> keys = new List<Vector2Int>(rooms.Keys);
        return keys[Random.Range(0, keys.Count)];
    }

    void CreateRoom(Vector2Int pos)
    {
        GameObject obj = Instantiate(roomPrefab, new Vector3(pos.x * 75, pos.y * 50, 0), Quaternion.identity);
        Room room = obj.GetComponent<Room>();

        room.gridPos = pos;

        rooms.Add(pos, room);
    }

    void ConnectAdjacentRooms(Vector2Int pos) //생성 방기준 상하좌우 탐색
    {
        Vector2Int[] directions = {Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right};

        foreach (var dir in directions)
        {
            Vector2Int neighborPos = pos + dir;

            if (rooms.ContainsKey(neighborPos)) //주변에 방이 존재하는지 확인
            {
                ConnectRooms(pos, neighborPos);
            }
        }
    }

    void ConnectRooms(Vector2Int a, Vector2Int b) //연결된 방 문 오브젝트 생성하기
    {
        Room roomA = rooms[a];
        Room roomB = rooms[b];

        Vector2Int dir = b - a;

        if (dir == Vector2Int.right)
        {
            roomA.SetConnection(DoorDirection.Right,roomB);
            roomB.SetConnection(DoorDirection.Left, roomA);
        }
        else if (dir == Vector2Int.left)
        {
            roomA.SetConnection(DoorDirection.Left , roomB);
            roomB.SetConnection(DoorDirection.Right , roomA);
        }
        else if (dir == Vector2Int.up)
        {
            roomA.SetConnection(DoorDirection.Up , roomB);
            roomB.SetConnection(DoorDirection.Down , roomA);
        }
        else if (dir == Vector2Int.down)
        {
            roomA.SetConnection(DoorDirection.Down , roomB);
            roomB.SetConnection(DoorDirection.Up, roomA);
        }

        //Debug.Log($"dir: {dir}");
        //Debug.Log($"Connect {a} ↔ {b}");
    }

    void AssignRoomTypes()
    {
        //시작방을 랜덤으로 잡고 그걸 기반으로 거리 탐색하기
        Vector2Int startPos = GetRandomRoom();

        //Debug.Log(startPos);

        //BFS로 거리계산
        Dictionary<Vector2Int, int> distance = new Dictionary<Vector2Int, int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        queue.Enqueue(startPos);
        distance[startPos] = 0;

        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right }; 

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            foreach (var dir in dirs)
            {
                Vector2Int next = current + dir;

                if (rooms.ContainsKey(next) && !distance.ContainsKey(next))
                {
                    distance[next] = distance[current] + 1;
                    rooms[next].depth = distance[next];
                    queue.Enqueue(next);
                }
            }
        }

        //가장 먼 방 찾기 > 보스방
        Vector2Int bossRoomPos = startPos;
        int maxDist = -1;

        foreach (var pair in distance)
        {
            if (pair.Value > maxDist)
            {
                maxDist = pair.Value;
                bossRoomPos = pair.Key;
                //Debug.Log(maxDist);
            }
        }

        rooms[bossRoomPos].type = RoomType.Boss;

        //막다른 방 찾기
        List<Vector2Int> deadEnds = new List<Vector2Int>();

        foreach (var roomPos in rooms.Keys)
        {
            int count = 0;

            foreach (var dir in dirs)
            {
                if (rooms.ContainsKey(roomPos + dir))
                    count++;
            }

            if (count == 1 && roomPos != startPos && roomPos != bossRoomPos)
            {
                deadEnds.Add(roomPos);
            }
        }

        //막다른 방은 보물방
        int treasureCount = Mathf.Min(1, deadEnds.Count);

        for (int i = 0; i < treasureCount; i++)
        {
            int rand = Random.Range(0, deadEnds.Count);
            Vector2Int pos = deadEnds[rand];

            rooms[pos].type = RoomType.Treasure;
            deadEnds.RemoveAt(rand);
        }

        //나머지는 일반방
        foreach (var room in rooms.Values)
        {
            if (room.type == 0)
            {
                room.type = RoomType.Normal;
            }
        }

        rooms[startPos].type = RoomType.Start; //시작점은 시작방
        DungeonManager.Instance.EnterRoom(rooms[startPos]); //시작방 입장처리
    }
}
