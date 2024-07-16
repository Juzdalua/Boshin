using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerStateController : MonoBehaviour
{
    GameObject player;
    PlayableCharacter mainCharacter;
    Animator _animator;
    PlayerInput _input;
    PlayerInputManager _inputManager;
    PlayerMovement _movement;
    Camera mainCam;
    Transform playerTransform;

    [Header("UI")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private GameObject hpText;
    [SerializeField] private GameObject damageText;
    float damage;
    [SerializeField] private GameObject staminaObject;
    private Image staminaImage;

    [Header("Stamina")]
    private float maxStamina = 30;
    private float currentStamina;
    [SerializeField] private float staminaUsePer = 3f;
    [SerializeField] private float staminaChargePer = 2f;
    [SerializeField] private bool isSprinting = false;
    public bool IsSprinting
    {
        get { return isSprinting; }
        set { isSprinting = value; }
    }
    [SerializeField] private int staminaFollowSpeed;
    private Transform staminaPos;
    private Vector3 staminaVector;
    bool isSetup = false;

    [Header("Audio")]
    [SerializeField] private AudioClip PlayerHitByMonsterAudioClip;
    [SerializeField] private AudioClip PlayerDeadAudioClip;
    [Range(0, 1)][SerializeField] private float AudioVolume = 1f;

    public enum StaminaState
    {
        Dash,
        Jump
    }

    public enum StaminaValue
    {
        sprint = 0,
        jump = 1,
        dash = 2
    }

    void Start()
    {
        SetActivePlayerComponents();

        mainCam = Camera.main;

        staminaPos = staminaObject.transform;
        staminaVector = new Vector3(0.2f, 1.1f, 0);
        currentStamina = maxStamina;
        staminaImage = staminaObject.transform.GetChild(1).GetComponent<Image>();
        ShowStamina(false);

        if (!isSetup) SetupPlayer();
    }

    void Update()
    {
        if (currentStamina <= maxStamina - 0.01)
        {
            currentStamina += staminaChargePer * Time.deltaTime;
            UpdateStamina(1);

            if (currentStamina >= maxStamina)
            {
                currentStamina = maxStamina;
                ShowStamina(false);
            }
        }
        // MoveStaminaBar();
    }

    void SetupPlayer()
    {
        player = PlayerManager.Instance.GetActivePlayer();
        mainCharacter = PlayerManager.Instance.GetPlayerById(GetComponent<ObjectData>().GetId());
        _input = player.GetComponent<PlayerInput>();
        _inputManager = player.GetComponent<PlayerInputManager>();
        _animator = GetComponent<Animator>();
        playerTransform = transform;
        isSetup = true;
    }

    public void SetActivePlayerComponents()
    {
        if (!isSetup) SetupPlayer();

        hpText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = mainCharacter.MaxHP.ToString();
        hpText.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = mainCharacter.CurrentHP.ToString();
        hpSlider.value = mainCharacter.CurrentHP / mainCharacter.MaxHP;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Monster_Weapon")
        {
            if (mainCharacter.CurrentHP > 0 && !GetComponent<PlayerMovement>().GetIsDash() && !GetComponent<PlayerSkillController>().GetIsChannelingQ())
            {
                _input.enabled = true;
                GetComponent<PlayerWeaponController>().SetWeaponCollider(false);
                StartCoroutine(DecreaseHP(other));
            }
        }
    }

    public void HealPlayer(int heal)
    {
        mainCharacter.CurrentHP += heal;
        if (mainCharacter.CurrentHP > mainCharacter.MaxHP) mainCharacter.CurrentHP = mainCharacter.MaxHP;
        hpText.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = ((int)mainCharacter.CurrentHP).ToString();

        StartCoroutine(ChangeHPBar());
    }

    public IEnumerator DecreaseHP(Collider monster)
    {
        _animator.SetTrigger("HitByMonster");
        GameObject damageTextPrefeb = Instantiate(damageText, transform.position, Quaternion.LookRotation(monster.transform.position - transform.position));

        damage = monster.gameObject.GetComponentInParent<MonsterController>().GetMonsterDamage();
        damageTextPrefeb.GetComponent<DamageTextController>().DisplayDamage(damage, false);

        mainCharacter.CurrentHP -= damage;
        if (mainCharacter.CurrentHP <= 0) mainCharacter.CurrentHP = 0;

        hpText.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = ((int)mainCharacter.CurrentHP).ToString();
        StartCoroutine(ChangeHPBar());

        if (mainCharacter.CurrentHP == 0) DeadPlayer();
        yield return new WaitForSeconds(0.5f);
    }

    private void DeadPlayer()
    {
        _animator.SetTrigger("Dead");
        mainCharacter.Q_currentGauge = 0;
        StartCoroutine(DeadPlayerCoroutine());
    }

    private IEnumerator ChangeHPBar()
    {
        float timer = 0f;
        while (timer < 0.5f)
        {
            timer += Time.deltaTime;
            hpSlider.value = Mathf.Lerp(hpSlider.value, mainCharacter.CurrentHP / mainCharacter.MaxHP, timer / 0.5f);
            yield return null;
        }
        yield return null;
        yield break;
    }

    private IEnumerator DeadPlayerCoroutine()
    {
        yield return null;
        _input.enabled = false;
        yield return new WaitForSeconds(2f);

        // SWAP
        bool allDeadFlag = false;
        List<PlayableCharacter> partyPlayers = PlayerManager.Instance.GetPartyPlayers();
        for (int i = 0; i < partyPlayers.Count; i++)
        {
            if (partyPlayers[i].Id == mainCharacter.Id) continue;
            if (partyPlayers[i].CurrentHP > 0)
            {
                // PlayerManager.Instance._playerSwapController.currentPlayerIndex = partyPlayers[i].id;

                _inputManager.swap = partyPlayers[i].Id;
                allDeadFlag = true;
                _input.enabled = true;
                yield break;
            }
            yield return null;
        }

        // Dead all
        if (!allDeadFlag)
        {
            PlayerManager.Instance.IsAlive = false;
            PlayerManager.Instance.PlayerDeadToMenu();
        }

        yield break;
    }

    private void OnPlayerHitByMonsterSound(AnimationEvent animationEvent)
    {
        if (PlayerHitByMonsterAudioClip != null)
        {
            SoundManager.Instance.SFXPlay("PlayerHitByMonster", PlayerHitByMonsterAudioClip);
        }
    }

    private void OnPlayerDeadSound(AnimationEvent animationEvent)
    {
        if (PlayerDeadAudioClip != null)
        {
            SoundManager.Instance.SFXPlay("PlayerDead", PlayerDeadAudioClip);
        }
    }

    private void ShowStamina(bool isShow)
    {
        staminaObject.GetComponent<CanvasGroup>().alpha = isShow ? 1 : 0;
    }

    private void UpdateStamina(int value)
    {
        staminaImage.fillAmount = currentStamina / maxStamina;

        if (value == 0)
        {
            ShowStamina(false);
        }
        else
        {
            ShowStamina(true);
        }
    }

    public void DecreaseStamina(StaminaState state)
    {
        int decreaseValue = 0;

        switch (state)
        {
            case StaminaState.Jump:
                decreaseValue = 1;
                break;

            case StaminaState.Dash:
                decreaseValue = 2;
                break;

            default:
                decreaseValue = 0;
                break;
        }

        if (currentStamina >= decreaseValue)
        {
            currentStamina -= decreaseValue;
            UpdateStamina(1);
            if (currentStamina <= 0)
            {
                ShowStamina(false);
            }
        }
    }

    public void SprintingWithStamina()
    {
        isSprinting = true;
        currentStamina -= staminaUsePer * Time.deltaTime;
        UpdateStamina(1);

        if (currentStamina >= maxStamina)
        {
            ShowStamina(false);
        }

        if (currentStamina <= 0)
        {
            _inputManager.dash = false;
        }
    }

    public bool HasStamina(string state)
    {
        switch (state)
        {
            case "sprint":
                return currentStamina > (int)StaminaValue.sprint ? true : false;

            case "jump":
                return currentStamina >= (int)StaminaValue.jump ? true : false;

            case "dash":
                return currentStamina >= (int)StaminaValue.dash ? true : false;

            default:
                return false;
        }
    }

    public void MoveStaminaBar()
    {
        // staminaPos.position = mainCam.WorldToScreenPoint(playerTransform.position + staminaVector);
        Vector3 pos = new Vector3(playerTransform.position.x, playerTransform.position.y, playerTransform.position.z);
        staminaPos.position = mainCam.WorldToScreenPoint(pos + staminaVector);
    }
}
