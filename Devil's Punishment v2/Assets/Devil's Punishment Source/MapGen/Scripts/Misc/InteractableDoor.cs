using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class InteractableDoor : NetworkBehaviour, IInteractable
{
    //separate the scripts if needed
    public enum DoorType
    {
        ventCover,
        door,
    }

    public GameState.gameStateType triggerState;

    public DoorType doorType;

    public float timeToPickUp = .5f;

    public GameObject brokenFloorCollidors;

    private string promptString = "Open vent cover";

    public MeshRenderer meshRenderer_renderPlane;

    /*[SyncVar(hook = "UpdateDoorPosOverNetwork")] */
    public float doorSidePos;
    private Transform door0;
    private Transform door1;

    public bool isDisregardMainDoorPosSinceItIsntSpawnedInCode = false;

    /// <summary>
    /// Main posiiton of door or vent cover
    /// </summary>
    [SyncVar] public Vector3 mainDoorPos;

    /// <summary>
    /// For vents; the corridor spawn idx of the corridor to which this vent cover is connected. 
    /// Look up the ventToCorridor List Data and Data2ndFloor to get the Transform of the corridors for 1st and 2nd floors respectively
    /// </summary>
    [SyncVar] public int ventCorridorIdx = -1;

    /// <summary>
    /// For vents; grill box collider (non trigger)
    /// </summary>
    [SerializeField] private BoxCollider grillCollider;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (isDisregardMainDoorPosSinceItIsntSpawnedInCode)
        {

        }
        else
        {
            transform.position = mainDoorPos;
        }
    }

    //private int l = 0;

    private void Start()
    {
        switch (doorType)
        {
            case DoorType.ventCover:
                promptString = "Open vent cover";
                break;
            case DoorType.door:
                promptString = "Open door";
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
                //FMODUnity.RuntimeManager.PlayOneShot("event:/World/Doors/Underground/Vent_Open_Underground", transform.position);
                PlayerRemoteCallsBehaviour.instance.Cmd_OpenVentCover(/*transform.parent.GetComponent<NetworkIdentity>().*/netId);
                PlayerRemoteCallsBehaviour.instance.Cmd_PlaySoundOneShotOnOtherClients("event:/World/Doors/Underground/Vent_Open_Underground", transform.position);
                //StartCoroutine(OpenVentCover());
                break;

            case DoorType.door:
                GameState.gameState.addState(triggerState);
                Debug.Log("Opening door");
                PlayerRemoteCallsBehaviour.instance.Cmd_OpenDoor(/*transform.parent.GetComponent<NetworkIdentity>().*/netId);
                FMODUnity.RuntimeManager.PlayOneShot("event:/World/Doors/Underground/Door_Open_Underground", transform.position);
                //PlayerRemoteCallsBehaviour.instance.Cmd_OpenDoorOrVentCover(/*transform.parent.GetComponent<NetworkIdentity>().*/netId);
                PlayerRemoteCallsBehaviour.instance.Cmd_PlaySoundOneShotOnOtherClients("event:/World/Doors/Underground/Door_Open_Underground", transform.position);
                //StartCoroutine(OpenDoor());
                break;

        }
    }

    [Server]
    public IEnumerator OpenVentCover()
    {
        float t = 0;

        Transform holderT = transform.GetChild(1).GetChild(0);
        while (holderT.localEulerAngles.z < 90 && t < 1.1f)
        {
            holderT.localEulerAngles = new Vector3(holderT.localEulerAngles.x, holderT.localEulerAngles.y, Mathf.Lerp(0, 90, t));
            //gameObject.SetActive(false);
            Debug.Log("t = " + t);
            t += Time.deltaTime * 0.4f;
            yield return new WaitForSeconds(0.01f);
        }
        PlayerRemoteCallsBehaviour.instance.Rpc_ReEnableVentCover(netId);

        yield return new WaitForSeconds(0.5f);
    }

    private float grillColliderSizeZ, grillColliderCentreZ;

    /// <summary>
    /// Reduce the grill collider size when opening so that it doesnt hinder players
    /// </summary>
    public void ReduceGrillColliderSize()
    {
        grillColliderSizeZ = grillCollider.size.z;
        grillCollider.size = new Vector3(grillCollider.size.x / 2, grillCollider.size.y / 2, .001f);
        grillColliderCentreZ = grillCollider.center.z;
        grillCollider.center = new Vector3(grillCollider.center.x / 2, grillCollider.center.y / 2, 0);
    }

    /// <summary>
    /// Increase back the grill collider size so that players cant stick their head cam up the vent to see into the corridor
    /// </summary>
    public void IncreaseGrillColliderSize()
    {
        grillColliderSizeZ = grillCollider.size.z;
        grillCollider.size = new Vector3(grillCollider.size.x * 2, grillCollider.size.y * 2, grillColliderSizeZ);
        grillColliderCentreZ = grillCollider.center.z;
        grillCollider.center = new Vector3(grillCollider.center.x * 2, grillCollider.center.y * 2, grillColliderCentreZ);
    }

    //private void UpdateDoorPosOverNetwork(float oldPos, float newPos)
    //{
    //    door0.position = new Vector3(door0.position.x, door0.position.y, -newPos);
    //    door1.position = new Vector3(door1.position.x, door1.position.y, newPos);
    //}

    [Server]
    public IEnumerator OpenDoor()
    {
        //if(NetworkManager.singleton.mode == NetworkManagerMode.ServerOnly ||
        //   NetworkManager.singleton.mode == NetworkManagerMode.Host)
        if(isServer)
        {
            float t = 0;

            door0 = transform.GetChild(0);
            door1 = transform.GetChild(1);

            Vector3 velocity = Vector3.zero;

            while (t < 1.1f)// && door0.localScale.z > 0.01f && door1.localScale.z > 0.01f)// && l < 1000)
            {

                //door0.localPosition = Vector3.Lerp(door0.localPosition, new Vector3(door0.localPosition.x, door0.localPosition.y, door0.localPosition.z - 0.5f), t);
                //door1.localPosition = Vector3.Lerp(door1.localPosition, new Vector3(door1.localPosition.x, door1.localPosition.y, door1.localPosition.z + 0.5f), t);

                //door0.localPosition = Vector3.SmoothDamp(door0.localPosition)
                doorSidePos = Mathf.Lerp(0.5f, 0.5f + 0.5f, t);
                door0.localPosition = new Vector3(door0.localPosition.x, door0.localPosition.y, -doorSidePos);
                door1.localPosition = new Vector3(door1.localPosition.x, door1.localPosition.y, doorSidePos);

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
        else
        {
            Debug.Log("Trying to callServer fn OpenDoor() in client");
        }
    }

    public Item GetGunItem()
    {
        return null;
    }

    public void SetPlayerController(PlayerController_Revamped playerController)
    {
        
    }

    public void OnFocus()
    {

    }

    public void OnReleaseFocus()
    {

    }
}
