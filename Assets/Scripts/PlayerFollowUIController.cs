using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollowUIController : MonoBehaviour
{
    public Transform player;
    void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(player.position);
        // transform.LookAt(Camera.main.transform.position);
    }
}
