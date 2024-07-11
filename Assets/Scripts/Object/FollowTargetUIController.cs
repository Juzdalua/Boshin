using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTargetUIController : MonoBehaviour
{
    public Vector3 offset;
    public GameObject[] players;
    GameObject activePlayer;
    Camera mainCam;

    void Start()
    {
        SetupActivePlayer();
        mainCam = Camera.main;
    }

    void Update()
    {
        if (activePlayer == null || !activePlayer.activeInHierarchy) SetupActivePlayer();

        if (activePlayer != null && activePlayer.activeInHierarchy) FollowTarget();
    }

    void SetupActivePlayer()
    {
        if (activePlayer == null || !activePlayer.activeInHierarchy)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].activeInHierarchy)
                {
                    activePlayer = players[i];
                    break;
                }
            }
        }
    }

    void FollowTarget()
    {
        Vector3 pos = mainCam.WorldToScreenPoint(activePlayer.transform.position + offset);

        if (transform.position != pos) transform.position = pos;
    }
}
