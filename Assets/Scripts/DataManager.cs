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
    [SerializeField] private string sceneName;
    public string SceneName
    {
        get { return sceneName; }
        set { sceneName = value; }
    }

    // Player
    [SerializeField] private int lastPlayerId;
    public int LastPlayerId{
        get { return lastPlayerId; }
        set { lastPlayerId = value; }
    }
    [SerializeField] private Vector3 playerPosition; // Position
    public Vector3 PlayerPosition{
        get { return playerPosition; }
        set { playerPosition = value; }
    }
    [SerializeField] private Quaternion playerRotation; // Rotation
    public Quaternion PlayerRotation{
        get { return playerRotation; }
        set { playerRotation = value; }
    }
    [SerializeField] private List<PlayableCharacter> playerDatabases = new List<PlayableCharacter>(); // Database
    public List<PlayableCharacter> PlayerDatabases{
        get { return playerDatabases; }
        set { playerDatabases = value; }
    }
    [SerializeField] private List<PlayableCharacter> partyPlayers = new List<PlayableCharacter>(); // Party
    public List<PlayableCharacter> PartyPlayers{
        get { return partyPlayers; }
        set { partyPlayers = value; }
    }

    // Item
    [SerializeField] private Inventory inventoryContainer; // Inventory
    public Inventory InventoryContainer{
        get { return inventoryContainer; }
        set { inventoryContainer = value; }
    }
    [SerializeField] private int useItemId = 0;// Z use Item
    public int UseItemId{
        get { return useItemId; }
        set { useItemId = value; }
    }

    // Quest
    [SerializeField] private List<Quest> quests = new List<Quest>();
    public List<Quest> Quests{
        get { return quests; }
        set { quests = value; }
    }
    [SerializeField] private List<int> doneQuest = new List<int>();
    public List<int> DoneQuest{
        get { return doneQuest; }
        set { doneQuest = value; }
    }
    [SerializeField] private List<int> ongoingQuest = new List<int>();
    public List<int> OngoingQuest{
        get { return ongoingQuest; }
        set { ongoingQuest = value; }
    }

    // Dialogue
    [SerializeField] private List<Dialogue> dialogues = new List<Dialogue>();
    public List<Dialogue> Dialogues{
        get { return dialogues; }
        set { dialogues = value; }
    }
    [SerializeField] private Dictionary<int, Dialogue> playerDialogue = new Dictionary<int, Dialogue>();
    public Dictionary<int, Dialogue> PlayerDialogue{
        get { return playerDialogue; }
        set { playerDialogue = value; }
    }

    // Trigger Event
    [SerializeField] private List<int> doneEventIds = new List<int>();
    public List<int> DoneEventIds{
        get { return doneEventIds; }
        set { doneEventIds = value; }
    }

    // Treasure Box
    [SerializeField] private List<int> doneTreasure = new List<int>();
    public List<int> DoneTreasure{
        get { return doneTreasure; }
        set { doneTreasure = value; }
    }

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
        saveObject.SceneName = sceneName;

        // Player Swap
        saveObject.LastPlayerId = activePlayer.GetComponent<ObjectData>().GetId();

        // Player Transform
        saveObject.PlayerPosition = activePlayer.transform.position;
        saveObject.PlayerRotation = activePlayer.transform.rotation;

        // Player Database
        saveObject.PlayerDatabases = PlayerManager.Instance.PlayerDatabase.Players;

        // Party
        // saveObject.partyPlayers = PlayerManager.Instance.playerDatabase.partyPlayers;

        // Player Inventory
        saveObject.InventoryContainer = inventory.Container;
        if (useItemUI.GetComponent<ObjectData>().GetId() != 0) saveObject.UseItemId = useItemUI.GetComponent<ObjectData>().GetId();

        // Quest
        saveObject.Quests = QuestManager.Instance.Database.Quests;
        saveObject.DoneQuest = QuestManager.Instance.QuestInventoryObject.doneQuest;
        saveObject.OngoingQuest = QuestManager.Instance.QuestInventoryObject.ongoingQuest;

        // Dialogue
        saveObject.Dialogues = DialogueManager.Instance.database.dialogues;
        saveObject.PlayerDialogue = DialogueManager.Instance.playerDialogue;

        // Trigger Event
        saveObject.DoneEventIds = EventManager.Instance.doneEventIds;

        // Treasure Box
        saveObject.DoneTreasure = EventManager.Instance.doneTreasure;

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
        if (saveObject.SceneName != null && saveObject.SceneName.Trim() != "") sceneName = saveObject.SceneName;

        SetActivePlayer();

        // Player Swap
        // PlayerManager.Instance._playerSwapController.currentPlayerIndex = saveObject.lastPlayerId - 1;
        // activePlayer.GetComponent<PlayerInputManager>().swap = saveObject.lastPlayerId;
        // PlayerManager.Instance._playerSwapController.PlayerSwap(false);
        // PlayerManager.Instance._playerSwapController.SetPartyUI();

        // Player Tranform
        activePlayer.transform.position = saveObject.PlayerPosition;
        activePlayer.transform.rotation = saveObject.PlayerRotation;

        // Player Database
        // PlayerManager.Instance.playerDatabase.players = saveObject.playerDatabases;
        PlayerManager.Instance.LoadPlayerInformation(saveObject.PlayerDatabases);
        mainCharacter = PlayerManager.Instance.GetMainCharacter();

        // Party
        // PlayerManager.Instance.playerDatabase.partyPlayers = saveObject.partyPlayers;
        // if (saveObject.partyPlayers.Count > 1)
        if (PlayerManager.Instance.GetPartyPlayers().Count > 1)
        {
            isJoinYelan = true;
            activePlayer.GetComponent<PlayerInputManager>().canSwap = true;
        }

        qSkillUI.transform.GetChild(1).GetComponent<Image>().fillAmount = mainCharacter.Q_currentGauge / mainCharacter.Q_maxGauge;
        hpSlider.value = mainCharacter.CurrentHP / mainCharacter.MaxHP;
        hpText.text = mainCharacter.CurrentHP.ToString("n0");

        // Player Inventory
        inventory.Container = saveObject.InventoryContainer;
        if (saveObject.UseItemId != 0) useItemUI.GetComponent<ObjectData>().SetId(saveObject.UseItemId);

        // Weapon
        activePlayer.GetComponent<PlayerWeaponController>().SwapWeapon(mainCharacter.Weapon.Id);

        // Quest
        QuestManager.Instance.Database.Quests = saveObject.Quests;
        QuestManager.Instance.QuestInventoryObject.doneQuest = saveObject.DoneQuest;
        QuestManager.Instance.QuestInventoryObject.ongoingQuest = saveObject.OngoingQuest;
        if (saveObject.OngoingQuest.Count > 0)
        {
            QuestManager.Instance.SetQuest(QuestManager.Instance.GetQuestByQuestId(saveObject.OngoingQuest[0]));
        }

        // Dialogue
        DialogueManager.Instance.database.dialogues = saveObject.Dialogues;
        DialogueManager.Instance.playerDialogue = saveObject.PlayerDialogue;

        // Trigger Event
        EventManager.Instance.doneEventIds = saveObject.DoneEventIds;

        // Treasure Box
        EventManager.Instance.doneTreasure = saveObject.DoneTreasure;
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
