using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public abstract class MapGenBase : MonoBehaviour
{
    [Header("Dev")]
    public bool isDevMode = false;
    public bool isSampleRoomsMeshRend = false;

    public Transform mapGenHolderTransform;
    public GameObject roomsLoaderPrefab;

    [Header("Rooms")]
    [Tooltip("Including Elevator")]
    public int numberOfRooms = 6; //Including Elevator 
    protected int n;

    public GameObject[] staticRooms;
    public GameObject mainRoomIndicator, liftRoom, generatorRoom, startRoom, endRoom, laserRoom;
    public GameObject[] corridors;

    public ArrayList allRooms = new ArrayList();
    [SerializeField] protected float xSize = 48f, zSize = 48f;

    protected Vector2 mapCentre;
    [SerializeField] protected int mapSizeX = 4, mapSizeZ = 2;

    //For Vents
    [Header("Vents")]
    public GameObject[] vents;
    protected float ventCoverProbabilty = 0;//1;
    public GameObject ventCover;

    public RoomNew roomNewScript;

    [Header("Test")]
    public GameObject testGridCube;
    public GameObject testGridPlane;
    public Transform testGridPlaneHolder;
    public float aStarVisualisationTime = 0;
    public TextMeshProUGUI seedText;
    public List<Transform> cubes;

    public SquareGrid squareGrid;
    protected float roomHeight;
    public string floorDoorTag;
    public string floorRoomTag;
    public string floorNameForHolder;
    public string ventCoverTag;

    private ItemGen itemGenScript;

    protected virtual void Start()
    {
        SetRoomTag();
        SetDoorTag();
        SetRoomHeight();
        SetFloorNameForHolder();
        SetVentCoverTag();
    }

    //Set tag for vent cover
    protected abstract void SetVentCoverTag();

    //Set floor name for holder
    protected abstract void SetFloorNameForHolder();

    //Set room tag
    protected abstract void SetRoomTag();

    //Set door tag
    protected abstract void SetDoorTag();

    //Set room Height
    protected abstract void SetRoomHeight();

    //not for 1st floor (1st floor returns always false)
    protected virtual bool AddingLiftForHigherFloors(int k)
    {
        //Debug.Log("lift stuff in base for 2nd floor");
        if(k == 0)
        {
            float[] arr1 = new float[2];
            arr1[0] = -Data2ndFloor.instance.liftRoomPos.z;
            arr1[1] = -Data2ndFloor.instance.liftRoomPos.x;
            allRooms.Add(arr1);
            n++;
            return true;
        }
        return false;
    }

    protected virtual void FirstFloorExtraInitRoom()
    {

    }

    protected virtual void syncronizeSeeds(int seed)
    {

    }

    protected abstract RoomNew AddRoomNewCorrectly();

    public void startMapGeneration(int seed)
    {
        n = numberOfRooms + 1;





        float x = -(48 * ((float)(mapSizeX - 1) / 2)) - 28;
        float z = -(48 * ((float)(mapSizeZ - 1) / 2)) - 28;
        mapCentre = new Vector2(x, z);
        //Debug.Log(mapCentre);


        FirstFloorExtraInitRoom();
        CreateHolderForMapGen();
        //Random.InitState(100);
        //Rooms();

        // StartCoroutine(WaitForaWhile());

        //Random.state = GoodStates.states[0];
        //syncronizeSeeds(seed);

        syncronizeSeeds(seed);
        if (isDevMode)
        {
            if (seed == -1) seed = Random.Range(0, 1000);
            seedText.text = seed + "";
        }
        Random.InitState(seed);



        itemGenScript = GetComponent<ItemGen>();
        roomNewScript = AddRoomNewCorrectly();
        roomNewScript.corridors = corridors;
        roomNewScript.vents = vents;
        roomNewScript.allRooms = allRooms;
        roomNewScript.ventCover = ventCover;
        roomNewScript.mapGenHolderTransform = mapGenHolderTransform;
        roomNewScript.itemGenScript = itemGenScript;
        roomNewScript.mapSizeX = mapSizeX;
        roomNewScript.mapSizeZ = mapSizeZ;
        roomNewScript.xSize = xSize;
        roomNewScript.zSize = zSize;
        roomNewScript.roomHeight = roomHeight;
        roomNewScript.ventCoverTag = ventCoverTag;
        roomNewScript.isDevMode = isDevMode;
        roomNewScript.isSampleRoomsMeshRend = isSampleRoomsMeshRend;
        if (isDevMode)
        {
            roomNewScript.testGridCube = testGridCube;
        }
        //roomNewScript.ventCoverProbabilty = ventCoverProbabilty;
        Data.instance.roomNewScript = roomNewScript;

        Data.instance.xSize = xSize;
        Data.instance.zSize = zSize;

        //StateData.states.Add(Random.state);
        Rooms();

        Data.instance.roomsLoaderPrefab = roomsLoaderPrefab;
        Data.instance.corridorT1 = corridors[3];
        Data.instance.corridorT2 = corridors[4];
        Data.instance.corridorX = corridors[5];
        Data.instance.ventT = vents[3];
        Data.instance.ventX = vents[5];
    }

    protected virtual void RoomPos(ref int k, int l)
    {
        while (k < n && l < 1000)
        {


            // Lift stuff
            if (AddingLiftForHigherFloors(k))
            {
                //Debug.Log("lift stuff");
                k++;
                l++;
                continue;
            }


            float[] arr = new float[2];

            // ------------------- BOUNDS or SIZE of the grid -------------------

            // 400 - 20 = 380 (MAX)
            // 0 + 20 = 20 (MIN)
            //Increments of 40

            //arr[0] = 40 * Random.Range(0, 9) + 20;  //9 coz -> 9 * 40 + 20 = 380
            //arr[1] = 40 * Random.Range(0, 9) + 20;


            // 480 - 20 = 460 (MAX)
            // 0 + 28 = 28 (MIN)
            //Increments of 40

            arr[0] = 48 * Random.Range(0, mapSizeZ) + 28;  //9 coz -> 9 * 48 + 28 = 460
            arr[1] = 48 * Random.Range(0, mapSizeX) + 28;

            //arr[0] = Random.Range(/*11*/ + 1 + (int)(zSize/2), /*-11*/ -1 + 399 - (int)(xSize / 2)); //0,0 is the top left cell
            //arr[1] = Random.Range(/*11*/ + 1 + (int)(zSize / 2), /*-11*/ -1 + 399 - (int)(xSize / 2)); //0,0 is the top left cell


            /*
            if(k == 0)
            {
                arr[1] = 50;
                arr[0] = 80;
            }
            else
            {
                arr[1] = 50;
                arr[0] = 10;
            }
            */
            // ------------------- Integer positions in GRID / positions according to sizes of rooms in GRID fashion -------------------
            //arr[0] = Mathf.Round(((((int)arr[0])/zSize) * zSize) / zSize) * zSize;
            //arr[1] = Mathf.Round(((((int)arr[1])/xSize) * xSize) / zSize) * zSize;
            //Debug.Log("room positions");
            // ------------------- Checks for collisions between rooms  -------------------
            if (NoCollisions(arr))
            {
                allRooms.Add(arr);
                ++k;
            }
            ++l;
            MapgenProgress.instance.addProgress(3);

            //Debug.Log("pos " + k + " = " + arr[0]);
            //Debug.Log("pos " + k + " = " + arr[1]);
        }
        for (int i = 0; i < allRooms.Count; i++)
        {
            //Debug.Log("pos " + i + " = " + ((float[])allRooms[i])[0]);
            //Debug.Log("pos " + i + " = " + ((float[])allRooms[i])[1]);
        }
    }

    protected virtual void AddLiftPosForLowerRoom(int i, Vector3 roomPos)
    {

    }

    public virtual void Rooms()
    {
        //Debug.Log("Rooms started");


        squareGrid = AStarSearch.InitialiseSquareGrid(xSize, zSize, mapSizeX, mapSizeZ, out int xOverall, out int zOverall);


        /*
        if (ReloadGoodStatesData.isReloadingGoodStates)
        {
            Random.state = GoodStates.states[ReloadGoodStatesData.i];
            //ReloadGoodStatesData.isReloadingGoodStates = false;
            ReloadGoodStatesData.i++;
            if (ReloadGoodStatesData.i >= GoodStates.states.Count)
            {
                ReloadGoodStatesData.i = 0;
            }
        }
        */
        //Make this while and next loop into one? will Collisions be a prob?
        int k = 0, l = 0;

        RoomPos(ref k, l);

        MapgenProgress.instance.addProgress(3);
        /*
        List<GameObject> staticRooms = new List<GameObject>();
        staticRooms.Add(liftRoom);
        staticRooms.Add(startRoom);
        staticRooms.Add(generatorRoom);
        staticRooms.Add(endRoom);
        // ------------------- Static Rooms ------------------- 
        for (int i = 0; i < staticRooms.Count; i++)
        {
            Instantiate(staticRooms[i], )
        }
        */
        // ------------------- RANDOMLY choosing ROOMS to spawn  -------------------
        //ItemGen itemGenScript = GetComponent<ItemGen>();
        for (int i = 1; i < k; i++)
        {
            //Debug.Log("static rooms");
            GameObject roomToSpawn = generatorRoom;
            float yCoord = 1.0f; // Beware, its for gen room
            if (i - 1 < staticRooms.Length)
            {
                roomToSpawn = staticRooms[i - 1];
                if (staticRooms[i - 1].name.Equals("Start Room"))
                {
                    yCoord = 0.064f;
                }
                else if (staticRooms[i - 1].name.Equals("End Room"))
                {
                    yCoord = 0.5f;
                }
                else if (staticRooms[i - 1].name.Equals("Laser Room"))
                {
                    yCoord = 1f;
                }
                else if (staticRooms[i - 1].name.Equals("Elevator Room"))
                {
                    yCoord = 0.2f;
                }
                else
                {
                    if (!staticRooms[i - 1].name.Equals("Elevator Room"))
                    {
                        //yCoord = 0f;
                        //Debug.Log("not in static room list; gen room yCoord used; error!");
                    }
                }
            }
            else
            {
                //Debug.Log("non static rooms");
                switch (Random.Range(3, 4)) // yes modular room added // for dev purposes 1, 4 change later
                {
                    case 0:
                        roomToSpawn = startRoom;
                        yCoord = 0.064f;
                        break;
                    case 1:
                        roomToSpawn = endRoom;
                        yCoord = 0.5f;
                        break;
                    case 2:
                        roomToSpawn = laserRoom;
                        yCoord = 1;
                        break;
                    case 3:
                        yCoord = -1.72f; //-1.5f - 0.739f;
                        roomToSpawn = null;
                        break;
                        /*
                case 3:
                    roomToSpawn = roomT;
                    break;
                case 4:
                    roomToSpawn = room4;
                    break;*/
                }
            }


            //if (isDevMode)
            //{
            //    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.L));
            //}

            float yRotation = LookToMapCentre(new Vector2(-((float[])allRooms[i])[1], -((float[])allRooms[i])[0]));//Random.Range(0, 4) * 90;
            Vector3 roomPos = new Vector3(-((float[])allRooms[i])[1], yCoord + roomHeight, -((float[])allRooms[i])[0]);

            AddLiftPosForLowerRoom(i, roomPos);

            GameObject spawnedRoom; // = Instantiate(roomToSpawn, roomPos, Quaternion.Euler(0, yRotation, 0), mapGenHolderTransform);
            RoomReferencesBase roomReferencesBase;

            //Debug.Log("hi");
            //roomToSpawn = null;
            if (roomToSpawn == null)
            {
                Data.instance.modularRoomAssembler.room_start_Transform = new GameObject("Room Start").transform;
                Data.instance.modularRoomAssembler.room_start_Transform.position = roomPos + new Vector3(0, 0, 20); //Not Sure!!!;

                Transform roomHolderTransform = new GameObject("Modular Room 1").transform;
                Data.instance.modularRoomAssembler.roomHolderTransform = roomHolderTransform;
                spawnedRoom = roomHolderTransform.gameObject;

                RoomReferencesModular roomReferencesModular = spawnedRoom.AddComponent<RoomReferencesModular>();
                roomReferencesBase = roomReferencesModular;
                //roomReferencesModular.doors = Data.instance.modularRoomAssembler.doors;
                roomReferencesModular.ventParent = new GameObject("Vent Parent").transform;

                roomReferencesModular.roomFloors = new List<Vector3>();

                RoomDoorsInfo roomDoorsInfo1 = spawnedRoom.AddComponent<RoomDoorsInfo>();

                Data.instance.roomsFloor1Modular.Add(spawnedRoom);
                Data.instance.modularRoomAssembler.StartScript(roomReferencesModular, roomDoorsInfo1);

#if UNITY_EDITOR
                roomDoorsInfo1.ConvertTransformToClassInfo();
#endif

                //roomReferencesModular.ventParent.position = roomReferencesModular.roomFloors[Random.Range(0, roomReferencesModular.roomFloors.Count)];
                //if (i != 1)
                //    SpawnVentCoverInRoom(i, k, roomReferencesModular.ventParent);
                //Data.instance.roomsFloor1Modular.Add(spawnedRoom);


            }
            else
            {
                spawnedRoom = Instantiate(roomToSpawn, roomPos, Quaternion.Euler(0, yRotation, 0), mapGenHolderTransform);
                //Debug.Log("Room name = " + spawnedRoom.name);
                RoomReferencesStatic roomReferencesStatic = spawnedRoom.GetComponent<RoomReferencesStatic>();
                roomReferencesBase = roomReferencesStatic;
                CallOffsetAndDoorAndSqGridFns(spawnedRoom, yRotation, roomReferencesStatic);


                #region tile Floor

                DoorHelper2(roomReferencesStatic, isDevMode, ref squareGrid, ref cubes, testGridCube);

                #endregion



                itemGenScript.SpawnItems(roomReferencesStatic.bottomLeftItemGen.position, roomReferencesStatic.topRightItemGen.position, 6, spawnedRoom.transform);

                if (i != 1)
                    SpawnVentCoverInRoom(i, k, roomReferencesStatic.ventParent);
            }

            //if RoomDoorsInfo exists
            if (spawnedRoom.TryGetComponent(out RoomDoorsInfo roomDoorsInfo))
            {
                Debug.Log("in 4321");
                //server
                if(NetworkManager.singleton.mode == NetworkManagerMode.Host ||
                   NetworkManager.singleton.mode == NetworkManagerMode.ServerOnly)
                {
                    Debug.Log("in 4321 host");
                    for (int r = 0; r < roomDoorsInfo.roomDoorsTransform.Length; r++)
                    {
                        Debug.Log("in 4321 host loop, r = " + r);
                        //roomDoorsInfo.roomDoorsTransform[r].gameObject.AddComponent<NetworkIdentity>();
                        //roomDoorsInfo.roomDoorsTransform[r].gameObject.AddComponent<NetworkTransform>();
                        //roomDoorsInfo.roomDoorsTransform[r].GetChild(0).gameObject.AddComponent<NetworkTransformChild>();
                        //roomDoorsInfo.roomDoorsTransform[r].GetChild(1).gameObject.AddComponent<NetworkTransformChild>();

                        //Server_DoorSpawner.instance.Spawn_ServerDoor(roomDoorsInfo.roomDoorsTransform[r].gameObject);
                        //Debug.Log("0987 " + spawnedRoom.transform.position);
                        //Debug.Log("0987 " + roomDoorsInfo.roomDoorsTransformInfo[r].pos);
                        //Vector3 pos = roomDoorsInfo.roomDoorsTransformInfo[r].pos;
                        //pos.x *= roomDoorsInfo.roomDoorsTransformInfo[r].scale.x;
                        //pos.y *= roomDoorsInfo.roomDoorsTransformInfo[r].scale.y;
                        //pos.z *= roomDoorsInfo.roomDoorsTransformInfo[r].scale.z;
                        Server_DoorSpawner.instance.Spawn_ServerDoor(roomDoorsInfo.roomDoorsTransform[r].position,
                                                                     roomDoorsInfo.roomDoorsTransformInfo[r].rot,
                                                                     roomDoorsInfo.roomDoorsTransformInfo[r].scale,
                                                                     roomDoorsInfo.roomDoorsTransformInfo[r].triggerState);
                    }
                }
                //client
                else
                {
                    Debug.Log("in 4321 client");
                    for (int r = 0; r < roomDoorsInfo.roomDoorsTransform.Length; r++)
                    {
                        Debug.Log("in 4321 client loop, r = " + r);
                        roomDoorsInfo.roomDoorsTransform[r].gameObject.SetActive(false);
                    }
                }
            }

            //room tag
            spawnedRoom.tag = floorRoomTag;

            int x, z;
            string doorName;
            for (int q = 0; q < roomReferencesBase.doors.Length; q++)
            {
                //tag
                roomReferencesBase.doors[q].tag = floorDoorTag;

                //other stuff xD
                x = Mathf.RoundToInt(roomReferencesBase.doors[q].transform.position.x / -4);
                z = Mathf.RoundToInt(roomReferencesBase.doors[q].transform.position.z / -4);

                //Debug.Log("X = " + x);
                //Debug.Log("Z = " + z);
                //Debug.Log("actual door pos = " + roomReferencesBase.doors[q].transform.position);
                //Debug.Log("pos = " + GetPos(new int[] { x, z }));
                doorName = roomReferencesBase.doors[q].name[4].ToString() + roomReferencesBase.doors[q].name[5].ToString();

                squareGrid.tiles[x, z].tile = TileType.Floor;
                for (int j = 0; j < 2; j++)
                {
                    DoorFurtherHelper(doorName + "");
                    //Debug.Log("hey doorname = " + doorName);
                    //Debug.Log("X = " + x);
                    //Debug.Log("Z = " + z);
                    //Debug.Log("pos = " + GetPos(new int[] { x, z }));
                    if (x >= xOverall ||
                        z >= zOverall)
                    {
                        continue;
                    }
                    squareGrid.tiles[x, z].tile = TileType.Floor;
                }
                if (isDevMode)
                {
                    Instantiate(testGridCube, new Vector3(x * -4, 2, z * -4), Quaternion.identity)
                        .transform.GetChild(0).GetComponent<Renderer>().sharedMaterial.color = Color.blue;
                }
            }

            // -------- prevent doors being isolated in the map (Shouldnt make room floors walkable!!!!!) --------
            void DoorFurtherHelper(string doorLastName)
            {
                switch (doorLastName)
                {
                    case "+z":
                        z--;
                        break;
                    case "+x":
                        x--;
                        break;
                    case "-z":
                        z++;
                        break;
                    case "-x":
                        x++;
                        break;
                }
            }

            Vector3 GetPos(int[] idx)
            {
                return new Vector3(idx[0] * -4, 0.5f, idx[1] * -4);
            }




            // ------------------- Attaches RoomNew Script to last spawned Room and passes the corridors array (all types,I,4,T,L,etc) -------------------
            if (i == k - 1)
            {/*
                RoomNew roomNewScript = spawnedRoom.AddComponent<RoomNew>();
                roomNewScript.corridors = corridors;
                roomNewScript.vents = vents;
                roomNewScript.allRooms = allRooms;
                roomNewScript.ventCover = ventCover;
                roomNewScript.mapGenHolderTransform = mapGenHolderTransform;
                roomNewScript.itemGenScript = itemGenScript;
                //roomNewScript.ventCoverProbabilty = ventCoverProbabilty;
                Data.instance.roomNewScript = roomNewScript;
                */
                roomNewScript.aStarVisualisationTime = aStarVisualisationTime;
                roomNewScript.squareGrid = squareGrid;
                if (isDevMode)
                {
                    roomNewScript.testGridPlaneHolder = testGridPlaneHolder;
                }
                //for (int d = 36; d < 38; d++)
                //{
                //    for (int f = 0; f < 20; f++)
                //    {
                //        squareGrid.tiles[d, f].tile = TileType.Wall;
                //        //Debug.Log(squareGrid.tiles[d, f]);
                //    }
                //}
                StartCoroutine(roomNewScript.StartScript());
                //ConnectToMapGen(roomNewScript);

            }

            MapgenProgress.instance.addProgress(1);

            //gameObjectDetails.Add(roomScript);

        }


        /*
        ////Debug ALL ROOM POSITIONS

        for (int i = 0; i < n; i++)
        {
            //Debug.Log("_________________" + i + "___________________");
            float[] ddd = ((float[])allRooms[i]);
            //Debug.Log(ddd[0]);
            //Debug.Log(ddd[1]);
        }
        //Debug.Log("_________________DONE___________________");
        */

        Data.instance.allRooms = allRooms;
        Data.instance.xSize = xSize;
        Data.instance.zSize = zSize;

    }

    private float LookToMapCentre(Vector2 pos)
    {
        int xChange, yChange;
        xChange = (int)(mapCentre.x - pos.x);
        yChange = (int)(mapCentre.y - pos.y);
        float yRotation;
        if (Mathf.Abs(xChange) > Mathf.Abs(yChange))
        {
            if (xChange > 0)
            {
                yRotation = 90f;
            }
            else
            {
                yRotation = -90f;
            }
        }
        else
        {
            if (yChange > 0)
            {
                yRotation = 0;
            }
            else
            {
                yRotation = 180;
            }
        }
        return yRotation;
    }

    // ---------------------------- Call offset functions accordingly ----------------------------
    protected void CallOffsetAndDoorAndSqGridFns(GameObject spawnedRoom, float yRotation, RoomReferencesStatic roomReferences)
    {
        if (yRotation == 90)
        {
            ChangeDoorNames();
            //ChangeDoorNames(spawnedRoom, "Door+x");
            GiveOffsetToRoom(0.226f);
            //spawnedRoom.transform.localPosition = new Vector3(spawnedRoom.transform.localPosition.x + 0.226f,  //*
            //                                                  spawnedRoom.transform.localPosition.y,           //* This is for Start Room
            //                                                  spawnedRoom.transform.localPosition.z + 0.065f); //*


            //ExistingCorridorsFn(roomReferences, (int)yRotation, 0.226f);


            //Transform corridorsOfRoom = spawnedRoom.transform.GetChild(2);

            //for (int z = 0; z < corridorsOfRoom.childCount; z++)
            //{
            //    corridorsOfRoom.GetChild(z).localPosition = new Vector3(0, 0, 0.226f);
            //}


        }
        else if (yRotation == 180 || yRotation == 270 || yRotation == -90)
        {
            float reqYRotationForCorridor = 0;
            if (yRotation == 180)
            {
                ChangeDoorNames();
                //ChangeDoorNames(spawnedRoom, "Door-z");
                GiveOffsetToRoom(-0.08f);
                //ExistingCorridorsFn(roomReferences, (int)yRotation, -0.08f);
                reqYRotationForCorridor = 0;

                //-----------------234567-----------------
                //if (spawnedRoom.name.Equals("End Room(Clone)"))
                {
                    spawnedRoom.transform.GetChild(0).localPosition = new Vector3(spawnedRoom.transform.GetChild(0).localPosition.x - 0.303f, spawnedRoom.transform.GetChild(0).localPosition.y, spawnedRoom.transform.GetChild(0).localPosition.z + 0.31f);
                }

            }
            else if (yRotation == 270 || yRotation == -90)
            {
                ChangeDoorNames();
                //ChangeDoorNames(spawnedRoom, "Door-x");
                GiveOffsetToRoom(0.226f);
                //spawnedRoom.transform.localPosition = new Vector3(spawnedRoom.transform.localPosition.x + 0.226f,  //*
                //                                                  spawnedRoom.transform.localPosition.y,           //* This is for Start Room
                //                                                  spawnedRoom.transform.localPosition.z - 0.065f); //*

                //ExistingCorridorsFn(roomReferences, (int)yRotation, 0.226f);

                //Transform corridorsOfRoom = spawnedRoom.transform.GetChild(2);
                //for (int z = 0; z < corridorsOfRoom.childCount; z++)
                //{
                //    corridorsOfRoom.GetChild(z).localPosition = new Vector3(0, 0, 0.226f);
                //}


                reqYRotationForCorridor = 90;

                //-----------------234567-----------------
                //if (spawnedRoom.name.Equals("End Room(Clone)"))
                {
                    spawnedRoom.transform.GetChild(0).localPosition = new Vector3(spawnedRoom.transform.GetChild(0).localPosition.x - 0.31f, spawnedRoom.transform.GetChild(0).localPosition.y, spawnedRoom.transform.GetChild(0).localPosition.z - 0.303f);
                }

            }

            for (int j = 0; j < spawnedRoom.transform.GetChild(2).childCount; j++)
            {
                spawnedRoom.transform.GetChild(2).GetChild(j).rotation = Quaternion.Euler(0, reqYRotationForCorridor, 0);
            }



        }
        //probably +z....
        else
        {
            GiveOffsetToRoom(-0.08f);
            //ExistingCorridorsFn(roomReferences, (int)yRotation, -0.08f);
        }


        // ---------------------------- Shift/Give offset to room prefab correctly ----------------------------
        void GiveOffsetToRoom(float offset)
        {
            spawnedRoom.transform.GetChild(0).localPosition = new Vector3(spawnedRoom.transform.GetChild(0).localPosition.x, spawnedRoom.transform.GetChild(0).localPosition.y, spawnedRoom.transform.GetChild(0).localPosition.z + offset);
            //Transform corridorsOfRoomParent = spawnedRoom.GetChild(2);
            //for (int i = 0; i < corridorsOfRoomParent.childCount; i++)
            //{
            //    corridorsOfRoomParent.GetChild(i).GetChild(0).localPosition = new Vector3(0, 0, offset);
            //}
        }


        // -------------------- Change door names --------------------
        void ChangeDoorNames()
        {
            GameObject[] doors = spawnedRoom.GetComponent<RoomReferencesStatic>().doors;
            for (int i = 0; i < doors.Length; i++)
            {
                //Debug.Log("doorRot ; yRotInt = " + ((int)yRotation / 90) % 4);
                spawnedRoom.GetComponent<RoomReferencesStatic>().doors[i].name = FindDoorName(((int)yRotation / 90) % 4, spawnedRoom.GetComponent<RoomReferencesStatic>().doors[i].name);
            }
        }

        string FindDoorName(int yRotInt, string oldName)
        {
            if (yRotInt == 0)
            {
                return oldName;
            }
            else
            {
                //yRotInt = Mathf.Abs(yRotInt);
                //Debug.Log("doorRot ; oldName 4 and 5 idx = " + (oldName[4] + "" + oldName[5]));
                int idx = Data.instance.doorRotationHelper.IndexOf((oldName[4] + "" + oldName[5]).ToString());
                //Debug.Log("doorRot ; idx b4 = " + idx);
                idx += yRotInt;
                //Debug.Log("doorRot ; idx b/w = " + idx);
                idx %= 4;
                if (idx < 0)
                {
                    idx += 4;
                }
                //Debug.Log("doorRot ; idx after = " + idx);
                return "Door" + Data.instance.doorRotationHelper[idx];
            }
        }
    }


    protected void DoorHelper2(RoomReferencesStatic roomReferencesStatic, bool isDevMode
        , ref SquareGrid squareGrid, ref List<Transform> cubes, GameObject testGridCube)
    {

        float tempPosVal;
        if (roomReferencesStatic.topRightItemGen.position.x > roomReferencesStatic.bottomLeftItemGen.position.x)
        {

        }
        else
        {
            tempPosVal = roomReferencesStatic.topRightItemGen.position.x;
            roomReferencesStatic.topRightItemGen.position = new Vector3(roomReferencesStatic.bottomLeftItemGen.position.x
                                                                 , roomReferencesStatic.topRightItemGen.position.y
                                                                 , roomReferencesStatic.topRightItemGen.position.z);
            roomReferencesStatic.bottomLeftItemGen.position = new Vector3(tempPosVal
                                                                 , roomReferencesStatic.bottomLeftItemGen.position.y
                                                                 , roomReferencesStatic.bottomLeftItemGen.position.z);

            tempPosVal = roomReferencesStatic.topRightMapGen.position.x;
            roomReferencesStatic.topRightMapGen.position = new Vector3(roomReferencesStatic.bottomLeftMapGen.position.x
                                                                 , roomReferencesStatic.topRightMapGen.position.y
                                                                 , roomReferencesStatic.topRightMapGen.position.z);
            roomReferencesStatic.bottomLeftMapGen.position = new Vector3(tempPosVal
                                                                 , roomReferencesStatic.bottomLeftMapGen.position.y
                                                                 , roomReferencesStatic.bottomLeftMapGen.position.z);

        }
        if (roomReferencesStatic.topRightItemGen.position.z > roomReferencesStatic.bottomLeftItemGen.position.z)
        {

        }
        else
        {
            tempPosVal = roomReferencesStatic.topRightItemGen.position.z;
            roomReferencesStatic.topRightItemGen.position = new Vector3(roomReferencesStatic.topRightItemGen.position.x
                                                                 , roomReferencesStatic.topRightItemGen.position.y
                                                                 , roomReferencesStatic.bottomLeftItemGen.position.z);
            roomReferencesStatic.bottomLeftItemGen.position = new Vector3(roomReferencesStatic.bottomLeftItemGen.position.x
                                                                 , roomReferencesStatic.bottomLeftItemGen.position.y
                                                                 , tempPosVal);

            tempPosVal = roomReferencesStatic.topRightMapGen.position.z;
            roomReferencesStatic.topRightMapGen.position = new Vector3(roomReferencesStatic.topRightMapGen.position.x
                                                                 , roomReferencesStatic.topRightMapGen.position.y
                                                                 , roomReferencesStatic.bottomLeftMapGen.position.z);
            roomReferencesStatic.bottomLeftMapGen.position = new Vector3(roomReferencesStatic.bottomLeftMapGen.position.x
                                                                 , roomReferencesStatic.bottomLeftMapGen.position.y
                                                                 , tempPosVal);

        }

        //this.roomReferences = roomReferencesStatic;

        for (int q = -Mathf.CeilToInt(roomReferencesStatic.topRightMapGen.position.x / 4); q <= -Mathf.FloorToInt(roomReferencesStatic.bottomLeftMapGen.position.x / 4); q++)
        {
            //Debug.Log("|");
            for (int r = -Mathf.CeilToInt(roomReferencesStatic.topRightMapGen.position.z / 4); r <= -Mathf.FloorToInt(roomReferencesStatic.bottomLeftMapGen.position.z / 4); r++)
            {
                //Debug.Log("- q, r =" + q + ", " + r);

                if (squareGrid.InBounds(new Location(q, r)))
                {
                    squareGrid.tiles[q, r].tile = TileType.Wall;
                    if (isDevMode)
                    {
                        cubes.Add(Instantiate(testGridCube, new Vector3(q * -4, 0, r * -4), Quaternion.identity).transform);
                        //Instantiate(testGridCube, new Vector3(q * -4, 0, r * -4), Quaternion.identity);
                    }
                }
            }
        }
    }

    // ----------------------- Spawn Vent Cover in room -----------------------
    public virtual void SpawnVentCoverInRoom(int i, int k, Transform ventParentTransform)
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
                Instantiate(ventCover, ventSpawnPos, Quaternion.Euler(0, Random.Range(0, 3) * 90, 0), ventParentTransform).transform.GetChild(1).GetChild(1).tag = ventCoverTag;
            //}
        }
    }

    private void CreateHolderForMapGen()
    {
        GameObject mapGenHolder = new GameObject(floorNameForHolder);
        mapGenHolder.layer = 18;
        mapGenHolderTransform = mapGenHolder.transform;//Instantiate(mapGenHolder).transform;
        Data2ndFloor.instance.mapGenHolderTransform = mapGenHolderTransform;
    }

    // --------------------------------- Checks for collisions between ROOMS ---------------------------------
    protected bool NoCollisions(float[] arr)
    {
        for (int i = 0; i < allRooms.Count; i++)
        {
            if ((Mathf.Abs(arr[0] - ((float[])allRooms[i])[0]) < xSize) && (Mathf.Abs(arr[1] - ((float[])allRooms[i])[1]) < zSize))
            {
                return false;
            }
        }
        return true;
    }

}
