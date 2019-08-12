using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableLoot : MonoBehaviour, IInteractable
{

    public Item item;
    public int stock;
    public float timeToPickUp = .5f;
    public bool isDoor = false;
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
        if (isDoor)
        {
            StartCoroutine(OpenVentCover());
        }
        else
        {
            Debug.Log("Picked up " + item.name + " x" + stock + ".");
            gameObject.SetActive(false);
            Inventory.instance.AddItem(item, stock);
            if (gameObject.name.Contains("(Clone)"))
            {
                Destroy(gameObject);
            }
        }
	}

    private IEnumerator OpenVentCover()
    {
        while (transform.localEulerAngles.z >= 89) //get sth better
        {
            transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, transform.localEulerAngles + new Vector3(0, 0, 90), Time.deltaTime);
            yield return new WaitForSeconds(0.1f);
        }
    }

}
