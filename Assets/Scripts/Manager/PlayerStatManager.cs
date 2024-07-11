using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatManager : Singleton<PlayerStatManager>
{
    GameObject player;
    PlayableCharacter mainCharacter;

    PlayerInputManager _inputManager;
    public GameObject statUI;
    public GameObject[] characterUis;

    [Header("Character Swap")]
    public GameObject[] characterButtons;
    public GameObject[] characterModels;

    [Header("Left Button Swap")]
    public GameObject[] leftButtons;
    public GameObject[] centerContents;
    public GameObject[] rightContents;

    [Header("01 - Character Detail Information")]
    public GameObject playerStatDetailContainer;
    Transform nameContainer;
    Transform hpContainer;
    Transform atkContainer;
    Transform defContainer;
    Transform criRateContainer;
    Transform criDmgContainer;

    [Header("02 - Character Weapon Information")]
    public GameObject playerWeaponDetailContainer;
    Transform characterModelContainer;
    Transform weaponContainer;
    Transform weaponNameContainer;
    Transform weaponAtkContainer;
    Transform weaponCriRateContainer;
    Transform weaponCriDmgContainer;

    [Header("03 - Character Skill Information")]
    public GameObject[] skillButtons;
    Transform skillContainer;

    [Header("04 - Character Voice Information")]
    public GameObject[] voiceButtons;

    float blendIdle;
    public bool isUseStat;
    List<int> characterButtonClickList = new List<int>();

    [Header("Audio")]
    public AudioClip InventoryOpenAudioClip;

    void Awake()
    {
        for (int i = 0; i < characterButtons.Length; i++)
        {
            int id = characterButtons[i].GetComponent<ObjectData>().GetId();
            characterButtons[i].GetComponent<Button>().onClick.AddListener(() => OnClickCharacterButton(id));
        }

        for (int i = 0; i < leftButtons.Length; i++)
        {
            int id = leftButtons[i].GetComponent<ObjectData>().GetId();
            leftButtons[i].GetComponent<Button>().onClick.AddListener(() => OnClickLeftButton(id));
        }

        statUI.SetActive(false);
        nameContainer = playerStatDetailContainer.transform.GetChild(0);
        hpContainer = playerStatDetailContainer.transform.GetChild(1);
        atkContainer = playerStatDetailContainer.transform.GetChild(2);
        defContainer = playerStatDetailContainer.transform.GetChild(3);
        criRateContainer = playerStatDetailContainer.transform.GetChild(4);
        criDmgContainer = playerStatDetailContainer.transform.GetChild(5);

        weaponNameContainer = playerWeaponDetailContainer.transform.GetChild(0);
        weaponAtkContainer = playerWeaponDetailContainer.transform.GetChild(1);
        weaponCriRateContainer = playerWeaponDetailContainer.transform.GetChild(2);
        weaponCriDmgContainer = playerWeaponDetailContainer.transform.GetChild(3);

        characterModelContainer = centerContents[0].transform;
        weaponContainer = centerContents[1].transform;
        skillContainer = centerContents[2].transform;

        SetActivePlayer();
        characterButtons[1].SetActive(false);
    }

    void Update()
    {
        if (!player.activeInHierarchy) SetActivePlayer();

        if (_inputManager.isOpenStatInventory)
        {
            SetActivePlayer();
            SetCharacterStat();
            OpenStatInventory();
        }

        InventoryControll();
    }
    public void InventoryControll()
    {
        if (isUseStat)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseInventory();
            }
        }
    }

    public void SetCharacterStat()
    {
        statUI.GetComponent<Image>().color = new Color32(mainCharacter.elementColor[0], mainCharacter.elementColor[1], mainCharacter.elementColor[2], 255);

        // 00 - Character Model
        int characterId = player.GetComponent<ObjectData>().GetId();
        for (int i = 0; i < characterModels.Length; i++)
        {
            if (characterModels[i].GetComponent<ObjectData>().GetId() == characterId)
            {
                characterModels[i].SetActive(true);
            }
            else
            {
                characterModels[i].SetActive(false);
            }
        }

        // 01 - Character Info
        nameContainer.GetChild(0).GetComponent<TextMeshProUGUI>().text = mainCharacter.characterName;
        hpContainer.GetChild(1).GetComponent<TextMeshProUGUI>().text = mainCharacter.maxHP.ToString("n0");

        float atk = mainCharacter.atk;
        if (mainCharacter.weapon != null) atk += mainCharacter.weapon.atkBonus;
        atkContainer.GetChild(1).GetComponent<TextMeshProUGUI>().text = atk.ToString("n0");

        defContainer.GetChild(1).GetComponent<TextMeshProUGUI>().text = mainCharacter.def.ToString("n0");

        float criRate = mainCharacter.criRate * 100f;
        if (mainCharacter.weapon != null) criRate += mainCharacter.weapon.criRateBonus * 100f;
        criRateContainer.GetChild(1).GetComponent<TextMeshProUGUI>().text = criRate.ToString("n0") + "%";

        float criDmg = mainCharacter.criDmg * 100f;
        if (mainCharacter.weapon != null) criDmg += mainCharacter.weapon.criDmgBonus * 100f;
        criDmgContainer.GetChild(1).GetComponent<TextMeshProUGUI>().text = criDmg.ToString("n0") + "%";

        // 02-Weapon
        weaponContainer.GetChild(1).GetComponentInChildren<Image>().sprite = mainCharacter.weapon.itemImage;
        weaponNameContainer.GetChild(0).GetComponent<TextMeshProUGUI>().text = mainCharacter.weapon.itemName;
        weaponAtkContainer.GetChild(1).GetComponent<TextMeshProUGUI>().text = mainCharacter.weapon.atkBonus.ToString("n0");
        weaponCriRateContainer.GetChild(1).GetComponent<TextMeshProUGUI>().text = (mainCharacter.weapon.criRateBonus * 100f).ToString("n0") + "%";
        weaponCriDmgContainer.GetChild(1).GetComponent<TextMeshProUGUI>().text = (mainCharacter.weapon.criDmgBonus * 100f).ToString("n0") + "%";

        // Prevent sound duplicate 
        if (characterButtonClickList.FindIndex(characterId => characterId == mainCharacter.id) != -1) return;

        // 03-Skill
        for (int i = 0; i < skillButtons.Length; i++)
        {
            int buttonId = skillButtons[i].GetComponent<ObjectData>().GetId();
            skillButtons[i].GetComponent<Button>().onClick.AddListener(() => OnClickSkillButton(buttonId, false));
        }

        // 04-Voice
        for (int i = 0; i < voiceButtons.Length; i++)
        {
            int buttonId = voiceButtons[i].GetComponent<ObjectData>().GetId();
            voiceButtons[i].GetComponent<Button>().onClick.AddListener(() => OnClickVoiceButton(buttonId));
        }
        characterButtonClickList.Add(mainCharacter.id);
    }

    // Todo- 클릭하면 sound 중복됨 
    public void OnClickCharacterButton(int characterId)
    {
        OnInventoryOpenSound();
        mainCharacter = PlayerManager.Instance.GetPlayerById(characterId);
        statUI.GetComponent<Image>().color = new Color32(mainCharacter.elementColor[0], mainCharacter.elementColor[1], mainCharacter.elementColor[2], 255);

        // 00 - Character Model
        for (int i = 0; i < characterModels.Length; i++)
        {
            if (characterModels[i].GetComponent<ObjectData>().GetId() == characterId)
            {
                characterModels[i].SetActive(true);
            }
            else
            {
                characterModels[i].SetActive(false);
            }
        }

        // 01-Info
        nameContainer.GetChild(0).GetComponent<TextMeshProUGUI>().text = mainCharacter.characterName;
        hpContainer.GetChild(1).GetComponent<TextMeshProUGUI>().text = mainCharacter.maxHP.ToString("n0");

        float atk = mainCharacter.atk;
        if (mainCharacter.weapon != null) atk += mainCharacter.weapon.atkBonus;
        atkContainer.GetChild(1).GetComponent<TextMeshProUGUI>().text = atk.ToString("n0");

        defContainer.GetChild(1).GetComponent<TextMeshProUGUI>().text = mainCharacter.def.ToString("n0");

        float criRate = mainCharacter.criRate * 100f;
        if (mainCharacter.weapon != null) criRate += mainCharacter.weapon.criRateBonus * 100f;
        criRateContainer.GetChild(1).GetComponent<TextMeshProUGUI>().text = criRate.ToString("n0") + "%";

        float criDmg = mainCharacter.criDmg * 100f;
        if (mainCharacter.weapon != null) criDmg += mainCharacter.weapon.criDmgBonus * 100f;
        criDmgContainer.GetChild(1).GetComponent<TextMeshProUGUI>().text = criDmg.ToString("n0") + "%";

        // 02-Weapon       
        weaponContainer.GetChild(1).GetComponentInChildren<Image>().sprite = mainCharacter.weapon.itemImage;
        weaponNameContainer.GetChild(0).GetComponent<TextMeshProUGUI>().text = mainCharacter.weapon.itemName;
        weaponAtkContainer.GetChild(1).GetComponent<TextMeshProUGUI>().text = mainCharacter.weapon.atkBonus.ToString("n0");
        weaponCriRateContainer.GetChild(1).GetComponent<TextMeshProUGUI>().text = (mainCharacter.weapon.criRateBonus * 100f).ToString("n0") + "%";
        weaponCriDmgContainer.GetChild(1).GetComponent<TextMeshProUGUI>().text = (mainCharacter.weapon.criDmgBonus * 100f).ToString("n0") + "%";

        OnClickSkillButton(1, false);

        // Prevent sound duplicate 
        if (characterButtonClickList.FindIndex(characterId => characterId == mainCharacter.id) != -1) return;
        
        // 03-Skill
        for (int i = 0; i < skillButtons.Length; i++)
        {
            int buttonId = skillButtons[i].GetComponent<ObjectData>().GetId();
            skillButtons[i].GetComponent<Button>().onClick.AddListener(() => OnClickSkillButton(buttonId, false));
        }

        // 04-Voice
        for (int i = 0; i < voiceButtons.Length; i++)
        {
            int buttonId = voiceButtons[i].GetComponent<ObjectData>().GetId();
            voiceButtons[i].GetComponent<Button>().onClick.AddListener(() => OnClickVoiceButton(buttonId));
        }
        characterButtonClickList.Add(mainCharacter.id);
    }

    public void OnClickLeftButton(int buttonId)
    {
        OnInventoryOpenSound();
        for (int i = 0; i < rightContents.Length; i++)
        {
            if (rightContents[i].GetComponent<ObjectData>().GetId() == buttonId)
            {
                rightContents[i].SetActive(true);
            }
            else
            {
                rightContents[i].SetActive(false);
            }
        }

        for (int i = 0; i < centerContents.Length; i++)
        {
            if (i == buttonId - 1) centerContents[i].SetActive(true);
            else
            {
                centerContents[i].SetActive(false);
                centerContents[i].SetActive(false);
                centerContents[i].SetActive(false);
            }

            if (i == 2) OnClickSkillButton(1, false);
        }

        // todo
        if (buttonId == 4)
        {
            centerContents[0].SetActive(true);
            centerContents[1].SetActive(false);
            centerContents[2].SetActive(false);
        }
    }

    public void OnClickSkillButton(int buttonId, bool isSoundOn)
    {
        if (isSoundOn) OnInventoryOpenSound();
        if (buttonId == 1)
        {
            skillContainer.GetChild(0).GetComponent<Image>().sprite = mainCharacter.e_image;
            skillContainer.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = mainCharacter.e_desctiption;
            skillContainer.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = mainCharacter.e_cooltime.ToString("n0") + "초";
            skillContainer.GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>().text = mainCharacter.e_gauge.ToString("n0");
            skillContainer.GetChild(3).gameObject.SetActive(true);
            skillContainer.GetChild(4).gameObject.SetActive(false);
            skillContainer.GetChild(5).GetChild(1).GetComponent<TextMeshProUGUI>().text = mainCharacter.e_damage.ToString("n0");
        }
        else if (buttonId == 2)
        {
            skillContainer.GetChild(0).GetComponent<Image>().sprite = mainCharacter.q_image;
            skillContainer.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = mainCharacter.q_desctiption;
            skillContainer.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = mainCharacter.q_cooltime.ToString("n0") + "초";
            skillContainer.GetChild(4).GetChild(1).GetComponent<TextMeshProUGUI>().text = mainCharacter.q_maxGauge.ToString("n0");
            skillContainer.GetChild(3).gameObject.SetActive(false);
            skillContainer.GetChild(4).gameObject.SetActive(true);
            skillContainer.GetChild(5).GetChild(1).GetComponent<TextMeshProUGUI>().text = mainCharacter.q_damage.ToString("n0");
        }
    }

    public void OnClickVoiceButton(int buttonId)
    {
        OnInventoryOpenSound();
        if (buttonId < 4) SoundManager.Instance.SFXPlay(mainCharacter.e_audios[buttonId - 1].name, mainCharacter.e_audios[buttonId - 1]);
        else SoundManager.Instance.SFXPlay(mainCharacter.q_audios[buttonId - 4].name, mainCharacter.q_audios[buttonId - 4]);
    }

    // Open & Close Controller
    public void OpenStatInventory()
    {
        if (!isUseStat)
        {
            ShowCharacterUIs(false);
            isUseStat = true;
            OnInventoryOpenSound();
            Cursor.lockState = CursorLockMode.None;
            statUI.SetActive(true);
            Time.timeScale = 0;
            _inputManager.canMove = false;
        }
        _inputManager.isOpenStatInventory = false;
    }

    public void CloseInventory()
    {
        if (isUseStat)
        {
            ShowCharacterUIs(true);
            statUI.SetActive(false);
            isUseStat = false;

            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
            _inputManager.canMove = true;
        }
        _inputManager.isOpenStatInventory = false;
    }

    private void PlayerIdleBlend()
    {
        if (blendIdle > 0)
        {
            blendIdle = Mathf.Lerp(blendIdle, 1f, Time.deltaTime);
        }
        else
        {
            blendIdle = Mathf.Lerp(blendIdle, 0f, Time.deltaTime);
        }
        player.GetComponent<Animator>().SetFloat("Blend_Idle", blendIdle, 0.05f, Time.deltaTime);
    }

    public bool IsUseStat()
    {
        return isUseStat;
    }

    void SetActivePlayer()
    {
        player = PlayerManager.Instance.GetActivePlayer();
        mainCharacter = PlayerManager.Instance.GetPlayerById(player.GetComponent<ObjectData>().GetId());
        _inputManager = player.GetComponent<PlayerInputManager>();
    }

    private void OnInventoryOpenSound()
    {
        if (InventoryOpenAudioClip != null)
        {
            SoundManager.Instance.SFXPlay("InventoryOpen", InventoryOpenAudioClip);
        }
    }

    private void ShowCharacterUIs(bool isShow)
    {
        for (int i = 0; i < characterUis.Length; i++)
        {
            characterUis[i].SetActive(isShow);
        }
    }

    public void SetYelanJoin()
    {
        characterButtons[1].SetActive(true);
    }
}
