using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public Dictionary<string, QuestState> questStates = new Dictionary<string, QuestState>(); //퀘스트 상태를 딕셔너리로 관리

    public Action<int> OnQuestAccepted;
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
    }

    public QuestData GetPriorityQuestForNPC(int npcId)
    {
        QuestData data = null;

        foreach (var quest in QuestDatabase.Instance.questList)
        {
            if (quest.npcId != npcId)
                continue;

            //완료 가능
            if (IsQuestCompleted(quest.questId))
                return quest;

            //진행 중
            if (IsQuestAccepted(quest.questId))
                data = quest;

            //수락 가능
            else if (IsQuestAvailable(quest.questId) && data == null)
                data = quest;
        }

        return data;
    }

    public bool IsQuestAvailable(string questid)
    {
        if (questStates.ContainsKey(questid))
            return false;

        QuestData quest = QuestDatabase.Instance.GetQuestById(questid);

        if (quest == null)
            return false;

        //레벨 조건 체크 
        if (ExperienceManager.Instance.level < quest.requiredLevel)
            return false;

        return true;
    }

    public void AcceptQuest(QuestData quest) //퀘스트 수락하기
    {
        if (questStates.ContainsKey(quest.questId))
            return;

        QuestState newQuest = new QuestState
        {
            questData = quest,
            status = QuestStatus.InProgress,
            currentProgress = 0,
        };

        questStates.Add(quest.questId, newQuest);
        QuestUIManager.Instance.AddQuestLog(newQuest);
        OnQuestAccepted?.Invoke(newQuest.questData.npcId);
        Debug.Log($"{quest.questName} 퀘스트 수락");
    }

    public void AddProgress(QuestType type, string targetId, int amount) //진행상황
    {
        foreach (var quest in questStates.Values)
        {
            if (quest.status == QuestStatus.Complete)
                continue;

            if (quest.questData.questType != type)
                continue;

            if (quest.questData.targetid != targetId)
                continue;

            quest.currentProgress += amount;
            QuestUIManager.Instance.UpdateQuest();
            Debug.Log("퀘스트 타입 : " + type);
            Debug.Log($"{targetId} 1마리");
            if (quest.currentProgress >= quest.questData.targetProgress)
            {
                CompleteQuest(quest.questData.questId);
            }
        }
    }

    public void CompleteQuest(string questId) //퀘스트 완료
    {
        if (!questStates.ContainsKey(questId))
            return;

        QuestState quest = questStates[questId];

        quest.status = QuestStatus.Complete;

        Debug.Log($"{quest.questData.questName} 완료");
    }

    public void RewardQuest(string questId)
    {
        if (!questStates.ContainsKey(questId))
            return;

        questStates[questId].status = QuestStatus.Rewarded;
    }

    public QuestState GetTalkQuestForNPC(string npcname)
    {
        foreach (QuestState questState in questStates.Values)
        {
            if (questState.status != QuestStatus.InProgress)
                continue;

            if(questState.questData.questType != QuestType.TalkToNPC)
                continue;

            if (questState.questData.targetid == npcname)
            {
                Debug.Log("말걸기퀘스트확인");
                return questState;
            }
        }

        return null;
    }

    public bool IsQuestAccepted(string questId)
    {
        return questStates.ContainsKey(questId) && questStates[questId].status == QuestStatus.InProgress;
    }

    public bool IsQuestCompleted(string questId)
    {
        return questStates.ContainsKey(questId) && questStates[questId].status == QuestStatus.Complete;
    }

    public List<QuestStateData> GetSaveData() //딕셔너리는 Jsonutility로 저장이 안되므로 List로 변환
    {
        List<QuestStateData> list = new List<QuestStateData>();

        foreach (var pair in questStates)
        {
            var q = pair.Value;

            list.Add(new QuestStateData
            {
                questId = pair.Key,
                status = q.status,
                currentProgress = q.currentProgress
                
        });
        }
        return list;
    }

    public void GetLoadData(List<QuestStateData> list)
    {
        questStates.Clear(); // 기존 데이터 초기화

        foreach (var data in list)
        {
            QuestState state = new QuestState
            {
                status = data.status,
                currentProgress = data.currentProgress
            };

            state.questData = QuestDatabase.Instance.GetQuestById(data.questId);
            questStates[data.questId] = state;

            Debug.Log("로드 questId: " + data.questId);
        }

        QuestUIManager.Instance.RefreshUI();
        QuestUIManager.Instance.UpdateQuest();
    }

    public QuestState GetQuestStateForNPC(int id) 
    {
        foreach(var quest in questStates)
        {
            if (quest.Value.questData.npcId == id)
            {
                return quest.Value;
            }
        }

        return null;
    }
}
