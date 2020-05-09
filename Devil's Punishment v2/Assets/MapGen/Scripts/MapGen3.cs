﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class MapGen3 : MonoBehaviour
{
    //first we'll see the ground floor
    //10 x 10 cube
    //later//private int gridSize;

    public GameObject roomsLoaderPrefab, mapGenHolder;
    public Transform mapGenHolderTransform;

    [Header("Rooms")]
    [Tooltip("Including Elevator")]
    public int numberOfRooms = 6; //Including Elevator
    private int n;
    public ArrayList allRooms = new ArrayList();
    //public List<Vector2> allRooms = new List<Vector2>();

    private ArrayList gameObjectDetails = new ArrayList();

    public GameObject[] staticRooms;
    public GameObject mainRoomIndicator, liftRoom, generatorRoom, startRoom, endRoom, laserRoom;
    
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

    public RoomNew roomNewScript;

    [Header("Test")]
    public GameObject testGridSquare;

    public SquareGrid squareGrid;

    private IEnumerator WaitForaWhile()
    {
        yield return new WaitUntil(() => Input.GetKey(KeyCode.P));
        Random.InitState(mapseed);  // changed this to our given mapseed
        Rooms();
    }

    public int mapseed;

    public void syncronizeSeeds(int seed)
    {
       


        if (GetComponent<MapGen2ndFloor>() != null)// .TryGetComponent(typeof(HingeJoint), out Component component))//(out MapGen2ndFloor mapGen2ndFloor))
        {
            GetComponent<MapGen2ndFloor>().setSeed(seed);
        }
    }


    public void startMapGeneration(int seed)
    {
        n = numberOfRooms + 1;





        float x = -(48 * ((float)(mapSizeX - 1) / 2)) - 28;
        float z = -(48 * ((float)(mapSizeZ - 1) / 2)) - 28;
        mapCentre = new Vector2(x, z);
        Debug.Log(mapCentre);

        float[] arr = new float[2];
        arr[0] = 28;
        arr[1] = 28;
        allRooms.Add(arr);
        CreateHolderForMapGen();
        //Random.InitState(100);
        //Rooms();

        // StartCoroutine(WaitForaWhile());

        //Random.state = GoodStates.states[0];
        //syncronizeSeeds(seed);

        syncronizeSeeds(seed);
        Random.InitState(seed);



        ItemGen itemGenScript = GetComponent<ItemGen>();
        roomNewScript = gameObject.AddComponent<RoomNew>();
        roomNewScript.corridors = corridors;
        roomNewScript.vents = vents;
        roomNewScript.allRooms = allRooms;
        roomNewScript.ventCover = ventCover;
        roomNewScript.mapGenHolderTransform = mapGenHolderTransform;
        roomNewScript.itemGenScript = itemGenScript;
        roomNewScript.mapSizeX = mapSizeX;
        roomNewScript.mapSizeZ = mapSizeZ;
        roomNewScript.testGridSquare = testGridSquare;
        //roomNewScript.ventCoverProbabilty = ventCoverProbabilty;
        Data.instance.roomNewScript = roomNewScript;



        //StateData.states.Add(Random.state);
        Rooms();

        Data.instance.roomsLoaderPrefab = roomsLoaderPrefab;
        Data.instance.corridorT1 = corridors[3];
        Data.instance.corridorT2 = corridors[4];
        Data.instance.corridorX = corridors[5];
        Data.instance.ventT = vents[3];
        Data.instance.ventX = vents[5];
        Data.instance.xSize = xSize;
        Data.instance.zSize = zSize;
    }



    public void CreateHolderForMapGen()
    {
        mapGenHolder = new GameObject("1st Floor");
        mapGenHolder.layer = 18;
        mapGenHolderTransform = mapGenHolder.transform;//Instantiate(mapGenHolder).transform;
        Data.instance.mapGenHolderTransform = mapGenHolderTransform;
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

        CreateHolderForMapGen();

        Rooms();
        
        //rooms();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Rooms()
    {

        int xOverall = (int)xSize * mapSizeX;
        int zOverall = (int)zSize * mapSizeZ;
        squareGrid = new SquareGrid(0, 0, xOverall, zOverall);
        squareGrid.tiles = new TileType[xOverall, zOverall];
        for (int i = 0; i < xOverall; i++)
        {
            for (int j = 0; j < zOverall; j++)
            {
                squareGrid.tiles[i, j] = TileType.Floor;
            }
        }

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
            MapgenProgress.instance.addProgress(3);

        }

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
                switch (Random.Range(1, 3)) // no modular room added
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

            float yRotation = LookToMapCentre(new Vector2(-((float[])allRooms[i])[1], -((float[])allRooms[i])[0]));//Random.Range(0, 4) * 90;
            Vector3 roomPos = new Vector3(-((float[])allRooms[i])[1], yCoord, -((float[])allRooms[i])[0]);
            if (i == 1)
            {
                Data2ndFloor.instance.liftRoomPos = roomPos;
            }

            GameObject spawnedRoom; // = Instantiate(roomToSpawn, roomPos, Quaternion.Euler(0, yRotation, 0), mapGenHolderTransform);
            RoomReferences roomReferences;

            //roomToSpawn = null;
            if (roomToSpawn == null)
            {
                Data.instance.modularRoomAssembler.door_corridor_Transform = new GameObject("Door+z").transform;
                Data.instance.modularRoomAssembler.door_corridor_Transform.position = roomPos + new Vector3(0, 0, 20); //Not Sure!!!;
                Data.instance.modularRoomAssembler.StartScript();
                spawnedRoom = Data.instance.modularRoomAssembler.roomHolderTransform.gameObject;
                roomReferences = spawnedRoom.AddComponent<RoomReferences>();
                roomReferences.doors = Data.instance.modularRoomAssembler.doors;
                roomReferences.ventParent = new GameObject("Vent Parent").transform;
            }
            else
            {
                spawnedRoom = Instantiate(roomToSpawn, roomPos, Quaternion.Euler(0, yRotation, 0), mapGenHolderTransform);
                roomReferences = spawnedRoom.GetComponent<RoomReferences>();
                CallOffsetAndDoorFns(spawnedRoom, yRotation);
            }



            Debug.Log(roomReferences.topRightCorner.position.x / -4);
            Debug.Log(roomReferences.bottomLeftCorner.position.x / -4);
            Debug.Log(roomReferences.topRightCorner.position.z / -4);
            Debug.Log(roomReferences.bottomLeftCorner.position.z / -4);

            float tempPosVal;
            if(roomReferences.topRightCorner.position.x > roomReferences.bottomLeftCorner.position.x)
            {

            }
            else
            {
                tempPosVal = roomReferences.topRightCorner.position.x;
                roomReferences.topRightCorner.position = new Vector3(roomReferences.bottomLeftCorner.position.x
                                                                     , roomReferences.topRightCorner.position.y
                                                                     , roomReferences.topRightCorner.position.z);
                roomReferences.bottomLeftCorner.position = new Vector3(tempPosVal
                                                                     , roomReferences.bottomLeftCorner.position.y
                                                                     , roomReferences.bottomLeftCorner.position.z);
            }
            if (roomReferences.topRightCorner.position.z > roomReferences.bottomLeftCorner.position.z)
            {

            }
            else
            {
                tempPosVal = roomReferences.topRightCorner.position.z;
                roomReferences.topRightCorner.position = new Vector3(roomReferences.topRightCorner.position.x
                                                                     , roomReferences.topRightCorner.position.y
                                                                     , roomReferences.bottomLeftCorner.position.z);
                roomReferences.bottomLeftCorner.position = new Vector3(roomReferences.bottomLeftCorner.position.x
                                                                     , roomReferences.bottomLeftCorner.position.y
                                                                     , tempPosVal);
            }

            for (int q = (int)(roomReferences.topRightCorner.position.x / -4); q < roomReferences.bottomLeftCorner.position.x / -4; q++)
            {
                Debug.Log("|");
                for (int r = (int)(roomReferences.topRightCorner.position.z / -4) ; r < roomReferences.bottomLeftCorner.position.z / -4; r++)
                {
                    Debug.Log("-");
                    squareGrid.tiles[q, r] = TileType.Wall;
                    Instantiate(testGridSquare, new Vector3(q * -4, 0, r * -4), Quaternion.identity);
                }
            }




            //itemGenScript.SpawnItems(roomReferences.bottomLeftCorner.position, roomReferences.topRightCorner.position, 6, spawnedRoom.transform);

            if (i != 1)
                SpawnVentCoverInRoom(i, k, roomReferences.ventParent);


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
                roomNewScript.squareGrid = squareGrid;
                for (int d = 0; d < 40; d++)
                {
                    for (int f = 0; f < 40; f++)
                    {
                        Debug.Log(squareGrid.tiles[d, f]);
                    }
                }
                roomNewScript.StartScript();
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
        if(Mathf.Abs(xChange) > Mathf.Abs(yChange))
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
    private IEnumerator AddRoomNewVents(GameObject gb)
    {
        yield return new WaitForSeconds(5f);
        //gb.AddComponent<RoomNewVents>().corridors = vents;
    }

    // ---------------------------- Connect init pos to map gen nearest room ----------------------------
    private void ConnectToMapGen(RoomNew roomNewScript)
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
            StartCoroutine(roomNewScript.ConnectTwoRooms(new Vector3(-((float[])allRooms[minIdx])[1] + 24, 1, -((float[])allRooms[minIdx])[0]), new Vector3(-48, 1, -24), "Door+x", "Door-z", Vector3.zero, new Vector3(-44, 1, -24 + 24), true)); 
        }
        else
        {
            Debug.Log("ERROR!!!!");
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
                StartCoroutine(AddRoomNewVents(gb));
            }
            else
            {
                //Instantiate(ventCover, spawnedRoomTransform.GetChild(0).GetChild(0).position, Quaternion.Euler(0, Random.Range(0, 3) * 90, 0), spawnedRoomTransform);
                Instantiate(ventCover, ventParentTransform.position, Quaternion.Euler(0, Random.Range(0, 3) * 90, 0), ventParentTransform);
            }
        }
    }

    // -------------------- Change door names --------------------
    public void ChangeDoorNames(GameObject spawnedRoom, float yRotation)
    {
        GameObject[] doors = spawnedRoom.GetComponent<RoomReferences>().doors;
        for (int i = 0; i < doors.Length; i++)
        {
            Debug.Log("doorRot ; yRotInt = " + ((int)yRotation / 90) % 4);
            spawnedRoom.GetComponent<RoomReferences>().doors[i].name = FindDoorName(((int)yRotation / 90) % 4, spawnedRoom.GetComponent<RoomReferences>().doors[i].name);
        }
    }

    private string FindDoorName(int yRotInt, string oldName)
    {
        if(yRotInt == 0)
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
            if(idx < 0)
            {
                idx += 4;
            }
            //Debug.Log("doorRot ; idx after = " + idx);
            return "Door" + Data.instance.doorRotationHelper[idx];
        }
    }

    // ---------------------------- Call offset functions accordingly ----------------------------
    public void CallOffsetAndDoorFns(GameObject spawnedRoom, float yRotation)
    {
        if (yRotation == 90)
        {
            ChangeDoorNames(spawnedRoom, yRotation);
            //ChangeDoorNames(spawnedRoom, "Door+x");
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
                ChangeDoorNames(spawnedRoom, yRotation);
                //ChangeDoorNames(spawnedRoom, "Door-z");
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
                ChangeDoorNames(spawnedRoom, yRotation);
                //ChangeDoorNames(spawnedRoom, "Door-x");
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
