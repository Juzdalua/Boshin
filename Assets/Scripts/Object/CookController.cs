using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookController : MonoBehaviour, IInteractable
{
    public void Done(Transform transform, int dialogueId = 0)
    {

    }

    public List<Dictionary<string, object>> GetDialogue()
    {
        return null;
    }

    public Sprite GetGroundItemImage()
    {
        return null;
    }

    public string GetInteractText()
    {
        return "[F] 요리하기";
    }

    public string GetInteractType()
    {
        return "Cook";
    }

    public string GetItemName()
    {
        return null;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void Interact(Transform interactorTransform)
    {
        CookManager.Instance.OpenCook();
    }

}
