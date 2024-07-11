using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    CinemachineComponentBase componentBase;
    float cameraDistance;
    public float sensitivity = 10f;

    void Start()
    {
        if (componentBase == null)
        {
            componentBase = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
        }

        (componentBase as Cinemachine3rdPersonFollow).CameraDistance = 3;
    }

    private void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            cameraDistance = Input.GetAxis("Mouse ScrollWheel") * sensitivity;

            if (componentBase is Cinemachine3rdPersonFollow)
            {

                (componentBase as Cinemachine3rdPersonFollow).CameraDistance -= cameraDistance;

                if ((componentBase as Cinemachine3rdPersonFollow).CameraDistance < 1)
                {
                    (componentBase as Cinemachine3rdPersonFollow).CameraDistance = 1;
                }
                else if ((componentBase as Cinemachine3rdPersonFollow).CameraDistance > 7)
                {
                    (componentBase as Cinemachine3rdPersonFollow).CameraDistance = 7;
                }
            }
        }
    }
}
