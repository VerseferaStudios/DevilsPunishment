using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class RoomNew : MonoBehaviour
{
    public bool isDevMode = false;

    //private List<Transform> spawnPoints = new List<Transform>();
    [SerializeField] private List<GameObject> spawnPoints = new List<GameObject>();
    public GameObject[] corridors;
    public GameObject[] vents;
    private Transform corridorsParent;
    //private MapGen3 mapGen3;
    private float nextTime = 0f;
    private bool breakLoop = false;
    private List<Vector3> visitedRooms = new List<Vector3>();
    private Vector3 spawnNowAt;
    private int k = 0, l = 0;
    public int mapSizeX, mapSizeZ;

    /// <summary>
    /// If we are taking an extra turn forming a `L shape rather than an L shape 
    /// (if both doors and either x and x or z and z and meet other conditions)
    /// </summary>
    private bool isExtraTurn = false;

    //storedOpening is for the next L corridor in line
    private int storedOpening;

    public ArrayList allRooms = new ArrayList();
    public Transform mapGenHolderTransform;
    public float ventCoverProbabilty = 0.390f;
    public GameObject ventCover;

    public ItemGen itemGenScript;
    
    private int counter = 0;

    private bool isDoneSpawnHalf = false;

    public bool fail_room_connect;

    Color sample_room_col;

    [Header("Test")]
    public GameObject testGridCube;
    public Transform testGridPlaneHolder;
    Color testGridOrigCol;
    public float aStarVisualisationTime = 0.001f;

    public SquareGrid squareGrid;
    private bool isDoneAStarHelper = false;

    protected string corridorSpawnPointTag = "Corridor Spawn Points";
    public float roomHeight;
    protected bool isSetCorridorSPawnPointTag = false;

    public void initSeed(int seed)
    {
        UnityEngine.Random.InitState(seed);
    }

    protected virtual void SetCorridorSpawnPointTag()
    {
        isSetCorridorSPawnPointTag = true;
    }

    protected virtual void Start()
    {
        SetCorridorSpawnPointTag();
        Debug.Log("start room new");
        //StartScript(); its couroutine now man
    }

    protected virtual void SquareGridWallPopulate()
    {
        RoomReferencesModular roomReferencesModular;
        int x, z;
        for (int i = 0; i < Data.instance.roomsFloor1Modular.Count; i++)
        {
            roomReferencesModular = Data.instance.roomsFloor1Modular[i].GetComponent<RoomReferencesModular>();
            for (int j = 0; j < roomReferencesModular.roomFloors.Count; j++)
            {
                //Debug.Log("Time mod room = " + Time.time);
                x = Mathf.RoundToInt(roomReferencesModular.roomFloors[j].x / -4);
                z = Mathf.RoundToInt(roomReferencesModular.roomFloors[j].z / -4);
                squareGrid.tiles[x, z].tile = TileType.Wall;
                if (isDevMode)
                {
                    Instantiate(testGridCube, new Vector3(x * -4, 2, z * -4), Quaternion.identity);
                }
            }
        }
    }

    public IEnumerator StartScript()
    {
        
        yield return new WaitUntil(() => isSetCorridorSPawnPointTag);

        Debug.Log("corridors spawn tag = " + corridorSpawnPointTag);

        SquareGridWallPopulate();


        //mapGen3 = GameObject.FindGameObjectWithTag("Rooms(MapGen)").GetComponent<MapGen3>();

        // ------------------- Get array of doors / spawnPoints -------------------
        GameObject[] tempSpawnPoints = GameObject.FindGameObjectsWithTag(corridorSpawnPointTag);
        //string f = tempSpawnPoints[0].GetComponentsInChildren<Transform>()[0].gameObject.name;

        // ------------------- Convert array of doors / spawnPoints into list -------------------
        spawnPoints.AddRange(tempSpawnPoints);
        //Debug.Log("spawnPoints.Count = " + spawnPoints.Count);

        #region Old Exactly Overlapping Doors
        //// ------------------- Find exactly overlapping doors/spawnPoints, spawn a corridor at thst position, and destroy both doors/spawnPoints -------------------
        //for (int i = 0; i < spawnPoints.Count; i++)
        //{
        //    bool isFound = false;
        //    int lastIdx = i;
        //    for (int j = 0; j < spawnPoints.Count; j++)
        //    {
        //        if (i == j)
        //        {
        //            continue;
        //        }
        //        ////Debug.Log("i = " + i + " & j = " + j);
        //        if (spawnPoints[i].transform.position == spawnPoints[j].transform.position)
        //        {
        //            isFound = true;
        //            lastIdx = j;
        //            break;
        //        }
        //    }
        //    if (isFound)
        //    {
        //        GameObject currentCorridor = Instantiate(corridors[0], spawnPoints[i].transform.position, Quaternion.identity, Data.instance.mapGenHolderTransform);
        //        currentCorridor.layer = 18;
        //        currentCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(spawnPoints[i].transform.parent.position);
        //        currentCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(spawnPoints[lastIdx].transform.parent.position);
        //        if (spawnPoints[i].name.EndsWith("x"))
        //        {
        //            currentCorridor.transform.rotation = Quaternion.Euler(0, 90, 0);
        //        }
        //        //Debug.Log("Spawn1");
        //        //Data.instance.corridorCount++;

        //        // ------------------- Added parents position to List<Vector3> to avoid future doors of the room -------------------
        //        visitedRooms.Add(spawnPoints[i].transform.parent.position);
        //        visitedRooms.Add(spawnPoints[lastIdx].transform.parent.position);

        //        //CheckDuplicatesAndConnect(spawnPoints[i].transform.parent.transform.position, spawnPoints[lastIdx].transform.parent.transform.position);

        //        Data.instance.connectedRooms.Add(visitedRooms);

        //        ////Debug.Log(spawnPoints[i].transform.position + "______________________________________________________");
        //        spawnPoints.RemoveAt(i);

        //        // -------------- decrease lastIdx if greater than i --------------
        //        if (lastIdx > i)
        //        {
        //            lastIdx--;
        //        }

        //        i--;

        //        spawnPoints.RemoveAt(lastIdx);

        //        // -------------- decrease i if greater than lastIdx --------------
        //        if (i > lastIdx)
        //        {
        //            i--;
        //        }
        //        lastIdx--;

        //        visitedRooms = new List<Vector3>();

        //        isFound = false;
        //    }
        //}
        #endregion

        //give data the first door according to which we r sorting
        //if (spawnPoints.Count > 0)
        //{
        //    Data.instance.spawnPointsFirstPos = spawnPoints[0].transform.position;
        //}
        // ------------------- Connect two doors of different rooms with suitable corridor shapes -------------------
        StartCoroutine(CallConnectRooms());
    }

    private IEnumerator CallConnectRooms()
    {
        int times = 0;
        bool x;
        for (k = 0; k < spawnPoints.Count; k++)//or k+=2 does it matter?
        {

            //// ------------------- Remove door/spawnPoint if its of the same room -------------------
            //if (/*times == 0 && spawnPoints.Count >= 9 && */Data.instance.CheckIfVisited(spawnPoints[k].transform.parent.transform.position))
            //{
            //    ////Debug.Log("Removed a door of ____ " + spawnPoints[k].transform.parent.transform.position);
            //    spawnPoints.RemoveAt(k);
            //    k--;
            //    continue;
            //}

            for (l = 0; l < spawnPoints.Count; l++) 
            {
                if (k == l)
                {
                    continue;
                }
                //x = !CheckIfSameOrAdjacentRoom(k, l);
                //Debug.Log("Counter = " + counter + " ; !CheckIfSameOrAdjacentRoom(k, l) = " + x); 
                // ------------------------ if k and i are not in the same room ------------------------
                //if (x)
                {
                    //StartCoroutine(ShowRoomsBeingConnected(k, l, spawnPoints[k].transform.position, spawnPoints[l].transform.position));
                    Data.instance.isDoneConnectTwoRooms = false;
                    fail_room_connect = false;

                    Vector3 kParentPos = spawnPoints[k].transform.parent.position;
                    Vector3 lParentPos = spawnPoints[l].transform.parent.position;
                    if (spawnPoints[k].transform.parent.name.StartsWith("Modular"))
                    {
                        kParentPos = spawnPoints[k].transform.parent.parent.position;
                    }
                    if (spawnPoints[l].transform.parent.name.StartsWith("Modular"))
                    {
                        lParentPos = spawnPoints[l].transform.parent.parent.position;
                    }
                    if (lParentPos == kParentPos) continue;
                    Debug.Log("B444444 CONNECT TWO ROOMS = " + spawnPoints[k].transform.position + " " + spawnPoints[l].transform.position + " " +
                                    spawnPoints[k].name + " " + spawnPoints[l].name + " " +
                                    spawnPoints[k].transform.parent.position + " " + spawnPoints[l].transform.parent.position);


                    // ----------------------------------------------------------------------------------

                    // ---------------------------------- AStar Trials ----------------------------------

                    //Debug.Log("k = " + k);
                    //Debug.Log("l = " + l);
                    isDoneAStarHelper = false;

                    //if (isDevMode)
                    //    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.L));

                    StartCoroutine(AStarHelper(spawnPoints[k].transform.position, spawnPoints[l].transform.position,
                                               spawnPoints[k].name[4].ToString() + spawnPoints[k].name[5].ToString(),
                                               spawnPoints[l].name[4].ToString() + spawnPoints[l].name[5].ToString(),
                                               kParentPos, lParentPos, false));
                    yield return new WaitUntil(() => isDoneAStarHelper);

                    // ---------------------------------- AStar Trials ----------------------------------

                    // ----------------------------------------------------------------------------------

                    //spawnPoints[k].transform.parent.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial.color = sample_room_col;
                    //spawnPoints[l].transform.parent.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial.color = sample_room_col;

                    if (isDevMode)
                    {
                        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.L));
                        for (int i = 0; i < testGridPlaneHolder.childCount; i++)
                        {
                            testGridPlaneHolder.GetChild(i).GetComponent<Renderer>().material.color = testGridOrigCol;
                        }
                    }


                    //yield return new WaitUntil(() => Data.instance.isDoneConnectTwoRooms); // not needed
                    if (fail_room_connect)
                    {
                        continue;
                    }
                    break;
                }
            }


            // ------------------- In case we missed a room -------------------
            /*
            if (times < 10 && k >= spawnPoints.Count - 1 && spawnPoints.Count != 0)
            {
                k = 0;
                times++;
                //Debug.Log("times!!!!!!!!!!!!!!!!!!!!!!!!! = " + times + " && spawnPoints.Count = " + spawnPoints.Count);
            }
            */

            //Should only happen once!!!
            if (k == spawnPoints.Count - 1)
            {
                StartCoroutine(InstantiateAllCorridors());

                Debug.Log("---------------------aesrdtfgyuhij0------------------------------------");
                MapgenProgress.instance.addProgress(2);
                
		        //StartCoroutine(Data.instance.DoConnectedComponents());
                //StartCoroutine(Data.instance.DoCheckPerSecond());

                Data.instance.canStartCorridorTestSpawner = true;

            }
            //Debug.LogError(Data.instance.ctr1);

        }
        yield return null;
    }

    protected virtual void VentPos(ref Vector3 currPos)
    {

    }

    protected virtual void ASDFQWERTY(int nextMove, int currMove, int locationsCount, int i, Vector3 currLoc, int zOverall)
    {

        if (((nextMove == 0 || nextMove == 2) && (currMove == 1 || currMove == 3)) ||
            ((nextMove == 1 || nextMove == 3) && (currMove == 0 || currMove == 2)))
        {
            //L Corridor!!!!!!!
            //Debug.Log("currMove = " + currMove);
            //Debug.Log("nextMove = " + nextMove);
            List<int> openings1 = new List<int>();
            openings1.Add(nextMove);
            openings1.Add(currMove);
            if (i == -1 /*|| i == 0*/ || i + 1 == locationsCount)
            {
                Debug.LogWarning(currLoc);
            }
            AddLCorridorSpawnInfo(openings1, currLoc, zOverall, 0, 0); // (i == -1 /*|| i == 0*/ || i + 1 == locationsCount /*|| i == locations.Count*/) ? 90 : 0, 0);
            //InstantiateLCorridor(GetIdx(currLoc), currLoc, Vector3.zero, Vector3.zero);

            if (currLoc.x == -116 && currLoc.z == -36)
            {
                Debug.LogWarning("1");
            }
        }
        else //if((prevMove == 0 && currMove == 2) || (prevMove == 1 || currMove == 3) ||
             //   (prevMove == 2 && currMove == 0) || (prevMove == 3 || currMove == 1))
        {
            //! Corridor
            AddICorridorSpawnInfo(currLoc, nextMove, false, zOverall);
            //InstantiateICorridor(currLoc, Vector3.zero, Vector3.zero);

            if (currLoc.x == -116 && currLoc.z == -36)
            {
                Debug.LogWarning("1");
            }
        }
    }

    protected virtual void DoorHelper(out int move, string doorName)
    {
        move = Data.instance.doorRotationHelper.IndexOf(doorName);
    }

    public IEnumerator AStarHelper(Vector3 kPos, Vector3 lPos, string kName, string lName, Vector3 kParentPos, Vector3 lParentPos, bool fromDataSingleton)
    {
        //Debug.Log("lName = " + lName);
        //Debug.Log("kName = " + kName);
        if (isDevMode)
        {
            testGridOrigCol = testGridPlaneHolder.GetChild(0).GetComponent<Renderer>().material.color;
        }


        //Debug.Log("sqr width = " + squareGrid.width);
        //Debug.Log("sqr height = " + squareGrid.height);
        int zOverall = (int)Data.instance.zSize * mapSizeZ / 4;
        AStarSearch aStarSearch = new AStarSearch(squareGrid
                                                 , new Location(kPos / -4)
                                                 , new Location(lPos / -4)
                                                 , testGridPlaneHolder, zOverall
                                                 , aStarVisualisationTime
                                                 , isDevMode);

        StartCoroutine(aStarSearch.ShowAStar());
        yield return new WaitUntil(() => Data.instance.isDoneConnectTwoRooms);

        List<Location> locations = aStarSearch.FindPath();
        //Debug.Log("Locations count = " + locations.Count);

        Vector3 prevLoc = kPos;
        Vector3 currLoc;
        Vector3 nextLoc;
        //int prevMove = Data.instance.doorRotationHelper.IndexOf(lName);
        int currMove;
        int nextMove;

        for (int i = -1; i < locations.Count; i++)
        {
            if (i == -1)
            {
                currLoc = kPos;
                VentPos(ref currLoc);
                DoorHelper(out currMove, kName);
                //Debug.Log("Door " + kName);
                //Debug.Log("currMove = " + currMove);
            }
            else
            {
                currLoc = locations[i].vector3() * -4;
                VentPos(ref currLoc);
                //Debug.Log("currLoc = " + currLoc);
                currMove = MoveHelper(prevLoc, currLoc);
            }
            currMove += 2;
            currMove %= 4;
            if (i + 1 < locations.Count)
            {
                nextLoc = locations[i + 1].vector3() * -4;
                nextMove = MoveHelper(currLoc, nextLoc);
            }
            else
            {
                DoorHelper(out nextMove, lName);
                nextMove += 2;
                nextMove %= 4;
                //Debug.Log("Door " + lName);
                //Debug.Log("nextMove = " + nextMove);
                //Debug.Log("currMove = " + currMove);
            }
            //yield return new WaitForSeconds(0.1f);

            ASDFQWERTY(nextMove, currMove, locations.Count, i, currLoc, zOverall);

            if(currLoc.x == -116 && currLoc.z == -36)
            {
                Debug.LogWarning("1");
            }
            //Instantiate(testGridSquare, currLoc, Quaternion.identity);
            prevLoc = currLoc;
            //prevMove = currMove;
        }

        isDoneAStarHelper = true;
        fail_room_connect = false;
    }

    // -------------- Move helper assuming the cells are adjacent --------------
    private int MoveHelper(Vector3 prevLoc, Vector3 currLoc)
    {
        if(prevLoc.x == currLoc.x)
        {
            if(currLoc.z > prevLoc.z)
            {
                return 0;
            }
            else
            {
                return 2;
            }
        }
        else
        {
            if (currLoc.x > prevLoc.x)
            {
                return 1;
            }
            else
            {
                return 3;
            }
        }
    }

    protected int[] GetIdx(Vector3 pos)
    {
        int x = Mathf.RoundToInt(pos.x / -4);
        int z = Mathf.RoundToInt(pos.z / -4);

        return new int[] { x, z };
    }

    protected virtual Vector3 GetPos(int[] idx)
    {
        return new Vector3(idx[0] * -4, 0.5f + roomHeight, idx[1] * -4);
    }

    private IEnumerator InstantiateAllCorridors()//Vector3 kParentPos, Vector3 lParentPos)
    {
        Vector3 kParentPos = Vector3.zero;//REMOVE LATER!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        Vector3 lParentPos = Vector3.zero;
        for (int i = 0; i < (int)Data.instance.xSize * mapSizeX / 4; i++)
        {
            //Debug.Log("i = " + i);
            for (int j = 0; j < (int)Data.instance.zSize * mapSizeZ / 4; j++)
            {
                //Debug.Log("j = " + j);
                if (squareGrid.tiles[i, j].corridorIdx != -1)
                {
                    string corridorName = CorridorsListIdxToCorridorName(squareGrid.tiles[i, j].corridorIdx);
                    int[] kIdx = new int[] { i, j };
                    Vector3 pos = GetPos(kIdx);
                    if (corridorName.EndsWith("I"))
                    {
                        InstantiateICorridor(pos, kParentPos, lParentPos);
                    }
                    else if (corridorName.EndsWith("L"))
                    {
                        InstantiateLCorridor(kIdx, pos, kParentPos, lParentPos);
                    }
                    else if (corridorName.EndsWith("T"))
                    {
                        InstantiateTCorridor(kIdx, pos, kParentPos, lParentPos);
                    }
                    else if (corridorName.EndsWith("X"))
                    {
                        InstantiateXCorridor(pos, kParentPos, lParentPos);
                    }
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
        yield return null;
    }

    private void DistanceHelper(int i, out Vector3 newSpawnPos, Vector3 newSpawnPosOriginal)
    {
        newSpawnPos = newSpawnPosOriginal;
        if (i == 0)
        {
            //Debug.Log("dh = 0");
            newSpawnPos.z += 4;
        }
        else if (i == 1)
        {
            //Debug.Log("dh = 1");
            newSpawnPos.x += 4;
        }
        else if (i == 2)
        {
            //Debug.Log("dh = 2");
            newSpawnPos.z += -4;
        }
        else if (i == 3)
        {
            //Debug.Log("dh = 3");
            newSpawnPos.x += -4;
        }
    }

    protected void AddICorridorSpawnInfo(Vector3 posI, int movesI, bool isOverride, int zOverall)
    {
        //Debug.Log("I info");
        int[] kIdx = GetIdx(posI);
        int yRotation = (movesI == 0 || movesI == 2) ? 0 : 90;


        //Debug corridors AStar
        if (isDevMode)
        {
            if (testGridPlaneHolder != null)
            {
                int idx = kIdx[0] * zOverall + kIdx[1];
                testGridPlaneHolder.GetChild(idx).GetComponent<Renderer>().material.color = Color.green;
            }
        }


        /*
        if(Vector3.Distance(posI, new Vector3(-124, 0, -20)) < 6)
        {
            //Debug.Log("I Corr collision check => ");
            //Debug.Log("squareGrid.tiles[kIdx[0]][kIdx[1]].corridorIdx = " + squareGrid.tiles[kIdx[0]][kIdx[1]].corridorIdx);
            //Debug.Log("squareGrid.tiles[kIdx[0]][kIdx[1]].corridorYRot = " + squareGrid.tiles[kIdx[0]][kIdx[1]].corridorYRot);
        }
        */

        if (squareGrid.tiles[kIdx[0], kIdx[1]].corridorIdx == -1
           && squareGrid.tiles[kIdx[0], kIdx[1]].corridorYRot == -1)
        {
            squareGrid.tiles[kIdx[0], kIdx[1]].corridorIdx = 0;
            squareGrid.tiles[kIdx[0], kIdx[1]].corridorYRot = yRotation;
        }
        else if (!(squareGrid.tiles[kIdx[0], kIdx[1]].corridorIdx == 0
                && squareGrid.tiles[kIdx[0], kIdx[1]].corridorYRot == (int)yRotation))
        /*(squareGrid.tiles[kIdx[0], kIdx[1]].corridorIdx == 0 
        && (squareGrid.tiles[kIdx[0], kIdx[1]].corridorYRot == yRotation + 90
        || squareGrid.tiles[kIdx[0], kIdx[1]].corridorYRot == yRotation - 90)
        && !isOverride)*/
        {
            AddCollisionInfoHelper(kIdx, yRotation, "CorridorI", zOverall, squareGrid.tiles[kIdx[0], kIdx[1]].childEulerZ,
                                                                           squareGrid.tiles[kIdx[0], kIdx[1]].childEulerX);
        }
    }

    protected virtual void AddLCorridorSpawnInfo(List<int> openings, Vector3 posToSpawn, int zOverall, int childEulerZ, int childEulerX)
    {
        //Debug.Log("L info");
        float yRotation = Data.instance.ConvertToRotation(openings);
        int corridorIdx = ChooseLCorridor(yRotation);
        int[] kIdx = GetIdx(posToSpawn);


        //Debug corridors AStar
        if (isDevMode)
        {
            if (testGridPlaneHolder != null)
            {
                int idx = kIdx[0] * zOverall + kIdx[1];
                testGridPlaneHolder.GetChild(idx).GetComponent<Renderer>().material.color = Color.red;
            }
        }


        if (squareGrid.tiles[kIdx[0], kIdx[1]].corridorIdx == -1
           && squareGrid.tiles[kIdx[0], kIdx[1]].corridorYRot == -1)
        {
            squareGrid.tiles[kIdx[0], kIdx[1]].corridorIdx = corridorIdx;
            squareGrid.tiles[kIdx[0], kIdx[1]].corridorYRot = (int)yRotation;
            squareGrid.tiles[kIdx[0], kIdx[1]].childEulerZ = childEulerZ;
            squareGrid.tiles[kIdx[0], kIdx[1]].childEulerX = childEulerX;
        }
        else if (!(squareGrid.tiles[kIdx[0], kIdx[1]].corridorIdx == corridorIdx
            && squareGrid.tiles[kIdx[0], kIdx[1]].corridorYRot == (int)yRotation))
        //&& !isOverride)
        {
            AddCollisionInfoHelper(kIdx, (int)yRotation, "CorridorL", zOverall, childEulerZ, childEulerX);
        }
    }

    protected virtual void AddTCorridorSpawnInfo(int[] kIdx, int yRotationNew, int zOverall, int childEulerZ, int childEulerX, bool isOverride)
    {

            Debug.LogWarning("1234");

        //Debug corridors AStar
        if (isDevMode)
        {
            if (testGridPlaneHolder != null)
            {
                int idx = kIdx[0] * zOverall + kIdx[1];
                testGridPlaneHolder.GetChild(idx).GetComponent<Renderer>().material.color = Color.black;
            }
        }


        //Debug.Log("T info");
        if (squareGrid.tiles[kIdx[0], kIdx[1]].corridorIdx == -1
           && squareGrid.tiles[kIdx[0], kIdx[1]].corridorYRot == -1 ||
           isOverride)
        {
            squareGrid.tiles[kIdx[0], kIdx[1]].corridorIdx = 3; //or 4
            squareGrid.tiles[kIdx[0], kIdx[1]].corridorYRot = yRotationNew;
            squareGrid.tiles[kIdx[0], kIdx[1]].childEulerZ = childEulerZ;
            squareGrid.tiles[kIdx[0], kIdx[1]].childEulerX = childEulerX;
        }
        else if (!(squareGrid.tiles[kIdx[0], kIdx[1]].corridorIdx == 3
            && squareGrid.tiles[kIdx[0], kIdx[1]].corridorYRot == yRotationNew))
        //&& !isOverride)
        {
            AddCollisionInfoHelper(kIdx, yRotationNew, "CorridorT", zOverall, childEulerZ, childEulerX);
        }
    }

    protected void AddXCorridorSpawnInfo(int[] kIdx, int zOverall)//, Vector3 kParentPos, Vector3 lParentPos, List<int> openings)
    {


        //Debug corridors AStar
        if (isDevMode)
        {
            if (testGridPlaneHolder != null)
            {
                int idx = kIdx[0] * zOverall + kIdx[1];
                testGridPlaneHolder.GetChild(idx).GetComponent<Renderer>().material.color = Color.white;
            }
        }


        //Debug.Log("X info");
        squareGrid.tiles[kIdx[0], kIdx[1]].corridorIdx = 5;
        squareGrid.tiles[kIdx[0], kIdx[1]].corridorYRot = 0;
    }

    protected virtual void AddCollisionInfoHelper(int[] kIdx, int yRotation, string corridorName, int zOverall, int childEulerZ, int childEulerX)
    {
        List<int> openings1, openings2;
        openings1 = Data.instance.ConvertToOpenings(corridorName, yRotation, false);
        openings2 = Data.instance.ConvertToOpenings(CorridorsListIdxToCorridorName(squareGrid.tiles[kIdx[0], kIdx[1]].corridorIdx),
                                                    squareGrid.tiles[kIdx[0], kIdx[1]].corridorYRot, false);

        //Debug.Log("T or X pos = " + GetPos(kIdx));
        //Debug.Log("corridorName = " + corridorName);
        //Debug.Log("yRotation = " + yRotation);
        //Debug.Log("openings1 = " + openings1);
        //foreach (var item in openings1)
        //{
        //    Debug.Log(item);
        //}
        //Debug.Log("openings2 = " + openings2);
        //foreach (var item in openings2)
        //{
        //    Debug.Log(item);
        //}


        openings1.AddRange(openings2);

        openings1 = openings1.Distinct().ToList();


        if (openings1.Count == 3)
        {
            if (corridorName.EndsWith("T"))
                //&& squareGrid.tiles[kIdx[0], kIdx[1]].corridorYRot == yRotation) //no Need
            {
                squareGrid.tiles[kIdx[0], kIdx[1]].corridorIdx = 3; //or 4
                squareGrid.tiles[kIdx[0], kIdx[1]].corridorYRot = yRotation;
                return;
            }
            float yRotationNew = Data.instance.ConvertToRotation(openings1);
            AddTCorridorSpawnInfo(kIdx, (int)yRotationNew, zOverall, childEulerZ, childEulerX, true);
        }
        else if (openings1.Count == 4)
        {
            AddXCorridorSpawnInfo(kIdx, zOverall);
        }
    }

    protected virtual void HelperIInstantiate(GameObject currentCorridor, int[] kIdx, Vector3 posI)
    {
        currentCorridor.transform.GetChild(0).localPosition = new Vector3(0, 0, (squareGrid.tiles[kIdx[0], kIdx[1]].corridorYRot == 0) ? -0.08f : 0.226f);

        //For now, later remove and put outside this else block

        if (UnityEngine.Random.Range(0.0f, 1.0f) < ventCoverProbabilty)
        {
            Debug.Log("spawning vent cover at " + posI);
            Instantiate(ventCover, posI, Quaternion.Euler(0, UnityEngine.Random.Range(0, 3) * 90, 0), currentCorridor.transform);
        }

        // ----------- Item Gen -----------
        if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.1f)
        {
            itemGenScript.SpawnItems(posI - new Vector3(1, 0, 1), posI + new Vector3(1, 0, 1), 1, currentCorridor.transform);
            Data.instance.ctr1++;
        }
    }

    protected virtual void IRotationHelper(int yRotation, GameObject currentCorridor)
    {
        currentCorridor.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    private void InstantiateICorridor(Vector3 posI, Vector3 kParentPos, Vector3 lParentPos)
    {
        int[] kIdx = GetIdx(posI);
        GameObject currentCorridor;
        currentCorridor = Instantiate(corridors[0]/*corridors[squareGrid.tiles[kIdx[0], kIdx[1]].corridorIdx]*/, posI, Quaternion.identity, mapGenHolderTransform);

        currentCorridor.layer = 18;
        /*
        //Move CollisionDetector of corridor I by -0.25f in x axis to keep it in grid
        Transform collisionDetectorTransform = currentCorridor.transform.GetChild(1);
        collisionDetectorTransform.position = new Vector3(collisionDetectorTransform.position.x - 0.25f, collisionDetectorTransform.position.y, collisionDetectorTransform.position.z);
        */
        //currentCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(kParentPos);
        //currentCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(lParentPos);

        IRotationHelper(squareGrid.tiles[kIdx[0], kIdx[1]].corridorYRot, currentCorridor);
        //Data.instance.corridorCount++;

        HelperIInstantiate(currentCorridor, kIdx, posI);

    }

    protected virtual void LRotScaleHelper(int yRotation, GameObject currCorridor1, int childEulerZ)
    {
        currCorridor1.transform.rotation = Quaternion.Euler(0, yRotation, 0);
        if (yRotation == 0) //&& squareGrid.tiles[kIdx[0], kIdx[1]].childEulerZ != 0
        {
            //currCorridor1.GetComponentInChildren<BoxCollider>().enabled = false;
            currCorridor1.transform.localScale = new Vector3(-1, 1, 1);
            //currCorridor1.GetComponentInChildren<BoxCollider>().enabled = true;
            currCorridor1.transform.rotation = Quaternion.Euler(0, 90, 0);
        }
    }

    private void InstantiateLCorridor(int[] kIdx, Vector3 posToSpawn, Vector3 kParentPos, Vector3 lParentPos)
    {
        //posToSpawn.y += (squareGrid.tiles[kIdx[0], kIdx[1]].childEulerZ != 0) ? 2 : 0;
        GameObject currCorridor1 = Instantiate(corridors[squareGrid.tiles[kIdx[0], kIdx[1]].corridorIdx], posToSpawn, Quaternion.identity, Data.instance.mapGenHolderTransform);
        currCorridor1.layer = 18;
        //currCorridor1.GetComponentInChildren<CorridorNew>().rooms.Add(kParentPos);
        //currCorridor1.GetComponentInChildren<CorridorNew>().rooms.Add(lParentPos);

        LRotScaleHelper(squareGrid.tiles[kIdx[0], kIdx[1]].corridorYRot, currCorridor1, squareGrid.tiles[kIdx[0], kIdx[1]].childEulerZ);

    }

    protected virtual void HelperTInstantiate(float yRotation, GameObject currCorridor)
    {
        if (yRotation == 0)
        {
            //isErroneousTCorr = true;
            currCorridor.transform.GetChild(0).localPosition = new Vector3(0.15f, 0, -0.155f);
            //currCorridor.transform.localPosition += new Vector3(0, 5, 0);
        }
        if (yRotation == 270 || yRotation == -90 || yRotation == 180)
        {

            if (yRotation == -90 || yRotation == 270)
            {
                currCorridor.transform.GetChild(0).localPosition = new Vector3(0.156f, 0, -0.156f);
            }

            //MeshCollider bc = currCorridor.GetComponentInChildren<MeshCollider>();
            //Destroy(bc);
            currCorridor.transform.localScale = new Vector3(-1, 1, 1);
            //currCorridor.transform.Find("CollisionDetector").gameObject.AddComponent<MeshCollider>().size = new Vector3(1, 0.5f, 1);
        }
    }

    protected virtual void TRotationScaleHelper(float yRotation, GameObject currCorridor, int childEulerX)
    {
        currCorridor.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    private void InstantiateTCorridor(int[] kIdx, Vector3 posToSpawn, Vector3 kParentPos, Vector3 lParentPos)
    {
        float yRotation = squareGrid.tiles[kIdx[0], kIdx[1]].corridorYRot;
        //posToSpawn.y += (squareGrid.tiles[kIdx[0], kIdx[1]].childEulerZ != 0) ? 2 : 0;
        GameObject currCorridor = Instantiate((yRotation == 0 || yRotation == 270 || yRotation == -90) ? corridors[4] : corridors[3], posToSpawn, Quaternion.identity, mapGenHolderTransform);
        //MapgenProgress.instance.addProgress(1);

        HelperTInstantiate(yRotation, currCorridor);

        TRotationScaleHelper(yRotation, currCorridor, squareGrid.tiles[kIdx[0], kIdx[1]].childEulerX);

        //currCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(kParentPos);
        //currCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(lParentPos);

        // TODO: Commented out for console clear 10/02/19
        // Debug.Log("added T at " + currCorridor.transform.position + " with yRot " + yRotation + " and scale " + currCorridor.transform.localScale);
    }

    private void InstantiateXCorridor(Vector3 posToSpawn, Vector3 kParentPos, Vector3 lParentPos)
    {
        //spawnAtPos.x = Mathf.Round(posToSpawn.x);
        //spawnAtPos.z = Mathf.Round(posToSpawn.z);
        /*CorridorNew corridorNew = */Instantiate(corridors[5], posToSpawn, Quaternion.identity, mapGenHolderTransform)/*.GetComponentInChildren<CorridorNew>()*/;

        //corridorNew.rooms.Add(kParentPos);
        //corridorNew.rooms.Add(lParentPos);

        //MapgenProgress.instance.addProgress(2);
    }

    protected string CorridorsListIdxToCorridorName(int idx)
    {
        if (idx == 0)
        {
            return "CorridorI";
        }
        else if (idx == 3 || idx == 4)
        {
            return "CorridorT";
        }
        else if (idx == 5)
        {
            return "CorridorX";
        }
        else if (idx == 1 || idx == 2 || idx == 7)
        {
            return "CorridorL";
        }
        else //if(idx == 6)
        {
            return "CorridorDeadEnd";
        }
    }

    //public NavMeshAgent navMeshAgent;
    //public Vector3 NavMeshAI()
    //{
    //    navMeshAgent.enabled = true;
    //    if (navMeshAgent.isOnNavMesh)
    //    {
    //        NavMeshPath path = new NavMeshPath();
    //        if(navMeshAgent.CalculatePath(Vector3.zero, path))
    //        {
    //            navMeshAgent.path = path;
    //        }
    //    }
    //    Vector3 pos = navMeshAgent.steeringTarget;
    //    navMeshAgent.enabled = false;
    //    return pos;
    //}


    public IEnumerator ShowRoomsBeingConnected(int k, int l, Vector3 kPos, Vector3 lPos)
    {
        counter++;
        //if(roomsHelper != null)
        {
            //Destroy(roomsHelper.gameObject);
        }
        //roomsHelper = new GameObject("RoomsHelper " + counter).transform;

        Transform t = new GameObject(counter + " = " + k + ", " + l + " 1").transform;
        t.position = kPos;
        t = new GameObject(counter + " = " + k + ", " + l + " 2").transform;
        t.position = lPos;

        //yield return new WaitUntil(() => Input.GetKey(KeyCode.Tab));
        yield return null;
    }

    protected virtual int ChooseLCorridor(float yRotation)
    {
        return (yRotation == 0 || yRotation == 180) ? 2 : ((yRotation == 90) ? 7 : 1);
    }

    //public int Compare(GameObject x, GameObject y)
    //{
    //    return (int)(Vector3.Distance(x.transform.position, Data.instance.spawnPointsFirstPos)
    //                - Vector3.Distance(y.transform.position, Data.instance.spawnPointsFirstPos));
    //}

    #region Old Map Gen
    //public IEnumerator ConnectTwoRoomsOld(Vector3 kPos, Vector3 lPos, string kName, string lName, Vector3 kParentPos, Vector3 lParentPos, bool fromDataSingleton)
    //{
    //    //yield return new WaitUntil(() => Input.GetKey(KeyCode.Tab));
    //    //making all y coordinates of all corridors equal to 0.5f
    //    //StartCoroutine(ShowRoomsBeingConnected(k, l, kPos, lPos));
    //    kPos.y = lPos.y = 0.5f;
    //    //Debug.Log("kPos and lPos = " + kPos + " " + lPos);

    //    Vector3 targetPos = new Vector3(0, 3, 0);

    //    Vector3 From = kPos;

    //    Debug.Log("CONNECT 2 Rooms = " + From + " from and to " + lPos + "Door names ; kName = " + kName + " ; lName = " + lName);

    //    // ------------------- Doesnt (xD) Connects (x and x) or (z and z) doors in different rooms with I shape with no hindrance -------------------
    //    /*
    //    if (
    //        // -------------- if x doors AND z differnce == xSize --------------
    //        (kPos.x == lPos.x && Mathf.Abs(kPos.z - lPos.z) == Data.instance.xSize)
    //        ||
    //        // -------------- if z doors AND x differnce == 10 --------------
    //        (kPos.z == lPos.z && Mathf.Abs(kPos.x - lPos.x) == Data.instance.xSize)
    //        )
    //    {
    //        //targetPos = spawnPoints[i].transform.position;
    //        //Debug.Log("Spawn4--");
    //    }
    //    */
    //    if (kParentPos == lParentPos)
    //    {
    //        Debug.Log("Error CONNECT 2 Rooms = " + lPos + " and " + kPos);
    //        fail_room_connect = true;
    //        //isDoneConnectTwoRooms = true;
    //        yield return null;
    //    }
    //    // ------------------- Connects x and z doors with L shape with no hindrance -------------------
    //    else if (kName.EndsWith("x") && lName.EndsWith("z"))
    //    {
    //        targetPos = new Vector3(From.x, 0.5f, lPos.z);
    //        Debug.Log("Spawn2");
    //    }

    //    // ------------------- Connects z and x doors with L shape with no hindrance -------------------
    //    else if (kName.EndsWith("z") && lName.EndsWith("x"))
    //    {
    //        targetPos = new Vector3(lPos.x, 0.5f, From.z);
    //        Debug.Log("Spawn3");
    //    }

    //    // --------------------- Connects x and x doors ---------------------
    //    else if (kName.EndsWith("x") && lName.EndsWith("x"))
    //    {
    //        //-------------- Connects x and x doors with `L shape to avoid hindrance --------------
    //        if (kPos.x != lPos.x)
    //        {
    //            //check and go nearer to destination
    //            Vector3 to = kPos;
    //            to.z += Data.instance.xSize / 2;     //Check 5 or 6 !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

    //            // ------------------- Calls the actual spawning function -------------------
    //            isDoneSpawnHalf = false;
    //            StartCoroutine(spawnHalf(kPos, to, true, kName, lName, kParentPos, lParentPos));
    //            yield return new WaitUntil(() => isDoneSpawnHalf);
    //            isExtraTurn = true;
    //            From = to;
    //            targetPos = new Vector3(lPos.x, 0.5f, From.z);
    //            Debug.Log("Spawn5a");
    //            /*
    //            //Debug.Log("From = " + From);
    //            //Debug.Log(targetPos);
    //            //Debug.Log(spawnPoints[i].transform.position);
    //            */
    //        }
    //        //-------------- Connects x and x doors with I shape since there's no hindrance --------------
    //        else
    //        {
    //            Debug.Log("Spawn5b");
    //            targetPos = lPos;
    //        }
    //    }

    //    // --------------------- Connects z and z doors ---------------------
    //    else if (kName.EndsWith("z") && lName.EndsWith("z"))
    //    {
    //        //-------------- Connects z and z doors with `L shape to avoid hindrance --------------
    //        if (kPos.z != lPos.z)
    //        {
    //            //check and go nearer to destination
    //            Vector3 to = kPos;
    //            to.x += Data.instance.xSize / 2;     //Check 5 or 6 !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

    //            // ------------------- Calls the actual spawning function -------------------
    //            isDoneSpawnHalf = false;
    //            StartCoroutine(spawnHalf(kPos, to, true, kName, lName, kParentPos, lParentPos));
    //            yield return new WaitUntil(() => isDoneSpawnHalf);
    //            isExtraTurn = true;
    //            From = to;
    //            targetPos = new Vector3(From.x, 0.5f, lPos.z);
    //            Debug.Log("Spawn6a");
    //            /*
    //            //Debug.Log("From = " + From);
    //            //Debug.Log(targetPos);
    //            //Debug.Log(spawnPoints[i].transform.position);
    //            */
    //        }
    //        //-------------- Connects z and z doors with I shape since there's no hindrance --------------
    //        else
    //        {
    //            Debug.Log("Spawn6b");
    //            targetPos = lPos;
    //        }
    //    }

    //    if(targetPos == new Vector3(0, 3, 0))
    //    {
    //        fail_room_connect = true;
    //        //isDoneConnectTwoRooms = true;
    //        yield return null;
    //    }

    //    // ------------------- Calls the actual spawning function -------------------
    //    isDoneSpawnHalf = false;
    //    StartCoroutine(spawnHalf(From, targetPos, !isExtraTurn, kName, lName, kParentPos, lParentPos));
    //    yield return new WaitUntil(() => isDoneSpawnHalf);

    //    isExtraTurn = false;

    //    if (targetPos != lPos)
    //    {
    //        isDoneSpawnHalf = false;
    //        StartCoroutine(spawnHalf(targetPos, lPos, false, kName, lName, kParentPos, lParentPos));
    //        yield return new WaitUntil(() => isDoneSpawnHalf);
    //    }

    //    //GameObject currCorridor1 = Instantiate(corridors[ChooseLCorridor(yRotation)], spawnNowAt, Quaternion.identity, Data.instance.mapGenHolderTransform);

    //    // --------------- Add L corridor to door at end room ---------------
    //    List<int> openings = new List<int>();

    //    if (targetPos.x == lPos.x)
    //    {
    //        //Add the previously stored storedOpening meant for this L corridor
    //        openings.Add(storedOpening);
    //    }
    //    else if (targetPos.z == lPos.z)
    //    {
    //        //Add the previously stored storedOpening meant for this L corridor
    //        openings.Add(storedOpening);
    //    }

    //    //Add opening according to the door type wuth the help of Data.instance.nearDoorL
    //    openings.Add(Data.instance.NeardoorLIndexSearch(lName[4].ToString() + lName[5].ToString()));
    //    /*
    //    //Debug.Log(Data.instance.ConvertToRotation(openings) + " " + (storedOpening == 0 ? 2 : 0) + " " + (lName[4].ToString() + lName[5].ToString())
    //            + " " + kPos + " " + lPos + " "
    //            + spawnPoints[k].name + " " + lName);
    //    */
    //    //Debug.Log(Data.instance.ConvertToRotation(openings) + " " + openings[0] + " " + openings[1]
    //    //+ " " + kPos + " " + lPos);

    //    float yRotation = Data.instance.ConvertToRotation(openings);
    //    GameObject currCorridor1 = Instantiate(corridors[ChooseLCorridor(yRotation)], lPos, Quaternion.identity, Data.instance.mapGenHolderTransform); 
    //    currCorridor1.layer = 18;
    //    currCorridor1.GetComponentInChildren<CorridorNew>().rooms.Add(kParentPos);
    //    currCorridor1.GetComponentInChildren<CorridorNew>().rooms.Add(lParentPos);
    //    currCorridor1.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    //    if (yRotation == 0)
    //    {
    //        //currCorridor1.GetComponentInChildren<BoxCollider>().enabled = false;
    //        currCorridor1.transform.localScale = new Vector3(-1, 1, 1);
    //        //currCorridor1.GetComponentInChildren<BoxCollider>().enabled = true;
    //        currCorridor1.transform.rotation = Quaternion.Euler(0, 90, 0);
    //    }


    //    // ------------------- Added parents position to List<Vector3> to avoid future doors of the room -------------------
    //    visitedRooms.Add(lParentPos);
    //    visitedRooms.Add(kParentPos);

    //    //CheckDuplicatesAndConnect(lParentPos, spawnPoints[k].transform.parent.transform.position);

    //    Data.instance.connectedRooms.Add(visitedRooms);

    //    /*
    //    //Debug.Log("VISITED ROOMS!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
    //    foreach (var item in visitedRooms)
    //    {
    //        //Debug.Log(item);
    //    }
    //    //Debug.Log("CONNECTED ROOMS!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
    //    foreach (var item in Data.instance.connectedRooms)
    //    {
    //        foreach (var item1 in item)
    //        {
    //            //Debug.Log(item1);
    //        }
    //    }
    //    */

    //    visitedRooms = new List<Vector3>();

    //    ////Debug.Log("Added a door of ____ " + spawnPoints[i].transform.parent.transform.position);
    //    ////Debug.Log("Added a door of ____ " + spawnPoints[k].transform.parent.transform.position);

    //    // ---------------------- Removes the used doors ----------------------

    //    if (!fromDataSingleton)
    //    {
    //        spawnPoints.RemoveAt(l);

    //        // -------------- decrease k if greater than i --------------                
    //        if (k > l)
    //        {
    //            k--;
    //        }
    //        l--;

    //        spawnPoints.RemoveAt(k);

    //        // -------------- decrease k if greater than i --------------                
    //        if (l > k)
    //        {
    //            l--;
    //        }
    //        k--;
    //    }
    //    //isDoneConnectTwoRooms = true;
    //    yield return null;
    //}

    //// ---------------------- Spawns I corridors from "Vector3 From", to "Vector3 to" except start and finish (where L corridor is needed)----------------------
    //private IEnumerator spawnHalf(Vector3 From, Vector3 to, bool isFirst, string kName, string lName, Vector3 kParentPos, Vector3 lParentPos)
    //{
    //    // ----------- Variable for position to spawn at each for loop step -----------                
    //    spawnNowAt = From;
    //    Debug.Log("SPAWN HALF = " + From + " from and to " + to);
    //    // ----------- Variable for corridor to spawn at each for loop step -----------                
    //    GameObject corridorToSpawn = corridors[0];

    //    // -------------- Spawns corridors along z axis since x coord is constant --------------                
    //    if (From.x == to.x)
    //    {
    //        int increment = (From.z > to.z) ? -4 : 4;

    //        // ----------- Skips required corridors ----------- 
    //        int i = 1;

    //        //Instantiates L corridor in correct rotation at the door of a room
    //        if (isFirst)
    //        {
    //            List<int> openings = new List<int>();

    //            //Add opening according to the door type wuth the help of Data.instance.nearDoorL
    //            openings.Add(Data.instance.NeardoorLIndexSearch(kName[4].ToString() + kName[5].ToString()));

    //            //storedOpening is for the next L corridor in line
    //            storedOpening = (From.z > to.z) ? 0 : 2;

    //            //Add the opposite of storedOpening since this one will be facing the next L corridor
    //            openings.Add(storedOpening == 0 ? 2 : 0);

    //            //Debug.Log(Data.instance.ConvertToRotation(openings) + " " + (storedOpening == 0 ? 2 : 0) + " " + (kName[4].ToString() + kName[5].ToString())
    //                //+ " " + spawnPoints[k].transform.position + " " + spawnPoints[l].transform.position + " "
    //                //+ kName + " " + lName);

    //            float yRotation = Data.instance.ConvertToRotation(openings);

    //            GameObject currCorridor1 = Instantiate(corridors[ChooseLCorridor(yRotation)], spawnNowAt, Quaternion.identity, Data.instance.mapGenHolderTransform);
    //            currCorridor1.layer = 18;
    //            currCorridor1.GetComponentInChildren<CorridorNew>().rooms.Add(kParentPos);
    //            currCorridor1.GetComponentInChildren<CorridorNew>().rooms.Add(lParentPos);
    //            currCorridor1.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    //            if (yRotation == 0)
    //            {
    //                //currCorridor1.GetComponentInChildren<BoxCollider>().enabled = false;
    //                currCorridor1.transform.localScale = new Vector3(-1, 1, 1);
    //                //currCorridor1.GetComponentInChildren<BoxCollider>().enabled = true;
    //                currCorridor1.transform.rotation = Quaternion.Euler(0, 90, 0);
    //            }

    //            isFirst = false;
    //        }
    //        //Instantiate L corridor in correct rotation at the join of two straight corridors (which are in L shape)
    //        else
    //        {
    //            List<int> openings = new List<int>();

    //            //Debug.Log(Data.instance.ConvertToRotation(openings) + " " + storedOpening + " " + ((From.z > to.z) ? 2 : 0)
    //                //+ " " + spawnPoints[k].transform.position + " " + spawnPoints[l].transform.position + " "
    //                //+ kName + " " + lName);

    //            //Add the previously stored storedOpening meant for this L corridor
    //            openings.Add(storedOpening);

    //            //storedOpening is for the next L corridor in line
    //            storedOpening = (From.z > to.z) ? 0 : 2;

    //            //Add the opposite of storedOpening since this one will be facing the next L corridor
    //            openings.Add(storedOpening == 0 ? 2 : 0);

    //            float yRotation = Data.instance.ConvertToRotation(openings);

    //            GameObject currCorridor1 = Instantiate(corridors[ChooseLCorridor(yRotation)], spawnNowAt, Quaternion.identity, Data.instance.mapGenHolderTransform);
    //            currCorridor1.layer = 18;
    //            currCorridor1.GetComponentInChildren<CorridorNew>().rooms.Add(kParentPos);
    //            currCorridor1.GetComponentInChildren<CorridorNew>().rooms.Add(lParentPos);
    //            currCorridor1.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    //            if (yRotation == 0)
    //            {
    //                //currCorridor1.GetComponentInChildren<BoxCollider>().enabled = false;
    //                currCorridor1.transform.localScale = new Vector3(-1, 1, 1);
    //                //currCorridor1.GetComponentInChildren<BoxCollider>().enabled = true;
    //                currCorridor1.transform.rotation = Quaternion.Euler(0, 90, 0);
    //            }
    //        }
    //        spawnNowAt.z += increment;

    //        //Spawn I corridors
    //        for (; i < Mathf.Abs(From.z - to.z) / Data.instance.corridorSize; i++)
    //        {
    //            ////Debug.Log("Loop 1 = " + i);
    //            //yield return new WaitForSeconds(0.25f);
    //            GameObject currentCorridor = Instantiate(corridorToSpawn, spawnNowAt/*new Vector3(spawnNowAt.x + 0.15f/*- 0.25f, spawnNowAt.y, spawnNowAt.z)*/, Quaternion.identity, Data.instance.mapGenHolderTransform);
    //            currentCorridor.layer = 18;
    //            /*
    //            //Move CollisionDetector of corridor I by +0.25f in x axis to keep it in grid
    //            Transform collisionDetectorTransform = currentCorridor.transform.GetChild(1);
    //            collisionDetectorTransform.position = new Vector3(collisionDetectorTransform.position.x + 0.25f, collisionDetectorTransform.position.y, collisionDetectorTransform.position.z);
    //            */
    //            currentCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(kParentPos);
    //            currentCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(lParentPos);
    //            /*
    //            Data.instance.corridorCount++;
    //            if (Data.instance.isCollided)
    //            {
    //                Data.instance.isCollided = false;
    //                //check current corridor and rotation. check the already instantiated once type AND rotation (using other) ?????//check later

    //            }
    //            */
    //            currentCorridor.transform.GetChild(0).localPosition = new Vector3(0, 0, -0.08f);

    //            if (UnityEngine.Random.Range(0.0f, 1.0f) < ventCoverProbabilty)
    //            {
    //                Instantiate(ventCover, spawnNowAt, Quaternion.Euler(0, UnityEngine.Random.Range(0, 3) * 90, 0), currentCorridor.transform);
    //            }

    //            // ----------- Item Gen -----------
    //            if(UnityEngine.Random.Range(0.0f, 1.0f) < 0.1f)
    //            {
    //                itemGenScript.SpawnItems(spawnNowAt - new Vector3(1, 0, 1), spawnNowAt + new Vector3(1, 0, 1), 1, currentCorridor.transform);
    //                Data.instance.ctr1++;
    //            }

    //            spawnNowAt.z += increment;
    //        }
    //    }

    //    // -------------- Spawns corridors along x axis since z coord is constant --------------                
    //    else if (From.z == to.z)
    //    {
    //        int increment = (From.x > to.x) ? -4 : 4;

    //        // ----------- Skips required corridors ----------- 
    //        int i = 1;

    //        //Instantiates L corridor in correct rotation at the door of a room
    //        if (isFirst)
    //        {
    //            List<int> openings = new List<int>();

    //            //Add opening according to the door type wuth the help of Data.instance.nearDoorL
    //            openings.Add(Data.instance.NeardoorLIndexSearch(kName[4].ToString() + kName[5].ToString()));

    //            //storedOpening is for the next L corridor in line
    //            storedOpening = (From.x > to.x) ? 1 : 3;

    //            //Add the opposite of storedOpening since this one will be facing the next L corridor
    //            openings.Add(storedOpening == 1 ? 3 : 1);

    //            //Debug.Log(Data.instance.ConvertToRotation(openings) + " " + ((From.x > to.x) ? 3 : 1) + " " + (kName[4].ToString() + kName[5].ToString())
    //                //+ " " + spawnPoints[k].transform.position + " " + spawnPoints[l].transform.position + " "
    //                //+ kName + " " + lName);

    //            float yRotation = Data.instance.ConvertToRotation(openings);

    //            GameObject currCorridor1 = Instantiate(corridors[ChooseLCorridor(yRotation)], spawnNowAt, Quaternion.identity, Data.instance.mapGenHolderTransform);
    //            currCorridor1.layer = 18;
    //            currCorridor1.GetComponentInChildren<CorridorNew>().rooms.Add(kParentPos);
    //            currCorridor1.GetComponentInChildren<CorridorNew>().rooms.Add(lParentPos);
    //            currCorridor1.transform.rotation = Quaternion.Euler(0, yRotation, 0);

    //            if (yRotation == 0)
    //            {
    //                //currCorridor1.GetComponentInChildren<BoxCollider>().enabled = false;
    //                currCorridor1.transform.localScale = new Vector3(-1, 1, 1);
    //                //currCorridor1.GetComponentInChildren<BoxCollider>().enabled = true;
    //                currCorridor1.transform.rotation = Quaternion.Euler(0, 90, 0);
    //            }

    //            isFirst = false;
    //        }
    //        //Instantiate L corridor in correct rotation at the join of two straight corridors (which are in L shape)
    //        else
    //        {
    //            List<int> openings = new List<int>();

    //            //Debug.Log(Data.instance.ConvertToRotation(openings) + " " + storedOpening + " " + ((From.x > to.x) ? 3 : 1)
    //                //+ " " + spawnPoints[k].transform.position + " " + spawnPoints[l].transform.position + " "
    //                //+ kName + " " + lName);

    //            //Add the previously stored storedOpening meant for this L corridor
    //            openings.Add(storedOpening);

    //            //storedOpening is for the next L corridor in line
    //            storedOpening = (From.x > to.x) ? 1 : 3;

    //            //Add the opposite of storedOpening since this one will be facing the next L corridor
    //            openings.Add(storedOpening == 1 ? 3 : 1);

    //            float yRotation = Data.instance.ConvertToRotation(openings);

    //            GameObject currCorridor1 = Instantiate(corridors[ChooseLCorridor(yRotation)], spawnNowAt, Quaternion.identity, Data.instance.mapGenHolderTransform);
    //            currCorridor1.layer = 18;
    //            currCorridor1.GetComponentInChildren<CorridorNew>().rooms.Add(kParentPos);
    //            currCorridor1.GetComponentInChildren<CorridorNew>().rooms.Add(lParentPos);
    //            currCorridor1.transform.rotation = Quaternion.Euler(0, yRotation, 0);

    //            if (yRotation == 0)
    //            {
    //                //currCorridor1.GetComponentInChildren<BoxCollider>().enabled = false;
    //                currCorridor1.transform.localScale = new Vector3(-1, 1, 1);
    //                //currCorridor1.GetComponentInChildren<BoxCollider>().enabled = true;
    //                currCorridor1.transform.rotation = Quaternion.Euler(0, 90, 0);
    //            }

    //        }
    //        spawnNowAt.x += increment;

    //        //Spawn I corridors
    //        for (; i < Mathf.Abs(From.x - to.x) / Data.instance.corridorSize; i++)
    //        {
    //            ////Debug.Log("Loop 2 = " + i);
    //            //yield return new WaitForSeconds(0.25f);
    //            GameObject currentCorridor = Instantiate(corridorToSpawn, spawnNowAt/*new Vector3(spawnNowAt.x + 0.4f/*0.25f, spawnNowAt.y, spawnNowAt.z)*/, Quaternion.identity, Data.instance.mapGenHolderTransform);
    //            currentCorridor.layer = 18;
    //            /*
    //            //Move CollisionDetector of corridor I by -0.25f in x axis to keep it in grid
    //            Transform collisionDetectorTransform = currentCorridor.transform.GetChild(1);
    //            collisionDetectorTransform.position = new Vector3(collisionDetectorTransform.position.x - 0.25f, collisionDetectorTransform.position.y, collisionDetectorTransform.position.z);
    //            */
    //            currentCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(kParentPos);
    //            currentCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(lParentPos);
    //            currentCorridor.transform.rotation = Quaternion.Euler(0, 90, 0);
    //            //Data.instance.corridorCount++;

    //            currentCorridor.transform.GetChild(0).localPosition = new Vector3(0, 0, 0.226f);

    //            if (UnityEngine.Random.Range(0.0f, 1.0f) < ventCoverProbabilty)
    //            {
    //                Instantiate(ventCover, spawnNowAt, Quaternion.Euler(0, UnityEngine.Random.Range(0, 3) * 90, 0), currentCorridor.transform);
    //            }

    //            // ----------- Item Gen -----------
    //            if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.1f)
    //            {
    //                itemGenScript.SpawnItems(spawnNowAt - new Vector3(1, 0, 1), spawnNowAt + new Vector3(1, 0, 1), 1, currentCorridor.transform);
    //                Data.instance.ctr1++;
    //            }

    //            spawnNowAt.x += increment;

    //        }
    //    }
    //    isDoneSpawnHalf = true;
    //    yield return null;
    //}    

    // ---------------------- Checks if spawnPoints k and i belong to the same room or adjacent rooms ----------------------
    //private bool CheckIfSameOrAdjacentRoom(int k, int i)
    //{
    //    bool isDoorTypeX = spawnPoints[k].name.EndsWith("x") ? true : false;

    //    // ------------- Check x axis ------------- 
    //    if (Mathf.Abs(spawnPoints[k].transform.position.x - spawnPoints[i].transform.position.x) == Data.instance.xSize)
    //    {
    //        return true;
    //    }
    //    // ------------- Check z axis ------------- 
    //    else if (Mathf.Abs(spawnPoints[k].transform.position.z - spawnPoints[i].transform.position.z) == Data.instance.xSize)
    //    {
    //        return true;
    //    }

    //    // ------------- Check between x door and z door (in L shape) ------------- 
    //    if (Mathf.Abs(spawnPoints[k].transform.position.x - spawnPoints[i].transform.position.x) == Data.instance.xSize / 2
    //        && spawnPoints[k].transform.parent.position == spawnPoints[i].transform.parent.position)
    //    {
    //        return true;
    //    }

    //    return false;
    //}


    //private void CheckDuplicatesAndConnect(List<Vector3> rooms)
    //{
    //    for (int i = 0; i < Data.instance.connectedRooms.Count; i++)
    //    {
    //        for (int j = 0; j < Data.instance.connectedRooms[i].Count; j++)
    //        {
    //            if (rooms[0] == Data.instance.connectedRooms[i][j])
    //            {
    //                Data.instance.connectedRoomsThroughCollision.Add(new ConnectedComponent(rooms[0] / 2 + rooms[1] / 2, rooms));
    //            }
    //        }
    //    }
    //}

    //private void CompareAndAddAdjacent()
    //{
    //    Vector3 collidedConnectedRoomToSearch;
    //    for (int i = 0; i < Data.instance.connectedRoomsThroughCollision.Count; i++)
    //    {
    //        for (int j = 0; j < Data.instance.connectedRoomsThroughCollision[i].rooms.Count; j++)
    //        {
    //            collidedConnectedRoomToSearch = Data.instance.connectedRoomsThroughCollision[i].rooms[j];
    //            for (int k = 0; k < Data.instance.connectedRooms.Count; k++)
    //            {
    //                for (int q = 0; q < Data.instance.connectedRooms[k].Count; q++)
    //                {
    //                    // -------------- Now we are taking an element of connectedRoomsThroughCollision (collidedConnectedRoomToSearch)  -------------- 
    //                    // -------------- and comparing it with every element of connectedRooms -------------- 
    //                    // -------------- and adding the req ones to connectedRoomsThroughCollision --------------
    //                    // -------------- and removing the same ones from connectedRooms --------------
    //                    if (collidedConnectedRoomToSearch == Data.instance.connectedRooms[k][q])
    //                    {
    //                        Data.instance.connectedRoomsThroughCollision.Add(new ConnectedComponent(/*Data.instance.connectedRoomsThroughCollision[i].corridorPos*/ new Vector3(1, 1, 1), Data.instance.connectedRooms[k]));
    //                        Data.instance.connectedRooms.RemoveAt(k);
    //                        k--;
    //                        break;
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}

    //private void MakeInitHallways()
    //{
    //    ConnectTwoRooms(new Vector3(-16, 0, 0), new Vector3(4, 0, 0), "Door+x", "Door-x", new Vector3(-16 - 24, 0, 0), new Vector3(4 + 24, 0, 0), true);
    //    ConnectTwoRooms(new Vector3(-4, 0, 0), new Vector3(-4, 0, -8), "Door-z", "Door+z", new Vector3(-4, 0, -24), new Vector3(-4, 0, -8 + 24), true);
    //}
    #endregion

}
