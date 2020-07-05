using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Collections;

public class MapGen2ndFloor : MapGenBase
{
    //first we'll see the ground floor
    //10 x 10 cube
    //later//private int gridSize;

    public GameObject GUIProgress;

    private ArrayList gameObjectDetails = new ArrayList();

    [Header("ScriptableObjects")]
    public StateData StateData, GoodStates;
    public ReloadGoodStates ReloadGoodStatesData;

    protected override void Start()
    {
        base.Start();
        Debug.Log("mapgen2ndfloor");
    }

    protected override RoomNew AddRoomNewCorrectly()
    {
        return gameObject.AddComponent<RoomNew2ndFloor>();
    }

    protected override void SetVentCoverTag()
    {
        ventCoverTag = Constants.sRef.TAG_VENTSPAWNFLOOR2;
    }

    protected override void SetFloorNameForHolder()
    {
        floorNameForHolder = "2nd Floor";
    }

    protected override void SetDoorTag()
    {
        floorDoorTag = "Corridor Spawn Points 2nd Floor";
    }

    protected override void SetRoomTag()
    {
        floorRoomTag = "Room 2nd Floor";
    }

    protected override void SetRoomHeight()
    {
        roomHeight = Data2ndFloor.instance.floor2Height;
    }

    public void setSeed(int seed)
    {
        Random.InitState(seed);
        //StartGen();
        startMapGeneration(seed);
    }

    //change to an appropriate general fn name
    protected override void FirstFloorExtraInitRoom()
    {
        if (isDevMode)
        {
            testGridPlaneHolder.position += new Vector3(0, roomHeight, 0);
        }
    }

    private void StartGen()
    {
        n = numberOfRooms - 1;

        float x = - (48 * ((float)(mapSizeX - 1) / 2)) - 28;
        float z = - (48 * ((float)(mapSizeZ - 1) / 2)) - 28;
        mapCentre = new Vector2(x, z);
        Debug.Log(mapCentre);

        GUIProgress.SetActive(true);

        //CreateHolderForMapGen();

        //Random.state = GoodStates.states[0];
        //StateData.states.Add(Random.state);
        StartCoroutine(StartScriptAfterDelay());
        Data2ndFloor.instance.roomsLoaderPrefab = roomsLoaderPrefab;
        Data2ndFloor.instance.corridorT1 = corridors[3];
        Data2ndFloor.instance.corridorT2 = corridors[4];
        Data2ndFloor.instance.corridorX = corridors[5];
        Data2ndFloor.instance.ventT = vents[3];
        Data2ndFloor.instance.ventX = vents[5];
        Data2ndFloor.instance.xSize = xSize;
        Data2ndFloor.instance.zSize = zSize;
        //NavMeshScript.instance.updateNavMesh();
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
        //Data2ndFloor.instance.roomsLoaderPrefab = roomsLoaderPrefab;
        //Data2ndFloor.instance.StartInstantiateCo();

        Destroy(mapGenHolderTransform.gameObject);

        //CreateHolderForMapGen();

        StartCoroutine(StartScriptAfterDelay());

        //rooms();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ------------- Start Script after delay (to wait for lift room) -------------
    private IEnumerator StartScriptAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        Data.instance.canStartCorridorTestSpawner = false;
        Rooms();
    }

    //public override void Rooms()
    //{
    //    /*
    //    if (ReloadGoodStatesData.isReloadingGoodStates)
    //    {
    //        Random.state = GoodStates.states[ReloadGoodStatesData.i];
    //        //ReloadGoodStatesData.isReloadingGoodStates = false;
    //        ReloadGoodStatesData.i++;
    //        if (ReloadGoodStatesData.i >= GoodStates.states.Count)
    //        {
    //            ReloadGoodStatesData.i = 0;
    //        }
    //    }
    //    */
    //    //Make this while and next loop into one? will Collisions be a prob?

    //    RoomPos(k, l);

    //    //while (k < n && l < 1000)
    //    //{
    //    //    float[] arr = new float[2];

    //    //    // ------------------- BOUNDS or SIZE of the grid -------------------



    //    //    // 400 - 20 = 380 (MAX)
    //    //    // 0 + 20 = 20 (MIN)
    //    //    //Increments of 40

    //    //    //arr[0] = 40 * Random.Range(0, 9) + 20;  //9 coz -> 9 * 40 + 20 = 380
    //    //    //arr[1] = 40 * Random.Range(0, 9) + 20;


    //    //    // 480 - 20 = 460 (MAX)
    //    //    // 0 + 28 = 28 (MIN)
    //    //    //Increments of 40

    //    //    arr[0] = 48 * Random.Range(0, mapSizeZ) + 28;  //9 coz -> 9 * 48 + 28 = 460
    //    //    arr[1] = 48 * Random.Range(0, mapSizeX) + 28;


    //    //    //arr[0] = Random.Range(/*11*/ + 1 + (int)(zSize/2), /*-11*/ -1 + 399 - (int)(xSize / 2)); //0,0 is the top left cell
    //    //    //arr[1] = Random.Range(/*11*/ + 1 + (int)(zSize / 2), /*-11*/ -1 + 399 - (int)(xSize / 2)); //0,0 is the top left cell


