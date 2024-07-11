using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestCategory
{
    Main,
    Sub
}

public enum QuestState
{
    NotAllow,
    Standby,
    Ongoing,
    Done
}

public enum QuestType
{
    DefeatMonster,
    GetItem,
    Conversation,
    Arrive
}

[CreateAssetMenu(fileName ="New Quest Object", menuName = "Quest System/Quests/Quest")]
public class QuestObject : ScriptableObject
{
    public int id;
    public QuestState state;
    public QuestCategory questCategory = QuestCategory.Main;
    public QuestType questType;
    public int npcId;
    public int dialogueId;
    public int order;
    public string title;
    public int targetId;
    public bool isCompleted = false;
    public int conditionQuestId = 0;
    public int requireAmount;
    public int currentAmount = 0;
    [TextArea(15, 20)] public string desctiption;
    public int[] rewards;
    public int[] rewardAmount;
    public bool isAutoClear = true;
    public GameObject questTriggerWall;
}

[System.Serializable]
public class Quest{
    public int id;
    public QuestState state;
    public QuestCategory questCategory = QuestCategory.Main;
    public QuestType questType;
    public int npcId;
    public int dialogueId;
    public int order;
    public string title;
    public int targetId;
    public bool isCompleted = false;
    public int conditionQuestId = 0;
    public int requireAmount;
    public int currentAmount = 0;
    [TextArea(15, 20)] public string desctiption;
    public int[] rewards;
    public int[] rewardAmount;
    public bool isAutoClear = true;

    public Quest(QuestObject quest){
        id = quest.id;
        quest.state = QuestState.Ongoing;
        questCategory = quest.questCategory;
        questType = quest.questType;
        npcId = quest.npcId;
        dialogueId = quest.dialogueId;
        order = quest.order;
        title = quest.title;
        targetId = quest.targetId;
        desctiption = quest.desctiption;
        rewards = quest.rewards;
        rewardAmount = quest.rewardAmount;
        conditionQuestId = quest.conditionQuestId;
        requireAmount = quest.requireAmount;
        currentAmount = quest.currentAmount;
    }

    public int GetQuestId(){
        return id;
    }

    public string GetTitle(){
        return title;
    }
    
    public bool GetIsCompleted(){
        return isCompleted;
    }
    public int GetCurrentAmount(){
        return currentAmount;
    }

    public int GetRequireAmount(){
        return requireAmount;
    }
    
}