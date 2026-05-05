using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class QuestUIManager : MonoBehaviour
{
    public static QuestUIManager Instance;

    [Header("로그 생성 위치")]
    public Transform logParent;

    [Header("로그 프리팹")]
    public GameObject questLogPrefab;

    [Header("상세 패널")]
    public GameObject descPanel;
    public TextMeshProUGUI questNameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI expText;
    public TextMeshProUGUI goldText;

    public QuestState currentSelectedQuest;

    private Dictionary<string, GameObject> questLogs = new Dictionary<string, GameObject>(); //퀘스트 UI용 딕셔너리
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
        descPanel.SetActive(false);
    }

    public void AddQuestLog(QuestState questState)
    {
        GameObject log = Instantiate(questLogPrefab, logParent);
        questLogs.Add(questState.questData.questId, log);

        QuestLog questlog = log.GetComponent<QuestLog>();
        questlog.SetQuest(questState);
    }

    public void ShowQuestDescription(QuestState questState)
    {
        currentSelectedQuest = questState;
        descPanel.SetActive(true);

        questNameText.text = questState.questData.questName;
        descriptionText.text = questState.questData.description;
        UpdateQuest();
        expText.text = questState.questData.rewardExp.ToString();
        goldText.text = questState.questData.rewardGold.ToString();
    }

    public void UpdateQuest()
    {
        if (currentSelectedQuest == null)
            return;

        if (currentSelectedQuest.questData.questType == QuestType.KillMonster)
        {
            progressText.text = $"목표 마리수 : {currentSelectedQuest.currentProgress} / {currentSelectedQuest.questData.targetProgress}";
        }
        else if (currentSelectedQuest.questData.questType == QuestType.TalkToNPC)
        {
            progressText.text = $"{currentSelectedQuest.questData.targetid}와 대화하기";
        }
        else if (currentSelectedQuest.questData.questType == QuestType.CollectItem)
        {
            progressText.text = $"{currentSelectedQuest.questData.targetid} : {currentSelectedQuest.currentProgress} / {currentSelectedQuest.questData.targetProgress}";
        }
    }

    public void RemoveQuestLog(string questId)
    {
        if (questLogs.ContainsKey(questId))
        {
            Destroy(questLogs[questId]);
            questLogs.Remove(questId);
        }

        descPanel.SetActive(false);
    }

    public void RefreshUI()
    {
        //기존 UI 제거
        foreach (Transform child in logParent)
        {
            Destroy(child.gameObject);
        }

        questLogs.Clear();

        //다시 생성

        foreach (var pair in QuestManager.Instance.questStates)
        {
            var state = pair.Value;

            if (state.status == QuestStatus.InProgress || state.status == QuestStatus.Complete) //퀘스트가 진행중일때 표시
            {
                AddQuestLog(state);
            }
        }
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(logParent.GetComponent<RectTransform>());
    }
}
