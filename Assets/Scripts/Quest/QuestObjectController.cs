using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestObjectController : MonoBehaviour
{
    public int questId;
    private Quest quest;
    private int objectId;
    private ObjectType objectType;

    void Awake()
    {
        quest = QuestManager.Instance.GetQuestByQuestId(questId);
        objectId = GetComponent<ObjectData>().GetId();
        objectType = GetComponent<ObjectData>().GetObjectType();
    }
}
