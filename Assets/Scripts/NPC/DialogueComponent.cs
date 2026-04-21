using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueComponent : MonoBehaviour, IInteractable
{
    [TextArea]
    public string[] dialogueLines;

    private NPCController npcController;

    private void Awake()
    {
        npcController = GetComponent<NPCController>();
    }

    public void Interact()
    {
        StartDialogue();
    }

    public void StartDialogue()
    {
        QuestData bestQuest = QuestManager.Instance.GetPriorityQuestForNPC(npcController.id); //대표퀘스트
        QuestState talkQuest = QuestManager.Instance.GetTalkQuestForNPC(npcController.Name); //말걸기 퀘스트

        //Debug.Log("TalkQuest 있음?");
        //Debug.Log(talkQuest != null);

        //말걸기 퀘스트 전용 대사
        if (talkQuest != null)
        {
            DialogueManager.Instance.ShowDialogue(talkQuest.questData.targetNpcDialogueLines, npcController, false);
            return;
        }

        //기본 대사
        if (bestQuest == null)
        {
            DialogueManager.Instance.ShowDialogue(dialogueLines, npcController, false);
            return;
        }

        //퀘스트 전용 대사
        if (QuestManager.Instance.IsQuestCompleted(bestQuest.questId))
        {
            DialogueManager.Instance.RewardDialogue(bestQuest.completeDialogueLines, npcController, true, bestQuest);
        }
        else if (QuestManager.Instance.IsQuestAccepted(bestQuest.questId))
        {
            DialogueManager.Instance.ShowDialogue(bestQuest.progressDialogueLines, npcController, false);
        }
        else
        {
            DialogueManager.Instance.ShowDialogue(bestQuest.acceptDialogueLines,npcController,true, bestQuest);
        }

        Debug.Log(bestQuest);
    }
}
