using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollowCameraRoot : MonoBehaviour
{
    GameObject activePlayer;

    void Update()
    {
        SetActivePlayer();
        FollowCharacter();
    }

    void SetActivePlayer()
    {
        if (activePlayer == null || !activePlayer.activeInHierarchy)
        {
            activePlayer = PlayerManager.Instance.GetActivePlayer();
        }
    }

    void FollowCharacter()
    {
        transform.position = activePlayer.transform.position + new Vector3(0, activePlayer.GetComponent<PlayerMovement>().CameraHeight, 0);
    }
}
