using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class MapGen3 : MapGenBase
{
    //first we'll see the ground floor
    //10 x 10 cube
    //later//private int gridSize;
    
    public enum RoomSpawnType
    {
        Random,
        RoundRobin
    }
    [Header("Dev")]
    public RoomSpawnType roomSpawnMethod;

    //public List<Vector2> allRooms = new List<Vector2>();

    private ArrayList gameObjectDetails = new ArrayList();

    [Header("ScriptableObjects")]
    public StateData StateData, GoodStates;
    public ReloadGoodStates ReloadGoodStatesData;

    protected override void Start()
    {
        base.Start();
        Debug.Log("mapgen3");
    }

    protected override RoomNew AddRoomNewCorrectly()
    {
        return gameObject.AddComponent<RoomNew>();
    }

    protected override void SetFloorNameForHolder()
    {
        floorNameForHolder = "1st Floor";
    }

    protected override void SetDoorTag()
    {
        floorDoorTag = "Corridor Spawn Points";
    }

    protected override void SetRoomTag()
    {
        floorRoomTag = "Room";
    }

    protected override void SetRoomHeight()
    {
        roomHeight = 0;
    }

    protected override bool AddingLiftForHigherFloors(int k)
    {
        Debug.Log("lift stuff in 1st floor");
        return false;
    }

    protected override void AddLiftPosForLowerRoom(int i, Vector3 roomPos)
    {
        if (i == 1)
        {
            Data2ndFloor.instance.liftRoomPos = roomPos;
        }
    }

    protected override void FirstFloorExtraInitRoom()
    {
        float[] arr = new float[2];
        arr[0] = 28;
        arr[1] = 28;
        allRooms.Add(arr);
    }

    private IEnumerator WaitForaWhile()
    {
        yield return new WaitUntil(() => Input.GetKey(KeyCode.P));
        Random.InitState(mapseed);  // changed this to our given mapseed
        Rooms();
    }

    public int mapseed;

    //public RoomReferencesStatic roomReferences;
    public Vector3 prevTopRight, prevBottomLeft;

    //private void Update()
    //{
    //    if (roomReferences.topRightMapGen.position != prevTopRight ||
    //       roomReferences.bottomLeftMapGen.position != prevBottomLeft)
    //    {
    //        prevTopRight = roomReferences.topRightMapGen.position;
    //        prevBottomLeft = roomReferences.bottomLeftMapGen.position;

    //        int ctr = 0;
    //        for (int q = -Mathf.RoundToInt(Mathf.RoundToInt(roomReferences.topRightMapGen.position.x) / 4); q <= -Mathf.RoundToInt(Mathf.RoundToInt(roomReferences.bottomLeftMapGen.position.x) / 4); q++)
    //        {
    //            //Debug.Log("|");
    //            for (int r = -Mathf.RoundToInt(Mathf.RoundToInt(roomReferences.topRightMapGen.position.z) / 4); r <= -Mathf.RoundToInt(Mathf.RoundToInt(roomReferences.bottomLeftMapGen.position.z) / 4); r++)
    //            {
    //                //Debug.Log("-");

    //                squareGrid.tiles[q, r].tile = TileType.Wall;
    //                if (isDevMode)
    //                {
    //                    if (ctr < cubes.Count)
    //                    {
    //                        cubes[ctr].position = new Vector3(q * -4, 0, r * -4);
    //                    }
    //                    else
    //                    {
    //                        cubes.Add(Instantiate(testGridCube, new Vector3(q * -4, 0, r * -4), Quaternion.identity).transform);
    //                        //Instantiate(testGridCube, new Vector3(q * -4, 0, r * -4), Quaternion.identity);
    //                    }
    //                    ctr++;
    //                }
    //            }
    //        }

    //    }
    //}

    protected override void syncronizeSeeds(int seed)
    {
        if (GetComponent<MapGen2ndFloor>() != null)// .TryGetComponent(typeof(HingeJoint), out Component component))//(out MapGen2ndFloor mapGen2ndFloor))
        {
            GetComponent<MapGen2ndFloor>().setSeed(seed);
        }
    }
    
    // ---------------- Reload Map Gen with good states ----------------
    public void ReloadMapGen()
    {

        ReloadGoodStatesData.isReloadingGoodStates = true;
        /*
        Random.state = GoodStates.states[1];
        //ReloadGoodStatesData.isReloadingGoodStates = false;
        ReloadGoodStatesData.i++;
        if (ReloadGoodStatesData.i >= GoodStates.states.Count)
        {
            ReloadGoodStatesData.i = 0;
        }
        */
        //Data.instance.roomsLoaderPrefab = roomsLoaderPrefab;
        //Data.instance.StartInstantiateCo();

        Destroy(mapGenHolderTransform.gameObject);

        //CreateHolderForMapGen();

        Rooms();
        
        //rooms();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ---------------------------- Connect init pos to map gen nearest room ----------------------------
    //private void ConnectToMapGen(RoomNew roomNewScript)
    //{
    //    float min = 9999;
    //    int minIdx = -1;
    //    //-48, 0, -24
    //    for (int i = 0; i < allRooms.Count; i++)
    //    {
    //        float current = -48 + ((float[])allRooms[i])[1] + -24 + ((float[])allRooms[i])[0];
    //        if(current < min)
    //        {
    //            min = current;
    //            minIdx = i;
    //        }
    //    }
    //    if(minIdx != -1)
    //    {
    //        StartCoroutine(roomNewScript.ConnectTwoRooms(new Vector3(-((float[])allRooms[minIdx])[1] + 24, 1, -((float[])allRooms[minIdx])[0]), new Vector3(-48, 1, -24), "Door+x", "Door-z", Vector3.zero, new Vector3(-44, 1, -24 + 24), true)); 
    //    }
    //    else
    //    {
    //        Debug.Log("ERROR!!!!");
    //    }
    //}

    // ---------------------------- Existing corridors offset and Square Grid stuff for AStar ----------------------------
    private void ExistingCorridorsFn(RoomReferencesStatic roomReferences, int yRotation, float offset)
    {
        for (int q = 0; q < roomReferences.existingCorridors.Length; q++)
        {
            int x = Mathf.RoundToInt(roomReferences.existingCorridors[q].transform.position.x / -4);
            int z = Mathf.RoundToInt(roomReferences.existingCorridors[q].transform.position.z / -4);
            squareGrid.tiles[x, z].corridorIdx = 0; // For I corrdor
            squareGrid.tiles[x, z].corridorIdx = yRotation;

            //roomReferences.existingCorridors[q].transform.localPosition = new Vector3(roomReferences.existingCorridors[q].transform.localPosition.x, roomReferences.existingCorridors[q].transform.localPosition.y, roomReferences.existingCorridors[q].transform.localPosition.z + offset);

        }
    }

    /*
    private GameObject corridorTypes(int corridorCode)
    {
        if (corridorCode == 0)
        {
            return noDoor;
        }
        else if (corridorCode == 1)
        {
            return narrowDoor;
        }
        else //if (corridorCode == 2)
        {
            return wideDoor;
        }
    }
    */
}
