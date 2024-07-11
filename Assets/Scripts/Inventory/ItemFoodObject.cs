using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Food Object", menuName = "Inventory System/Items/Food")]
public class ItemFoodObject : ItemObject
{
    public int restoreHealthValue;
    public bool canCook = false;
    public List<ItemObject> recipes = new List<ItemObject>();
    public List<int> recipeAmount = new List<int>();
    public bool isUseable = false;
    
    public void Awake()
    {
        itemType = ItemType.Food;
    }
}
