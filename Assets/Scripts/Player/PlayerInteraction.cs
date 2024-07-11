using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    GameObject player;
    PlayerInputManager _playerInputManager;
    public DialogueManager _dialogueManager;

    private float rotationSpeed = 100f;

    private string objectTag;
    private int dialougeCount = 1;
    private int userDialogueStage = 0;
    private bool isAvailableInteract = false;
    private bool isConversationNow = false;

    [Header("Camera")]
    public CinemachineVirtualCamera playerCamera;
    public CinemachineVirtualCamera[] virtualCameras;

    [Header("Show UI")]
    public GameObject[] characterUIs;

    [Header("Audio")]
    public AudioClip PressFAudioClip;
    public AudioClip OnPressFLootItemAudioClip;

    void Start()
    {
        player = PlayerManager.Instance.GetActivePlayer();
        _playerInputManager = GetComponent<PlayerInputManager>();

        ShowCharacterUIs(true);
        SwitchToCamera(playerCamera);
    }

    void Update()
    {
        if (!player.activeInHierarchy)
        {
            player = PlayerManager.Instance.GetActivePlayer();
            _playerInputManager = GetComponent<PlayerInputManager>();
        }

        IInteractable interactable = GetInteractableObject();

        if (interactable != null)
        {
            // Interaction UI 
            if (objectTag == "NPC" && interactable.GetDialogue() != null && interactable.GetDialogue().Count > 0)
            {
                isAvailableInteract = true;
            }
            else if (objectTag == "Item")
            {
                isAvailableInteract = true;
            }
            else
            {
                isAvailableInteract = true;
            }


            if (_playerInputManager.fInteraction)
            {
                if (objectTag == "NPC")
                {
                    if (interactable.GetDialogue() != null && interactable.GetDialogue().Count > 0)
                    {
                        isConversationNow = true;
                        _playerInputManager.canMove = false;
                        _playerInputManager.fInteraction = false;
                        _playerInputManager.StopPosition();
                        interactable.Interact(transform);

                        // 플레이어가 NPC 바라보기
                        Vector3 lookNPC = (interactable.GetTransform().position - transform.position).normalized;
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookNPC), Time.deltaTime * rotationSpeed);

                        _playerInputManager.isInteraction = true;
                        ShowCharacterUIs(false);
                        dialougeCount = _dialogueManager.Action(userDialogueStage, interactable.GetDialogue());
                        if (dialougeCount == 0)
                        {
                            DoneInteraction(interactable, int.Parse(interactable.GetDialogue()[0]["id"].ToString()));
                        }
                    }
                    else
                    {
                        isAvailableInteract = false;
                    }
                }

                else if (objectTag == "Item")
                {
                    interactable.Interact(transform);
                    _playerInputManager.fInteraction = false;
                    _playerInputManager.canMove = true;
                }

                else if (objectTag == "Object")
                {
                    interactable.Interact(transform);
                    _playerInputManager.fInteraction = false;
                    _playerInputManager.canMove = true;
                }

                else if (objectTag == "Treasure")
                {
                    interactable.Interact(transform);
                    _playerInputManager.fInteraction = false;
                    _playerInputManager.canMove = true;
                }
            }
        }
        else
        {
            _playerInputManager.fInteraction = false;
            _playerInputManager.canMove = true;
        }

    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        string tag = hit.collider.tag;
        if (tag.StartsWith("Interaction_"))
        {
        }

    }

    public IInteractable GetInteractableObject()
    {
        List<IInteractable> interactables = new List<IInteractable>();

        float interactRange = 2f;
        Collider[] colliderArr = Physics.OverlapSphere(transform.position, interactRange);
        foreach (Collider collider in colliderArr)
        {
            string tag = collider.tag;
            if (tag.StartsWith("Interaction_"))
            {
                IInteractable interactable = collider.GetComponent<IInteractable>();

                interactables.Add(interactable);
                objectTag = tag.Substring(12);
            }
        }
        IInteractable closeInteractable = null;
        foreach (IInteractable interactable in interactables)
        {
            if (closeInteractable == null)
            {
                closeInteractable = interactable;
            }
            else
            {
                if (Vector3.Distance(transform.position, interactable.GetTransform().position) < Vector3.Distance(transform.position, closeInteractable.GetTransform().position))
                {
                    closeInteractable = interactable;
                }
            }
        }
        return closeInteractable;
    }

    private void DoneInteraction(IInteractable interactable, int dialogueId)
    {
        _playerInputManager.isInteraction = false;
        SwitchToCamera(playerCamera);
        ShowCharacterUIs(true);
        _playerInputManager.canMove = true;
        interactable.Done(transform, dialogueId);
        userDialogueStage = 0;
        isConversationNow = false;
    }

    public bool IsAvailableInteract()
    {
        return isAvailableInteract;
    }
    
    public bool IsConversationNow()
    {
        return isConversationNow;
    }

    public int GetDialougeCount()
    {
        return dialougeCount;
    }

    public int GetUserDialogueStage()
    {
        return userDialogueStage;
    }

    public void SetUserDialogueStage()
    {
        userDialogueStage++;
    }

    public void SwitchToCamera(CinemachineVirtualCamera targetCamera)
    {
        foreach (CinemachineVirtualCamera camera in virtualCameras)
        {
            camera.enabled = camera == targetCamera;
        }
    }

    public void OnPressFSound()
    {
        if (PressFAudioClip != null)
        {
            SoundManager.Instance.SFXPlay("PressF", PressFAudioClip);
        }
    }

    public void OnPressFLootItemSound()
    {
        if (OnPressFLootItemAudioClip != null)
        {
            SoundManager.Instance.SFXPlay("PressF", OnPressFLootItemAudioClip);
        }
    }

    public void ShowCharacterUIs(bool isShow){
        for(int i=0; i<characterUIs.Length; i++) characterUIs[i].SetActive(isShow);
    }
}
