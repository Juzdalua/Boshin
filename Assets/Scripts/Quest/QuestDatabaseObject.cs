using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest Database", menuName = "Quest System/Quests/Database")]

public class QuestDatabaseObject : ScriptableObject
{
   [SerializeField] private QuestObject[] questObjects;
   public QuestObject[] QuestObjects => questObjects;
   [SerializeField] private List<Quest> quests = new List<Quest>();
   public List<Quest> Quests
   {
      get { return quests; }
      set { quests = value; }
   }

   void OnEnable()
   {
      SetQuestDatabase();
   }

   public void SetQuestDatabase()
   {
      quests = new List<Quest>();
      for (int i = 0; i < questObjects.Length; i++)
      {
         quests.Add(new Quest(questObjects[i]));
      }
      quests = quests.OrderBy(ele => ele.Order).GroupBy(ele => ele.Id).Select(ele => ele.First()).ToList();
   }
}
