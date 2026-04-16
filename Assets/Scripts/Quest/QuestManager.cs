using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    private Dictionary<string, QuestState> questStates = new Dictionary<string, QuestState>(); //퀘스트 상태를 딕셔너리로 관리

    private void Awake()
    {
        Instance = this;
    }

    public void AcceptQuest(QuestData quest) //퀘스트 수락하기
    {
        if (questStates.ContainsKey(quest.questId))
            return;

        QuestState newQuest = new QuestState
        {
            questData = quest,
            isAccepted = true,
            isCompleted = false,
            currentProgress = 0,
            targetProgress = quest.targetProgress
        };

        questStates.Add(quest.questId, newQuest);
        QuestUIManager.Instance.AddQuestLog(newQuest);
        Debug.Log($"{quest.questName} 퀘스트 수락");
    }

    public void AddProgress(QuestType type, string targetId, int amount) //진행상황
    {
        foreach (var quest in questStates.Values)
        {
            if (quest.isCompleted)
                continue;

            if (quest.questData.questType != type)
                continue;

            if (quest.questData.targetid != targetId)
                continue;

            quest.currentProgress += amount;
            QuestUIManager.Instance.UpdateQuest();
            Debug.Log("퀘스트 타입 : " + type);
            Debug.Log($"{targetId} 1마리");
            if (quest.currentProgress >= quest.targetProgress)
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

        quest.isCompleted = true;

        Debug.Log($"{quest.questData.questName} 완료");
    }

    public void RemoveQuest(QuestComponent questComponent, QuestData questData) //퀘스트 삭제
    {
        if (questComponent != null)
        {
            questComponent.RemoveQuest(questData);
            questStates.Remove(questData.questId);
            QuestUIManager.Instance.RemoveQuestLog(questData.questId);
            Debug.Log("퀘스트 삭제 완료");
        }
    }

    public QuestState GetTalkQuestForNPC(string npcId)
    {
        foreach (QuestState questState in questStates.Values)
        {
            if (!questState.isAccepted)
                continue;

            if (questState.isCompleted)
                continue;

            if (questState.questData.questType != QuestType.TalkToNPC)
                continue;

            if (questState.questData.targetid == npcId)
            {
                Debug.Log("말걸기퀘스트확인");
                return questState;
            }
        }

        return null;
    }



    public bool IsQuestAccepted(string questId)
    {
        return questStates.ContainsKey(questId) && questStates[questId].isAccepted;
    }

    public bool IsQuestCompleted(string questId)
    {
        return questStates.ContainsKey(questId) && questStates[questId].isCompleted;
    }
}
