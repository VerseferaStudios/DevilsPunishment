using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableLoot : MonoBehaviour, IInteractable
{

    public Item item;
    public int stock;
    public float timeToPickUp = .5f;
    public string Prompt() {
        return "Pick up " + item.name + " (" + stock + ")";
    }

    public float TimeToInteract() {
        return timeToPickUp;
    }

    public GameObject GetGameObject() {
        return gameObject;
    }

    public void OnInteract(){
        Debug.Log("Picked up " + item.name + " x" + stock + ".");
		gameObject.SetActive(false);
        Inventory.instance.AddItem(item, stock);
		if (gameObject.name.Contains("(Clone)"))
		{
			Destroy(gameObject);
		}

        //FMOD Gun PU Events
        if (gameObject.tag == "Shotgun")
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Pickups/WeaponPU/Shotgun_PU", GetComponent<Transform>().position);
        }
        if (gameObject.tag == "Handgun")
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Pickups/WeaponPU/Handgun_PU", GetComponent<Transform>().position);
        }
        if (gameObject.tag == "Rifle")
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Pickups/WeaponPU/Rifle_PU", GetComponent<Transform>().position);
        }
        if (gameObject.tag == "Flashlight")
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Pickups/Items/Flashlight_PU", GetComponent<Transform>().position);
        }
        if (gameObject.tag == "Pills")
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Pickups/Items/Pills_PU", GetComponent<Transform>().position);
        }
        if (gameObject.tag == "CuffKey")
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Pickups/Items/CuffKey_PU", GetComponent<Transform>().position);
        }
        if (gameObject.tag == "Handcuffs")
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Pickups/Items/Handcuffs_PU", GetComponent<Transform>().position);
        }
        if (gameObject.tag == "Medkit")
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Pickups/Items/MedKit_PU", GetComponent<Transform>().position);
        }
        if (gameObject.tag == "GlowSticks")
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Pickups/Items/Glowstick_PU", GetComponent<Transform>().position);
        }
        if (gameObject.tag == "LSD")
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/UI/Inventory/Inventory_Open", GetComponent<Transform>().position);
        }
        if (gameObject.tag == "GeneratorPart")
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/UI/Inventory/Inventory_Open", GetComponent<Transform>().position);
        }
        if (gameObject.tag == "HandgunAmmo")
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Pickups/Ammo/Handgun_Ammo_PU", GetComponent<Transform>().position);
        }
        if (gameObject.tag == "ShotgunAmmo")
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Pickups/Ammo/Shotgun_Ammo_PU", GetComponent<Transform>().position);
        }
        if (gameObject.tag == "RifleAmmo")
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Pickups/Ammo/Rifle_Ammo_PU", GetComponent<Transform>().position);
        }
    }

    public Item GetGunItem()
    {
        return item;



    }

    public void SetPlayerController(PlayerController playerController)
    {

    }

    public void OnFocus()
    {
        
    }

    public void OnReleaseFocus()
    {
        
    }
        

}
