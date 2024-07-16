using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool dash = false;
    public bool attack;
    public bool mouseLeftClick;
    public bool eSkill;
    public bool qSkill;
    public bool fInteraction;
    public bool zInteraction;
    public bool isOpenInventory;
    public bool isOpenQuestInventory;
    public bool isOpenCookInventory;
    public bool isOpenStatInventory;
    public bool isOpenMenu;
    public bool isInteraction = false;
    public int swap;

    public bool canSwap = false;
    public bool canMove = true;

    PlayerAttackController _playerAttackController;
    PlayerStateController _state;
    PlayerInteraction _interaction;
    public InventoryController _inventory;
    public QuestController _quest;
    public PlayerStatManager _stat;

    void Start()
    {
        _playerAttackController = GetComponent<PlayerAttackController>();
        _state = GetComponent<PlayerStateController>();
        _interaction = GetComponent<PlayerInteraction>();
        swap = GetComponent<ObjectData>().GetId();
    }
    void Update()
    {
        if (_inventory.IsUseInventory() || _quest.IsUseInventory() || CookManager.Instance.IsUseCook() || _stat.IsUseStat() || MenuManager.Instance.IsUseInventory() || !PlayerManager.Instance.IsAlive || InformationManager.Instance.IsShowUI())
        {
            StopPosition();
            canMove = false;
        }
    }

    void OnMove(InputValue value)
    {
        if (canMove)
        {
            move = value.Get<Vector2>();
        }
    }

    void OnLook(InputValue value)
    {
        if (canMove)
        {
            look = value.Get<Vector2>();
        }
    }

    void OnFire(InputValue value)
    {
        if (canMove)
        {
            _playerAttackController.OnAttack();
        }

        if (_inventory.IsUseInventory() || _quest.IsUseInventory() && CookManager.Instance.IsUseCook() && _stat.IsUseStat() && MenuManager.Instance.IsUseInventory() && !InformationManager.Instance.IsShowUI() && !_interaction.IsConversationNow())
        {
            mouseLeftClick = value.isPressed;
        }
    }

    void OnJump(InputValue value)
    {
        if (canMove)
        {
            jump = value.isPressed;
        }
    }

    void OnDash(InputValue value)
    {
        if (canMove)
        {
            if (!_state.HasStamina("dash"))
            {
                return;
            }

            dash = value.isPressed;
            if (!GetComponent<PlayerMovement>().GetIsDash())
            {
                _playerAttackController.GetComponent<PlayerMovement>().StartDash();
            }
        }
    }

    void OnFInteraction(InputValue value)
    {
        if (!_inventory.IsUseInventory() && !_quest.IsUseInventory() && !CookManager.Instance.IsUseCook() && !_stat.IsUseStat() && !MenuManager.Instance.IsUseInventory())
        {
            fInteraction = value.isPressed;
        }
    }

    void OnZInteraction(InputValue value)
    {
        if (!_inventory.IsUseInventory() && !_quest.IsUseInventory() && !CookManager.Instance.IsUseCook() && !_stat.IsUseStat() && !MenuManager.Instance.IsUseInventory()&& !InformationManager.Instance.IsShowUI() && !_interaction.IsConversationNow())
        {
            zInteraction = value.isPressed;
        }
    }

    void OnCStat(InputValue value)
    {
        if (!_inventory.IsUseInventory() && !_quest.IsUseInventory() && !CookManager.Instance.IsUseCook() && !MenuManager.Instance.IsUseInventory()&& !InformationManager.Instance.IsShowUI() && !_interaction.IsConversationNow())
        {
            isOpenStatInventory = value.isPressed;
        }
    }

    void OnESkill(InputValue value)
    {

        if (canMove)
        {
            eSkill = value.isPressed;
        }
    }

    void OnQSkill(InputValue value)
    {

        if (canMove)
        {
            qSkill = value.isPressed;
        }
    }

    void OnInventory(InputValue value)
    {
        if (!_inventory.IsUseInventory() && !_quest.IsUseInventory() && !CookManager.Instance.IsUseCook() && !_stat.IsUseStat() && !MenuManager.Instance.IsUseInventory()&& !InformationManager.Instance.IsShowUI() && !_interaction.IsConversationNow())
        {
            isOpenInventory = value.isPressed;
            StopPosition();
            canMove = false;
        }
    }

    void OnQuest(InputValue value)
    {
        if (!_inventory.IsUseInventory() && !_quest.IsUseInventory() && !CookManager.Instance.IsUseCook() && !_stat.IsUseStat() && !MenuManager.Instance.IsUseInventory()&& !InformationManager.Instance.IsShowUI() && !_interaction.IsConversationNow())
        {
            isOpenQuestInventory = value.isPressed;
            StopPosition();
            canMove = false;
        }
    }
    
    void OnMenu(InputValue value)
    {
        if (!_inventory.IsUseInventory() && !_quest.IsUseInventory() && !CookManager.Instance.IsUseCook() && !_stat.IsUseStat()&& !InformationManager.Instance.IsShowUI() && !_interaction.IsConversationNow())
        {
            isOpenMenu = value.isPressed;
            StopPosition();
            canMove = false;
        }
    }

    void OnSwap(InputValue value)
    {
        if (canMove && value != null && value.Get() != null && canSwap)
        {
            swap = int.Parse(value.Get().ToString());
        }
    }

    public void StopPosition()
    {
        move.x = 0;
        move.y = 0;
        look.x = 0;
        look.y = 0;
    }
}
