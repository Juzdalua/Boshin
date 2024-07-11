using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollowIcon : MonoBehaviour
{
    GameObject activePlayer;
    Camera mainCam;

    void Start(){
        mainCam = Camera.main;
    }

    void LateUpdate()
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
        transform.position = new Vector3(activePlayer.transform.position.x, 0.1f, activePlayer.transform.position.z);
        // transform.rotation = Quaternion.Euler(-90, (mainCam.transform.position - activePlayer.transform.position).normalized.y, 0);
        transform.eulerAngles = new Vector3(90, mainCam.transform.eulerAngles.y, 0);
    }
}
