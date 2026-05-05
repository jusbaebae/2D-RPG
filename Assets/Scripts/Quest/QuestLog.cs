using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestLog : MonoBehaviour
{
    public TextMeshProUGUI questName;
    public GameObject completeTextObj; //완료 가능 텍스트

    private QuestState questState;

    public void SetQuest(QuestState state)
    {
        questState = state;
        questName.text = state.questData.questName;

        GetComponent<Button>().onClick.AddListener(OnClickLog);

        if (completeTextObj != null)
        {
            completeTextObj.SetActive(state.status == QuestStatus.Complete);
        }
    }

    private void OnClickLog()
    {
        QuestUIManager.Instance.ShowQuestDescription(questState);
    }
}
