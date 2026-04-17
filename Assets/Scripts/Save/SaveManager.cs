using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private string path;

    private void Awake()
    {
        Instance = this;
        path = Application.persistentDataPath + "/save.json";
    }
    public void SaveGame()
    {
        SaveData data = new SaveData();

        data.player = PlayerMovement.Instance.GetSaveData();
        data.inventory = InventoryManager.Instance.GetSaveData();
        data.equip = InventoryManager.Instance.GetEquipSaveData();
        data.quests = QuestManager.Instance.GetSaveData();

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }
}
