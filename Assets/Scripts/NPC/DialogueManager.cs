using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    private NPCController currentNPC;
    private QuestData currentQuestData;
    public PlayerMovement pmove;

    public static event Action<int> OnRewardexp;
    public static event Action<int> OnRewardGold;

    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject rewardbar;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private GameObject nextIcon;
    [SerializeField] private GameObject acceptButton;
    [SerializeField] private GameObject denyButton;
    [SerializeField] private GameObject rewardButton;

    private string[] currentLines;
    private int currentIndex;

    private bool canNext = false;
    public bool isquest; //퀘스트 마지막라인은 자동 스킵 불가능하게

    //타이핑 텍스트 효과
    private bool isTyping;
    private Coroutine typingCoroutine;
    private string currentFullLine;

    private void Awake()
    {
        Instance = this;
        dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (!dialoguePanel.activeSelf || currentLines == null) return;

        bool isLastQuestLine = currentIndex == currentLines.Length - 1 && isquest; //퀘스트 마지막 라인

        if (!isLastQuestLine && canNext && Input.GetButtonDown("Interact"))
        {
            if (isTyping)
            {
                CompleteCurrentLine();
            }
            else
            {
                NextDialogue();
            }
        }

        if (dialoguePanel.activeSelf && Input.GetButtonDown("Cancel"))
        {
            CloseDialogue();
        }
    }

    public void ShowDialogue(string[] lines, NPCController npc, bool isq, QuestData questData = null)
    {
        currentLines = lines;
        currentIndex = 0;
        currentNPC = npc;
        currentQuestData = questData;

        isquest = isq;
        pmove.isinteract = true;

        dialoguePanel.SetActive(true);
        nextIcon.SetActive(true);
        StartTyping(currentLines[currentIndex]);
        nameText.text = npc.Name;

        UiManager.Instance.isInteract = true;

        canNext = false;
        StartCoroutine(EnableNextInput()); //대사 스킵 버그 방지
    }

    public void RewardDialogue(string[] lines, NPCController npc, bool isq, QuestData questData = null)
    {
        currentLines = lines;
        currentIndex = 0;
        currentNPC = npc;
        currentQuestData = questData;

        isquest = isq;
        pmove.isinteract = true;

        dialoguePanel.SetActive(true);
        rewardbar.SetActive(true);
        rewardButton.SetActive(true);
        goldText.text = questData.rewardGold.ToString();
        expText.text = questData.rewardExp.ToString();
        StartTyping(currentLines[currentIndex]);
        nameText.text = npc.Name;

        UiManager.Instance.isInteract = true;

        canNext = false;
        StartCoroutine(EnableNextInput());
    }

    public void NextDialogue()
    {
        currentIndex++;
        if(currentIndex == currentLines.Length - 1 && isquest)
        {
            nextIcon.SetActive(false);
            acceptButton.SetActive(true);
            denyButton.SetActive(true);
        }
        else if (currentIndex >= currentLines.Length && !isquest)
        {
            CloseDialogue();
            return;
        }

        StartTyping(currentLines[currentIndex]);
    }

    public void CloseDialogue()
    {
        currentNPC?.OnDialogueClosed();
        currentNPC = null;

        UiManager.Instance.isInteract = false;

        pmove.isinteract = false;
        dialoguePanel.SetActive(false);
        nextIcon.SetActive(false);
        acceptButton.SetActive(false);
        denyButton.SetActive(false);
        rewardbar.SetActive(false);
        rewardButton.SetActive(false);
    }

    private IEnumerator EnableNextInput()
    {
        yield return null; //다음 프레임까지 대기
        canNext = true;
    }

    private void StartTyping(string line)
    {
        currentFullLine = line;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(line));
    }

    private IEnumerator TypeText(string line) //타이핑 텍스트
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.04f);
        }

        isTyping = false;
    }

    private void CompleteCurrentLine() //타이핑중 텍스트 전체 출력
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueText.text = currentFullLine;
        isTyping = false;
    }

    private void HandleDialogueInput()
    {
        if (currentIndex != currentLines.Length - 1 || !isquest)
        {
            if (isTyping)
            {
                CompleteCurrentLine();
            }
            else
            {
                NextDialogue();
            }
        }
        
    }

    public void OnDialoguePanelClick() //클릭으로 대화 넘기기
    {
        HandleDialogueInput();
    }

    public void OnAcceptQuest()
    {
        if (currentNPC == null) return;

        QuestManager.Instance.AcceptQuest(currentQuestData);

        CloseDialogue();
    }

    public void OnReward()
    {
        OnRewardexp(currentQuestData.rewardExp);
        OnRewardGold(currentQuestData.rewardGold);
        QuestComponent quest = currentNPC.GetComponent<QuestComponent>();
        if(quest != null)
        {
            quest.RemoveQuest(currentQuestData);
            Debug.Log(currentQuestData);
            Debug.Log("퀘스트 삭제 완료");
        }
        currentQuestData = null;
        CloseDialogue();
    }
}
