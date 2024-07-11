using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayInventory : MonoBehaviour
{
    public InventoryObject inventory;
    public GameObject inventoryPrefab;
    public GameObject itemDetail;
    public GameObject addUseItemUI;
    public GameObject equipItemUI;
    public GameObject characterUseItemUI;
    Transform useItemAmountUI;
    PlayerWeaponController _weaponController;

    public int X_START = -500;
    public int Y_START = 390;
    public int X_PADDING = 220;
    public int Y_PADDING = 220;
    public int NUMBER_OF_COLUMN = 5;
    Dictionary<InventorySlot, GameObject> itemDisplayed = new Dictionary<InventorySlot, GameObject>();

    void Start()
    {
        // CreateDisplay();
        ShowItemDetail(false);
        useItemAmountUI = characterUseItemUI.transform.GetChild(0).GetChild(4).GetChild(0);
        useItemAmountUI.gameObject.SetActive(false);
        addUseItemUI.SetActive(false);
        equipItemUI.SetActive(false);
    }

    void Update()
    {
        UpdateDisplay();
    }

    public Vector3 GetPosition(int order)
    {
        return new Vector3(X_START + (X_PADDING * (order % NUMBER_OF_COLUMN)), Y_START + (-Y_PADDING * (order / NUMBER_OF_COLUMN)), 0);
    }

    // public void CreateDisplay()
    // {
    //     for (int i = 0; i < inventory.Container.Items.Count; i++)
    //     {
    //         InventorySlot slot = inventory.Container.Items[i];

    //         var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);
    //         obj.transform.GetChild(0).GetComponentInChildren<Image>().sprite = inventory.GetItemById(slot.item.id).itemImage;
    //         obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
    //         obj.GetComponentInChildren<TextMeshProUGUI>().text = slot.amount.ToString("n0");
    //     }
    // }

    public void UpdateDisplay()
    {
        for (int i = 0; i < inventory.Container.Items.Count; i++)
        {
            InventorySlot slot = inventory.Container.Items[i];

            if (itemDisplayed.ContainsKey(slot))
            {
                itemDisplayed[slot].GetComponentInChildren<TextMeshProUGUI>().text = slot.amount.ToString("n0");
            }
            else
            {
                GameObject obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);
                obj.transform.GetChild(0).GetComponentInChildren<Image>().sprite = inventory.GetItemById(slot.item.id).itemImage;
                obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = slot.amount.ToString("n0");

                obj.GetComponent<ObjectData>().SetId(slot.id);
                obj.GetComponent<ObjectData>().SetObjectType(ObjectType.Item);

                obj.GetComponent<Button>().onClick.AddListener(() => ClickItemDetail(obj));

                itemDisplayed.Add(slot, obj);
            }
        }
    }

    public void ShowItemDetail(bool isShow)
    {
        itemDetail.SetActive(isShow);
    }

    public void ClickItemDetail(GameObject itemObject)
    {
        int itemId = itemObject.GetComponent<ObjectData>().GetId();
        var item = inventory.GetItemById(itemId);

        itemDetail.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = item.itemImage;
        itemDetail.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.desctiption;

        if (item.canUse)
        {
            addUseItemUI.SetActive(true);
            addUseItemUI.GetComponent<Button>().onClick.AddListener(() => AddUseItemUI(itemId));
            equipItemUI.SetActive(false);
        }
        else if (item.canEquip)
        {
            equipItemUI.SetActive(true);
            addUseItemUI.SetActive(false);

            _weaponController = PlayerManager.Instance.GetActivePlayer().GetComponent<PlayerWeaponController>();
            if (item.id == _weaponController.GetWeaponInfo().id)
            {
                equipItemUI.GetComponentInChildren<TextMeshProUGUI>().text = "장착중";
            }
            else
            {
                equipItemUI.GetComponentInChildren<TextMeshProUGUI>().text = "장착";
                equipItemUI.GetComponent<Button>().onClick.AddListener(() => EquipItemUI(itemId));
            }

        }
        else
        {
            addUseItemUI.SetActive(false);
            equipItemUI.SetActive(false);
        }

        ShowItemDetail(true);
    }

    private void AddUseItemUI(int itemId)
    {
        Item item = inventory.GetItemById(itemId);
        InventorySlot playerItem = inventory.GetPlayerItemById(itemId);
        if (playerItem.amount > 0)
        {
            characterUseItemUI.GetComponent<ObjectData>().SetId(itemId);

            characterUseItemUI.transform.GetChild(0).GetChild(4).GetComponent<Image>().sprite = item.itemImage;
            characterUseItemUI.transform.GetChild(0).GetChild(4).GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            useItemAmountUI.gameObject.SetActive(true);
            characterUseItemUI.transform.GetChild(0).GetChild(4).GetComponentInChildren<TextMeshProUGUI>().text = playerItem.amount.ToString("n0");
        }
    }

    private void EquipItemUI(int itemId)
    {
        Item item = inventory.GetItemById(itemId);
        if (item == null) return;

        ItemWeaponObject activeWeapon = _weaponController.GetWeaponInfo();
        if (itemId == activeWeapon.id) return;

        InventorySlot playerItem = inventory.GetPlayerItemById(itemId);
        if (playerItem.amount <= 0) return;

        _weaponController.SwapWeapon(itemId);
    }
}