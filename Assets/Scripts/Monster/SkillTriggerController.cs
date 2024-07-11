using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTriggerController : MonoBehaviour
{
   GameObject player;
   private PlayerSkillController _skillController;

   void Awake()
   {
      player = PlayerManager.Instance.GetActivePlayer();
   }

   void Start()
   {
      if (transform.tag == "Player_ESkill")
         Destroy(gameObject, 1f);
      else if (transform.tag == "Player_QSkill")
         Destroy(gameObject, 5f);

      _skillController = player.GetComponent<PlayerSkillController>();
   }

   void Update()
   {
      if (!player.activeInHierarchy) player = PlayerManager.Instance.GetActivePlayer();
   }

   private void OnTriggerEnter(Collider other)
   {
      if (other.tag == "Background")
      {
         Destroy(gameObject);
      }
      else if (other.tag.StartsWith("Monster_"))
      {
         if (transform.tag == "Player_ESkill")
         {
            _skillController.ChargeQGauge();
         }
         else if (transform.tag == "Player_Weapon")
         {
            _skillController.ChargeQGauge();
         }
         Destroy(gameObject);
      }
   }
}
