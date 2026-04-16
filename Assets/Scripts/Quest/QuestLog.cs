using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestLog : MonoBehaviour
{
    public TextMeshProUGUI questName;
    private QuestState questState;

    public void SetQuest(QuestState state)
    {
        questState = state;
        questName.text = state.questData.questName;

        GetComponent<Button>().onClick.AddListener(OnClickLog);
    }

    private void OnClickLog()
    {
        QuestUIManager.Instance.ShowQuestDescription(questState);
    }

    
}
