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
    [SerializeField] private int id;
    public int Id => id;
    [SerializeField] private string itemName;
    public string ItemName => itemName;
    [SerializeField] private ItemType itemType;
    public ItemType ItemType
    {
        get { return itemType; }
        set { itemType = value; }
    }
    [SerializeField] private Sprite itemImage;
    public Sprite ItemImage => itemImage;
    [TextArea(15, 20)][SerializeField] private string desctiption;
    public string Desctiption => desctiption;
    [SerializeField] private bool canUse;
    public bool CanUse => canUse;
    [SerializeField] private bool canEquip;
    public bool CanEquip => canEquip;
}

[System.Serializable]
public class Item
{
    public string name;
    [SerializeField] private int id;
    public int Id => id;
    [SerializeField] private string itemName;
    public string ItemName => itemName;
    [SerializeField] private ItemType itemType;
    public ItemType ItemType
    {
        get { return itemType; }
        set { itemType = value; }
    }
    [SerializeField] private Sprite itemImage;
    public Sprite ItemImage => itemImage;
    [TextArea(15, 20)][SerializeField] private string desctiption;
    public string Desctiption => desctiption;
    [SerializeField] private bool canUse;
    public bool CanUse => canUse;
    [SerializeField] private bool canEquip;
    public bool CanEquip => canEquip;
    public Item(ItemObject item)
    {
        name = item.name;
        id = item.Id;
        itemName = item.ItemName;
        itemImage = item.ItemImage;
        desctiption = item.Desctiption;
        canUse = item.CanUse;
        canEquip = item.CanEquip;
    }
}
