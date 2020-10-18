using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerRemoteCallsBehaviour : NetworkBehaviour
{
    public static PlayerRemoteCallsBehaviour instance;
    public PlayerRefsDataBehaviour playerRefsData;

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
    /// <param name="netId"></param>
    [Command]
    public void Cmd_OpenDoor(uint netId)
    {
        if (NetworkIdentity.spawned.TryGetValue(netId, out NetworkIdentity netIdentity))
        {
            DisableDoorTriggerCollider(netIdentity.GetComponent<BoxCollider>());
            Rpc_DisableDoorTriggerCollider(netId);
            InteractableDoor interactableDoor = netIdentity.GetComponent<InteractableDoor>();
            StartCoroutine(interactableDoor.OpenDoor());
        }
        else
        {
            Debug.LogWarning("No such door on server; uint netId = " + netId);
        }
    }
    
    /// <summary>
    /// Helper function to disable a door collider
    /// </summary>
    /// <param name="boxCollider"></param>
    public void DisableDoorTriggerCollider(BoxCollider boxCollider)
    {
        boxCollider.enabled = false;
    }

    /// <summary>
    /// Disables the door collider in all clients
    /// </summary>
    /// <param name="netId"> The netowrk identity uint of the door whose collider has to be disabled </param>
    [ClientRpc]
    private void Rpc_DisableDoorTriggerCollider(uint netId)
    {
        if (NetworkIdentity.spawned.TryGetValue(netId, out NetworkIdentity netIdentity))
        {
            DisableDoorTriggerCollider(netIdentity.GetComponent<BoxCollider>());
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

}
