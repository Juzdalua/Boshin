using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryController : MonoBehaviour
{
    [SerializeField] private InventoryObject inventory;
    public InventoryObject Inventory => inventory;
    [SerializeField] private ItemDatabaseObject itemDatabase;
    private PlayerInteraction _playerInteraction;
    private PlayerInteractUI _interactionUI;

    void Awake()
    {
        _playerInteraction = GetComponent<PlayerInteraction>();
    }

    private void OnApplicationQuit()
    {
        inventory.Container.Items.Clear();
        itemDatabase.items.Clear();

    }

    public void AddLootItem(ItemObject itemObject, int amount = 1)
    {
        inventory.AddItem(new Item(itemObject), amount);
    }

}
