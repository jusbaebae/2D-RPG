using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    private TriggerDetector trigger;
    private IInteractable interactable;
    public int id;
    public string Name;
    public NPCState currentState;

    private void Awake()
    {
        trigger = GetComponent<TriggerDetector>();
        interactable = GetComponent<IInteractable>();
    }
    private void Start()
    {
        QuestManager.Instance.OnQuestAccepted += HandleQuestAccepted;
    }
    private void OnDisable()
    {
        QuestManager.Instance.OnQuestAccepted -= HandleQuestAccepted;
    }
    private void Update()
    {
        if (Input.GetButtonDown("Interact") && currentState == NPCState.PlayerDetected)
        {
            Interact();
        }
    }
    private void HandleQuestAccepted(int npcId)
    {
        if (id != npcId) return;

        QuestState state = QuestManager.Instance.GetQuestStateForNPC(id);
        trigger.UpdateNPCIcon(state);
    }

    public void Interact()
    {
        currentState = NPCState.Interacting;
        interactable?.Interact();
        QuestManager.Instance.AddProgress(QuestType.TalkToNPC, Name, 1);
    }

    public void OnDialogueClosed() //상태 복구 함수
    {
        currentState = NPCState.PlayerDetected;
    }
}


public enum NPCState
{
    Idle,
    PlayerDetected,
    Interacting
}