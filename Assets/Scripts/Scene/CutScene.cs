using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CutScene : MonoBehaviour
{

    private GameObject player;
    private CutScene component;

    private PlayableDirector _pd;
    public TimelineAsset[] _ta;

    void Start()
    {
        player = PlayerManager.Instance.GetActivePlayer();
        _pd = GetComponent<PlayableDirector>();
        component = GetComponent<CutScene>();
    }

    void Update(){
        if(!player.activeInHierarchy) player = PlayerManager.Instance.GetActivePlayer();
    }

    private void OnTriggerExit(Collider other)
    {   
        if (other.tag == "Player")
        {
            _pd.Play(_ta[0]);

            PlayerInputStop();
            Invoke("PlayerInputStart", (float)_ta[0].duration);
        }
    }

    private void PlayerInputStop()
    {
        player.GetComponent<PlayerInput>().enabled = false;
    }

    private void PlayerInputStart(){
        player.GetComponent<PlayerInput>().enabled = true;
        component.GetComponent<BoxCollider>().enabled = false;
    }

    
}
