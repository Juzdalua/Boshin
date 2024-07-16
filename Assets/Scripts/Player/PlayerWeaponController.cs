using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    [SerializeField] private ItemWeaponObject[] weapons;
    [SerializeField] private GameObject[] weaponObjects;
    [SerializeField] private GameObject activeWeaponObject;
    [SerializeField] private ItemWeaponObject activeWeapon;
    BoxCollider _weaponCollider;
    InventoryObject _inventory;
    [SerializeField] private AudioClip swapWeaponAudioClip;

    void Awake()
    {
        SetActiveWeapon();
    }

    void Start()
    {
        _inventory = GetComponent<PlayerInventoryController>().Inventory;

        if (_inventory.Container.Items.FindIndex(ele => ele.id == activeWeapon.Id) == -1)
        {
            _inventory.AddItem(new Item(activeWeapon), 1);
        }
    }

    void Update()
    {
        if (activeWeaponObject == null || !activeWeaponObject.activeInHierarchy) SetActiveWeapon();
    }

    void SetActiveWeapon()
    {
        for (int i = 0; i < weaponObjects.Length; i++)
        {
            if (weaponObjects[i].activeInHierarchy)
            {
                activeWeaponObject = weaponObjects[i];
                break;
            }
        }
        activeWeapon = Array.Find(weapons, ele => ele.Id == activeWeaponObject.GetComponent<ObjectData>().GetId());

        if (activeWeapon.WeaponType == PlayerWeaponType.Sword)
        {
            _weaponCollider = activeWeaponObject.GetComponent<BoxCollider>();
        }

        PlayerManager.Instance.GetPlayerById(GetComponent<ObjectData>().GetId()).Weapon = activeWeapon;
        PlayerManager.Instance.GetPlayerById(GetComponent<ObjectData>().GetId()).WeaponId = activeWeapon.Id;
    }

    public ItemWeaponObject GetWeaponInfo()
    {
        return activeWeapon;
    }

    public void SetWeaponCollider(bool isTrue)
    {
        if (_weaponCollider != null) _weaponCollider.enabled = isTrue;
    }

    public void SwapWeapon(int weaponId)
    {
        if (activeWeapon == null) SetActiveWeapon();
        if (activeWeapon.Id == weaponId) return;

        for (int i = 0; i < weaponObjects.Length; i++)
        {
            if (weaponObjects[i].GetComponent<ObjectData>().GetId() == weaponId) weaponObjects[i].SetActive(true);
            else weaponObjects[i].SetActive(false);
        }

        activeWeapon = Array.Find(weapons, ele => ele.Id == weaponId);
        if (activeWeapon.WeaponType == PlayerWeaponType.Sword) _weaponCollider = activeWeaponObject.GetComponent<BoxCollider>();
    }

    private void SwapWeaponAudio()
    {
        if (swapWeaponAudioClip != null) SoundManager.Instance.SFXPlay(swapWeaponAudioClip.name, swapWeaponAudioClip);
    }
}
