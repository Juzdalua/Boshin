using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Weapon,
    Food,
    Etc,
    Default,
    Currency
}

public abstract class ItemObject : ScriptableObject
{
    public int id;
    public string itemName;
    public ItemType itemType;
    public Sprite itemImage;
    [TextArea(15, 20)] public string desctiption;
    public bool canUse;
    public bool canEquip;
}

[System.Serializable]
public class Item{
    public string name;
    public int id;
    public string itemName;
    public Sprite itemImage;
    [TextArea(15, 20)] public string desctiption;
    public bool canUse;
    public bool canEquip;
    public Item(ItemObject item){
        name = item.name;
        id = item.id;
        itemName = item.itemName;
        itemImage = item.itemImage;
        desctiption = item.desctiption;
        canUse = item.canUse;
        canEquip = item.canEquip;
    }
}
