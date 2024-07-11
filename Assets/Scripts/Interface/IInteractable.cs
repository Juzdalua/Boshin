using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable{
    void Interact(Transform interactorTransform);
    string GetInteractText();
    Transform GetTransform();
    List<Dictionary<string, object>> GetDialogue();
    void Done(Transform transform, int dialogueId = 0);
    string GetInteractType();
    Sprite GetGroundItemImage();
    string GetItemName();
}