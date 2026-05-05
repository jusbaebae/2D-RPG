using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine;
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    public Vector3 respawnPosition;
    public string respawnScene;

    private string path;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); //이 객체를 유지
        }
        else
        {
            Destroy(gameObject); //이미 있으면 새로 생긴 건 삭제
            return;
        }
        path = Application.persistentDataPath + "/save.json";
    }
    public void SaveGame()
    {
        SaveData data = new SaveData();

        //Debug.Log("PlayerMovement: " + PlayerMovement.Instance);
        //Debug.Log("InventoryManager: " + InventoryManager.Instance);
        //Debug.Log("QuestManager: " + QuestManager.Instance);

        data.player = PlayerMovement.Instance.GetSaveData();
        data.inventory = InventoryManager.Instance.GetSaveItemData();
        data.equip = InventoryManager.Instance.GetEquipSaveData();
        data.quests = QuestManager.Instance.GetSaveData();
        data.equipmentItem = InventoryManager.Instance.GetSaveEquipItemData();
        data.skills = SkillTreeManager.Instance.GetSaveSkillData();
        data.savedSceneType = SceneController.Instance.CurrentSceneType;
        data.respawnPosition = respawnPosition;
        data.respawnScene = respawnScene;
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);

        //Debug.Log(respawnPosition);
        //Debug.Log(respawnScene);
        //Debug.Log("파일이 저장되었습니다" + path + "위치입니다.");
    }

    public void LoadGame()
    {
        if (!File.Exists(path))
            return;

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        StartCoroutine(LoadSceneAndRestoreData(data));

        //Debug.Log("현재파일: "+ data +"로드가 정상적으로 완료되었습니다");
    }
    public bool HasSaveData()
    {
        if (!File.Exists(path)) return false;

        return true;
    }

    public void OnEnterTown()
    {
        StartCoroutine(SetRespawn());
    }

    IEnumerator SetRespawn()
    {
        yield return new WaitUntil(() => PlayerMovement.Instance != null);

        respawnPosition = FindFirstObjectByType<SpawnManager>().transform.position;
        respawnScene = SceneManager.GetActiveScene().name;
    }

    private IEnumerator LoadSceneAndRestoreData(SaveData data)
    {
        //씬 로드가 완료될 때까지 기다림
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(data.respawnScene);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        QuestManager.Instance.GetLoadData(data.quests);
        InventoryManager.Instance.GetLoadItemData(data.inventory);
        InventoryManager.Instance.GetLoadEquipData(data.equip);
        InventoryManager.Instance.GetLoadEquipItemData(data.equipmentItem);
        SkillTreeManager.Instance.GetLoadSkillData(data.skills);
        SkillManager.Instance.GetLoadSkillUIData(data.skills);

        //플레이어 위치 설정 (마을 여부에 따른 분기 처리 가능)
        if (PlayerMovement.Instance != null)
        {
            PlayerMovement.Instance.GetLoadData(data.player);
            if (data.savedSceneType == SceneType.Town)
            {
                 PlayerMovement.Instance.transform.position = data.respawnPosition;
            }
        }

        Debug.Log("데이터 로드 및 씬 복구가 완료되었습니다.");
    }
}
