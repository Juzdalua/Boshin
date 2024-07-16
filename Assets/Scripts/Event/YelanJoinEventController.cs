using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YelanJoinEventController : MonoBehaviour
{
    private PlayerInputManager _inputManager;

    [Header("Join Yelan in party")]
    private bool isJoin = false;
    [SerializeField] private PlayerObject yelanObject;

    void Update()
    {
        if (!isJoin)
        {
            if (DialogueManager.Instance.GetDoneDialogueIds().FindIndex(ele => ele == 3) != -1)
            {
                if (_inputManager != null)
                {
                    _inputManager.canSwap = true;
                    isJoin = true;

                    PlayerManager.Instance.GetPlayerById(yelanObject.Id).IsJoinParty = true;
                    PlayerManager.Instance.GetPlayerById(yelanObject.Id).PartyOrder = 1;
                    PlayerManager.Instance.PlayerSwapController.SetPartyUI();
                    PlayerStatManager.Instance.SetYelanJoin();

                    QuestManager.Instance.UpdateQuest(2, QuestType.Conversation);

                    GameObject.Destroy(transform.gameObject);
                }
            }
        }

        if (DataManager.Instance.isJoinYelan) LoadDataAfterYelanJoin();
    }

    public void SetPlayerInputManager(PlayerInputManager _input)
    {
        _inputManager = _input;
    }

    public void LoadDataAfterYelanJoin()
    {
        isJoin = true;

        PlayerManager.Instance.PlayerSwapController.SetPartyUI();
        PlayerStatManager.Instance.SetYelanJoin();

        GameObject.Destroy(transform.gameObject);
    }
}
