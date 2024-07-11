using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType
{
    NPC,
    Portal,
    Monster,
    Player,
    Item,
    Quest,
    Button,
    Treasure
}

public class ObjectData : MonoBehaviour
{

    [SerializeField] private int id;
    [SerializeField] private ObjectType objectType;

    public int GetId()
    {
        return id;
    }

    public ObjectType GetObjectType()
    {
        return objectType;
    }

    public void SetId(int itemId){
        id = itemId;
    }

    public void SetObjectType(ObjectType itemType){
        objectType = itemType;
    }
}
