using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableElevator : MonoBehaviour, IInteractable
{

    public float timeToPickUp = .5f;
    private float elevatorCoolDownTime = 15f, elevatorInteractTimestamp;
    public Transform doorLGround, doorRGround;
    public Transform doorLTop, doorRTop;

    private string promptString = "Close elevator door";

    //[SerializeField] private bool isElevatorOpen = true;

    //public Transform interactableClose;
    private Vector3 colliderSizeClose = new Vector3(0.5f, 0.5f, 0.5f);//, interactableOpen, colliderSizeOpen;

    public Elevator elevatorScript;

    public ElevatorInteractable_Disable_Controller elevatorInteractable_Disable_Script;

    //private int l = 0;

    private void Start()
    {

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
        elevatorInteractable_Disable_Script.EnableOrDisableInteractables(false);
        //gameObject.GetComponent<BoxCollider>().enabled = false;
        /*
        if (!isElevatorOpen)
        {
            CallOpenElevatorFns();
        }
        else*/
        {
            Debug.Log("Closing elevator");
            elevatorInteractTimestamp = Time.time;
            StartCoroutine(CloseElevator());
            //promptString = "Open elevator door";
            //transform.localPosition = interactableOpen;
            //GetComponent<BoxCollider>().size = colliderSizeOpen;
        }
        //isElevatorOpen = !isElevatorOpen;
    }

    public void CallOpenElevatorFns()
    {
        Debug.Log("Opening elevator");
        StartCoroutine(OpenElevator());
        //promptString = "Close elevator door";
        //transform.localPosition = interactableClose.localPosition;
        //GetComponent<BoxCollider>().size = colliderSizeClose;
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
        yield return new WaitForSeconds(elevatorCoolDownTime - (Time.time - elevatorInteractTimestamp));
        elevatorInteractable_Disable_Script.EnableOrDisableInteractables(true);
        //gameObject.GetComponent<BoxCollider>().enabled = true;
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
