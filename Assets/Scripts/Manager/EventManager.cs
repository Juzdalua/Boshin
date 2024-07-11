using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    public List<int> doneEventIds = new List<int>();
    public List<int> doneTreasure = new List<int>();

    public void SetEventManager()
    {
        doneEventIds = new List<int>();
        doneTreasure = new List<int>();
    }

    public void SetDoneEventById(int id)
    {
        doneEventIds.Add(id);
    }

    public bool IsDoneEventById(int id)
    {
        return doneEventIds.FindIndex(ele => ele == id) != -1;
    }

    public void AddTreasure(int treasureId)
    {
        doneTreasure.Add(treasureId);
    }
}
