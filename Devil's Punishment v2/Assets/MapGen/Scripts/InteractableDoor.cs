using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDoor : MonoBehaviour, IInteractable
{

    //public Item item;
    //public int stock;
    public float timeToPickUp = .5f;
    public bool isVentCover = true;
    public string Prompt()
    {
        return "Open vent cover";// + item.name + " (" + stock + ")";
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
        StartCoroutine(OpenVentCoverOrDoor());
        /*
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
        */
    }

    private IEnumerator OpenVentCoverOrDoor()
    {
        while (transform.parent.localEulerAngles.z <= 80)
        {
            if (isVentCover)
            {
                Debug.Log("Opened vent cover");
                transform.parent.localEulerAngles += Vector3.Lerp(transform.parent.localEulerAngles, transform.parent.localEulerAngles + new Vector3(0, 0, 90), Time.deltaTime);
            }
            else
            {

            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    public Item GetGunItem()
    {
        return null;
    }
}
