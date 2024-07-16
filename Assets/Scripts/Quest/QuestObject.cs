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

[CreateAssetMenu(fileName = "New Quest Object", menuName = "Quest System/Quests/Quest")]
public class QuestObject : ScriptableObject
{
    [SerializeField] private int id;
    public int Id => id;
    [SerializeField] private QuestState state;
    public QuestState State
    {
        get { return state; }
        set { state = value; }
    }
    [SerializeField] private QuestCategory questCategory = QuestCategory.Main;
    public QuestCategory QuestCategory => questCategory;
    [SerializeField] private QuestType questType;
    public QuestType QuestType => questType;
    [SerializeField] private int npcId;
    public int NpcId => npcId;
    [SerializeField] private int dialogueId;
    public int DialogueId => dialogueId;
    [SerializeField] private int order;
    public int Order => order;
    [SerializeField] private string title;
    public string Title => title;
    [SerializeField] private int targetId;
    public int TargetId => targetId;
    [SerializeField] private bool isCompleted = false;
    public bool IsCompleted => isCompleted;
    [SerializeField] private int conditionQuestId = 0;
    public int ConditionQuestId => conditionQuestId;
    [SerializeField] private int requireAmount;
    public int RequireAmount => requireAmount;
    [SerializeField] private int currentAmount = 0;
    public int CurrentAmount
    {
        get { return currentAmount; }
        set { currentAmount = value; }
    }
    [TextArea(15, 20)][SerializeField] private string desctiption;
    public string Desctiption => desctiption;
    [SerializeField] private int[] rewards;
    public int[] Rewards => rewards;
    [SerializeField] private int[] rewardAmount;
    public int[] RewardAmount => rewardAmount;
    [SerializeField] private bool isAutoClear = true;
    public bool IsAutoClear => isAutoClear;
    [SerializeField] private GameObject questTriggerWall;
    public GameObject QuestTriggerWall => questTriggerWall;
}

[System.Serializable]
public class Quest
{
    [SerializeField] private int id;
    public int Id => id;
    [SerializeField] private QuestState state;
    public QuestState State
    {
        get { return state; }
        set { state = value; }
    }
    [SerializeField] private QuestCategory questCategory = QuestCategory.Main;
    public QuestCategory QuestCategory => questCategory;
    [SerializeField] private QuestType questType;
    public QuestType QuestType => questType;
    [SerializeField] private int npcId;
    public int NpcId => npcId;
    [SerializeField] private int dialogueId;
    public int DialogueId => dialogueId;
    [SerializeField] private int order;
    public int Order => order;
    [SerializeField] private string title;
    public string Title => title;
    [SerializeField] private int targetId;
    public int TargetId => targetId;
    [SerializeField] private bool isCompleted = false;
    public bool IsCompleted => isCompleted;
    [SerializeField] private int conditionQuestId = 0;
    public int ConditionQuestId => conditionQuestId;
    [SerializeField] private int requireAmount;
    public int RequireAmount => requireAmount;
    [SerializeField] private int currentAmount = 0;
    public int CurrentAmount
    {
        get { return currentAmount; }
        set { currentAmount = value; }
    }
    [TextArea(15, 20)][SerializeField] private string desctiption;
    public string Desctiption => desctiption;
    [SerializeField] private int[] rewards;
    public int[] Rewards => rewards;
    [SerializeField] private int[] rewardAmount;
    public int[] RewardAmount => rewardAmount;
    [SerializeField] private bool isAutoClear = true;
    public bool IsAutoClear => isAutoClear;
    [SerializeField] private GameObject questTriggerWall;
    public GameObject QuestTriggerWall => questTriggerWall;

    public Quest(QuestObject quest)
    {
        id = quest.Id;
        quest.State = QuestState.Ongoing;
        questCategory = quest.QuestCategory;
        questType = quest.QuestType;
        npcId = quest.NpcId;
        dialogueId = quest.DialogueId;
        order = quest.Order;
        title = quest.Title;
        targetId = quest.TargetId;
        desctiption = quest.Desctiption;
        rewards = quest.Rewards;
        rewardAmount = quest.RewardAmount;
        conditionQuestId = quest.ConditionQuestId;
        requireAmount = quest.RequireAmount;
        currentAmount = quest.CurrentAmount;
    }

    public int GetQuestId()
    {
        return id;
    }

    public string GetTitle()
    {
        return title;
    }

    public bool GetIsCompleted()
    {
        return isCompleted;
    }
    public int GetCurrentAmount()
    {
        return currentAmount;
    }

    public int GetRequireAmount()
    {
        return requireAmount;
    }

}