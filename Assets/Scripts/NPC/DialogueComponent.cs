using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueComponent : MonoBehaviour , IInteractable
{
    [TextArea]
    public string[] dialogueLines;

    private NPCController npcController;
    private QuestComponent quest;
    private ExperienceManager playerLevel;

    private void Awake()
    {
        npcController = GetComponent<NPCController>();
        quest = GetComponent<QuestComponent>();
        playerLevel = FindAnyObjectByType<ExperienceManager>();
    }

    public void Interact()
    {
        StartDialogue();
    }

    public void StartDialogue()
    {
        QuestData availableQuest = quest.GetAvailableQuest(playerLevel.level);

        if (availableQuest == null)
        {
            DialogueManager.Instance.ShowDialogue(dialogueLines, npcController, false);
            return;
        }

        if (QuestManager.Instance.IsQuestCompleted(availableQuest.questId))
        {
            DialogueManager.Instance.RewardDialogue(availableQuest.completeDialogueLines, npcController, true, availableQuest);
        }
        else if (QuestManager.Instance.IsQuestAccepted(availableQuest.questId))
        {
            DialogueManager.Instance.ShowDialogue(availableQuest.progressDialogueLines, npcController, false);
        }
        else
        {
            DialogueManager.Instance.ShowDialogue(availableQuest.acceptDialogueLines, npcController, true, availableQuest);
        }
    }
}
