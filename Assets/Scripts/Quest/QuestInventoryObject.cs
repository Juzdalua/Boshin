using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest Inventory", menuName = "Quest System/Quest Inventory")]
public class QuestInventoryObject : ScriptableObject
{
    public List<int> doneQuest = new List<int>();
    public List<int> ongoingQuest = new List<int>();

    public void SetQuestInventory()
    {
        doneQuest = new List<int>();
        ongoingQuest = new List<int>();
    }

    public void Clear()
    {
        doneQuest.Clear();
        ongoingQuest.Clear();
    }

    // Quest Action
    public void AddOngoingQuest(int questId)
    {
        if (IsAvailableAcceptQuest(questId)) ongoingQuest.Add(questId);
        else
        {
            Debug.LogError("AddOngoingQuest Error");
            throw new System.Exception();
        }
    }

    public void AddDoneQuest(int questId)
    {
        if (IsOngoingQuest(questId))
        {
            ongoingQuest.Remove(questId);
            doneQuest.Add(questId);
        }
        else
        {
            Debug.LogError("AddDoneQuest Error");
            throw new System.Exception();
        }
    }

    public void CalcelQuest(int questId)
    {
        if (IsOngoingQuest(questId)) ongoingQuest.Remove(questId);
        else
        {
            Debug.LogError("CalcelQuest Error");
            throw new System.Exception();
        }
    }

    // Get Quest Info
    public List<int> GetOngoingQuests()
    {
        return ongoingQuest;
    }

    public List<int> GetDoneQuests()
    {
        return doneQuest;
    }

    public bool IsAvailableAcceptQuest(int questId)
    {
        return doneQuest.FindIndex(ele => ele == questId) == -1 && ongoingQuest.FindIndex(ele => ele == questId) == -1;
    }

    public bool IsOngoingQuest(int questId)
    {
        return ongoingQuest.FindIndex(ele => ele == questId) != -1 && doneQuest.FindIndex(ele => ele == questId) == -1;
    }


}
