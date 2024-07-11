using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureBox : MonoBehaviour, IInteractable
{
    [Header("Items")]
    public GameObject[] lootItemPrefabs;

    [Header("Audio")]
    public AudioClip treasureAudioClip;

    [Header("Animation")]
    public GameObject closeObejct;
    public GameObject openObejct;
    bool isOpen = false;

    void Start()
    {
        closeObejct.SetActive(true);
        openObejct.SetActive(false);
    }

    void Update(){
        if(EventManager.Instance.doneTreasure.FindIndex(id => id == GetComponent<ObjectData>().GetId()) != -1){
            Destroy(gameObject);
        }
    }

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
        return "[F] 열기";
    }

    public string GetInteractType()
    {
        return "Treasure";
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
        if(isOpen) return;
        isOpen = true;
        if (treasureAudioClip != null) SoundManager.Instance.SFXPlay(treasureAudioClip.name, treasureAudioClip);

        for (int i = 0; i < lootItemPrefabs.Length; i++)
        {
            float itemRandomX = Random.Range(-2.5f, 2.5f);
            float itemRandomZ = Random.Range(-2.5f, 2.5f);
            Instantiate(lootItemPrefabs[i], transform.position + new Vector3(itemRandomX, 0.1f, itemRandomZ), Quaternion.identity);
        }
        closeObejct.SetActive(false);
        openObejct.SetActive(true);
        Destroy(gameObject, 1f);

        EventManager.Instance.AddTreasure(GetComponent<ObjectData>().GetId());
    }

}
