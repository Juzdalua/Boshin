using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    [SerializeField] private AudioClip restoreHPClip;
    private float healTime = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // HealPlayer(other);
            HealAllPlayers(other);
        }
    }

    void HealPlayer(Collider player)
    {
        if (healTime == 0 || Time.time > healTime + restoreHPClip.length)
        {
            healTime = Time.time;
            if (restoreHPClip != null) SoundManager.Instance.SFXPlay(restoreHPClip.name, restoreHPClip);

            player.GetComponent<PlayerStateController>().HealPlayer(100);
        }
    }

    void HealAllPlayers(Collider player)
    {
        if (healTime == 0 || Time.time > healTime + restoreHPClip.length)
        {
            healTime = Time.time;
            if (restoreHPClip != null) SoundManager.Instance.SFXPlay(restoreHPClip.name, restoreHPClip);
            player.GetComponent<PlayerStateController>().HealPlayer(100);

            // var players = PlayerManager.Instance._playerSwapController.players;
            var players = PlayerManager.Instance.GetPartyPlayers();
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].Id == player.GetComponent<ObjectData>().GetId()) continue;
                players[i].CurrentHP = players[i].MaxHP;
            }
            PlayerManager.Instance.PlayerSwapController.SetPartyUI();
        }
    }
}
