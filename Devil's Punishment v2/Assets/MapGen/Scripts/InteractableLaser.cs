using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableLaser : MonoBehaviour, IInteractable
{

    //public Item item;
    //public int stock;
    public float timeToPickUp = 5f;
    public bool isVentCover = true;

    public GameObject brokenFloorCollidors;

    //private int l = 0;
    private PlayerController playerController;

    public string Prompt()
    {
        return "Using laset";// + item.name + " (" + stock + ")";
    }

    public float TimeToInteract()
    {
        return timeToPickUp;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void OnInteract()
    {
        playerController.isInteractLaser = true;
    }

    public Item GetGunItem()
    {
        return null;
    }

    public void SetPlayerController(PlayerController playerController) //use int and network playerlist?
    {
        this.playerController = playerController;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            playerController.isInteractLaser = false;
        }
    }
}
