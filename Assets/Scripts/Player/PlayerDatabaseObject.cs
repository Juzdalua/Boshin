using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Database", menuName = "Player System/Database")]
public class PlayerDatabaseObject : ScriptableObject
{
    public PlayerObject[] playerObjects;
    public List<PlayableCharacter> players = new List<PlayableCharacter>();
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
        players = players.GroupBy(ele => ele.id).Select(ele => ele.First()).ToList();
    }
}
