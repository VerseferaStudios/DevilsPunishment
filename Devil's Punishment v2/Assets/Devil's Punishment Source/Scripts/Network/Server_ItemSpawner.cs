using UnityEngine;
using Mirror;

public class Server_ItemSpawner : NetworkBehaviour
{
    public static Server_ItemSpawner sRef;
    private void Awake()
    {
        sRef = this;
    }

    [Server]
    public void Server_SpawnItem (GameObject prefab, Vector3 position, Vector3 eulerAngles, Transform parent)
    {
        Server_Spawnitem
            (
            new ItemSpawnInstructions_Server(prefab, position, eulerAngles, parent)
            );
    }

    [Server]
    public void Server_Spawnitem (ItemSpawnInstructions_Server serverInstructions)
    {
        Debug.Log(">>>>>SPAWNED OBJECT ON SERVER");
        // First, spawn the item on the server (or the client who is the server)
        GameObject newItem = Instantiate(serverInstructions.prefab, serverInstructions.position, Quaternion.Euler(serverInstructions.eulerAngles));/*, serverInstructions.parent);*/
        //newItem.GetComponent<InteractableLoot>().pos = serverInstructions.position;

        // Then, spawn the item over the network
        NetworkServer.Spawn(newItem);
    }

    [Server]
    public void Server_SpawnItemFromClient (ItemSpawnInstructions_Client clientInstructions)
    {
        Debug.Log(">>>>>SPAWNED OBJECT ON SERVER, FROM CLIENT");
        // First, spawn the item on the server (or the client who is the server)
        GameObject newItem = Instantiate(ResourceManager.instance.getResource(clientInstructions.resourceID), clientInstructions.position, Quaternion.Euler(clientInstructions.eulerAngles), null);

        // Set the item count
        newItem.GetComponent<InteractableLoot>().stock = clientInstructions.count;

        // Then, spawn the item over the network
        NetworkServer.Spawn(newItem);
    }

    [Server]
    public void Server_DestroyItemOnPickup(GameObject item)
    {
        NetworkServer.Destroy(item);
    }

}

/// <summary>
/// Item instructions that the server uses only
/// </summary>
[System.Serializable]
public struct ItemSpawnInstructions_Server
{
    public GameObject prefab;
    public Vector3 position;
    public Vector3 eulerAngles;
    public Transform parent;

    public ItemSpawnInstructions_Server (GameObject prefab, Vector3 position, Vector3 eulerAngles, Transform parent)
    {
        this.prefab = prefab;
        this.position = position;
        this.eulerAngles = eulerAngles;
        this.parent = parent;
    }

}

/// <summary>
/// Item instructions that clients send to the server
/// </summary>
[System.Serializable]
public struct ItemSpawnInstructions_Client
{
    public string resourceID;
    public int count;
    public Vector3 position;
    public Vector3 eulerAngles;

    public ItemSpawnInstructions_Client (string resourceID, int count, Vector3 position, Vector3 eulerAngles)
    {
        this.resourceID = resourceID;
        this.count = count;
        this.position = position;
        this.eulerAngles = eulerAngles;
    }
}