    //    //    /*
    //    //    if(k == 0)
    //    //    {
    //    //        arr[1] = 50;
    //    //        arr[0] = 80;
    //    //    }
    //    //    else
    //    //    {
    //    //        arr[1] = 50;
    //    //        arr[0] = 10;
    //    //    }
    //    //    */
    //    //    // ------------------- Integer positions in GRID / positions according to sizes of rooms in GRID fashion -------------------
    //    //    //arr[0] = Mathf.Round(((((int)arr[0])/zSize) * zSize) / zSize) * zSize;
    //    //    //arr[1] = Mathf.Round(((((int)arr[1])/xSize) * xSize) / zSize) * zSize;

    //    //    // ------------------- Checks for collisions between rooms  -------------------
    //    //    if (NoCollisions(arr))
    //    //    {
    //    //        allRooms.Add(arr);
    //    //        ++k;
    //    //    }
    //    //    ++l;
    //    //}

    //    // ------------------- RANDOMLY choosing ROOMS to spawn  -------------------
    //    ItemGen itemGenScript = GetComponent<ItemGen>();
    //    for (int i = 1; i < k; i++)
    //    {
    //        GameObject roomToSpawn = generatorRoom;
    //        float yCoord = 1.5f; // Beware, its for gen room
    //        if(i - 1 < staticRooms.Length)
    //        {
    //            roomToSpawn = staticRooms[i - 1];
    //            if (staticRooms[i - 1].name.Equals("Start Room"))
    //            {
    //                yCoord = 0.064f;
    //            }
    //            else if (staticRooms[i - 1].name.Equals("End Room"))
    //            {
    //                yCoord = 0.5f;
    //            }
    //            else if (staticRooms[i - 1].name.Equals("Laser Room"))
    //            {
    //                yCoord = 1f;
    //            }
    //            else if (staticRooms[i - 1].name.Equals("Elevator Room"))
    //            {
    //                yCoord = 0.2f;
    //            }
    //            else
    //            {
    //                yCoord = 0f;
    //            }
    //        }
    //        else
    //        {
    //            switch (Random.Range(1, 4))
    //            {
    //                case 0:
    //                    roomToSpawn = startRoom;
    //                    yCoord = 0.064f;
    //                    break;
    //                case 1:
    //                    roomToSpawn = endRoom;
    //                    yCoord = 0.5f;
    //                    break;
    //                case 2:
    //                    roomToSpawn = laserRoom;
    //                    yCoord = 1;
    //                    break;
    //                    /*
    //            case 3:
    //                roomToSpawn = roomT;
    //                break;
    //            case 4:
    //                roomToSpawn = room4;
    //                break;*/
    //            }
    //        }

    //        float yRotation = LookToMapCentre(new Vector2(-((float[])allRooms[i])[1], -((float[])allRooms[i])[0]));//Random.Range(0, 4) * 90;
    //        Vector3 roomPos = new Vector3(-((float[])allRooms[i])[1], Data2ndFloor.instance.floor2Height + yCoord, -((float[])allRooms[i])[0]);
    //        /*
    //        if (i == 0)
    //        {
    //            float oldY = roomPos.y;
    //            roomPos = Data2ndFloor.instance.liftRoomPos;
    //            roomPos.y = oldY;
    //        }
    //        */
    //        GameObject spawnedRoom = Instantiate(roomToSpawn, roomPos, Quaternion.Euler(0, yRotation, 0), mapGenHolderTransform);

    //        spawnedRoom.tag = "Room 2nd Floor";

    //        spawnedRoom.transform.GetChild(1).tag = "Corridor Spawn Points 2nd Floor";

    //        RoomReferencesStatic roomReferences = spawnedRoom.GetComponent<RoomReferencesStatic>();

    //        itemGenScript.SpawnItems(roomReferences.bottomLeftItemGen.position, roomReferences.topRightItemGen.position, 6, spawnedRoom.transform);

    //        SpawnVentCoverInRoom(i, k, roomReferences.ventParent);

    //        CallOffsetAndDoorAndSqGridFns(spawnedRoom, yRotation, roomReferences);

    //        // ------------------- Attaches RoomNew Script to last spawned Room and passes the corridors array (all types,I,4,T,L,etc) -------------------
    //        if (i == k - 1)
    //        {
    //            RoomNew2ndFloor roomNew2ndFloorScript = spawnedRoom.AddComponent<RoomNew2ndFloor>();
    //            roomNew2ndFloorScript.corridors = corridors;
    //            roomNew2ndFloorScript.vents = vents;
    //            roomNew2ndFloorScript.allRooms = allRooms;
    //            roomNew2ndFloorScript.ventCover = ventCover;
    //            roomNew2ndFloorScript.mapGenHolderTransform = mapGenHolderTransform;
    //            roomNew2ndFloorScript.itemGenScript = itemGenScript;
    //            //roomNew2ndFloorScript.ventCoverProbabilty = ventCoverProbabilty;
    //            Data2ndFloor.instance.roomNew2ndFloorScript = roomNew2ndFloorScript;

