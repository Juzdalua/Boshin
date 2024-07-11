using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveObject
{
    // Scene
    public string sceneName;

    // Player
    public int lastPlayerId;
    public Vector3 playerPosition; // Position
    public Quaternion playerRotation; // Rotation
    public List<PlayableCharacter> playerDatabases = new List<PlayableCharacter>(); // Database
    public List<PlayableCharacter> partyPlayers = new List<PlayableCharacter>(); // Party

    // Item
    public Inventory Container; // Inventory
    public int useItemId = 0;// Z use Item

    // Quest
    public List<Quest> quests = new List<Quest>();
    public List<int> doneQuest = new List<int>();
    public List<int> ongoingQuest = new List<int>();

    // Dialogue
    public List<Dialogue> dialogues = new List<Dialogue>();
    public Dictionary<int, Dialogue> playerDialogue = new Dictionary<int, Dialogue>();

    // Trigger Event
    public List<int> doneEventIds = new List<int>();

    // Treasure Box
    public List<int> doneTreasure = new List<int>();

    // Party
}

public class DataManager : Singleton<DataManager>
{
    [Header("Player Information")]
    GameObject activePlayer;
    PlayableCharacter mainCharacter;
    int playerId;
    public bool isJoinYelan = false;

    [Header("Active Player UI")]
    public Slider hpSlider;
    public TextMeshProUGUI hpText;
    public GameObject qSkillUI;

    [Header("Item")]
    public InventoryObject inventory;
    public GameObject useItemUI;

    [Header("Scene")]
    public string sceneName;

    [Header("Save Information")]
    SaveObject saveObject = new SaveObject();

    void Awake()
    {
        Load();
    }

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.F4) || Input.GetKeyDown(KeyCode.Escape))
        if (Input.GetKeyDown(KeyCode.P))
        {
            Save();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Load();
        }
    }

    public void Save()
    {
        saveObject = GetPlayerInformation(saveObject);

        string saveData = JsonUtility.ToJson(saveObject);
        File.WriteAllText($"{Application.persistentDataPath}/save", saveData);
    }

    SaveObject GetPlayerInformation(SaveObject saveObject)
    {
        SetActivePlayer();

        // Scene
        saveObject.sceneName = sceneName;

        // Player Swap
        saveObject.lastPlayerId = activePlayer.GetComponent<ObjectData>().GetId();

        // Player Transform
        saveObject.playerPosition = activePlayer.transform.position;
        saveObject.playerRotation = activePlayer.transform.rotation;

        // Player Database
        saveObject.playerDatabases = PlayerManager.Instance.playerDatabase.players;

        // Party
        // saveObject.partyPlayers = PlayerManager.Instance.playerDatabase.partyPlayers;

        // Player Inventory
        saveObject.Container = inventory.Container;
        if (useItemUI.GetComponent<ObjectData>().GetId() != 0) saveObject.useItemId = useItemUI.GetComponent<ObjectData>().GetId();

        // Quest
        saveObject.quests = QuestManager.Instance.database.quests;
        saveObject.doneQuest = QuestManager.Instance.questInventoryObject.doneQuest;
        saveObject.ongoingQuest = QuestManager.Instance.questInventoryObject.ongoingQuest;

        // Dialogue
        saveObject.dialogues = DialogueManager.Instance.database.dialogues;
        saveObject.playerDialogue = DialogueManager.Instance.playerDialogue;

        // Trigger Event
        saveObject.doneEventIds = EventManager.Instance.doneEventIds;

        // Treasure Box
        saveObject.doneTreasure = EventManager.Instance.doneTreasure;

        return saveObject;
    }

    public void Load()
    {
        if (File.Exists($"{Application.persistentDataPath}/save"))
        {
            string saveData = File.ReadAllText($"{Application.persistentDataPath}/save");
            saveObject = JsonUtility.FromJson<SaveObject>(saveData);

            SetPlayerInformation();
        }
    }

    void SetPlayerInformation()
    {
        // Scene
        if (saveObject.sceneName != null && saveObject.sceneName.Trim() != "") sceneName = saveObject.sceneName;

        SetActivePlayer();

        // Player Swap
        // PlayerManager.Instance._playerSwapController.currentPlayerIndex = saveObject.lastPlayerId - 1;
        // activePlayer.GetComponent<PlayerInputManager>().swap = saveObject.lastPlayerId;
        // PlayerManager.Instance._playerSwapController.PlayerSwap(false);
        // PlayerManager.Instance._playerSwapController.SetPartyUI();

        // Player Tranform
        activePlayer.transform.position = saveObject.playerPosition;
        activePlayer.transform.rotation = saveObject.playerRotation;

        // Player Database
        // PlayerManager.Instance.playerDatabase.players = saveObject.playerDatabases;
        PlayerManager.Instance.LoadPlayerInformation(saveObject.playerDatabases);
        mainCharacter = PlayerManager.Instance.GetMainCharacter();

        // Party
        // PlayerManager.Instance.playerDatabase.partyPlayers = saveObject.partyPlayers;
        // if (saveObject.partyPlayers.Count > 1)
        if (PlayerManager.Instance.GetPartyPlayers().Count > 1)
        {
            isJoinYelan = true;
            activePlayer.GetComponent<PlayerInputManager>().canSwap = true;
        }

        qSkillUI.transform.GetChild(1).GetComponent<Image>().fillAmount = mainCharacter.q_currentGauge / mainCharacter.q_maxGauge;
        hpSlider.value = mainCharacter.currentHP / mainCharacter.maxHP;
        hpText.text = mainCharacter.currentHP.ToString("n0");

        // Player Inventory
        inventory.Container = saveObject.Container;
        if (saveObject.useItemId != 0) useItemUI.GetComponent<ObjectData>().SetId(saveObject.useItemId);

        // Weapon
        activePlayer.GetComponent<PlayerWeaponController>().SwapWeapon(mainCharacter.weapon.id);

        // Quest
        QuestManager.Instance.database.quests = saveObject.quests;
        QuestManager.Instance.questInventoryObject.doneQuest = saveObject.doneQuest;
        QuestManager.Instance.questInventoryObject.ongoingQuest = saveObject.ongoingQuest;
        if (saveObject.ongoingQuest.Count > 0)
        {
            QuestManager.Instance.SetQuest(QuestManager.Instance.GetQuestByQuestId(saveObject.ongoingQuest[0]));
        }

        // Dialogue
        DialogueManager.Instance.database.dialogues = saveObject.dialogues;
        DialogueManager.Instance.playerDialogue = saveObject.playerDialogue;

        // Trigger Event
        EventManager.Instance.doneEventIds = saveObject.doneEventIds;

        // Treasure Box
        EventManager.Instance.doneTreasure = saveObject.doneTreasure;
    }

    public void NewGame()
    {
        if (File.Exists($"{Application.persistentDataPath}/save")) File.Delete($"{Application.persistentDataPath}/save");
    }

    public string GetSceneName()
    {
        return sceneName;
    }

    void OnApplicationQuit()
    {
        // Save();
    }

    void SetActivePlayer()
    {
        activePlayer = PlayerManager.Instance.GetActivePlayer();
        mainCharacter = PlayerManager.Instance.GetMainCharacter();
        playerId = activePlayer.GetComponent<ObjectData>().GetId();
    }
}
