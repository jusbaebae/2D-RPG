using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TriggerDetector : MonoBehaviour
{
    public Animator anim;
    private NPCController npc;

    public TextMeshPro npcname;
    public GameObject npcnamebox;
    public GameObject[] icons;
    private void Awake()
    {
        npc = GetComponent<NPCController>();
        if(npcname != null && npcnamebox != null)
        {
            npcname.text = npc.Name;
            npcnamebox.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(anim != null)
            {
                anim.SetBool("PlayerInRange", true);
            }
            npc.currentState = NPCState.PlayerDetected;
            QuestState state = QuestManager.Instance.GetQuestStateForNPC(npc.id);
            UpdateNPCIcon(state);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (anim != null)
            {
                anim.SetBool("PlayerInRange", false);
            }
            npc.currentState = NPCState.Idle;
            for (int i = 0; i < icons.Length; i++)
            {
                icons[i].SetActive(false);
            }
        }
    }

    public void UpdateNPCIcon(QuestState state) //NPC 머리위 아이콘 업데이트
    {
        QuestData quest = QuestManager.Instance.GetPriorityQuestForNPC(npc.id);
        NPCIconType type = ConvertStateToIcon(quest, state);

        for (int i = 0; i < icons.Length; i++)
        {
            icons[i].SetActive(i == (int)type);
        }
    }

    private NPCIconType ConvertStateToIcon(QuestData quest, QuestState state) //아이콘 상태 전환
    {
        //퀘스트 자체가 없음
        if (quest == null)
        {
            return NPCIconType.Dialogue;
        }

        //아직 퀘스트 안 받은 상태
        if (state == null)
        {
            bool canAccept = QuestManager.Instance.IsQuestAvailable(quest.questId);
            return canAccept ? NPCIconType.QuestAvailable : NPCIconType.Dialogue;
        }

        //이미 받은 상태
        switch (state.status)
        {
            case QuestStatus.Complete:
                return NPCIconType.QuestComplete;

            case QuestStatus.InProgress:
                return NPCIconType.QuestInProgress;

            default:
                return NPCIconType.Dialogue;
        }
    }
}

public enum NPCIconType
{
    Dialogue,
    QuestAvailable,
    QuestInProgress,
    QuestComplete
}