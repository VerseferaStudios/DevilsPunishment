using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerRemoteCallsBehaviour : NetworkBehaviour
{
    public static PlayerRemoteCallsBehaviour instance;

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

    [Command]
    public void Cmd_OpenDoor(uint netId)
    {
        if (NetworkIdentity.spawned.TryGetValue(netId, out NetworkIdentity netIdentity))
        {
            RemoveDoorTriggerCollider(netIdentity.GetComponent<BoxCollider>());
            Rpc_RemoveDoorTriggerCollider(netId);
            InteractableDoor interactableDoor = netIdentity.GetComponent<InteractableDoor>();
            StartCoroutine(interactableDoor.OpenDoor());
        }
        else
        {
            Debug.LogWarning("No such door on server; uint netId = " + netId);
        }
    }
    
    public void RemoveDoorTriggerCollider(BoxCollider boxCollider)
    {
        boxCollider.enabled = false;
    }

    [ClientRpc]
    private void Rpc_RemoveDoorTriggerCollider(uint netId)
    {
        if (NetworkIdentity.spawned.TryGetValue(netId, out NetworkIdentity netIdentity))
        {
            RemoveDoorTriggerCollider(netIdentity.GetComponent<BoxCollider>());
        }
        else
        {
            Debug.LogWarning("No such door on server; uint netId = " + netId);
        }
    }

}
