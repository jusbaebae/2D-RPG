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

        Debug.Log("PlayerMovement: " + PlayerMovement.Instance);
        Debug.Log("InventoryManager: " + InventoryManager.Instance);
        Debug.Log("QuestManager: " + QuestManager.Instance);

        data.player = PlayerMovement.Instance.GetSaveData();
        data.inventory = InventoryManager.Instance.GetSaveItemData();
        data.equip = InventoryManager.Instance.GetEquipSaveData();
        data.quests = QuestManager.Instance.GetSaveData();
        data.equipmentItem = InventoryManager.Instance.GetSaveEquipItemData();
        data.skills = SkillTreeManager.Instance.GetSaveSkillData();

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        
        Debug.Log("파일이 저장되었습니다" + path + "위치입니다.");
    }

    public void LoadGame()
    {
        if (!System.IO.File.Exists(path))
            return;

        string json = System.IO.File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        QuestManager.Instance.GetLoadData(data.quests);
        InventoryManager.Instance.GetLoadItemData(data.inventory);
        InventoryManager.Instance.GetLoadEquipData(data.equip);
        PlayerMovement.Instance.GetLoadData(data.player);
        InventoryManager.Instance.GetLoadEquipItemData(data.equipmentItem);
        SkillTreeManager.Instance.GetLoadSkillData(data.skills);

        Debug.Log("현재파일: "+ data +"로드가 정상적으로 완료되었습니다"); ;
    }
}
