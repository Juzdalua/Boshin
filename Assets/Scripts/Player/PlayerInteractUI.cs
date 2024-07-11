using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractUI : MonoBehaviour
{
    GameObject player;
    PlayerInteraction playerInteraction;
    public GameObject containerGameObject;
    public TextMeshProUGUI interactTextMeshProUGUI;

    void Start()
    {
        containerGameObject.SetActive(false);

        player = PlayerManager.Instance.GetActivePlayer();
        playerInteraction = player.GetComponent<PlayerInteraction>();
    }

    void Update()
    {
        if (!player.activeInHierarchy)
        {
            player = PlayerManager.Instance.GetActivePlayer();
            playerInteraction = player.GetComponent<PlayerInteraction>();
        }

        if (playerInteraction.GetInteractableObject() != null && playerInteraction.IsAvailableInteract())
        {
            Show(playerInteraction.GetInteractableObject());
        }
        else
        {
            Hide();
        }
    }

    private void Show(IInteractable interactable)
    {
        containerGameObject.SetActive(true);
        if (interactTextMeshProUGUI != null)
        {
            interactTextMeshProUGUI.text = interactable.GetInteractText();

            if (interactable.GetInteractType() == "NPC")
            {
                containerGameObject.transform.GetChild(0).gameObject.SetActive(true);
                containerGameObject.transform.GetChild(1).gameObject.SetActive(false);
                containerGameObject.transform.GetChild(3).gameObject.SetActive(false);
            }
            else if (interactable.GetInteractType() == "Item")
            {
                containerGameObject.transform.GetChild(0).gameObject.SetActive(false);
                containerGameObject.transform.GetChild(1).gameObject.SetActive(true);
                containerGameObject.transform.GetChild(1).GetComponent<Image>().sprite = interactable.GetGroundItemImage();
                containerGameObject.transform.GetChild(3).gameObject.SetActive(true);
                containerGameObject.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = interactable.GetItemName();
            }
            else if (interactable.GetInteractType() == "Cook" || interactable.GetInteractType() == "Treasure")
            {
                containerGameObject.transform.GetChild(0).gameObject.SetActive(false);
                containerGameObject.transform.GetChild(1).gameObject.SetActive(false);
                containerGameObject.transform.GetChild(3).gameObject.SetActive(false);
            }
        }
    }

    private void Hide()
    {
        containerGameObject.SetActive(false);
    }
}
