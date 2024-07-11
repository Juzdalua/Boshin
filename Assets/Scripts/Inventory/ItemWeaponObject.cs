using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerWeaponType
{
    Sword,
    Gun
}

[CreateAssetMenu(fileName = "New Weapon Object", menuName = "Inventory System/Items/Weapon")]
public class ItemWeaponObject : ItemObject
{
    public PlayerWeaponType weaponType;
    public float atkBonus;
    public float criRateBonus;
    public float criDmgBonus;
    
    public void Awake()
    {
        itemType = ItemType.Weapon;
    }
}
