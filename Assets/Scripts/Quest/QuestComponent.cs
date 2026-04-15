using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestComponent : MonoBehaviour
{
    public List<QuestData> questList = new List<QuestData>();

    public QuestData GetAvailableQuest(int playerLevel)
    {
        foreach (var quest in questList)
        {
            if (playerLevel >= quest.requiredLevel)
            {
                return quest;
            }
        }

        return null;
    }

    public void RemoveQuest(QuestData quest)
    {
        questList.Remove(quest);
    }
}
