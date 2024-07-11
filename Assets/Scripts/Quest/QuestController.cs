using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    GameObject player;
    PlayerInputManager _inputManager;
    GameObject questUI;
    public GameObject questDetailUI;

    private bool isUseQuestInventory = false;

    [Header("Audio")]
    public AudioClip QuestOpenAudioClip;

    void Start()
    {
        player = PlayerManager.Instance.GetActivePlayer();
        _inputManager = player.GetComponent<PlayerInputManager>();
        questUI = transform.GetChild(0).gameObject;

        questUI.SetActive(false);
    }

    void Update()
    {
        if (!player.activeInHierarchy)
        {
            player = PlayerManager.Instance.GetActivePlayer();
            _inputManager = player.GetComponent<PlayerInputManager>();
        }

        if (_inputManager.isOpenQuestInventory) OpenInventory();
        InventoryControll();
    }

    public void OpenInventory()
    {
        if (!isUseQuestInventory)
        {
            isUseQuestInventory = true;
            OnInventoryOpenSound();
            Cursor.lockState = CursorLockMode.None;
            questUI.SetActive(true);
            Time.timeScale = 0;
            _inputManager.canMove = false;
        }
        _inputManager.isOpenQuestInventory = false;
    }

    public void CloseInventory()
    {
        if (isUseQuestInventory)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
            questDetailUI.GetComponent<DisplayQuest>().ShowQuestDetail(false);
            questUI.SetActive(false);
            isUseQuestInventory = false;
            _inputManager.canMove = true;
        }
        _inputManager.isOpenQuestInventory = false;
    }

    public void InventoryControll()
    {
        if (isUseQuestInventory)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseInventory();
            }
        }
    }

    public bool IsUseInventory()
    {
        return isUseQuestInventory;
    }

    private void OnInventoryOpenSound()
    {
        if (QuestOpenAudioClip != null)
        {
            SoundManager.Instance.SFXPlay("InventoryOpen", QuestOpenAudioClip);
        }
    }
}
