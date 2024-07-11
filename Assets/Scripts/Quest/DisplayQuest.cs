using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayQuest : MonoBehaviour
{
    public QuestInventoryObject questInventory;
    public GameObject questPrefab;
    public GameObject questDetail;
    public GameObject rewardPrefab;
    public InventoryObject itemInventoryObject;
    Transform questDetailRewards;
    private Dictionary<int, GameObject> questDisplayed = new Dictionary<int, GameObject>();
    private List<GameObject> questDetailDisplayed = new List<GameObject>();

    void Start()
    {
        // CreateDisplay();
        questDetailRewards = questDetail.transform.GetChild(1);
    }

    void Update()
    {
        UpdateDisplay();
    }

    // public void CreateDisplay()
    // {
    //     for (int i = 0; i < questInventory.GetOngoingQuests().Count; i++)
    //     {
    //         Quest quest = QuestManager.Instance.GetQuestByQuestId(questInventory.GetOngoingQuests()[i]);

    //         var obj = Instantiate(questPrefab, Vector3.zero, Quaternion.identity, transform);
    //         obj.GetComponentInChildren<TextMeshProUGUI>().text = quest.title;
    //         questDisplayed.Add(quest.id, obj);
    //     }
    // }

    public void UpdateDisplay()
    {
        for (int i = 0; i < questInventory.GetOngoingQuests().Count; i++)
        {
            Quest quest = QuestManager.Instance.GetQuestByQuestId(questInventory.GetOngoingQuests()[i]);
            if (!questDisplayed.ContainsKey(questInventory.GetOngoingQuests()[i]))
            {
                GameObject obj = Instantiate(questPrefab, Vector3.zero, Quaternion.identity, transform);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = quest.title;

                obj.GetComponent<ObjectData>().SetId(quest.id);
                obj.GetComponent<ObjectData>().SetObjectType(ObjectType.Quest);

                questDisplayed.Add(quest.id, obj);
                obj.GetComponent<Button>().onClick.AddListener(() => ClickQuestDetail(obj));
            }
        }

        if (questDisplayed.Count > 0)
        {
            List<int> questKeys = questDisplayed.Keys.ToList();
            for (int i = 0; i < questKeys.Count; i++)
            {
                if (questInventory.GetOngoingQuests().FindIndex(ele => ele == questKeys[i]) == -1)
                {
                    GameObject.Destroy(questDisplayed[questKeys[i]]);
                    questDisplayed.Remove(questKeys[i]);
                }
            }
        }
    }

    public void ShowQuestDetail(bool isShow)
    {
        questDetail.SetActive(isShow);
    }

    public void ClickQuestDetail(GameObject questObject)
    {
        int questId = questObject.GetComponent<ObjectData>().GetId();
        var quest = QuestManager.Instance.GetQuestByQuestId(questId);

        questDetail.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = quest.desctiption;
        ShowQuestDetail(true);

        // rewards
        if (quest.rewards.Length > 0 && questDetailDisplayed.Count == 0)
        {
            for (int i = 0; i < quest.rewards.Length; i++)
            {
                GameObject obj = Instantiate(rewardPrefab, Vector3.zero, Quaternion.identity, questDetailRewards.GetChild(1));
                obj.transform.GetChild(0).GetComponent<Image>().sprite = itemInventoryObject.GetItemById(quest.rewards[i]).itemImage;
                obj.GetComponentInChildren<TextMeshProUGUI>().text = quest.rewardAmount[i].ToString("n0");
                questDetailDisplayed.Add(obj);
            }
        }
    }
}
