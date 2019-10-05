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

    private LaserCutter laserCutterScript;

    public Transform laserSpotTransform;

    private bool isFirstInteract = true;

    private void Start()
    {
        laserCutterScript = GetComponent<LaserCutter>();
    }

    public string Prompt()
    {
        return "Using monitor for laser";// + item.name + " (" + stock + ")";
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
        if (isFirstInteract)
        {
            playerController.isInteractLaser = true;
            isFirstInteract = false;
        }
        //laserCutterScript.BeginSequences();
    }

    public Item GetGunItem()
    {
        return null;
    }

    public void SetPlayerController(PlayerController playerController) //use int and network playerlist?
    {
        this.playerController = playerController;
        playerController.laserSpot = laserSpotTransform.position;
        playerController.laserMonitor = transform.position; //cube.014 Monitor
        playerController.laserCutterScript = laserCutterScript;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            playerController.isInteractLaser = false;
        }
    }
}
