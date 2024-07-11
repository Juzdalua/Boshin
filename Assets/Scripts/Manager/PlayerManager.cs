using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : Singleton<PlayerManager>
{
    public PlayerSwapController _playerSwapController;
    public PlayerDatabaseObject playerDatabase;
    public GameObject[] allPlayers;
    GameObject activePlayer;
    public Canvas playerDeadCanvas;
    public bool isAlive = true;

    void Awake()
    {
        SetPlayerManager();
    }

    public void SetPlayerManager()
    {
        SetActivePlayer();
        _playerSwapController = GetComponent<PlayerSwapController>();

        playerDeadCanvas.gameObject.SetActive(false);
        isAlive = true;
    }

    void Update()
    {
        if (!activePlayer.activeInHierarchy)
        {
            SetActivePlayer();
        }
    }

    private void OnApplicationQuit()
    {
        playerDatabase.Clear();
    }

    public GameObject GetActivePlayer()
    {
        if (activePlayer == null) SetActivePlayer();
        return activePlayer;
    }

    public PlayableCharacter GetMainCharacter()
    {
        if (activePlayer == null) SetActivePlayer();
        return GetPlayerById(activePlayer.GetComponent<ObjectData>().GetId());
    }

    private void SetActivePlayer()
    {
        for (int i = 0; i < allPlayers.Length; i++)
        {
            if (allPlayers[i].activeInHierarchy)
            {
                activePlayer = allPlayers[i];
            }
        }
    }

    public PlayableCharacter GetPlayerById(int id)
    {
        return playerDatabase.players.Find(ele => ele.id == id);
    }

    // public List<PlayableCharacter> GetPartyPlayers()
    // {
    //     for (int i = 0; i < playerDatabase.players.Count; i++)
    //     {
    //         if (playerDatabase.players[i].isJoinParty)
    //         {
    //             playerDatabase.partyPlayers.Add(playerDatabase.players[i]);
    //         }
    //     }
    //     playerDatabase.partyPlayers = playerDatabase.partyPlayers.OrderBy(ele => ele.partyOrder).GroupBy(ele => ele.id).Select(ele => ele.First()).ToList();
    //     return playerDatabase.partyPlayers;
    // }

    public List<PlayableCharacter> GetPartyPlayers()
    {
        List<PlayableCharacter> partyPlayers = new List<PlayableCharacter>();
        for (int i = 0; i < playerDatabase.players.Count; i++)
        {
            if (playerDatabase.players[i].isJoinParty)
            {
                partyPlayers.Add(playerDatabase.players[i]);
            }
        }
        partyPlayers = partyPlayers.OrderBy(ele => ele.partyOrder).GroupBy(ele => ele.id).Select(ele => ele.First()).ToList();
        return partyPlayers;
    }

    public void PlayerDeadToMenu()
    {
        playerDeadCanvas.gameObject.SetActive(true);
        StartCoroutine(PlayerDeadToMenuCoroutine());
        SoundManager.Instance.PlayerDeadSfxPlay();
    }

    IEnumerator PlayerDeadToMenuCoroutine()
    {
        CanvasGroup deadCanvasGroup = playerDeadCanvas.GetComponent<CanvasGroup>();

        float timer = 0f;
        while (timer < 2f)
        {
            timer += Time.deltaTime;
            deadCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer);
            yield return null;

            if (deadCanvasGroup.alpha >= 1f) yield break;
        }
    }

    public void LoadPlayerInformation(List<PlayableCharacter> loadPlayerDatabases)
    {
        for (int i = 0; i < loadPlayerDatabases.Count; i++)
        {
            var loadPlayer = loadPlayerDatabases[i];
            var player = GetPlayerById(loadPlayer.id);
            player.level = loadPlayer.level;
            player.currentExp = loadPlayer.currentExp;
            player.currentHP = loadPlayer.currentHP;
            player.currentExp = loadPlayer.currentExp;
            player.weaponId = loadPlayer.weaponId;
            player.q_currentGauge = loadPlayer.q_currentGauge;
            player.isJoinParty = loadPlayer.isJoinParty;
            player.partyOrder = loadPlayer.partyOrder;
        }
    }
}
