using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerSwapController : MonoBehaviour
{
    public GameObject[] players;
    public GameObject partyUI;
    List<PlayableCharacter> partyPlayers;
    int partyMemberLimit = 2;

    public CinemachineVirtualCamera mainCinemachineCamera;
    public CinemachineFreeLook qSkillCinemachineCamera;

    private PlayerInputManager _inputManager;
    private GameObject currentPlayer;
    public int currentPlayerIndex = 0;

    [Header("Camera")]
    public Transform worldCameraRoot;
    public Transform characterCameraRoot;
    private Transform skillCameraRoot;
    public Vector3 tempCameraRoot;
    public bool isSwap = false;

    [Header("Audio")]
    public AudioClip playerSwapSound;

    void Start()
    {
        SetActivePlayer();
        _inputManager.swap = 1;

        SetPartyUI();
        partyUI.SetActive(true);
    }

    void Update()
    {
        if (currentPlayerIndex != _inputManager.swap - 1)
        {
            if (CheckSwapCondition(_inputManager.swap))
            {
                PlayerSwap();
                SetPartyUI();
            }
        }
    }

    void SetActivePlayer()
    {
        currentPlayer = PlayerManager.Instance.GetActivePlayer();

        _inputManager = currentPlayer.GetComponent<PlayerInputManager>();
    }

    public bool CheckSwapCondition(int swapKey)
    {
        if (swapKey > partyMemberLimit) return false;

        if (PlayerManager.Instance.GetPartyPlayers()[swapKey - 1].currentHP <= 0)
        {
            _inputManager.swap = currentPlayerIndex + 1;
            return false;
        }

        return true;
    }

    public void PlayerSwap(bool isSound = true)
    {
        if (isSound) OnSwapSound();
        if (currentPlayer == null) SetActivePlayer();

        tempCameraRoot = worldCameraRoot.eulerAngles;
        Transform tempTransform = currentPlayer.transform;
        currentPlayerIndex = _inputManager.swap - 1;
        _inputManager.StopPosition();

        for (int i = 0; i < players.Length; i++)
        {
            if (i == currentPlayerIndex)
            {
                // Player Setup
                currentPlayer.GetComponent<PlayerMovement>().PlayerSwapSetAction();

                currentPlayer = players[i];
                currentPlayer.transform.SetPositionAndRotation(tempTransform.position, tempTransform.rotation);
                _inputManager = currentPlayer.GetComponent<PlayerInputManager>();
                _inputManager.swap = currentPlayerIndex + 1;

                // Follow Camera root Setup
                skillCameraRoot = currentPlayer.transform.GetChild(1);

                players[i].SetActive(true);
            }
            else players[i].SetActive(false);
        }
        currentPlayer.GetComponent<PlayerSkillController>().SetSkillUIWithCharacterSwap();

        qSkillCinemachineCamera.LookAt = skillCameraRoot.GetChild(0);
        qSkillCinemachineCamera.Follow = skillCameraRoot.GetChild(1);

        if (!currentPlayer.GetComponent<PlayerInputManager>().canSwap)
        {
            currentPlayer.GetComponent<PlayerInputManager>().canSwap = true;
        }
        currentPlayer.GetComponent<PlayerStateController>().SetActivePlayerComponents();
        isSwap = true;

        if (!currentPlayer.GetComponent<PlayerInput>().enabled) currentPlayer.GetComponent<PlayerInput>().enabled = true;
        currentPlayer.GetComponent<PlayerInput>().gameObject.SetActive(false);
        currentPlayer.GetComponent<PlayerInput>().gameObject.SetActive(true);
    }

    private void OnSwapSound()
    {
        if (playerSwapSound != null)
        {
            SoundManager.Instance.SFXPlay("PlayerSwap", playerSwapSound);
        }
    }

    public void SetPartyUI()
    {
        partyPlayers = PlayerManager.Instance.GetPartyPlayers();

        for (int i = 0; i < partyMemberLimit; i++)
        {
            if (i < partyPlayers.Count)
            {
                partyUI.transform.GetChild(0).GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                partyUI.transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < partyPlayers.Count; i++)
        {
            if (currentPlayer.GetComponent<ObjectData>().GetId() == partyPlayers[i].id)
            {
                partyUI.transform.GetChild(0).GetChild(i).GetChild(0).gameObject.SetActive(false);
                partyUI.transform.GetChild(0).GetChild(i).GetChild(1).GetChild(1).gameObject.SetActive(false);
            }
            else
            {
                partyUI.transform.GetChild(0).GetChild(i).GetChild(0).gameObject.SetActive(true);
                partyUI.transform.GetChild(0).GetChild(i).GetChild(1).GetChild(1).gameObject.SetActive(true);

                partyUI.transform.GetChild(0).GetChild(i).GetChild(0).GetChild(0).GetComponent<Image>().sprite = partyPlayers[i].q_image;
                partyUI.transform.GetChild(0).GetChild(i).GetChild(0).GetChild(2).GetComponent<Image>().fillAmount = partyPlayers[i].q_currentGauge / partyPlayers[i].q_maxGauge;
                partyUI.transform.GetChild(0).GetChild(i).GetChild(0).GetChild(2).GetComponent<Image>().color = new Color32(partyPlayers[i].elementColor[0], partyPlayers[i].elementColor[1], partyPlayers[i].elementColor[2], 80);
            }

            partyUI.transform.GetChild(0).GetChild(i).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = partyPlayers[i].characterName;
            partyUI.transform.GetChild(0).GetChild(i).GetChild(1).GetChild(1).GetComponentInChildren<Slider>().value = partyPlayers[i].currentHP / partyPlayers[i].maxHP;

            partyUI.transform.GetChild(0).GetChild(i).GetChild(2).GetComponent<Image>().sprite = partyPlayers[i].profileImage;
            if (partyPlayers[i].currentHP == 0) partyUI.transform.GetChild(0).GetChild(i).GetChild(2).GetComponent<Image>().color = new Color32(255, 255, 255, 40);
            else partyUI.transform.GetChild(0).GetChild(i).GetChild(2).GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
    }
}
