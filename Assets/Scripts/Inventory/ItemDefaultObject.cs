using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Default Object", menuName = "Inventory System/Items/Default")]
public class ItemDefaultObject : ItemObject
{
    public void Awake()
    {
        ItemType = ItemType.Default;
    }
}
