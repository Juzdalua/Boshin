using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest Database", menuName = "Quest System/Quests/Database")]

public class QuestDatabaseObject : ScriptableObject
{
   public QuestObject[] questObjects;
   public List<Quest> quests = new List<Quest>();

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
      quests = quests.OrderBy(ele => ele.order).GroupBy(ele => ele.id).Select(ele => ele.First()).ToList();
   }
}
