using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Monster/MonsterData")]
public class MonsterData : ScriptableObject
{
    public GameObject prefab;

    [Header("등장 조건")]
    public int minDepth;
    public int maxDepth;

    [Header("등장 확률")]
    public int weight;

    [Header("방 타입")]
    public RoomType[] allowedRooms;
}
