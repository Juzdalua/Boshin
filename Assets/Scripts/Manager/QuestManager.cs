using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    [SerializeField] private GameObject questUI;
    [SerializeField] private QuestDatabaseObject database;
    public QuestDatabaseObject Database => database;
    [SerializeField] private QuestInventoryObject questInventoryObject;
    public QuestInventoryObject QuestInventoryObject => questInventoryObject;
    [SerializeField] private InventoryObject itemInventory;

    Transform questImageNotDoneTransform;
    Transform questImageDoneTransform;
    Transform questMessageTransform;
    [SerializeField] private AudioClip acceptQuestAudioClip;
    [SerializeField] private AudioClip doneQuestAudioClip;
    Coroutine questUICoroutine;

    bool isSetup = false;

    void Awake()
    {
        if (!isSetup)
        {
            SetUpQuestTransform();
            isSetup = true;
        }
    }

    void SetUpQuestTransform()
    {
        questImageNotDoneTransform = questUI.transform.GetChild(0);
        questImageDoneTransform = questUI.transform.GetChild(1);
        questMessageTransform = questUI.transform.GetChild(2);

        questUI.GetComponent<CanvasGroup>().alpha = 0;
    }

    private void OnApplicationQuit()
    {
        database.Quests.Clear();
        questInventoryObject.Clear();
    }

    public void AcceptQuest(Quest quest)
    {
        questInventoryObject.AddOngoingQuest(quest.Id);
        SetQuest(quest);
    }

    public void CancelQuest(Quest quest)
    {
        questInventoryObject.CalcelQuest(quest.Id);
        DialogueManager.Instance.CancelQuestWithDialogue(quest.DialogueId);
    }

    public void DoneQuest(Quest quest)
    {
        OnDoneQuestSound();
        questImageNotDoneTransform.GetComponent<CanvasGroup>().alpha = 0;
        questImageDoneTransform.GetComponent<CanvasGroup>().alpha = 1;
        questMessageTransform.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough;

        questInventoryObject.AddDoneQuest(quest.Id);

        if (quest.Rewards.Length > 0)
        {
            for (int i = 0; i < quest.Rewards.Length; i++)
            {
                var reward = itemInventory.GetItemById(quest.Rewards[i]);
                itemInventory.AddItem(reward, quest.RewardAmount[i]);
            }
        }
        if (questUICoroutine != null) StopCoroutine(questUICoroutine);
        StartCoroutine(ShowQuestUI(false));
    }

    public Quest GetQuestByQuestId(int questId)
    {
        return database.Quests.Find(ele => ele.Id == questId);
    }

    public void SetQuest(Quest quest)
    {
        if (!isSetup)
        {
            SetUpQuestTransform();
            isSetup = true;
        }
        
        if (quest != null && !quest.GetIsCompleted())
        {
            OnAcceptQuestSound();
            questImageNotDoneTransform.GetComponent<CanvasGroup>().alpha = 1;
            questImageDoneTransform.GetComponent<CanvasGroup>().alpha = 0;
            questMessageTransform.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Normal;

            string questMessage;
            if (quest.QuestType == QuestType.DefeatMonster) questMessage = SetDefeatMonsterQuestMessage(quest.GetTitle(), 0, quest.GetRequireAmount());
            else questMessage = quest.GetTitle();

            questMessageTransform.GetComponent<TextMeshProUGUI>().text = questMessage;

            if (questUICoroutine != null) StopCoroutine(questUICoroutine);
            StartCoroutine(ShowQuestUI(true));
        }
    }

    private string SetDefeatMonsterQuestMessage(string message, int current, int goal)
    {
        return message + "( " + current + " / " + goal + ")";
    }

    // Todo
    public void UpdateQuest(int questId, QuestType questType, int targetId = 0)
    {
        Quest quest = GetQuestByQuestId(questId);
        if (quest != null && !quest.GetIsCompleted())
        {
            // Defeat Monster
            if (questType == QuestType.DefeatMonster && targetId != 0 && targetId == quest.TargetId)
            {
                quest.CurrentAmount++;
                string updateMessage = SetDefeatMonsterQuestMessage(quest.Title, quest.CurrentAmount, quest.RequireAmount);
                questMessageTransform.GetComponent<TextMeshProUGUI>().text = updateMessage;

                if (quest.CurrentAmount >= quest.RequireAmount)
                {
                    DoneQuest(quest);
                }
            }
            else if (questType == QuestType.Conversation)
            {
                DoneQuest(quest);
            }
        }
    }

    private IEnumerator ShowQuestUI(bool isShow)
    {
        yield return null;

        float timer = 0f;
        float endTimer = isShow ? 1f : 3f;

        while (timer < endTimer)
        {
            timer += Time.deltaTime;
            questUI.GetComponent<CanvasGroup>().alpha = isShow ? Mathf.Lerp(0f, 1f, timer / endTimer) : Mathf.Lerp(1f, 0f, timer / endTimer);
            yield return null;

            if (isShow ? questUI.GetComponent<CanvasGroup>().alpha == 1 : questUI.GetComponent<CanvasGroup>().alpha == 0)
            {
                yield break;
            }
        }
        yield return null;
        yield break;
    }

    private void OnAcceptQuestSound()
    {
        if (acceptQuestAudioClip != null)
        {
            SoundManager.Instance.SFXPlay("acceptQuest", acceptQuestAudioClip);
        }
    }

    private void OnDoneQuestSound()
    {
        if (doneQuestAudioClip != null)
        {
            SoundManager.Instance.SFXPlay("doneQuest", doneQuestAudioClip);
        }
    }
}
