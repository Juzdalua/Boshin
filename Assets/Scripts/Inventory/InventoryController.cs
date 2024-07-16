using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    GameObject player;
    PlayableCharacter mainCharacter;
    PlayerInputManager _inputManager;
    GameObject inventoryUI;
    [SerializeField] private GameObject inventoryDetailUI;
    [SerializeField] private GameObject useItemUI;
    [SerializeField] private InventoryObject inventory;

    private bool isUseInventory = false;
    int currentUseItemAmount;

    [Header("Audio")]
    [SerializeField] private AudioClip InventoryOpenAudioClip;

    void Start()
    {
        player = PlayerManager.Instance.GetActivePlayer();
        _inputManager = player.GetComponent<PlayerInputManager>();
        inventoryUI = transform.GetChild(0).gameObject;

        inventoryUI.SetActive(false);
    }

    void Update()
    {
        if (!player.activeInHierarchy)
        {
            player = PlayerManager.Instance.GetActivePlayer();
            _inputManager = player.GetComponent<PlayerInputManager>();
        }

        if (_inputManager.isOpenInventory) OpenInventory();
        InventoryControll();

        if (_inputManager.zInteraction) UseItem();
        SetUseItemUI();
    }

    public void OpenInventory()
    {
        if (!isUseInventory)
        {
            isUseInventory = true;
            OnInventoryOpenSound();
            Cursor.lockState = CursorLockMode.None;
            inventoryUI.SetActive(true);
            Time.timeScale = 0;
            _inputManager.canMove = false;
        }
        _inputManager.isOpenInventory = false;
    }

    public void CloseInventory()
    {
        if (isUseInventory)
        {
            inventoryDetailUI.GetComponent<DisplayInventory>().ShowItemDetail(false);
            inventoryUI.SetActive(false);
            isUseInventory = false;

            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
            _inputManager.canMove = true;
        }
        _inputManager.isOpenInventory = false;
    }

    public void InventoryControll()
    {
        if (isUseInventory)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseInventory();
            }
        }
    }

    public bool IsUseInventory()
    {
        return isUseInventory;
    }

    private void OnInventoryOpenSound()
    {
        if (InventoryOpenAudioClip != null)
        {
            SoundManager.Instance.SFXPlay("InventoryOpen", InventoryOpenAudioClip);
        }
    }

    private void UseItem()
    {
        _inputManager.zInteraction = false;
        int itemId = useItemUI.GetComponent<ObjectData>().GetId();
        if (itemId != 0)
        {
            InventorySlot item = inventory.GetPlayerItemById(itemId);
            if (item.amount <= 0) return;

            OnInventoryOpenSound();
            item.amount--;
            useItemUI.transform.GetChild(0).GetChild(4).GetComponentInChildren<TextMeshProUGUI>().text = item.amount.ToString("n0");

            // Item 효과
            player.GetComponent<PlayerStateController>().HealPlayer(3);

            if (item.amount == 0)
            {
                useItemUI.transform.GetChild(0).GetChild(4).GetComponent<Image>().color = new Color32(255, 255, 255, 80);
            }
        }
    }

    public void SetUseItemUI()
    {
        int itemId = useItemUI.GetComponent<ObjectData>().GetId();
        if (itemId != 0)
        {
            InventorySlot item = inventory.GetPlayerItemById(itemId);
            if (currentUseItemAmount == 0 || currentUseItemAmount != item.amount)
            {
                currentUseItemAmount = item.amount;

                useItemUI.transform.GetChild(0).GetChild(4).GetComponentInChildren<TextMeshProUGUI>().text = item.amount.ToString("n0");
                if (item.amount == 0) useItemUI.transform.GetChild(0).GetChild(4).GetComponent<Image>().color = new Color32(255, 255, 255, 80);
                else useItemUI.transform.GetChild(0).GetChild(4).GetComponent<Image>().color = new Color32(255, 255, 255, 255);

                useItemUI.transform.GetChild(0).GetChild(4).GetComponent<Image>().sprite = item.item.ItemImage;
            }
        }
    }
}
