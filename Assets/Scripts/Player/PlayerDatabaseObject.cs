using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Database", menuName = "Player System/Database")]
public class PlayerDatabaseObject : ScriptableObject
{
    [SerializeField] private PlayerObject[] playerObjects;
    [SerializeField] private List<PlayableCharacter> players = new List<PlayableCharacter>();
    public List<PlayableCharacter> Players => players;
    // public List<PlayableCharacter> partyPlayers = new List<PlayableCharacter>();

    void OnEnable()
    {
        SetPlayerDatabase();
    }

    public void Clear()
    {
        players.Clear();
        // partyPlayers.Clear();
    }

    public void SetPlayerDatabase()
    {
        players = new List<PlayableCharacter>();
        for (int i = 0; i < playerObjects.Length; i++)
        {
            players.Add(new PlayableCharacter(playerObjects[i]));
        }
        players = players.GroupBy(ele => ele.Id).Select(ele => ele.First()).ToList();
    }
}