    //            //ConnectToMapGen(roomNew2ndFloorScript);

    //        }

    //        MapgenProgress.instance.addProgress(1);

    //        //gameObjectDetails.Add(roomScript);

    //    }
        

    //    /*
    //    ////Debug ALL ROOM POSITIONS

    //    for (int i = 0; i < n; i++)
    //    {
    //        //Debug.Log("_________________" + i + "___________________");
    //        float[] ddd = ((float[])allRooms[i]);
    //        //Debug.Log(ddd[0]);
    //        //Debug.Log(ddd[1]);
    //    }
    //    //Debug.Log("_________________DONE___________________");
    //    */

    //    Data2ndFloor.instance.allRooms = allRooms;
    //    Data2ndFloor.instance.xSize = xSize;
    //    Data2ndFloor.instance.zSize = zSize;

        
    //}

    // ------------------------ Add RoomNewVents script after delay ------------------------

    private IEnumerator AddRoomNewVents2ndFloor(GameObject gb)
    {
        yield return new WaitForSeconds(5f);
        gb.AddComponent<RoomNewVents2ndFloor>().corridors = vents;
    }

    // ---------------------------- Connect init pos to map gen nearest room ----------------------------
    private void ConnectToMapGen(RoomNew2ndFloor roomNew2ndFloorScript)
    {
        float min = 9999;
        int minIdx = -1;
        //-48, 0, -24
        for (int i = 0; i < allRooms.Count; i++)
        {
            float current = -48 + ((float[])allRooms[i])[1] + -24 + ((float[])allRooms[i])[0];
            if(current < min)
            {
                min = current;
                minIdx = i;
            }
        }
        if(minIdx != -1)
        {
            roomNew2ndFloorScript.ConnectTwoRooms(new Vector3(-((float[])allRooms[minIdx])[1] + 24, Data2ndFloor.instance.floor2Height + 1, -((float[])allRooms[minIdx])[0]), new Vector3(-48, Data2ndFloor.instance.floor2Height + 1, -24), "Door+x", "Door-z", Vector3.zero, new Vector3(-44, Data2ndFloor.instance.floor2Height + 1, -24 + 24), true); 
        }
        else
        {
            Debug.Log("ERROR!!!!");
        }
    }

    // ----------------------- Spawn Vent Cover in room -----------------------
    public override void SpawnVentCoverInRoom(int i, int k, Transform ventParentTransform)
    {

        if (Random.Range(0.0f, 1.0f) < ventCoverProbabilty || i == k - 1)
        {
            Vector3 ventSpawnPos = ventParentTransform.position;
            //ventSpawnPos.y = 0.5f; //not really required
            //if (i == k - 1)
            //{
            //GameObject gb = Instantiate(ventCover, spawnedRoomTransform.GetChild(0).GetChild(0).position, Quaternion.Euler(0, Random.Range(0, 3) * 90, 0), spawnedRoomTransform);
            //    GameObject gb = Instantiate(ventCover, ventSpawnPos, Quaternion.Euler(0, Random.Range(0, 3) * 90, 0), ventParentTransform);

            //}
            //else
            //{
            //Instantiate(ventCover, spawnedRoomTransform.GetChild(0).GetChild(0).position, Quaternion.Euler(0, Random.Range(0, 3) * 90, 0), spawnedRoomTransform);
            Instantiate(ventCover, ventSpawnPos, Quaternion.Euler(0, Random.Range(0, 3) * 90, 0), ventParentTransform);
            //}
        }
        //if (Random.Range(0.0f, 1.0f) < ventCoverProbabilty || i == k - 1)
        //{
        //    if (i == k - 1)
        //    {
        //        //GameObject gb = Instantiate(ventCover, spawnedRoomTransform.GetChild(0).GetChild(0).position, Quaternion.Euler(0, Random.Range(0, 3) * 90, 0), spawnedRoomTransform);
        //        GameObject gb = Instantiate(ventCover, ventParentTransform.position, Quaternion.Euler(0, Random.Range(0, 3) * 90, 0), ventParentTransform);
        //        gb.transform.GetChild(1).tag = "Vent Spawn Points 2nd Floor";
        //        StartCoroutine(AddRoomNewVents2ndFloor(gb));
        //    }
        //    else
        //    {
        //        //Instantiate(ventCover, spawnedRoomTransform.GetChild(0).GetChild(0).position, Quaternion.Euler(0, Random.Range(0, 3) * 90, 0), spawnedRoomTransform).transform.GetChild(1).tag = "Vent Spawn Points 2nd Floor";
        //        Instantiate(ventCover, ventParentTransform.position, Quaternion.Euler(0, Random.Range(0, 3) * 90, 0), ventParentTransform).transform.GetChild(1).tag = "Vent Spawn Points 2nd Floor";
        //    }
        //}
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
