using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    GameObject activePlayer;

    void Update()
    {
        SetActivePlayer();
        SetCameraPosition();
    }

    void SetActivePlayer()
    {
        if (activePlayer == null || !activePlayer.activeInHierarchy)
        {
            activePlayer = PlayerManager.Instance.GetActivePlayer();
        }
    }

    void SetCameraPosition()
    {
        transform.position = new Vector3(activePlayer.transform.position.x, transform.position.y, activePlayer.transform.position.z);
    }
}
