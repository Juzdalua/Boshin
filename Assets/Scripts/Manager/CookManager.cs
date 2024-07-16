using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CookManager : Singleton<CookManager>
{
    GameObject player;
    PlayerInputManager _inputManager;
    GameObject cookUI;
    Button makingButton;
    bool isUseCook = false;
    int hasRecipeCount = 0;

    [SerializeField] private Transform cookScreenUI;
    [SerializeField] private GameObject cookDetailUI;
    [SerializeField] private List<ItemFoodObject> cookList = new List<ItemFoodObject>();
    [SerializeField] private InventoryObject playerInventory;
    [SerializeField] private InventoryController _inventory;
    [SerializeField] private GameObject successUI;

    [Header("Audio")]
    [SerializeField] private AudioClip CookOpenAudioClip;
    [SerializeField] private AudioClip CookSuccessAudioClip;

    [Header("Display")]
    [SerializeField] private GameObject cookPrefab;
    [SerializeField] private GameObject cookRecipePrefab;
    [SerializeField] private int X_START = -500;
    [SerializeField] private int Y_START = 390;
    [SerializeField] private int X_PADDING = 220;
    [SerializeField] private int Y_PADDING = 220;
    [SerializeField] private int NUMBER_OF_COLUMN = 5;
    Dictionary<int, GameObject> cookDisplayed = new Dictionary<int, GameObject>();
    List<GameObject> cookDetailDisplayed = new List<GameObject>();

    void Start()
    {
        player = PlayerManager.Instance.GetActivePlayer();
        _inputManager = player.GetComponent<PlayerInputManager>();
        cookUI = transform.GetChild(0).gameObject;
        makingButton = cookDetailUI.transform.GetChild(2).GetComponent<Button>();

        cookUI.SetActive(false);
        makingButton.gameObject.SetActive(false);
        successUI.SetActive(false);

        for (int i = 0; i < cookList.Count; i++)
        {
            if (!cookList[i].CanCook)
            {
                cookList.RemoveAt(i);
            }
        }
    }

    void Update()
    {
        if (!player.activeInHierarchy)
        {
            player = PlayerManager.Instance.GetActivePlayer();
            _inputManager = player.GetComponent<PlayerInputManager>();
        }

        CookControll();
        if (isUseCook)
        {
            UpdateDisplay();
        }

        if (successUI.activeInHierarchy && Input.GetKeyDown(KeyCode.Escape))
        {
            successUI.SetActive(false);
        }
    }

    public Vector3 GetPosition(int order)
    {
        return new Vector3(X_START + (X_PADDING * (order % NUMBER_OF_COLUMN)), Y_START + (-Y_PADDING * (order / NUMBER_OF_COLUMN)), 0);
    }

    public void UpdateDisplay()
    {
        for (int i = 0; i < cookList.Count; i++)
        {
            int itemId = cookList[i].Id;
            if (!cookDisplayed.ContainsKey(cookList[i].Id))
            {
                GameObject obj = Instantiate(cookPrefab, Vector3.zero, Quaternion.identity, cookScreenUI);
                obj.transform.GetChild(0).GetComponentInChildren<Image>().sprite = cookList[i].ItemImage;
                obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = cookList[i].ItemName.Length <= 4 ? cookList[i].ItemName : cookList[i].ItemName.Substring(0, 4) + "...";

                obj.GetComponent<ObjectData>().SetId(cookList[i].Id);
                obj.GetComponent<ObjectData>().SetObjectType(ObjectType.Item);
                obj.GetComponent<Button>().onClick.AddListener(() => ClickCookDetail(itemId));

                cookDisplayed.Add(cookList[i].Id, obj);

            }
        }
    }

    public void ShowCookDetail(bool isShow)
    {
        cookDetailUI.SetActive(isShow);
    }

    private ItemFoodObject GetFoodObjectByItemId(int itemId)
    {
        return cookList.Find(ele => ele.Id == itemId);
    }

    public void ClickCookDetail(int itemId)
    {
        ItemFoodObject foodObjcet = GetFoodObjectByItemId(itemId);
        if (foodObjcet != null)
        {
            cookDetailUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = foodObjcet.Desctiption;
            ShowCookDetail(true);

            makingButton.onClick.AddListener(() => MakeFood(itemId));

            // recipe create
            if (foodObjcet.Recipes.Count > 0 && cookDetailDisplayed.Count == 0)
            {
                for (int i = 0; i < foodObjcet.Recipes.Count; i++)
                {
                    GameObject obj = Instantiate(cookRecipePrefab, Vector3.zero, Quaternion.identity, cookDetailUI.transform.GetChild(1).GetChild(1));
                    obj.transform.GetChild(0).GetComponent<Image>().sprite = foodObjcet.Recipes[i].ItemImage;
                    obj.GetComponentInChildren<TextMeshProUGUI>().text = foodObjcet.RecipeAmount[i].ToString("n0");
                    cookDetailDisplayed.Add(obj);
                }
            }

            // recipe check
            if (foodObjcet.Recipes.Count > 0)
            {
                if (CheckRecipe(GetFoodObjectByItemId(itemId)))
                {
                    makingButton.gameObject.SetActive(true);
                }
                hasRecipeCount = 0;
            }
        }
    }

    private bool CheckRecipe(ItemFoodObject foodObjcet)
    {
        for (int i = 0; i < foodObjcet.Recipes.Count; i++)
        {
            if (playerInventory.Container.Items.FindIndex(ele => ele.id == foodObjcet.Recipes[i].Id) != -1)
            {
                //check amount
                var recipe = playerInventory.Container.Items.Find(ele => ele.id == foodObjcet.Recipes[i].Id);
                if (recipe.amount >= foodObjcet.RecipeAmount[i])
                {
                    hasRecipeCount++;
                }
            }
        }

        if (hasRecipeCount == foodObjcet.Recipes.Count)
        {
            hasRecipeCount = 0;
            return true;
        }
        return false;
    }

    public void MakeFood(int itemId)
    {
        ItemFoodObject foodObjcet = GetFoodObjectByItemId(itemId);
        if (foodObjcet != null)
        {
            for (int i = 0; i < foodObjcet.Recipes.Count; i++)
            {
                var recipe = playerInventory.Container.Items.Find(ele => ele.id == foodObjcet.Recipes[i].Id);
                recipe.amount -= foodObjcet.RecipeAmount[i]; // Error todo
                if (recipe.amount == 0) playerInventory.Container.Items.Remove(recipe);
            }

            if(CookSuccessAudioClip != null) SoundManager.Instance.SFXPlay(CookSuccessAudioClip.name, CookSuccessAudioClip);
            Item newItem = new Item(foodObjcet);
            playerInventory.AddItem(newItem, 1);
            successUI.transform.GetChild(0).GetComponent<Image>().sprite = foodObjcet.ItemImage;
            successUI.SetActive(true);
        }
    }

    // Cook Display
    public void OpenCook()
    {
        if (!isUseCook)
        {
            cookUI.SetActive(true);
            isUseCook = true;
            OnCookOpenSound();

            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            _inputManager.canMove = false;
        }
        _inputManager.isOpenInventory = false;
    }

    public void CloseCook()
    {
        if (isUseCook)
        {
            // cookDetailUI.GetComponent<DisplayInventory>().ShowItemDetail(false);
            cookUI.SetActive(false);
            ShowCookDetail(false);
            isUseCook = false;
            hasRecipeCount = 0;
            makingButton.gameObject.SetActive(false);
            successUI.SetActive(false);

            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
            _inputManager.canMove = true;
        }
        _inputManager.isOpenInventory = false;
    }

    public void CookControll()
    {
        if (isUseCook)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseCook();
            }
        }
    }

    public bool IsUseCook()
    {
        return isUseCook;
    }

    private void OnCookOpenSound()
    {
        if (CookOpenAudioClip != null)
        {
            SoundManager.Instance.SFXPlay("InventoryOpen", CookOpenAudioClip);
        }
    }
}
