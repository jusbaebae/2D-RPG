using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Vector2Int gridPos;
    public Vector3 playerEnterPos;
    public RoomType type;

    public List<DoorSlot> doorSlots;
    public Transform spawnPoint;
    public List<GameObject> BossSpawnPoint;
    public PolygonCollider2D confiner;

    public int depth;
    public bool isCleared = false;
    private bool playerInside = false;

    [SerializeField] private List<MonsterData> Monsters;
    public List<Enemy_Health> spawnedMonsters = new List<Enemy_Health>();
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private GameObject TreasureBox;
    [SerializeField] private LayerMask obstacleLayer;
    private int aliveMonsterCount = 0;
    

    Vector3 debugPos; //디버그용

    void Awake()
    {
        foreach (var slot in doorSlots)
        {
            slot.wall.SetActive(true);
            slot.door.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerInside) return;

        if (collision.CompareTag("Player"))
        {
            //Debug.Log("플레이어 인식완료!");
            playerInside = true;

            if (!isCleared)
            {
                CloseDoors();
                if (type == RoomType.Start)
                {
                    ClearRoom();
                }
                else if (type == RoomType.Normal) 
                {
                    SpawnMonsters();
                } 
                else if (type == RoomType.Boss)
                {
                    SpawnMonsters();
                    SpawnBoss(); //보스잡으면 나머지 몬스터 다죽게 설정해보기
                    aliveMonsterCount++; //보스 1마리추가
                }
                else if (type == RoomType.Treasure)
                {
                    ClearRoom();
                    TreasureBox.SetActive(true);
                }
            }
        }
    }

    public void SpawnMonsters()
    {
        //깊이별로 몬스터 구분
        List<MonsterData> validMonsters = Monsters.FindAll(m => depth >= m.minDepth && depth <= m.maxDepth && m.allowedRooms.Contains(this.type));

        if (validMonsters.Count == 0)
        {
            Debug.LogWarning("이 방에 소환 가능한 몬스터가 없습니다!");
            ClearRoom();
            return;
        }

        //깊이별로 몬스터가 구분되면 몬스터의 가중치를 더해서 토탈 가중치구하기
        int totalWeight = 0;
        foreach (var m in validMonsters) totalWeight += m.weight;

        int spawnCount = Random.Range(4, 8) + depth;
        List<Vector3> positions = new List<Vector3>();
        aliveMonsterCount = 0;
        
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 center = transform.position;
            float radius = 10f;
            Vector3 pos = GetValidPositionInCircle(center, radius, positions);
            debugPos = pos;

            if (float.IsInfinity(pos.x) || float.IsInfinity(pos.y)) continue; //불가능한 자리면 넘기기

            //가중치를 기반으로 몬스터 뽑기
            MonsterData selectedMonster = GetRandomMonster(validMonsters, totalWeight);
            GameObject monster = Instantiate(selectedMonster.prefab, pos, Quaternion.identity);
            monster.GetComponent<Enemy_Health>().Init(this);

            //생성된 몬스터들의 Enemy_Health를 리스트에 저장
            Enemy_Health enemyHealth = monster.GetComponent<Enemy_Health>();
            enemyHealth.Init(this);
            spawnedMonsters.Add(enemyHealth);

            positions.Add(pos);
            aliveMonsterCount++;
        }

       //Debug.Log("몬스터 생성");
    }

    //가중치 랜덤 선택
    private MonsterData GetRandomMonster(List<MonsterData> monsters, int totalWeight)
    {
        int pivot = Random.Range(0, totalWeight);
        int currentWeight = 0;

        foreach (var m in monsters)
        {
            currentWeight += m.weight;
            if (pivot < currentWeight)
                return m;
        }
        return monsters[0];
    }

    public void SpawnBoss()
    {
        if (BossSpawnPoint.Count == 0) return;

        //랜덤 위치 선택
        int index = Random.Range(0, BossSpawnPoint.Count);
        GameObject spawnPoint = BossSpawnPoint[index];

        GameObject boss = Instantiate(bossPrefab, spawnPoint.transform.position, Quaternion.identity);

        Boss_Health bossScript = boss.GetComponent<Boss_Health>();
        bossScript.Init(this);
        bossScript.Ondeath += () => SpawnPortal(boss.transform.position);
        bossScript.Ondeath += () => KillAllMonsters();
        bossScript.Ondeath += () => ClearRoom();
        //Debug.Log("보스 생성");
    }

    //보스가 죽으면 잡몹들 한번에 처리
    public void KillAllMonsters()
    {
        foreach (var monster in new List<Enemy_Health>(spawnedMonsters))
        {
            if (monster != null)
            {
                monster.InvokeDeath();
            }
        }
        spawnedMonsters.Clear();
    }

    void SpawnPortal(Vector3 pos)
    {
        Instantiate(portalPrefab, pos, Quaternion.identity);
    }

    Vector3 GetValidPositionInCircle(Vector3 center, float radius, List<Vector3> existingPositions)
    {
        for (int i = 0; i < 20; i++)
        {
            //원 안에서 랜덤 좌표 구하기
            Vector2 rand = Random.insideUnitCircle * radius;
            Vector3 pos = center + new Vector3(rand.x, rand.y, 0);

            //장애물 체크
            if (Physics2D.OverlapCircle(pos, 1f, obstacleLayer)) continue;

            //플레이어 주변에 생성 안되게
            if (Vector2.Distance(pos, playerEnterPos) < 5f) continue;

            
            bool tooClose = false;
            //몬스터 간 거리 체크
            foreach (var other in existingPositions)
            {
                if (Vector3.Distance(pos, other) < 3f)
                {
                    tooClose = true;
                    break;
                }
            }

            if (tooClose)
                continue;

            return pos;
        }

        return Vector3.negativeInfinity;
    }

    public void ClearRoom() //방안에 몬스터를 다 잡았는지 확인
    {
        //Debug.Log("클리어 처리 완료!");
        isCleared = true;
        OpenDoors(); //클리어하면 방안에 모든 문 열기
    }

    public void CloseDoors()
    {
        foreach (var slot in doorSlots)
        {
            if (slot.door.gameObject.activeSelf)
            {
                slot.door.Close();
            }
        }
    }

    public void OpenDoors()
    {
        foreach (var slot in doorSlots)
        {
            if (slot.door.gameObject.activeSelf)
            {
                slot.door.Open();
            }
        }
    }

    public void SetConnection(DoorDirection dir, Room room)
    {
        foreach (var slot in doorSlots)
        {
            if (slot.direction == dir)
            {
                slot.wall.SetActive(false);       // 벽 제거
                slot.door.gameObject.SetActive(true); // 문 생성
                CloseDoors();
                slot.door.connectedRoom = room; //문이랑 연결된 방 저장
                slot.door.doorDirection = dir; //문의 방향 저장
            }
        }
    }

    public Transform GetArrivalPoint(DoorDirection enteringFrom)
    {
        //만약 오른쪽 방에서 왔다면 현재 방의 왼쪽 문 스폰포인트가 필요
        DoorDirection targetDir = GetOppositeDirection(enteringFrom);
        var slot = doorSlots.Find(s => s.direction == targetDir);

        return slot != null ? slot.arrivalPoint : spawnPoint;
    }

    private DoorDirection GetOppositeDirection(DoorDirection dir)
    {
        switch (dir)
        {
            case DoorDirection.Up: return DoorDirection.Down;
            case DoorDirection.Down: return DoorDirection.Up;
            case DoorDirection.Left: return DoorDirection.Right;
            case DoorDirection.Right: return DoorDirection.Left;
            default: return DoorDirection.Up;
        }
    }

    public void OnMonsterDead()
    {
        aliveMonsterCount--;

        if (aliveMonsterCount <= 0)
        {
            ClearRoom();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(debugPos, 10f);
    }
}

public enum RoomType
{
    Start,
    Normal,
    Treasure,
    Boss
}
