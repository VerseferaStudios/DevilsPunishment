using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class Server_ItemSpawner : NetworkBehaviour
{
    public static Server_ItemSpawner sRef;
    private void Awake()
    {
        sRef = this;
    }

    private void OnEnable()
    {
        RoomNew.MapGenCorridorsDone += ItemRigidbodiesReEnableGravity;
    }

    private void OnDisable()
    {
        RoomNew.MapGenCorridorsDone -= ItemRigidbodiesReEnableGravity;
    }

    //variable for item gen script since now item gen script is only there in server (currently as a component of the same gameobject as this script, Server_ItemSpawner)
    public ItemGen itemGenScript;

    private List<Rigidbody> itemRigidbodies;

    private void Start()
    {
        itemGenScript = GetComponent<ItemGen>();
        itemRigidbodies = new List<Rigidbody>();
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
    public void Server_SpawnItem(string resourceID, int count, Vector3 position, Vector3 eulerAngles)
    {
        Server_SpawnItemFromClient
            (
            new ItemSpawnInstructions_Client(resourceID, count, position, eulerAngles)
            );
    }

    [Server]
    private void Server_Spawnitem (ItemSpawnInstructions_Server serverInstructions)
    {
        Debug.Log(">>>>>SPAWNED OBJECT ON SERVER");
        // First, spawn the item on the server (or the client who is the server)
        GameObject newItem = Instantiate(serverInstructions.prefab, serverInstructions.position, Quaternion.Euler(serverInstructions.eulerAngles));/*, serverInstructions.parent);*/
        //newItem.GetComponent<InteractableLoot>().pos = serverInstructions.position;

        if(newItem.TryGetComponent(out Rigidbody rigidbody))
        {
            itemRigidbodies.Add(rigidbody);
        }
        else
        {
            Debug.Log("Error in finding item rigidbody, destroying item");
            Destroy(newItem);
            return;
        }

        // Then, spawn the item over the network
        NetworkServer.Spawn(newItem);
    }

    //check if its used..
    [Server]
    private void Server_SpawnItemFromClient (ItemSpawnInstructions_Client clientInstructions)
    {
        Debug.Log(">>>>>SPAWNED OBJECT ON SERVER, FROM CLIENT");
        // First, spawn the item on the server (or the client who is the server)
        GameObject newItem = Instantiate(ResourceManager.instance.getResource(clientInstructions.resourceID), clientInstructions.position, Quaternion.Euler(clientInstructions.eulerAngles), null);

        // Set the item count
        newItem.GetComponent<InteractableLoot>().stock = clientInstructions.count;
        newItem.gameObject.SetActive(true);

        if (newItem.TryGetComponent(out Rigidbody rigidbody))
        {
            itemRigidbodies.Add(rigidbody);
        }
        else
        {
            Debug.Log("Error in finding item rigidbody, destroying item");
            Destroy(newItem);
            return;
        }

        // Then, spawn the item over the network
        NetworkServer.Spawn(newItem);
    }

    [Server]
    public void Server_DestroyItemOnPickup(GameObject item)
    {
        NetworkServer.Destroy(item);
    }

    private void ItemRigidbodiesReEnableGravity()
    {
        for (int i = 0; i < itemRigidbodies.Count; i++)
        {
            itemRigidbodies[i].useGravity = true;
        }
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
