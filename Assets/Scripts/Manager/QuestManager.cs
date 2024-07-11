using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    public GameObject questUI;
    public QuestDatabaseObject database;
    public QuestInventoryObject questInventoryObject;
    public InventoryObject itemInventory;

    Transform questImageNotDoneTransform;
    Transform questImageDoneTransform;
    Transform questMessageTransform;
    public AudioClip acceptQuestAudioClip;
    public AudioClip doneQuestAudioClip;
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
        database.quests.Clear();
        questInventoryObject.Clear();
    }

    public void AcceptQuest(Quest quest)
    {
        questInventoryObject.AddOngoingQuest(quest.id);
        SetQuest(quest);
    }

    public void CancelQuest(Quest quest)
    {
        questInventoryObject.CalcelQuest(quest.id);
        DialogueManager.Instance.CancelQuestWithDialogue(quest.dialogueId);
    }

    public void DoneQuest(Quest quest)
    {
        OnDoneQuestSound();
        questImageNotDoneTransform.GetComponent<CanvasGroup>().alpha = 0;
        questImageDoneTransform.GetComponent<CanvasGroup>().alpha = 1;
        questMessageTransform.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough;

        questInventoryObject.AddDoneQuest(quest.id);

        if (quest.rewards.Length > 0)
        {
            for (int i = 0; i < quest.rewards.Length; i++)
            {
                var reward = itemInventory.GetItemById(quest.rewards[i]);
                itemInventory.AddItem(reward, quest.rewardAmount[i]);
            }
        }
        if (questUICoroutine != null) StopCoroutine(questUICoroutine);
        StartCoroutine(ShowQuestUI(false));
    }

    public Quest GetQuestByQuestId(int questId)
    {
        return database.quests.Find(ele => ele.id == questId);
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
            if (quest.questType == QuestType.DefeatMonster) questMessage = SetDefeatMonsterQuestMessage(quest.GetTitle(), 0, quest.GetRequireAmount());
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
            if (questType == QuestType.DefeatMonster && targetId != 0 && targetId == quest.targetId)
            {
                quest.currentAmount++;
                string updateMessage = SetDefeatMonsterQuestMessage(quest.title, quest.currentAmount, quest.requireAmount);
                questMessageTransform.GetComponent<TextMeshProUGUI>().text = updateMessage;

                if (quest.currentAmount >= quest.requireAmount)
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
