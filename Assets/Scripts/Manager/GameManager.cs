using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] public string currentVilligeName;

    void Start()
    {
        Application.targetFrameRate = 60;
    }

    void Update(){
        if(Input.GetKey(KeyCode.LeftAlt)){
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void SetCurrentVilligeName(string villigeName){
        currentVilligeName = villigeName;
    }

    public string GetCurrentVilligeName(){
        return currentVilligeName;
    }
    
}
