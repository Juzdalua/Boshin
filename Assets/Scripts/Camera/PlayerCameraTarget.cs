using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraTarget : MonoBehaviour
{
    [SerializeField] private GameObject cameraRoot;
    Quaternion tempTransform;

    void Update()
    {
        transform.position = cameraRoot.transform.position + Vector3.forward;
        transform.rotation = cameraRoot.transform.rotation;
    }
}
