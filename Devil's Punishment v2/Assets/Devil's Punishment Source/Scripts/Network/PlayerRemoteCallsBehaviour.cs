using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerRemoteCallsBehaviour : NetworkBehaviour
{
    public static PlayerRemoteCallsBehaviour instance;
    public PlayerRefsDataBehaviour playerRefsData;
    public int seed;
    public int[] ventCoverIndices;

    private void OnEnable()
    {
        PlayerController_Revamped.CallbackAssignStaticInstances += AssignInstance;
    }
    private void OnDestroy()
    {
        PlayerController_Revamped.CallbackAssignStaticInstances -= AssignInstance;
    }

    private void AssignInstance()
    {
        instance = this;
    }

    //public override void OnStartServer()
    //{
    //    seed = Network_Transmitter.transmitter.getSeed();
    //    Random.InitState(seed);
    //    base.OnStartServer();
    //}

    /// <summary>
    /// Sending command to server to destroy item which we just picked up
    /// </summary>
    /// <param name="item"> item which we just picked, type GameObject </param>
    [Command]
    public void Cmd_DestroyItemOnPickup(GameObject item)
    {
        Server_ItemSpawner.sRef.Server_DestroyItemOnPickup(item);
    }

    /// <summary>
    /// Command sent to server to Drop item from player's inventory and hence spawn in world
    /// Take care for item duplicating cheats, check in server if player has item in his inventory firstly
    /// </summary>
    /// <param name="resourceID"></param>
    /// <param name="count"></param>
    /// <param name="position"></param>
    /// <param name="eulerAngles"></param>
    [Command]
    public void Cmd_DropItem(string resourceID, int count, Vector3 position, Vector3 eulerAngles)
    {
        Server_ItemSpawner.sRef.Server_SpawnItem(resourceID, count, gameObject.transform.position, gameObject.transform.eulerAngles);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="objectNetId"></param>
    [Command]
    public void Cmd_OpenDoor(uint objectNetId)
    {
        if (NetworkIdentity.spawned.TryGetValue(objectNetId, out NetworkIdentity netIdentity))
        {
            //if the door or vent cover netId exists, on the server

            ventCoverNetIds.Add(objectNetId);


            DisableDoorOrVentCoverTriggerCollider(netIdentity.GetComponent<BoxCollider>());
            Rpc_DisableDoorOrVentCoverTriggerCollider(objectNetId);
            InteractableDoor interactableDoor = netIdentity.GetComponent<InteractableDoor>();
            StartCoroutine(interactableDoor.OpenDoor());

            ////enable net transform and net transform child just b4 starting to open door?
            //Rpc_OpenDoorOrVentCoverInOtherClients(objectNetId);
        }
        else
        {
            //if the door or vent cover netId doesn't exist, on the server
            Debug.LogWarning("No such door on server; uint netId = " + objectNetId);
        }
    }


    /// <summary>
    /// List to store already opened vent covers, to avoid two different players opening it simultaneously (maybe not very important but still)
    /// </summary>
    private List<uint> ventCoverNetIds = new List<uint>();

    /// <summary>
    /// Main Command to open a vent cover
    /// Calls functions to disable trigger collider, and rpc for the same
    /// Calls the actual lerp to open vent cover (Server)
    /// Also calls rpc for vent open helper functions `Rpc_VentCoverOpenHelper(uint netIdObject)`
    /// </summary>
    /// <param name="objectNetId"></param>
    [Command]
    public void Cmd_OpenVentCover(uint objectNetId)
    {
        if (ventCoverNetIds.Contains(objectNetId))
        {
            //If door or vent cover was already opened
            Debug.LogWarning("Already opened door, or vent cover");
        }
        else
        {
            //If door or vent cover wasn't already opened

            if (NetworkIdentity.spawned.TryGetValue(objectNetId, out NetworkIdentity netIdentity))
            {
                //if the door or vent cover netId exists, on the server

                ventCoverNetIds.Add(objectNetId);


                DisableDoorOrVentCoverTriggerCollider(netIdentity.GetComponent<BoxCollider>());
                Rpc_DisableDoorOrVentCoverTriggerCollider(objectNetId);
                InteractableDoor interactableDoor = netIdentity.GetComponent<InteractableDoor>();

                //Calls the actual lerp to open vent cover (Server)
                StartCoroutine(interactableDoor.OpenVentCover());

                Rpc_VentCoverOpenHelper(objectNetId);

                ////enable net transform and net transform child just b4 starting to open door?
                //Rpc_OpenDoorOrVentCoverInOtherClients(objectNetId);
            }
            else
            {
                //if the door or vent cover netId doesn't exist, on the server
                Debug.LogWarning("No such door on server; uint netId = " + objectNetId);
            }
        }
    }

    /// <summary>
    /// Rpc to all clients, this takes care of vent cover opening side functions, helper functions like broken floor colliders etc
    /// Note that vent cover opening is handled in server only and network transform (child) takes care of the rest
    /// </summary>
    /// <param name="netIdObject"></param>
    [ClientRpc]
    private void Rpc_VentCoverOpenHelper(uint netIdObject)
    {

        if (NetworkIdentity.spawned.TryGetValue(netIdObject, out NetworkIdentity netIdentity))
        {
            //if the door or vent cover netId exists, on the server

            InteractableDoor interactableDoor = netIdentity.GetComponent<InteractableDoor>();

            //grill box collider size decrease
            interactableDoor.ReduceGrillColliderSize();
            interactableDoor.MoveInteractTriggerCollider(true);

            Debug.Log("netIdentity.transform.position.y = " + netIdentity.transform.position.y);
            Debug.Log("interactableDoor.ventCorridorIdx = " + interactableDoor.ventCorridorIdx);

            //Debug.Log("Data.instance.ventToCorridorDict.Count = " + Data.instance.ventToCorridorDict.Count);
            //Debug.Log(Data.instance.ventToCorridorDict.Keys);
            //Debug.Log(Data.instance.ventToCorridorDict.Keys.Count);

            //Debug.Log("Data2ndFloor.instance.ventToCorridorDict.Count = " + Data2ndFloor.instance.ventToCorridorDict.Count);
            //Debug.Log(Data2ndFloor.instance.ventToCorridorDict.Keys);
            //Debug.Log(Data2ndFloor.instance.ventToCorridorDict.Keys.Count);

            Transform t = (netIdentity.transform.position.y < 10) ?
                Data.instance.ventToCorridorDict[interactableDoor.ventCorridorIdx] :
                Data2ndFloor.instance.ventToCorridorDict[interactableDoor.ventCorridorIdx];
            //Debug.Log("t name  = " + t.name);
            //Debug.Log("t tag  = " + t.tag);
            //Debug.Log("t position  = " + t.position);
            if (t.tag.StartsWith("Corr"))
            {
                t = t.GetChild(2).GetChild(0); //WILL WORK ON CORRIDOR ONLY!!! TAKE ROOM SEPARATE
                Instantiate(interactableDoor.brokenFloorCollidors, t.position, t.rotation, t.parent);
                t.gameObject.SetActive(false);
            }

            //timeToPickUp = float.MaxValue;
            interactableDoor.meshRenderer_renderPlane.enabled = true; // this disappears the thing?
        }
        else
        {
            //if the door or vent cover netId doesn't exist, on the server
            Debug.LogWarning("No such door on client; uint netId = " + netIdObject);
        }

    }

    [ClientRpc]
    public void Rpc_ReEnableVentCover(uint netIdObject)
    {
        if (NetworkIdentity.spawned.TryGetValue(netIdObject, out NetworkIdentity netIdentity))
        {
            //if the door or vent cover netId exists, on the server
            //Transform holderT = transform.GetChild(1).GetChild(0);
            netIdentity.GetComponent<BoxCollider>().enabled = true;
            //holderT.gameObject.SetActive(true);
        }
        else
        {
            //if the door or vent cover netId doesn't exist, on the server
            Debug.LogWarning("No such door on client; uint netId = " + netIdObject);
        }
    }
    
    /// <summary>
    /// Helper function to disable a door collider
    /// </summary>
    /// <param name="boxCollider"></param>
    public void DisableDoorOrVentCoverTriggerCollider(BoxCollider boxCollider)
    {
        boxCollider.enabled = false;
    }

    /// <summary>
    /// Disables the door collider in all clients
    /// </summary>
    /// <param name="netId"> The netowrk identity uint of the door whose collider has to be disabled </param>
    [ClientRpc]
    private void Rpc_DisableDoorOrVentCoverTriggerCollider(uint netId)
    {
        if (NetworkIdentity.spawned.TryGetValue(netId, out NetworkIdentity netIdentity))
        {
            DisableDoorOrVentCoverTriggerCollider(netIdentity.GetComponent<BoxCollider>());
        }
        else
        {
            Debug.LogWarning("No such door on server; uint netId = " + netId);
        }
    }

    /// <summary>
    /// Command running at server, when player crouches, for player model visual offset
    /// </summary>
    /// <param name="newPosY"></param>
    [Command]
    public void Cmd_PlayerCrouchOffset(float newPosY, NetworkIdentity netIdPlayer)
    {
        //PlayerRefsDataBehaviour playerRefsData = GetComponent<PlayerRefsDataBehaviour>();//netIdPlayer.GetComponent<PlayerRefsDataBehaviour>();
        Vector3 playerModelPos = playerRefsData.playerModel.transform.localPosition;
        Vector3 newPos = new Vector3(playerModelPos.x, newPosY, playerModelPos.z);
        //if (i % 4 == 0 || i % 4 == 1)
        //{
        //StartCoroutine(LerpCrouchHelper(newPos)); //needed when using rpc?.. but it IS needed when using net transform(child)
        //}
        //else
        //{
        //    LerpHelperFn(newPos);
        //}
        //++i;
        Rpc_ShowCrouchOffsetToAllClients(newPos, netIdPlayer);
    }

    /// <summary>
    /// Rpc fuction to show the crouch offset on player model to all client inclusing current one
    /// This is used when not using Network Transform (Child)
    /// In this case there's no need to move the player model on the server.. (make sure)
    /// </summary>
    /// <param name="newOffset"></param>
    [ClientRpc]
    private void Rpc_ShowCrouchOffsetToAllClients(Vector3 newPos, NetworkIdentity netIdPlayer)
    {
        PlayerRefsDataBehaviour playerRefsData = GetComponent<PlayerRefsDataBehaviour>();//netIdPlayer.GetComponent<PlayerRefsDataBehaviour>();
        Debug.Log("lerp started + newPos = " + newPos);
        //playerRefsData.playerModel.transform.position = newPos;
        StartCoroutine(LerpCrouchHelper(newPos));
    }

    private void LerpHelperFn(Vector3 endPos)
    {
        playerRefsData.playerModel.transform.localPosition = endPos;
        Debug.Log("quick move done; endPos = " + endPos);
    }

    private int i = 0;
    private float crouchLerpSmooth = 0.25f;

    /// <summary>
    /// Lerp to smoothly move the player model which crouching
    /// </summary>
    /// <param name="playerModel"></param>
    /// <param name="endPos"></param>
    /// <returns></returns>
    private IEnumerator LerpCrouchHelper(Vector3 endPos)
    {
        Transform playerModel = playerRefsData.playerModel.transform;
        Vector3 startPos = new Vector3(endPos.x, endPos.y == -0.96f ? -0.474f : -0.96f, endPos.z);
        float t = 0;

        while (t < 1)
        {
            playerModel.localPosition = Vector3.Lerp(startPos, endPos, t);//new Vector3(endPos.x, Mathf.Lerp(startPos.y, endPos.y, t), endPos.z);
            t += Time.deltaTime / crouchLerpSmooth;
            yield return null;
        }
        playerModel.localPosition = endPos;
        Debug.Log("lerp done; endPos = " + endPos);
    }

    ///// <summary>
    ///// Return a random float number between min [inclusive] and max [inclusive] (Read Only).
    ///// </summary>
    ///// <param name="min"></param>
    ///// <param name="max"></param>
    ///// <returns></returns>
    //[Command]
    //public float Cmd_Random(float min, float max)
    //{
    //    return Random.Range(min, max);
    //}

    ///// <summary>
    ///// Return a random integer number between min [inclusive] and max [exclusive] (Read Only).
    ///// </summary>
    ///// <param name="min"></param>
    ///// <param name="max"></param>
    ///// <returns></returns>
    //[Command]
    //public int Cmd_Random(int min, int max)
    //{
    //    return Random.Range(min, max);
    //}

    // ================================ SOUNDS ================================ 
    //private enum Sound
    //{
    //    Door,
    //    VentCover
    //}
    [Command]
    public void Cmd_PlaySoundOneShotOnOtherClients(string path, Vector3 pos)
    {
        Rpc_PlaySoundOneShot(path, pos);
    }

    [ClientRpc]
    private void Rpc_PlaySoundOneShot(string path, Vector3 pos)
    {
        Debug.Log("Rpc here");
        FMODUnity.RuntimeManager.PlayOneShot(path, pos);
    }

    //[ClientRpc/*(excludeOwner = true)*/]
    //public void Rpc_VentCoverIndicies(int[] ventCoverIndices)
    //{
    //    this.ventCoverIndices = ventCoverIndices;
    //}

    /// <summary>
    /// Helper function for vent cover spawn
    /// </summary>
    /// <param name="posI"></param>
    /// <param name="ventCoverTag"></param>
    /// <param name="ventCover"></param>
    /// <param name="iCorrdorSpawnCtrForVent"> the corridor spawn idx of the corridor to which this vent cover is connected </param>
    [Server]
    public void VentCoverSpawnHelper(Vector3 posI, string ventCoverTag, GameObject ventCover, int iCorrdorSpawnCtrForVent)
    {
        Debug.Log("spawning vent cover at " + posI);
        GameObject ventCoverGb = Instantiate(ventCover, posI, Quaternion.Euler(0, /*PlayerRemoteCallsBehaviour.instance.Cmd_Random*/Random.Range(0, 3) * 90, 0)/*, currentCorridor.transform*/);
        ventCoverGb.transform.GetChild(1).GetChild(1).tag = ventCoverTag;
        if (ventCoverGb.TryGetComponent(out InteractableDoor interactableDoor))
        {
            //interactableDoor.triggerState = triggerState;
            interactableDoor.mainDoorPos = posI;
            Debug.Log("iCorrdorSpawnCtrForVent = " + iCorrdorSpawnCtrForVent);
            interactableDoor.ventCorridorIdx = iCorrdorSpawnCtrForVent;
        }
        else
        {
            //error
            Debug.LogError("Cant find InteractableDoor component on root of grill with frame on server");
        }
        NetworkServer.Spawn(ventCoverGb);
    }

}
