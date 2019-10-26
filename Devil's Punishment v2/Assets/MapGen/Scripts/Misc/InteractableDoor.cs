using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDoor : MonoBehaviour, IInteractable
{

    public enum DoorType
    {
        ventCover,
        door,
        elevator
    }

    public DoorType doorType;

    public float timeToPickUp = .5f;
    public bool isVentCover = true;
    public Transform doorLGround, doorRGround;
    public Transform doorLTop, doorRTop;

    public GameObject brokenFloorCollidors;

    private string promptString = "Open vent cover";

    //[SerializeField] private bool isElevatorOpen = true;

    //public Transform interactableClose;
    private Vector3 colliderSizeClose = new Vector3(0.5f, 0.5f, 0.5f);//, interactableOpen, colliderSizeOpen;

    private Elevator elevatorScript;

    //private int l = 0;

    private void Start()
    {
        elevatorScript = transform.parent.parent.GetComponent<Elevator>();
        switch (doorType)
        {
            case DoorType.ventCover:
                promptString = "Open vent cover";
                break;
            case DoorType.door:
                promptString = "Open door";
                break;
            case DoorType.elevator:
                promptString = "Close elevator door";
                //interactableOpen = transform.localPosition;
                //colliderSizeOpen = GetComponent<BoxCollider>().size;
                break;
        }
    }

    public string Prompt()
    {
        return promptString;// + item.name + " (" + stock + ")";
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
        gameObject.GetComponent<BoxCollider>().enabled = false;
        switch (doorType)
        {
            case DoorType.ventCover:
                Debug.Log("Opening vent cover");
                Transform t = transform.parent.parent.parent.GetChild(2).GetChild(0); //WILL WORK ON CORRIDOR ONLY!!! TAKE ROOM SEPARATE
                Instantiate(brokenFloorCollidors, t.position, t.rotation, t.parent);
                t.gameObject.SetActive(false);
                //timeToPickUp = float.MaxValue;
                StartCoroutine(OpenVentCover());
                break;

            case DoorType.door:
                Debug.Log("Opening door");
                StartCoroutine(OpenDoor());
                break;

            case DoorType.elevator:
                /*
                if (!isElevatorOpen)
                {
                    CallOpenElevatorFns();
                }
                else*/
                {
                    Debug.Log("Closing elevator");
                    StartCoroutine(CloseElevator());
                    //promptString = "Open elevator door";
                    //transform.localPosition = interactableOpen;
                    //GetComponent<BoxCollider>().size = colliderSizeOpen;
                }
                //isElevatorOpen = !isElevatorOpen;
                break;

        }
    }

    public void CallOpenElevatorFns()
    {
        Debug.Log("Opening elevator");
        StartCoroutine(OpenElevator());
        //promptString = "Close elevator door";
        //transform.localPosition = interactableClose.localPosition;
        //GetComponent<BoxCollider>().size = colliderSizeClose;
    }

    private IEnumerator OpenVentCover()
    {
        float t = 0;

        Transform parentGb = transform.parent;
        while (transform.parent.localEulerAngles.z < 90 && t < 1.1f)
        {
            parentGb.localEulerAngles = new Vector3(parentGb.localEulerAngles.x, parentGb.localEulerAngles.y, Mathf.Lerp(0, 90, t));
            //gameObject.SetActive(false);
            Debug.Log("t = " + t);
            t += Time.deltaTime * 0.4f;
            yield return new WaitForSeconds(0.01f);
        }
        transform.parent.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator OpenDoor()
    {
        float t = 0;

        Transform door0 = transform.parent.GetChild(1);
        Transform door1 = transform.parent.GetChild(2);

        Vector3 velocity = Vector3.zero;

        while (t < 1.1f)// && door0.localScale.z > 0.01f && door1.localScale.z > 0.01f)// && l < 1000)
        {

            //door0.localPosition = Vector3.Lerp(door0.localPosition, new Vector3(door0.localPosition.x, door0.localPosition.y, door0.localPosition.z - 0.5f), t);
            //door1.localPosition = Vector3.Lerp(door1.localPosition, new Vector3(door1.localPosition.x, door1.localPosition.y, door1.localPosition.z + 0.5f), t);

            //door0.localPosition = Vector3.SmoothDamp(door0.localPosition)

            door0.localPosition = new Vector3(door0.localPosition.x, door0.localPosition.y, Mathf.Lerp(-0.5f, -0.5f - 0.5f, t));
            door1.localPosition = new Vector3(door1.localPosition.x, door1.localPosition.y, Mathf.Lerp(0.5f, 0.5f + 0.5f, t));
            
            //door0.localScale = new Vector3(1, 1, Mathf.Lerp(door0.localScale.z, 0, t));
            //door1.localScale = new Vector3(1, 1, Mathf.Lerp(door1.localScale.z, 0, t));

            //door0.localScale = Vector3.Lerp(door0.localScale, door0.localScale - new Vector3(0, 0, 0.1f), Time.deltaTime * 100);
            //door1.localScale = Vector3.Lerp(door1.localScale, door1.localScale - new Vector3(0, 0, 0.1f), Time.deltaTime * 10000);
            //++l;
            //t += Time.deltaTime * 0.04f; //For Vector3.Lerp
            t += Time.deltaTime * 0.4f; //For Mathf.Lerp
            //Debug.Log("t = " + t);
            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator OpenElevator()
    {
        float t = 0;
        Vector3 velocity = Vector3.zero;

        Transform doorL = elevatorScript.elevatorDown ? doorLGround : doorLTop;
        Transform doorR = elevatorScript.elevatorDown ? doorRGround : doorRTop;

        if (!Mathf.Approximately(doorR.localPosition.z, 0.13f))
        {
            while (t < 1f)
            {
                doorL.localPosition = new Vector3(doorL.localPosition.x, doorL.localPosition.y, Mathf.Lerp(-1.98f, -4.346f, t));
                doorR.localPosition = new Vector3(doorR.localPosition.x, doorR.localPosition.y, Mathf.Lerp(-2.03f, 0.13f, t));
                t += Time.deltaTime * 0.4f;
                yield return new WaitForSeconds(0.01f);
            }
        }
        gameObject.GetComponent<BoxCollider>().enabled = true;
        //transform.parent.GetChild(transform.GetSiblingIndex() + 1).GetComponent<BoxCollider>().enabled = true;
    }

    private IEnumerator CloseElevator()
    {
        float t = 1;
        Vector3 velocity = Vector3.zero;

        Transform doorL = elevatorScript.elevatorDown ? doorLGround : doorLTop;
        Transform doorR = elevatorScript.elevatorDown ? doorRGround : doorRTop;

        if (!Mathf.Approximately(doorR.localPosition.z, -2.03f))
        {
            while (t > 0f)
            {
                doorL.localPosition = new Vector3(doorL.localPosition.x, doorL.localPosition.y, Mathf.Lerp(-1.98f, -4.346f, t));
                doorR.localPosition = new Vector3(doorR.localPosition.x, doorR.localPosition.y, Mathf.Lerp(-2.03f, 0.13f, t));
                t -= Time.deltaTime * 0.4f;
                yield return new WaitForSeconds(0.01f);
            }
        }
        //gameObject.GetComponent<BoxCollider>().enabled = true;
        elevatorScript.canUse = true;
        elevatorScript.ActivateElevator();
    }

    public Item GetGunItem()
    {
        return null;
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
