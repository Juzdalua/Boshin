using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundItem : MonoBehaviour, IInteractable
{
   public ItemObject itemObject;
   public int amount = 1;
   PlayerInteraction _playerInteraction;
   PlayerInventoryController _inventoryController;

   public Sprite GetGroundItemImage()
   {
      return itemObject.itemImage;
   }

   public string GetItemName()
   {
      return itemObject.itemName;
   }

   public List<Dictionary<string, object>> GetDialogue()
   {
      return null;
   }


   public string GetInteractText()
   {
      return "[F] 줍기";
   }

   public Transform GetTransform()
   {
      return transform;
   }

   public void Interact(Transform interactorTransform)
   {
      if (_playerInteraction == null) _playerInteraction = interactorTransform.GetComponent<PlayerInteraction>();
      _playerInteraction.OnPressFLootItemSound();

      if (_inventoryController == null) _inventoryController = interactorTransform.GetComponent<PlayerInventoryController>();
      _inventoryController.AddLootItem(itemObject, amount);

      Destroy(gameObject);
   }

   public void Done(Transform transform, int dialogueId = 0)
   {

   }

   public string GetInteractType()
   {
      return "Item";
   }
}
