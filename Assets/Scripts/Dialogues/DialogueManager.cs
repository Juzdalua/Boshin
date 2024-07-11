using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DialogueManager : Singleton<DialogueManager>
{

    private PlayerInteraction _playerInteraction;
    public TextMeshProUGUI interactableName;
    public TextMeshProUGUI content;
    public GameObject dialogueBox;

    public Dictionary<int, Dialogue> playerDialogue = new Dictionary<int, Dialogue>();
    public DialogueDatabaseObject database;

    void Start()
    {
        SetDialogueManager();
    }

    public void SetDialogueManager(){
        _playerInteraction = PlayerManager.Instance.GetActivePlayer().GetComponent<PlayerInteraction>();
        playerDialogue = new Dictionary<int, Dialogue>();
        dialogueBox.SetActive(false);
    }

    private void OnApplicationQuit()
    {
        database.dialogues.Clear();
    }

    public int Action(int userDialogueStage, List<Dictionary<string, object>> dialogue)
    {

        if (userDialogueStage <= dialogue.Count - 1)
        {
            dialogueBox.SetActive(true);
        }
        else if (userDialogueStage > dialogue.Count - 1)
        {
            dialogueBox.SetActive(false);
        }

        if (userDialogueStage <= dialogue.Count - 1)
        {
            interactableName.text = dialogue[userDialogueStage]["name"].ToString();
            content.text = dialogue[userDialogueStage]["dialogue"].ToString();
            _playerInteraction.SetUserDialogueStage();
        }
        return Math.Clamp(dialogue.Count - userDialogueStage, 0, dialogue.Count);
    }

    public void DoneDailogue(int dialogueId)
    {
        if (playerDialogue.ContainsKey(dialogueId))
        {
            Debug.LogError("Already exist dialogue");
        }
        else
        {
            playerDialogue.Add(dialogueId, GetDialogueById(dialogueId));
        }
    }

    public void CancelQuestWithDialogue(int dialogueId)
    {
        if (!playerDialogue.ContainsKey(dialogueId))
        {
            Debug.LogError("Not exist dialogue");
        }
        else
        {
            playerDialogue.Remove(dialogueId);
            database.dialogues.Find(ele => ele.id == dialogueId).SetIsDone(false);
        }
    }

    public Dialogue GetDialogueById(int dialogueId)
    {
        return database.dialogues.Find(ele => ele.id == dialogueId);
    }

    public Dialogue GetDialogueOfNotDoneById(int[] dialogueIds)
    {
        Array.Sort(dialogueIds);
        Dialogue currentDialogue = null;
        for (int i = 0; i < dialogueIds.Length; i++)
        {
            currentDialogue = database.dialogues.Find(ele => ele.id == dialogueIds[i]).GetIsDone() ? null : database.dialogues.Find(ele => ele.id == dialogueIds[i]);
            if (currentDialogue != null)
            {
                break;
            }
        }
        return currentDialogue;
    }

    public List<Dialogue> GetDoneDialogues()
    {
        List<Dialogue> doneDialogues = new List<Dialogue>();

        for (int i = 0; i < database.dialogues.Count; i++)
        {
            if (database.dialogues[i].isDone)
            {
                doneDialogues.Add(database.dialogues[i]);
            }
        }
        return doneDialogues;
    }

    public List<int> GetDoneDialogueIds()
    {
        
        List<int> doneDialoguesIds = new List<int>();

        for (int i = 0; i < GetDoneDialogues().Count; i++)
        {
           doneDialoguesIds.Add(GetDoneDialogues()[i].id);
        }
        return doneDialoguesIds;
    }

    // public Dialogue GetDialogueOfNotDoneByIds(int[] dialogueId)
    // {
    //     return database.dialogues.Find(ele => ele.id == dialogueId).GetIsDone() ? null : database.dialogues.Find(ele => ele.id == dialogueId);
    // }

}
