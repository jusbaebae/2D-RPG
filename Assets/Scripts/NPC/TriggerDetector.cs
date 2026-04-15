using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDetector : MonoBehaviour
{
    public Animator anim;
    private NPCController npc;

    public GameObject dialogueIcon;

    private void Awake()
    {
        npc = GetComponent<NPCController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(anim != null)
            {
                anim.SetBool("PlayerInRange", true);
                dialogueIcon.SetActive(true);
            }
            npc.currentState = NPCState.PlayerDetected;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (anim != null)
            {
                anim.SetBool("PlayerInRange", false);
                dialogueIcon.SetActive(false);
            }
            npc.currentState = NPCState.Idle;
        }
    }
}
