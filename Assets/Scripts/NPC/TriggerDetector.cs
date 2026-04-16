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
    public GameObject dialogueIcon;
    public GameObject questIcon;
    private void Awake()
    {
        npc = GetComponent<NPCController>();
        npcname.text = npc.Name;
        npcnamebox.SetActive(true);
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
                questIcon.SetActive(false);
                dialogueIcon.SetActive(false);
            }
            npc.currentState = NPCState.Idle;
        }
    }
}
