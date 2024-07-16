using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Food Object", menuName = "Inventory System/Items/Food")]
public class ItemFoodObject : ItemObject
{
    [SerializeField] private int restoreHealthValue;
    [SerializeField] private bool canCook = false;
    public bool CanCook => canCook;
    [SerializeField] private List<ItemObject> recipes = new List<ItemObject>();
    public List<ItemObject> Recipes => recipes;
    [SerializeField] private List<int> recipeAmount = new List<int>();
    public List<int> RecipeAmount => recipeAmount;
    [SerializeField] private bool isUseable = false;
    
    public void Awake()
    {
        ItemType = ItemType.Food;
    }
}
