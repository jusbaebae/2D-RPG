using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestState
{
    public QuestData questData;
    public QuestStatus status;
    public int currentProgress;
}

public enum QuestStatus
{
    None,  
    Available, 
    InProgress,
    Complete,
    Rewarded
}
