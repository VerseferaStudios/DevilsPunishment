using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableLaser : MonoBehaviour, IInteractable
{

    //public Item item;
    //public int stock;
    public float timeToPickUp = 20f;
    public bool isVentCover = true;

    public GameObject brokenFloorCollidors;

    //private int l = 0;
    private PlayerController playerController;

    private LaserCutter laserCutterScript;

    public Transform laserSpotTransform;

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
        //GetComponent<MeshCollider>().enabled = false; // Just for now. Get Interactabe Laser into a different gameobject later on
        //NO INPUT FROM USER

        //gameObject.SetActive(false);
        //timeToPickUp = float.PositiveInfinity;
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

    public void OnFocus()
    {
        playerController.isInteractLaser = true;
    }

    public void OnReleaseFocus()
    {
        laserCutterScript.ResetLaser();
        laserCutterScript.StopLaser();
    }
}
