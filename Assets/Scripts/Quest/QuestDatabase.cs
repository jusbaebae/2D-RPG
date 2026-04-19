using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestDatabase : MonoBehaviour
{
    public static QuestDatabase Instance;
    public List<QuestData> questList;
    private Dictionary<string, QuestData> questDict;

    private void Awake()
    {
        Instance = this;

        questDict = new Dictionary<string, QuestData>();
        foreach (var q in questList)
        {
            questDict[q.questId] = q;
        }
    }

    public QuestData GetQuestById(string id)
    {
        return questDict.TryGetValue(id, out var q) ? q : null; //id로 값찾기
    }

    public List<QuestData> GetQuestsByNPC(int npcId)
    {
        List<QuestData> result = new List<QuestData>();

        foreach (var quest in questList)
        {
            if (quest.npcId == npcId)
            {
                result.Add(quest);
            }
        }

        return result;
    }
}
