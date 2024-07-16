using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DialogueObject : ScriptableObject
{
    [SerializeField] private int id;
    public int Id => id;
    [SerializeField] private string interactableName;
    public string InteractableName => interactableName;
    [SerializeField] private int order;
    public int Order => order;
    [SerializeField] private int questId;
    public int QuestId => questId;
    [SerializeField] private bool isDone;
    public bool IsDone => isDone;
    [SerializeField] private int conditionQuestId;
    public int ConditionQuestId => conditionQuestId;
}

[System.Serializable]
public class Dialogue
{
    [SerializeField] private string fileName;
    public string FileName => fileName;
    [SerializeField] private int id;
    public int Id => id;
    [SerializeField] private string interactableName;
    public string InteractableName => interactableName;
    [SerializeField] private int order;
    public int Order => order;
    [SerializeField] private int questId;
    public int QuestId => questId;
    [SerializeField] private bool isDone;
    public bool IsDone => isDone;
    [SerializeField] private int conditionQuestId;
    public int ConditionQuestId => conditionQuestId;

    public Dialogue(DialogueObject dialogue)
    {
        fileName = dialogue.name;
        id = dialogue.Id;
        interactableName = dialogue.InteractableName;
        order = dialogue.Order;
        questId = dialogue.QuestId;
        isDone = dialogue.IsDone;
        conditionQuestId = dialogue.ConditionQuestId;
    }

    public List<Dictionary<string, object>> MatchDialogueWithId()
    {
        // List<Dictionary<string, object>> dialogueData = CSVReader.Read(dialogeTexts);
        List<Dictionary<string, object>> dialogueData = CSVReader.Read("Dialogues/DialogueData");
        List<Dictionary<string, object>> dialogueResult = new List<Dictionary<string, object>>();

        int dialogueId = 0;

        for (int i = 0; i < dialogueData.Count; i++)
        {
            if (int.TryParse(dialogueData[i]["id"].ToString(), out dialogueId))
            {
                if (dialogueId == id)
                {
                    dialogueResult.Add(dialogueData[i]);
                }
            }
        }

        return dialogueResult;
    }

    public bool GetIsDone()
    {
        return isDone;
    }

    public void SetIsDone(bool setIsDone)
    {
        isDone = setIsDone;
    }
}
