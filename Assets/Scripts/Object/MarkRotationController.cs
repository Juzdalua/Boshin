using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarkRotationController : MonoBehaviour
{
    Image mark;

    void Start(){
        mark = GetComponentInChildren<Image>();
    }

   void Update(){
        mark.transform.Rotate(new Vector3(0, Time.deltaTime * 100f, 0));
   }
}
