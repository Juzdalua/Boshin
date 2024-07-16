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
    [SerializeField] private PlayerWeaponType weaponType;
    public PlayerWeaponType WeaponType => weaponType;
    [SerializeField] private float atkBonus;
    public float AtkBonus => atkBonus;
    [SerializeField] private float criRateBonus;
    public float CriRateBonus => criRateBonus;
    [SerializeField] private float criDmgBonus;
    public float CriDmgBonus => criDmgBonus;
    
    public void Awake()
    {
        ItemType = ItemType.Weapon;
    }
}
