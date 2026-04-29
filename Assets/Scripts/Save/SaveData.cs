using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
    public PlayerData player;
    public InventoryData inventory;
    public EquipSaveData equip;
    public List<QuestStateData> quests;
    public EquipmentData equipmentItem;
    public List<SkillSaveData> skills;

    [Header("리스폰 포인트")]
    public Vector3 respawnPosition;
    public string respawnScene;
    public SceneType savedSceneType;
}
