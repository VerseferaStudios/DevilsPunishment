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

    public void OnInteract() {
        Debug.Log("Picked up " + item.name + " x" + stock + ".");
		gameObject.SetActive(false);
		Inventory.instance.AddItem(item, stock);
		if (gameObject.name.Contains("(Clone)"))
		{
			Destroy(gameObject);
		}
	}

    public Item GetGunItem()
    {
        return item;
    }

    public void SetPlayerController(PlayerController playerController)
    {

    }
}
