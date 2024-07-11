using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRotationController : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(new Vector3(0, Time.deltaTime * 100f, 0));
    }
}
