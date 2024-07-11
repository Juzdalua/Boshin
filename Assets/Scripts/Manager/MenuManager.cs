using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : Singleton<MenuManager>
{
    GameObject player;
    PlayerInputManager _inputManager;
    GameObject menuUI;

    [Header("Left Menu Buttons")]
    public GameObject[] menuButtons;

    [Header("Right Menu Contents")]
    public GameObject settingButton;
    Slider bgmSettingSlider;
    Slider sfxSettingSlider;
    TextMeshProUGUI bgmSettingValue;
    TextMeshProUGUI sfxSettingValue;
    float maxSoundVolume = 3f;
    public GameObject keysButton;

    [Header("Data")]
    public InventoryObject inventory;
    public ItemDatabaseObject itemDatabase;

    private bool isOpenMenu = false;

    [Header("Audio")]
    public AudioClip MenuOpenAudioClip;

    void Start()
    {
        player = PlayerManager.Instance.GetActivePlayer();
        _inputManager = player.GetComponent<PlayerInputManager>();
        menuUI = transform.GetChild(0).gameObject;

        menuUI.SetActive(false);
        settingButton.SetActive(false);
        keysButton.SetActive(false);

        SoundSettingStart();
    }

    void Update()
    {
        if (!player.activeInHierarchy)
        {
            player = PlayerManager.Instance.GetActivePlayer();
            _inputManager = player.GetComponent<PlayerInputManager>();
        }

        if (_inputManager.isOpenMenu) OpenInventory();

        SoundSetting();
    }

    public void OpenInventory()
    {
        if (isOpenMenu)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) CloseInventory();
            return;
        }

        if (!isOpenMenu)
        {
            isOpenMenu = true;
            OnInventoryOpenSound();
            Cursor.lockState = CursorLockMode.None;
            menuUI.SetActive(true);
            Time.timeScale = 0;
            _inputManager.canMove = false;
        }
        _inputManager.isOpenMenu = false;
    }

    public void CloseInventory()
    {
        if (isOpenMenu)
        {
            CloseRightMenus();
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
            menuUI.SetActive(false);
            isOpenMenu = false;
            _inputManager.canMove = true;
        }
        _inputManager.isOpenMenu = false;
    }

    public void InventoryControll()
    {
        if (isOpenMenu)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseInventory();
            }
        }
    }

    public bool IsUseInventory()
    {
        return isOpenMenu;
    }

    private void OnInventoryOpenSound()
    {
        if (MenuOpenAudioClip != null)
        {
            SoundManager.Instance.SFXPlay("MenuOpen", MenuOpenAudioClip);
        }
    }

    public void OnButtonHover(int buttonId)
    {
        var button = menuButtons[buttonId - 1];
        button.GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.Underline;
    }

    public void OnButtonHoverOut(int buttonId)
    {
        var button = menuButtons[buttonId - 1];
        button.GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.Normal;
    }

    public void OnClickPlayerDeadButton()
    {
        ResetGameData();

        Destroy(DialogueManager.Instance.gameObject);
        Destroy(GameManager.Instance.gameObject);
        Destroy(CookManager.Instance.gameObject);
        Destroy(EventManager.Instance.gameObject);
        Destroy(SoundManager.Instance.gameObject);
        SceneManager.LoadScene("01BoshinMainMenu");
        Destroy(gameObject);
        StopAllCoroutines();
    }

    public void OnClickButton(int buttonId)
    {
        // New Game
        if (buttonId == 1)
        {
            ResetGameData();

            CloseRightMenus();
            DataManager.Instance.NewGame();
            Destroy(DialogueManager.Instance.gameObject);
            Destroy(GameManager.Instance.gameObject);
            Destroy(CookManager.Instance.gameObject);
            Destroy(EventManager.Instance.gameObject);
            Destroy(SoundManager.Instance.gameObject);
            SceneManager.LoadScene("01BoshinMainMenu");

            Destroy(gameObject);
            Time.timeScale = 1;
            StopAllCoroutines();
        }

        // Setting
        else if (buttonId == 2)
        {
            settingButton.SetActive(true);
            keysButton.SetActive(false);
        }

        // Key info
        else if (buttonId == 3)
        {
            settingButton.SetActive(false);
            keysButton.SetActive(true);
        }

        // Save & Exit
        else if (buttonId == 4)
        {
            CloseRightMenus();
            DataManager.Instance.Save();
            Application.Quit();
        }
    }

    public void ResetGameData()
    {
        PlayerManager.Instance.SetPlayerManager();
        PlayerManager.Instance.playerDatabase.SetPlayerDatabase();
        DialogueManager.Instance.SetDialogueManager();
        DialogueManager.Instance.database.SetDialogueDatabase();
        QuestManager.Instance.questInventoryObject.SetQuestInventory();
        QuestManager.Instance.database.SetQuestDatabase();
        inventory.SetItemInventory();
        itemDatabase.SetItemDatabase();
        EventManager.Instance.SetEventManager();
        QuestManager.Instance.questInventoryObject.doneQuest.Clear();
        QuestManager.Instance.questInventoryObject.ongoingQuest.Clear();
    }

    void CloseRightMenus()
    {
        settingButton.SetActive(false);
        keysButton.SetActive(false);
    }

    void SoundSettingStart()
    {
        bgmSettingSlider = settingButton.transform.GetChild(0).GetComponentInChildren<Slider>();
        sfxSettingSlider = settingButton.transform.GetChild(1).GetComponentInChildren<Slider>();
        bgmSettingValue = settingButton.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
        sfxSettingValue = settingButton.transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>();

        SoundManager.Instance.bgmVolume = 1f;
        SoundManager.Instance.sfxVolume = 1f;

        bgmSettingSlider.value = SoundManager.Instance.bgmVolume;
        sfxSettingSlider.value = SoundManager.Instance.sfxVolume;
        bgmSettingValue.text = bgmSettingSlider.value.ToString("n0");
        sfxSettingValue.text = sfxSettingSlider.value.ToString("n0");
    }

    void SoundSetting()
    {
        bgmSettingSlider.value = (int)bgmSettingSlider.value;
        sfxSettingSlider.value = (int)sfxSettingSlider.value;

        bgmSettingValue.text = bgmSettingSlider.value.ToString("n0");
        sfxSettingValue.text = sfxSettingSlider.value.ToString("n0");

        SoundManager.Instance.bgmVolume = bgmSettingSlider.value / maxSoundVolume;
        SoundManager.Instance.sfxVolume = sfxSettingSlider.value / maxSoundVolume;
    }
}
