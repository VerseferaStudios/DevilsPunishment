using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDoor : MonoBehaviour, IInteractable
{

    //public Item item;
    //public int stock;
    public float timeToPickUp = .5f;
    public bool isVentCover = true;

    public GameObject brokenFloorCollidors;

    private int l = 0;

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
        if (isVentCover)
        {
            Debug.Log("Opening vent cover");
            gameObject.GetComponent<BoxCollider>().enabled = false;

            Transform t = transform.parent.parent.parent.GetChild(2).GetChild(0); //WILL WORK ON CORRIDOR ONLY!!! TAKE ROOM SEPARATE
            Instantiate(brokenFloorCollidors, t.position, t.rotation, t.parent);
            t.gameObject.SetActive(false);
            //timeToPickUp = float.MaxValue;

            StartCoroutine(OpenVentCover());
        }
        else
        {
            Debug.Log("Opening door");
            gameObject.GetComponent<BoxCollider>().enabled = false;
            StartCoroutine(OpenDoor());
        }

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

    private IEnumerator OpenVentCover()
    {
        

        Transform parentGb = transform.parent;
        while (transform.parent.localEulerAngles.z <= 80)
        {


            parentGb.localEulerAngles += Vector3.Lerp(parentGb.localEulerAngles, parentGb.localEulerAngles + new Vector3(0, 0, 90), Time.deltaTime);
            //gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator OpenDoor()
    {
        float t = 0;

        Transform door0 = transform.parent.GetChild(1);
        Transform door1 = transform.parent.GetChild(2);
        while (t < .99f && door0.localScale.z > 0.01f && door1.localScale.z > 0.01f)// && l < 1000)
        {
            door0.localScale = new Vector3(1, 1, Mathf.Lerp(door0.localScale.z, 0, t));
            //door0.localPosition = new Vector3(door0.localPosition.x, door0.localPosition.y, Mathf.Lerp(door0.localPosition.z, door0.localPosition.z - 0.5f, t));

            door1.localScale = new Vector3(1, 1, Mathf.Lerp(door1.localScale.z, 0, t));
            //door1.localPosition = new Vector3(door1.localPosition.x, door1.localPosition.y, Mathf.Lerp(door1.localPosition.z, door1.localPosition.z + 0.5f, t));

            //door0.localScale = Vector3.Lerp(door0.localScale, door0.localScale - new Vector3(0, 0, 0.1f), Time.deltaTime * 100);
            //door1.localScale = Vector3.Lerp(door1.localScale, door1.localScale - new Vector3(0, 0, 0.1f), Time.deltaTime * 10000);
            ++l;
            t += Time.deltaTime * 0.1f;
            Debug.Log("t = " + t);
            yield return new WaitForSeconds(0.1f);
        }
    }


    public Item GetGunItem()
    {
        return null;
    }
}
