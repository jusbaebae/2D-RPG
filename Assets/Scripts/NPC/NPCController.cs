using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    private IInteractable interactable;
    public string Name;
    public NPCState currentState;
    private QuestComponent questComponent;

    public QuestComponent Quest => questComponent;
    private void Awake()
    {
        interactable = GetComponent<IInteractable>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Interact") && currentState == NPCState.PlayerDetected)
        {
            Interact();
        }
    }

    public void Interact()
    {
        currentState = NPCState.Interacting;
        interactable?.Interact();
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