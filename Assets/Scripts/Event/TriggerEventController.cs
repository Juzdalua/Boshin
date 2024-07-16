using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TriggerType
{
    Quest,
    Dialogue
}

public enum TriggerStartType
{
    Ongoing,
    Done
}

public enum TriggerEventType
{
    DestroyWall,
    ShowInfo
}

public class TriggerEventController : MonoBehaviour
{
    [SerializeField] private TriggerType triggerType;
    [SerializeField] private TriggerStartType triggerStartType;
    [SerializeField] private TriggerEventType triggerEventType;
    [SerializeField] private int id;
    public int Id => id;
    [SerializeField] private GameObject triggerWall;
    [SerializeField] private string[] showInfoTexts;

    private List<int> targetIds;

    void Update()
    {
        GetTargetIds();

        // if (!EventManager.Instance.IsDoneEventById(id))
        // {
        //     // Quest trigger
        //     if (triggerType == TriggerType.Quest && targetIds.FindIndex(ele => ele == id) != -1)
        //     {
        //         StartTriggerEvent();
        //     }

        //     // Dialogue Trigger
        //     else if (triggerType == TriggerType.Dialogue && targetIds.FindIndex(ele => ele == id) != -1)
        //     {
        //         StartTriggerEvent();
        //     }
        // }

        // Quest trigger
        if (triggerType == TriggerType.Quest && targetIds.FindIndex(ele => ele == id) != -1)
        {
            StartTriggerEvent();
        }

        // Dialogue Trigger
        else if (triggerType == TriggerType.Dialogue && targetIds.FindIndex(ele => ele == id) != -1)
        {
            StartTriggerEvent();
        }
    }

    public void GetTargetIds()
    {
        if (triggerType == TriggerType.Quest)
        {
            if (triggerStartType == TriggerStartType.Ongoing)
            {
                targetIds = QuestManager.Instance.QuestInventoryObject.GetOngoingQuests();
            }
            else if (triggerStartType == TriggerStartType.Done)
            {
                targetIds = QuestManager.Instance.QuestInventoryObject.GetDoneQuests();
            }
        }
        else if (triggerType == TriggerType.Dialogue)
        {
            if (triggerStartType == TriggerStartType.Done)
            {
                targetIds = DialogueManager.Instance.GetDoneDialogueIds();
            }
        }
    }

    public void StartTriggerEvent()
    {
        if (triggerWall != null)
        {
            DestroyQuestTriggerWall();
        }

        if (showInfoTexts.Length > 0 && !EventManager.Instance.IsDoneEventById(id))
        {
            ShowInfoUI();
        }
    }

    public void DestroyQuestTriggerWall()
    {
        if (triggerWall != null)
        {
            GameObject.Destroy(triggerWall);
        }
    }

    public void ShowInfoUI()
    {
        if (showInfoTexts.Length > 0)
        {
            InformationManager.Instance.OpenInfomationText(showInfoTexts, this);
        }
    }
}
