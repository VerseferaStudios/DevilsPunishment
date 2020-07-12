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

    [Server]
    public void Spawn_ServerDoor(GameObject roomDoor)
    {
        NetworkServer.Spawn(roomDoor);
    }

}
