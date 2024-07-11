using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DialogueObject : ScriptableObject
{
    public int id;
    public string interactableName;
    public int order;
    public int questId;
    public bool isDone;
    public int conditionQuestId;
}

[System.Serializable]
public class Dialogue
{
    public string fileName;
    public int id;
    public string interactableName;
    public int order;
    public int questId;
    public bool isDone;
    public int conditionQuestId;

    public Dialogue(DialogueObject dialogue)
    {
        fileName = dialogue.name;
        id = dialogue.id;
        interactableName = dialogue.interactableName;
        order = dialogue.order;
        questId = dialogue.questId;
        isDone = dialogue.isDone;
        conditionQuestId = dialogue.conditionQuestId;
    }

    public string InteractableName { get => interactableName; set => interactableName = value; }

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
