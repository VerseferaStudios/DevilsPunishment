using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Collections;

public class MapGen2ndFloor : MonoBehaviour
{
    //first we'll see the ground floor
    //10 x 10 cube
    //later//private int gridSize;

    public GameObject GUIProgress;

    public GameObject roomsLoaderPrefab, mapGenHolder;
    public Transform mapGenHolderTransform;

    [Header("Rooms")]
    [Tooltip("Including Elevator")]
    public int numberOfRooms = 6; //Including Elevator 
    private int n;
    public ArrayList allRooms = new ArrayList();
    private ArrayList gameObjectDetails = new ArrayList();

    public GameObject[] staticRooms;
    public GameObject mainRoomIndicator, generatorRoom, startRoom, endRoom, laserRoom;
    
    private float xSize = 48f, zSize = 48f;

    public GameObject[] corridors;

    //For Vents
    [Header("Vents")]
    public GameObject[] vents;
    private float ventCoverProbabilty = 0;//1;
    public GameObject ventCover;

    [Header("ScriptableObjects")]
    public StateData StateData, GoodStates;
    public ReloadGoodStates ReloadGoodStatesData;

    private Vector2 mapCentre;
    private int mapSizeX = 4, mapSizeZ = 2;

    private void Start()
    {
        n = numberOfRooms - 1;

        float x = - (48 * ((float)(mapSizeX - 1) / 2)) - 28;
        float z = - (48 * ((float)(mapSizeZ - 1) / 2)) - 28;
        mapCentre = new Vector2(x, z);
        Debug.Log(mapCentre);

        GUIProgress.SetActive(true);
        CreateHolderForMapGen();
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

    public void CreateHolderForMapGen()
    {
        mapGenHolder = new GameObject("2nd Floor");
        mapGenHolder.layer = 18;
        mapGenHolderTransform = mapGenHolder.transform;//Instantiate(mapGenHolder).transform;
        Data2ndFloor.instance.mapGenHolderTransform = mapGenHolderTransform;
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

        CreateHolderForMapGen();

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

    public void Rooms()
    {
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

        float[] arr1 = new float[2];
        arr1[0] = -Data2ndFloor.instance.liftRoomPos.z;
        arr1[1] = -Data2ndFloor.instance.liftRoomPos.x;
        allRooms.Add(arr1);
        n++;
        int k = 1, l = 1;
        while (k < n && l < 1000)
        {
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

            // ------------------- Checks for collisions between rooms  -------------------
            if (NoCollisions(arr))
            {
                allRooms.Add(arr);
                ++k;
            }
            ++l;
        }

        // ------------------- RANDOMLY choosing ROOMS to spawn  -------------------
        ItemGen itemGenScript = GetComponent<ItemGen>();
        for (int i = 1; i < k; i++)
        {
            GameObject roomToSpawn = generatorRoom;
            float yCoord = 1.5f; // Beware, its for gen room
            if(i - 1 < staticRooms.Length)
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
                    yCoord = 0f;
                }
            }
            else
            {
                switch (Random.Range(1, 4))
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
                        /*
                case 3:
                    roomToSpawn = roomT;
                    break;
                case 4:
                    roomToSpawn = room4;
                    break;*/
                }
            }

            float yRotation = LookToMapCentre(new Vector2(-((float[])allRooms[i])[1], -((float[])allRooms[i])[0]));//Random.Range(0, 4) * 90;
            Vector3 roomPos = new Vector3(-((float[])allRooms[i])[1], Data2ndFloor.instance.floor2Height + yCoord, -((float[])allRooms[i])[0]);
            /*
            if (i == 0)
            {
                float oldY = roomPos.y;
                roomPos = Data2ndFloor.instance.liftRoomPos;
                roomPos.y = oldY;
            }
            */
            GameObject spawnedRoom = Instantiate(roomToSpawn, roomPos, Quaternion.Euler(0, yRotation, 0), mapGenHolderTransform);

            spawnedRoom.tag = "Room 2nd Floor";

            spawnedRoom.transform.GetChild(1).tag = "Corridor Spawn Points 2nd Floor";

            RoomReferences roomReferences = spawnedRoom.GetComponent<RoomReferences>();

            itemGenScript.SpawnItems(roomReferences.bottomLeftCorner.position, roomReferences.topRightCorner.position, 6, spawnedRoom.transform);

            SpawnVentCoverInRoom(i, k, roomReferences.ventParent);

            CallOffsetAndDoorFns(spawnedRoom, yRotation);

            // ------------------- Attaches RoomNew Script to last spawned Room and passes the corridors array (all types,I,4,T,L,etc) -------------------
            if (i == k - 1)
            {
                RoomNew2ndFloor roomNew2ndFloorScript = spawnedRoom.AddComponent<RoomNew2ndFloor>();
                roomNew2ndFloorScript.corridors = corridors;
                roomNew2ndFloorScript.vents = vents;
                roomNew2ndFloorScript.allRooms = allRooms;
                roomNew2ndFloorScript.ventCover = ventCover;
                roomNew2ndFloorScript.mapGenHolderTransform = mapGenHolderTransform;
                roomNew2ndFloorScript.itemGenScript = itemGenScript;
                //roomNew2ndFloorScript.ventCoverProbabilty = ventCoverProbabilty;
                Data2ndFloor.instance.roomNew2ndFloorScript = roomNew2ndFloorScript;

                //ConnectToMapGen(roomNew2ndFloorScript);

            }

            //MapgenProgress.instance.addProgress(1);

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

        Data2ndFloor.instance.allRooms = allRooms;
        Data2ndFloor.instance.xSize = xSize;
        Data2ndFloor.instance.zSize = zSize;

        
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

    // ---------------------------- Shift/Give offset to room prefab correctly ----------------------------
    private void GiveOffsetToRoom(Transform spawnedRoom, float offset)
    {
        spawnedRoom.GetChild(0).localPosition = new Vector3(spawnedRoom.GetChild(0).localPosition.x, spawnedRoom.GetChild(0).localPosition.y, spawnedRoom.GetChild(0).localPosition.z + offset);
        Transform corridorsOfRoomParent = spawnedRoom.GetChild(2);
        for (int i = 0; i < corridorsOfRoomParent.childCount; i++)
        {
            corridorsOfRoomParent.GetChild(i).GetChild(0).localPosition = new Vector3(0, 0, offset);
        }
    }

    // ----------------------- Spawn Vent Cover in room -----------------------
    public void SpawnVentCoverInRoom(int i, int k, Transform ventParentTransform)
    {
        if (Random.Range(0.0f, 1.0f) < ventCoverProbabilty || i == k - 1)
        {
            if (i == k - 1)
            {
                //GameObject gb = Instantiate(ventCover, spawnedRoomTransform.GetChild(0).GetChild(0).position, Quaternion.Euler(0, Random.Range(0, 3) * 90, 0), spawnedRoomTransform);
                GameObject gb = Instantiate(ventCover, ventParentTransform.position, Quaternion.Euler(0, Random.Range(0, 3) * 90, 0), ventParentTransform);
                gb.transform.GetChild(1).tag = "Vent Spawn Points 2nd Floor";
                StartCoroutine(AddRoomNewVents2ndFloor(gb));
            }
            else
            {
                //Instantiate(ventCover, spawnedRoomTransform.GetChild(0).GetChild(0).position, Quaternion.Euler(0, Random.Range(0, 3) * 90, 0), spawnedRoomTransform).transform.GetChild(1).tag = "Vent Spawn Points 2nd Floor";
                Instantiate(ventCover, ventParentTransform.position, Quaternion.Euler(0, Random.Range(0, 3) * 90, 0), ventParentTransform).transform.GetChild(1).tag = "Vent Spawn Points 2nd Floor";
            }
        }
    }

    // -------------------- Change door names --------------------
    public void ChangeDoorNames(GameObject spawnedRoom, string doorName)
    {
        GameObject[] doors = spawnedRoom.GetComponent<RoomReferences>().doors;
        for (int i = 0; i < doors.Length; i++)
        {
            spawnedRoom.GetComponent<RoomReferences>().doors[i].name = doorName;
        }
    }

    // ---------------------------- Call offset functions accordingly ----------------------------
    public void CallOffsetAndDoorFns(GameObject spawnedRoom, float yRotation)
    {
        if (yRotation == 90)
        {
            ChangeDoorNames(spawnedRoom, "Door+x");
            GiveOffsetToRoom(spawnedRoom.transform, 0.226f);
            //spawnedRoom.transform.localPosition = new Vector3(spawnedRoom.transform.localPosition.x + 0.226f,  //*
            //                                                  spawnedRoom.transform.localPosition.y,           //* This is for Start Room
            //                                                  spawnedRoom.transform.localPosition.z + 0.065f); //*

            /*
            Transform corridorsOfRoom = spawnedRoom.transform.GetChild(2);
            for (int z = 0; z < corridorsOfRoom.childCount; z++)
            {
                corridorsOfRoom.GetChild(z).localPosition = new Vector3(0, 0, 0.226f);
            }
            */

        }
        else if (yRotation == 180 || yRotation == 270 || yRotation == -90)
        {
            float reqYRotationForCorridor = 0;
            if (yRotation == 180)
            {
                ChangeDoorNames(spawnedRoom, "Door-z");
                GiveOffsetToRoom(spawnedRoom.transform, -0.08f);
                reqYRotationForCorridor = 0;

                //-----------------234567-----------------
                //if (spawnedRoom.name.Equals("End Room(Clone)"))
                {
                    spawnedRoom.transform.GetChild(0).localPosition = new Vector3(spawnedRoom.transform.GetChild(0).localPosition.x - 0.303f, spawnedRoom.transform.GetChild(0).localPosition.y, spawnedRoom.transform.GetChild(0).localPosition.z + 0.31f);
                }

            }
            else if (yRotation == 270 || yRotation == -90)
            {
                
                ChangeDoorNames(spawnedRoom, "Door-x");
                GiveOffsetToRoom(spawnedRoom.transform, 0.226f);
                //spawnedRoom.transform.localPosition = new Vector3(spawnedRoom.transform.localPosition.x + 0.226f,  //*
                //                                                  spawnedRoom.transform.localPosition.y,           //* This is for Start Room
                //                                                  spawnedRoom.transform.localPosition.z - 0.065f); //*

                /*
                Transform corridorsOfRoom = spawnedRoom.transform.GetChild(2);
                for (int z = 0; z < corridorsOfRoom.childCount; z++)
                {
                    corridorsOfRoom.GetChild(z).localPosition = new Vector3(0, 0, 0.226f);
                }
                */

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
            GiveOffsetToRoom(spawnedRoom.transform, -0.08f);
        }
    }

    // --------------------------------- Checks for collisions between ROOMS ---------------------------------
    private bool NoCollisions(float[] arr)
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
