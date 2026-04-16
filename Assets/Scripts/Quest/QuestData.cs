using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest")]
[System.Serializable]
public class QuestData : ScriptableObject
{    
    [Header("퀘스트 정보")]
    public string questId;
    public string questName;
    public QuestType questType;
    public string targetid;
    public int targetProgress;

    [TextArea(3, 10)]
    public string description;

    public int requiredLevel;

    [Header("보상")]
    public int rewardGold;
    public int rewardExp;

    [Header("퀘스트 전용 대사")]
    [TextArea]
    public string[] acceptDialogueLines; //수락전

    [TextArea]
    public string[] progressDialogueLines; //진행중

    [TextArea]
    public string[] completeDialogueLines; //완료후

    [Header("목표 NPC 전용 대사(TalkToNPC)")]
    [TextArea]
    public string[] targetNpcDialogueLines;
}

public enum QuestType
{
    KillMonster,
    CollectItem,
    TalkToNPC
}
