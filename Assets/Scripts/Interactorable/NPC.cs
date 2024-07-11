using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPC : MonoBehaviour, IInteractable
{
    private float rotationSpeed = 720f;
    public string interactText;
    private float time = 0;

    private PlayerInteraction _playerInteraction;
    private Animator _animator;
    private Rigidbody _rb;
    private CapsuleCollider _capsuleCollider;

    [SerializeField] private int[] dialogueIds;
    [SerializeField] private Canvas questMark;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        ShowQuestMark();
    }

    void FixedUpdate()
    {
        // 물리충돌에 의한 회전 방지
        _rb.angularVelocity = Vector3.zero;
    }

    public IEnumerator LookAtPosition(Transform playerPosition)
    {
        yield return null;

        Vector3 lookAtPlayer = (playerPosition.position - transform.position).normalized;
        Quaternion toPlayer = Quaternion.LookRotation(lookAtPlayer);

        float timer = 0f;
        while (timer < 2f)
        {
            timer += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, toPlayer, timer / 2f);
            yield return null;
        }
        yield return new WaitForSeconds(1f);
    }

    public void Interact(Transform interactorTransform)
    {
        if (_playerInteraction == null) _playerInteraction = interactorTransform.GetComponent<PlayerInteraction>();

        if (time == 0)
        {
            time = Time.time;
            StartCoroutine(LookAtPosition(interactorTransform));
            _animator.SetBool("isTalking", true);
            transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
        _playerInteraction.OnPressFSound();
    }

    public string GetInteractText()
    {
        return interactText;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public List<Dictionary<string, object>> GetDialogue()
    {
        Dialogue current = DialogueManager.Instance.GetDialogueOfNotDoneById(dialogueIds);
        if (current != null)
        {
            if (QuestManager.Instance.questInventoryObject.IsAvailableAcceptQuest(current.questId)
                && (QuestManager.Instance.questInventoryObject.GetDoneQuests().FindIndex(ele => ele == current.conditionQuestId) != -1
                    || current.conditionQuestId == 0))
            {
                return current.MatchDialogueWithId();
            }
        }

        return null;
    }

    public void Done(Transform player, int dialogueId)
    {
        _animator.SetBool("isTalking", false);
        Dialogue current = DialogueManager.Instance.GetDialogueById(dialogueId);
        current.SetIsDone(true);

        DialogueManager.Instance.DoneDailogue(dialogueId);

        Quest quest = QuestManager.Instance.GetQuestByQuestId(current.questId);
        if (quest != null)
        {
            QuestManager.Instance.AcceptQuest(quest);
        }

        var yelan = GetComponent<YelanJoinEventController>();
        if (yelan != null)
        {
            yelan.SetPlayerInputManager(player.GetComponent<PlayerInputManager>());
        }
    }

    private void ShowQuestMark()
    {
        if (questMark != null)
        {
            if (GetDialogue() != null && GetDialogue().Count > 0)
            {
                int dialogueId = 0;
                int.TryParse(GetDialogue()[0]["id"].ToString(), out dialogueId);

                if (dialogueId != 0)
                {
                    int questId = DialogueManager.Instance.GetDialogueById(dialogueId).questId;

                    if (QuestManager.Instance.questInventoryObject.IsAvailableAcceptQuest(questId))
                    {
                        questMark.GetComponent<CanvasGroup>().alpha = 1;
                    }
                    else
                    {
                        questMark.GetComponent<CanvasGroup>().alpha = 0;
                    }
                }
            }
            else
            {
                questMark.GetComponent<CanvasGroup>().alpha = 0;
            }
        }
    }
    
    public string GetInteractType(){
        return "NPC";
    }

    public Sprite GetGroundItemImage()
    {
        return null;
    }

    public string GetItemName(){
        return null;
    }
}
