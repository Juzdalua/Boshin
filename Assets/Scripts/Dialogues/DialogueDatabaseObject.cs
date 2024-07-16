using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Database", menuName = "Dialogue System/Database")]
public class DialogueDatabaseObject : ScriptableObject
{
    [SerializeField] private DialogueObject[] dialogueObjects;
    public List<Dialogue> dialogues = new List<Dialogue>();

    void OnEnable()
    {
        SetDialogueDatabase();
    }

    public void SetDialogueDatabase()
    {
        dialogues = new List<Dialogue>();
        for (int i = 0; i < dialogueObjects.Length; i++)
        {
            dialogues.Add(new Dialogue(dialogueObjects[i]));
        }
        // dialogues = dialogues.OrderBy(ele => ele.order).ToList();
        dialogues = dialogues.OrderBy(ele => ele.Order).GroupBy(ele => ele.Id).Select(ele => ele.First()).ToList();
    }
}
