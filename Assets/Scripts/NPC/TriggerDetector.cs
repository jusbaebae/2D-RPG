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
            //Debug.Log(state.status);
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
        if (npc == null) npc = GetComponent<NPCController>();

        QuestData quest = QuestManager.Instance.GetPriorityQuestForNPC(npc.id);

        if (quest == null)
        {
            Debug.LogWarning($"{npc.Name}의 ID({npc.id})로 조회된 퀘스트가 없습니다.");
        }

        NPCIconType type = ConvertStateToIcon(quest, state);

        for (int i = 0; i < icons.Length; i++)
        {
            icons[i].SetActive(i == (int)type);
        }
    }

    private NPCIconType ConvertStateToIcon(QuestData quest, QuestState state) //아이콘 상태 전환
    {
        //진행 중이거나 완료된 퀘스트 상태가 있다면 해당 아이콘 우선
        if (state != null)
        {
            if (state.status == QuestStatus.Complete) return NPCIconType.QuestComplete;
            if (state.status == QuestStatus.InProgress) return NPCIconType.QuestInProgress;
        }

        //퀘스트 데이터가 있고, 아직 받지 않은 상태라면 수락 가능 여부 체크
        if (quest != null)
        {
            // 선행 퀘스트나 레벨 조건이 맞는지 확인
            bool canAccept = QuestManager.Instance.IsQuestAvailable(quest.questId);
            return canAccept ? NPCIconType.QuestAvailable : NPCIconType.Dialogue;
        }

        //아무것도 해당 안 되면 기본 대화
        return NPCIconType.Dialogue;
    }
}

public enum NPCIconType
{
    Dialogue,
    QuestAvailable,
    QuestInProgress,
    QuestComplete
}