using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Server_DoorSpawner : NetworkBehaviour
{
    public static Server_DoorSpawner instance;
    private void Awake()
    {
        instance = this;
    }

    public GameObject roomDoorPrefab;

    [Server]
    public void Spawn_ServerDoor(Vector3 pos, Vector3 rot, Vector3 scale, GameState.gameStateType triggerState)
    {
        GameObject roomDoor = Instantiate(roomDoorPrefab, pos, Quaternion.Euler(rot));
        roomDoor.transform.localScale = scale;
        Debug.Log("0987 " + pos);
        if (roomDoor.TryGetComponent(out InteractableDoor interactableDoor))
        {
            interactableDoor.triggerState = triggerState;
            interactableDoor.mainDoorPos = pos;
        }
        NetworkServer.Spawn(roomDoor);
    }

}
