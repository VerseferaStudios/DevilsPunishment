using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNew : MonoBehaviour, IComparer<GameObject>
{
    //private List<Transform> spawnPoints = new List<Transform>();
    private List<GameObject> spawnPoints = new List<GameObject>();
    public GameObject[] corridors;
    public GameObject[] vents;
    private Transform corridorsParent;
    //private MapGen3 mapGen3;
    private float nextTime = 0f;
    private bool breakLoop = false;
    private List<Vector3> visitedRooms = new List<Vector3>();
    private Vector3 spawnNowAt;
    private int k = 0, l = 0;

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

    private bool isDoneSpawnHalf = false, isDoneConnectTwoRooms = false;

    public bool fail_room_connect;

    Color sample_room_col;

    public List<List<bool>> occupiedCells;

    public void initSeed(int seed)
    {
        UnityEngine.Random.InitState(seed);
    }

    void Start()
    {
        //StartScript();
    }

    private void PopulateOccupiedCells()
    {
        float max = Mathf.Max(Data.instance.zSize, Data.instance.xSize);
        Debug.Log(max);
        Debug.Log(Data.instance.zSize);
        Debug.Log(Data.instance.xSize);
        Debug.Log(Data.instance.mapSizeX * Data.instance.zSize / 4);
        Debug.Log(Data.instance.mapSizeZ * Data.instance.zSize / 4);

        occupiedCells = new List<List<bool>>();
        List<bool> occupiedCellsRow = new List<bool>();
        for (int i = 0; i < Data.instance.mapSizeZ * max / 4; i++)
        {
            occupiedCellsRow.Add(true);
        }
        for (int i = 0; i < Data.instance.mapSizeX * max / 4; i++)
        {
            occupiedCells.Add(occupiedCellsRow);
        }
        for (int i = 10; i < 40; i++)
        {
            for (int j = 10; j < 20; j++)
            {
                occupiedCells[i][j] = false;
            }
        }

        Debug.Log(occupiedCells.Count);
        Debug.Log(occupiedCells[0].Count);
        for (int i = 0; i < occupiedCells.Count; i++)
        {
            for (int j = 0; j < occupiedCells[i].Count; j++)
            {
                Debug.Log("occupiedCells[" + i + "][" + j + "] = " + occupiedCells[i][j]);
            }
        }

        RoomReferences roomReferences;
        for (int i = 0; i < Data.instance.roomsFloor1.Count; i++)
        {
            roomReferences = Data.instance.roomsFloor1[i].GetComponent<RoomReferences>();
            //do topright and bottom left after seeing rotation and localPos etc and 4 units squares/ cells OcupIeDDCElls
        }


    }

    public void StartScript()
    {
        PopulateOccupiedCells();
        //mapGen3 = GameObject.FindGameObjectWithTag("Rooms(MapGen)").GetComponent<MapGen3>();

        // ------------------- Get array of doors / spawnPoints -------------------
        GameObject[] tempSpawnPoints = GameObject.FindGameObjectsWithTag("Corridor Spawn Points");
        //string f = tempSpawnPoints[0].GetComponentsInChildren<Transform>()[0].gameObject.name;
        //GameObjects to transform
        /*
        for (int i = 0; i < tempSpawnPoints.Length; i++)
        {
            tempSpawnPoints[i] = tempSpawnPoints[i].transform.position;
        }
        */

        // ------------------- Convert array of doors / spawnPoints into list -------------------
        spawnPoints.AddRange(tempSpawnPoints);
        //Debug.Log("spawnPoints.Count = " + spawnPoints.Count);

        // ------------------- Find exactly overlapping doors/spawnPoints, spawn a corridor at thst position, and destroy both doors/spawnPoints -------------------
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            bool isFound = false;
            int lastIdx = i;
            for (int j = 0; j < spawnPoints.Count; j++)
            {
                if (i == j)
                {
                    continue;
                }
                ////Debug.Log("i = " + i + " & j = " + j);
                if (spawnPoints[i].transform.position == spawnPoints[j].transform.position)
                {
                    isFound = true;
                    lastIdx = j;
                    break;
                }
            }
            if (isFound)
            {
                GameObject currentCorridor = Instantiate(corridors[0], spawnPoints[i].transform.position, Quaternion.identity, Data.instance.mapGenHolderTransform);
                currentCorridor.layer = 18;
                currentCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(spawnPoints[i].transform.parent.position);
                currentCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(spawnPoints[lastIdx].transform.parent.position);
                if (spawnPoints[i].name.EndsWith("x"))
                {
                    currentCorridor.transform.rotation = Quaternion.Euler(0, 90, 0);
                }
                //Debug.Log("Spawn1");
                //Data.instance.corridorCount++;

                // ------------------- Added parents position to List<Vector3> to avoid future doors of the room -------------------
                visitedRooms.Add(spawnPoints[i].transform.parent.position);
                visitedRooms.Add(spawnPoints[lastIdx].transform.parent.position);

                //CheckDuplicatesAndConnect(spawnPoints[i].transform.parent.transform.position, spawnPoints[lastIdx].transform.parent.transform.position);

                Data.instance.connectedRooms.Add(visitedRooms);

                ////Debug.Log(spawnPoints[i].transform.position + "______________________________________________________");
                spawnPoints.RemoveAt(i);

                // -------------- decrease lastIdx if greater than i --------------
                if (lastIdx > i)
                {
                    lastIdx--;
                }

                i--;

                spawnPoints.RemoveAt(lastIdx);

                // -------------- decrease i if greater than lastIdx --------------
                if (i > lastIdx)
                {
                    i--;
                }
                lastIdx--;

                visitedRooms = new List<Vector3>();

                isFound = false;
            }
        }


        //put all adjacnt rooms in same component
        /*
        for (int i = 0; i < Data.instance.connectedRooms.Count; i++)
        {
            if (Mathf.Abs(Data.instance.connectedRooms[i][0].x - Data.instance.connectedRooms[i][1].x) == Data.instance.xSize 
                || Mathf.Abs(Data.instance.connectedRooms[i][0].z - Data.instance.connectedRooms[i][1].z) == Data.instance.zSize)
            {
                    
            }

        }
        */

        //give data the first door according to which we r sorting
        if(spawnPoints.Count > 0)
        {
            Data.instance.spawnPointsFirstPos = spawnPoints[0].transform.position;
        }

        //sort according to the comparer (ie according to the distance from the first door)
        //spawnPoints.Sort(Compare);
        //instead of sorting you can put a var max in the inner loop (l) and find the least distance one and use that !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        // ------------------- Connect two doors of different rooms with suitable corridor shapes -------------------
        StartCoroutine(CallConnectRooms());

        //corridorsParent = (GameObject.Find("Corridors") as GameObject).transform;
        //Debug.Log(Data.instance.corridorCount + "corridor count!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
    }

    private IEnumerator CallConnectRooms()
    {
        int times = 0;
        bool x;
        //sample_room_col = spawnPoints[0].transform.parent.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial.color;
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

            for (l = 0; l < spawnPoints.Count; l++) //i = 0 makes no diff; some rooms are getting overlooked, y //EXPT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            {
                Debug.Log("K = " + k + " && L = " + l);

                //StartCoroutine(ShowRoomsBeingConnected(k, l, spawnPoints[k].transform.position, spawnPoints[l].transform.position));
                if (k == l)
                {
                    continue;
                }
                //// why x
                //x = data.instance.checkifvisited(spawnpoints[l].transform.parent.position);
                ////debug.log("counter = " + counter + " ; data.instance.checkifvisited(spawnpoints[l].transform.parent.position) = " + x);
                ////why
                //if (/*times == 0 && spawnpoints.count >= 9 && */x)
                //{
                //    ////debug.log("removed a door of ____ " + spawnpoints[i].transform.parent.transform.position);
                //    spawnpoints.removeat(l);
                //    k--;
                //    break;
                //}

                //x = !CheckIfSameOrAdjacentRoom(k, l);
                //Debug.Log("Counter = " + counter + " ; !CheckIfSameOrAdjacentRoom(k, l) = " + x); 
                // ------------------------ if k and i are not in the same room ------------------------
                //if (x)
                {
                    //StartCoroutine(ShowRoomsBeingConnected(k, l, spawnPoints[k].transform.position, spawnPoints[l].transform.position));
                    isDoneConnectTwoRooms = false;
                    fail_room_connect = false;

                    //spawnPoints[k].transform.parent.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial.color = Color.red;
                    //spawnPoints[l].transform.parent.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial.color = Color.red;

                    Debug.Log("spawn Points");
                    for (int i = 0; i < spawnPoints.Count; i++)
                    {
                        Debug.Log(spawnPoints[i].transform.position);
                        Debug.Log(spawnPoints[i].name);
                    }
                    Debug.Log("K = " + k + " && L = " + l);
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
                    StartCoroutine(ConnectTwoRooms(spawnPoints[k].transform.position, spawnPoints[l].transform.position,
                                    spawnPoints[k].name, spawnPoints[l].name,
                                    kParentPos, lParentPos, false));

                    //yield return new WaitUntil(() => Input.GetKey(KeyCode.L));
                    yield return new WaitForSeconds(2);

                    //spawnPoints[k].transform.parent.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial.color = sample_room_col;
                    //spawnPoints[l].transform.parent.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial.color = sample_room_col;
                    
                    yield return new WaitUntil(() => isDoneConnectTwoRooms);
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

                //MakeInitHallways();

                Debug.Log("---------------------aesrdtfgyuhij0------------------------------------");
                MapgenProgress.instance.addProgress(2);
                
		        StartCoroutine(Data.instance.DoConnectedComponents());
                StartCoroutine(Data.instance.DoCheckPerSecond());

                Data.instance.canStartCorridorTestSpawner = true;

            }
            //Debug.LogError(Data.instance.ctr1);

        }
        yield return null;
    }

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

    private int[] GetIdx(Vector3 pos)
    {
        int x = (int)pos.x / -4;
        int z = (int)pos.z / -4;

        return new int[]{ x, z};
    }

    private Vector3 GetPos(int[] idx)
    {
        return new Vector3(idx[0] * -4, 0, idx[1] * -4);
    }

    public IEnumerator ConnectTwoRooms(Vector3 kPos, Vector3 lPos, string kName, string lName, Vector3 kParentPos, Vector3 lParentPos, bool fromDataSingleton)
    {

        Vector3 currentSpawnPos = kPos;
        Vector3 newSpawnPos = kPos;
        Vector3 prevSpawnPos = kPos;
        int[] kIdx = GetIdx(kPos);
        int[] lIdx = GetIdx(lPos);
        int prevMove = -1;
        int thisMove = -1;
        List<int> moves = new List<int>();
        List<Vector3> positions = new List<Vector3>();
        List<bool> freeSpaces = new List<bool>();
        for (int i = 0; i < 4; i++)
        {
            freeSpaces.Add(false);
        }

        float initDistance = Mathf.Abs(lPos.x - currentSpawnPos.x) + Mathf.Abs(lPos.z - currentSpawnPos.z);
        float newDistance = initDistance;
        List<float> manhattanDist = new List<float>();

        Debug.Log(kPos);
        Debug.Log(lPos);

        bool reachedDestination = false;
        while (!reachedDestination)
        {
            /*
            Debug.Log("kIdx[0] = " + kIdx[0]);
            Debug.Log("kIdx[1] + 1 = " + (int)(kIdx[1] + 1));
            Debug.Log(kName);
            Debug.Log(kParentPos);
            Debug.Log(kPos);
            Debug.Log(occupiedCells.Count);
            */
            if (kIdx[1] + 1 < occupiedCells[kIdx[0]].Count && occupiedCells[kIdx[0]][kIdx[1] + 1])  //0 or North // mirror image so add z
            {
                freeSpaces[0] = true;
            }
            else
            {
                freeSpaces[0] = false;
            }
            if (kIdx[0] + 1 < occupiedCells.Count && occupiedCells[kIdx[0] + 1][kIdx[1]])           //1 or East // mirror image so add x
            {
                freeSpaces[1] = true;
            }
            else
            {
                freeSpaces[1] = false;
            }
            if (kIdx[1] - 1 < occupiedCells[kIdx[0]].Count && occupiedCells[kIdx[0]][kIdx[1] - 1])  //2 or South // mirror image so minus z
            {
                freeSpaces[2] = true;
            }
            else
            {
                freeSpaces[2] = false;
            }
            if (kIdx[0] - 1 < occupiedCells.Count && occupiedCells[kIdx[0] - 1][kIdx[1]])           //1 or West // mirror image so minus x
            {
                freeSpaces[3] = true;
            }
            else
            {
                freeSpaces[3] = false;
            }

            int ctr = 0;
            for (int i = 0; i < 4; i++)
            {
                //newDistance = initDistance;
                if (freeSpaces[i])
                {
                    ctr++;
                }
                DistanceHelper(i, out newSpawnPos, newSpawnPos);
                newDistance = Mathf.Abs(lPos.x - newSpawnPos.x) + Mathf.Abs(lPos.z - newSpawnPos.z);
                manhattanDist.Add(newDistance);
            }
            /*
            if(ctr > 1)
            {
                //choose prevMove if possible (after swapping 0 to 2 and 1 to 3 
            }
            else
            {
                thisMove = freeSpaces.IndexOf(true);
                prevMove = thisMove;
            }
            */
            initDistance = Mathf.Min(manhattanDist.ToArray());//check equal too in prev looopppp
            for (int i = 0; i < manhattanDist.Count; i++)
            {
                //Debug.Log(" => " + manhattanDist[i]);
            }
            //Debug.Log("min is " + initDistance);
            thisMove = manhattanDist.IndexOf(initDistance);
            Debug.Log("newSpawnPos = " + newSpawnPos + "& thisMove = " + thisMove);
            DistanceHelper(thisMove, out prevSpawnPos, prevSpawnPos);
            newSpawnPos = prevSpawnPos;

            prevMove = thisMove;

            if(thisMove == 0)
            {
                currentSpawnPos.z += 4;
            }
            else if (thisMove == 1)
            {
                currentSpawnPos.x += 4;
            }
            else if (thisMove == 2)
            {
                currentSpawnPos.z += -4;
            }
            else if (thisMove == 3)
            {
                currentSpawnPos.x += -4;
            }

            moves.Add(thisMove);
            positions.Add(currentSpawnPos);
            /*
            GameObject currentCorridor;

            if (initDistance == Mathf.Abs(kPos.x - lPos.x) || initDistance == Mathf.Abs(kPos.z - lPos.z))
            {
                //L Corridor!!!!!!!
            }
            else
            {
                currentCorridor = Instantiate(corridors[0], currentSpawnPos, Quaternion.identity);

                currentCorridor.layer = 18;
                /*
                //Move CollisionDetector of corridor I by -0.25f in x axis to keep it in grid
                Transform collisionDetectorTransform = currentCorridor.transform.GetChild(1);
                collisionDetectorTransform.position = new Vector3(collisionDetectorTransform.position.x - 0.25f, collisionDetectorTransform.position.y, collisionDetectorTransform.position.z);
                *//*
                currentCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(kParentPos);
                currentCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(lParentPos);
                if (thisMove == 0 || thisMove == 2)
                {
                    currentCorridor.transform.rotation = Quaternion.Euler(0, 0, 0);
                    //Data.instance.corridorCount++;

                    currentCorridor.transform.GetChild(0).localPosition = new Vector3(0, 0, -0.08f);
                }
                else
                {
                    currentCorridor.transform.rotation = Quaternion.Euler(0, 90, 0);
                    //Data.instance.corridorCount++;

                    currentCorridor.transform.GetChild(0).localPosition = new Vector3(0, 0, 0.226f);
                }

                //For now, later remove and put outside this else block

                if (UnityEngine.Random.Range(0.0f, 1.0f) < ventCoverProbabilty)
                {
                    Instantiate(ventCover, currentSpawnPos, Quaternion.Euler(0, UnityEngine.Random.Range(0, 3) * 90, 0), currentCorridor.transform);
                }

                // ----------- Item Gen -----------
                if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.1f)
                {
                    itemGenScript.SpawnItems(currentSpawnPos - new Vector3(1, 0, 1), currentSpawnPos + new Vector3(1, 0, 1), 1, currentCorridor.transform);
                    Data.instance.ctr1++;
                }
                
            }
            */

            //spawnNowAt.x += increment;




            freeSpaces.Clear();
            manhattanDist.Clear();
            for (int i = 0; i < 4; i++)
            {
                freeSpaces.Add(false);
            }

            reachedDestination = (currentSpawnPos.x == lPos.x) && (currentSpawnPos.z == lPos.z);

            yield return new WaitForSeconds(0.1f);
            if (reachedDestination)
            {
                break;
            }
            //Use recursion?
        }

        for (int i = 0; i < moves.Count; i++)
        {
            GameObject currentCorridor;

            //if (initDistance == Mathf.Abs(kPos.x - lPos.x) || initDistance == Mathf.Abs(kPos.z - lPos.z))
            if (i + 1 < moves.Count && (moves[i] == 0 || moves[i] == 2) && (moves[i + 1] == 1 || moves[i + 1] == 3))
            {
                //L Corridor!!!!!!!
            }
            else
            {
                currentCorridor = Instantiate(corridors[0], positions[i], Quaternion.identity);

                currentCorridor.layer = 18;
                /*
                //Move CollisionDetector of corridor I by -0.25f in x axis to keep it in grid
                Transform collisionDetectorTransform = currentCorridor.transform.GetChild(1);
                collisionDetectorTransform.position = new Vector3(collisionDetectorTransform.position.x - 0.25f, collisionDetectorTransform.position.y, collisionDetectorTransform.position.z);
                */
                currentCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(kParentPos);
                currentCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(lParentPos);
                if (moves[i] == 0 || moves[i] == 2)
                {
                    currentCorridor.transform.rotation = Quaternion.Euler(0, 0, 0);
                    //Data.instance.corridorCount++;

                    currentCorridor.transform.GetChild(0).localPosition = new Vector3(0, 0, -0.08f);
                }
                else
                {
                    currentCorridor.transform.rotation = Quaternion.Euler(0, 90, 0);
                    //Data.instance.corridorCount++;

                    currentCorridor.transform.GetChild(0).localPosition = new Vector3(0, 0, 0.226f);
                }

                //For now, later remove and put outside this else block

                if (UnityEngine.Random.Range(0.0f, 1.0f) < ventCoverProbabilty)
                {
                    Instantiate(ventCover, positions[i], Quaternion.Euler(0, UnityEngine.Random.Range(0, 3) * 90, 0), currentCorridor.transform);
                }

                // ----------- Item Gen -----------
                if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.1f)
                {
                    itemGenScript.SpawnItems(positions[i] - new Vector3(1, 0, 1), positions[i] + new Vector3(1, 0, 1), 1, currentCorridor.transform);
                    Data.instance.ctr1++;
                }
                
            }
        }

        // ------------------- Added parents position to List<Vector3> to avoid future doors of the room -------------------
        visitedRooms.Add(lParentPos);
        visitedRooms.Add(kParentPos);

        Data.instance.connectedRooms.Add(visitedRooms);

        visitedRooms = new List<Vector3>();

        // ---------------------- Removes the used doors ----------------------

        if (!fromDataSingleton)
        {
            spawnPoints.RemoveAt(l);

            // -------------- decrease k if greater than i --------------                
            if (k > l)
            {
                k--;
            }
            l--;

            spawnPoints.RemoveAt(k);

            // -------------- decrease k if greater than i --------------                
            if (l > k)
            {
                l--;
            }
            k--;
        }

        isDoneConnectTwoRooms = true;
        yield return null;
    }

    private void DistanceHelper(int i, out Vector3 newSpawnPos, Vector3 newSpawnPosOriginal)
    {
        newSpawnPos = newSpawnPosOriginal;
        if (i == 0)
        {
            Debug.Log("0");
            newSpawnPos.z += 4;
        }
        else if (i == 1)
        {
            Debug.Log("1");
            newSpawnPos.x += 4;
        }
        else if (i == 2)
        {
            Debug.Log("2");
            newSpawnPos.z += -4;
        }
        else if (i == 3)
        {
            Debug.Log("3");
            newSpawnPos.x += -4;
        }
    }

    public IEnumerator ConnectTwoRoomsOld(Vector3 kPos, Vector3 lPos, string kName, string lName, Vector3 kParentPos, Vector3 lParentPos, bool fromDataSingleton)
    {
        //yield return new WaitUntil(() => Input.GetKey(KeyCode.Tab));
        //making all y coordinates of all corridors equal to 0.5f
        //StartCoroutine(ShowRoomsBeingConnected(k, l, kPos, lPos));
        kPos.y = lPos.y = 0.5f;
        //Debug.Log("kPos and lPos = " + kPos + " " + lPos);

        Vector3 targetPos = new Vector3(0, 3, 0);

        Vector3 From = kPos;

        Debug.Log("CONNECT 2 Rooms = " + From + " from and to " + lPos + "Door names ; kName = " + kName + " ; lName = " + lName);

        // ------------------- Doesnt (xD) Connects (x and x) or (z and z) doors in different rooms with I shape with no hindrance -------------------
        /*
        if (
            // -------------- if x doors AND z differnce == xSize --------------
            (kPos.x == lPos.x && Mathf.Abs(kPos.z - lPos.z) == Data.instance.xSize)
            ||
            // -------------- if z doors AND x differnce == 10 --------------
            (kPos.z == lPos.z && Mathf.Abs(kPos.x - lPos.x) == Data.instance.xSize)
            )
        {
            //targetPos = spawnPoints[i].transform.position;
            //Debug.Log("Spawn4--");
        }
        */
                if (kParentPos == lParentPos)
        {
            Debug.Log("Error CONNECT 2 Rooms = " + lPos + " and " + kPos);
            fail_room_connect = true;
            isDoneConnectTwoRooms = true;
            yield return null;
        }
        // ------------------- Connects x and z doors with L shape with no hindrance -------------------
        else if (kName.EndsWith("x") && lName.EndsWith("z"))
        {
            targetPos = new Vector3(From.x, 0.5f, lPos.z);
            Debug.Log("Spawn2");
        }

        // ------------------- Connects z and x doors with L shape with no hindrance -------------------
        else if (kName.EndsWith("z") && lName.EndsWith("x"))
        {
            targetPos = new Vector3(lPos.x, 0.5f, From.z);
            Debug.Log("Spawn3");
        }

        // --------------------- Connects x and x doors ---------------------
        else if (kName.EndsWith("x") && lName.EndsWith("x"))
        {
            //-------------- Connects x and x doors with `L shape to avoid hindrance --------------
            if (kPos.x != lPos.x)
            {
                //check and go nearer to destination
                Vector3 to = kPos;
                to.z += Data.instance.xSize / 2;     //Check 5 or 6 !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                // ------------------- Calls the actual spawning function -------------------
                isDoneSpawnHalf = false;
                StartCoroutine(spawnHalf(kPos, to, true, kName, lName, kParentPos, lParentPos));
                yield return new WaitUntil(() => isDoneSpawnHalf);
                isExtraTurn = true;
                From = to;
                targetPos = new Vector3(lPos.x, 0.5f, From.z);
                Debug.Log("Spawn5a");
                /*
                //Debug.Log("From = " + From);
                //Debug.Log(targetPos);
                //Debug.Log(spawnPoints[i].transform.position);
                */
            }
            //-------------- Connects x and x doors with I shape since there's no hindrance --------------
            else
            {
                Debug.Log("Spawn5b");
                targetPos = lPos;
            }
        }

        // --------------------- Connects z and z doors ---------------------
        else if (kName.EndsWith("z") && lName.EndsWith("z"))
        {
            //-------------- Connects z and z doors with `L shape to avoid hindrance --------------
            if (kPos.z != lPos.z)
            {
                //check and go nearer to destination
                Vector3 to = kPos;
                to.x += Data.instance.xSize / 2;     //Check 5 or 6 !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                // ------------------- Calls the actual spawning function -------------------
                isDoneSpawnHalf = false;
                StartCoroutine(spawnHalf(kPos, to, true, kName, lName, kParentPos, lParentPos));
                yield return new WaitUntil(() => isDoneSpawnHalf);
                isExtraTurn = true;
                From = to;
                targetPos = new Vector3(From.x, 0.5f, lPos.z);
                Debug.Log("Spawn6a");
                /*
                //Debug.Log("From = " + From);
                //Debug.Log(targetPos);
                //Debug.Log(spawnPoints[i].transform.position);
                */
            }
            //-------------- Connects z and z doors with I shape since there's no hindrance --------------
            else
            {
                Debug.Log("Spawn6b");
                targetPos = lPos;
            }
        }

        if(targetPos == new Vector3(0, 3, 0))
        {
            fail_room_connect = true;
            isDoneConnectTwoRooms = true;
            yield return null;
        }

        // ------------------- Calls the actual spawning function -------------------
        isDoneSpawnHalf = false;
        StartCoroutine(spawnHalf(From, targetPos, !isExtraTurn, kName, lName, kParentPos, lParentPos));
        yield return new WaitUntil(() => isDoneSpawnHalf);

        isExtraTurn = false;

        if (targetPos != lPos)
        {
            isDoneSpawnHalf = false;
            StartCoroutine(spawnHalf(targetPos, lPos, false, kName, lName, kParentPos, lParentPos));
            yield return new WaitUntil(() => isDoneSpawnHalf);
        }

        //GameObject currCorridor1 = Instantiate(corridors[ChooseLCorridor(yRotation)], spawnNowAt, Quaternion.identity, Data.instance.mapGenHolderTransform);

        // --------------- Add L corridor to door at end room ---------------
        List<int> openings = new List<int>();

        if (targetPos.x == lPos.x)
        {
            //Add the previously stored storedOpening meant for this L corridor
            openings.Add(storedOpening);
        }
        else if (targetPos.z == lPos.z)
        {
            //Add the previously stored storedOpening meant for this L corridor
            openings.Add(storedOpening);
        }

        //Add opening according to the door type wuth the help of Data.instance.nearDoorL
        openings.Add(Data.instance.NeardoorLIndexSearch(lName[4].ToString() + lName[5].ToString()));
        /*
        //Debug.Log(Data.instance.ConvertToRotation(openings) + " " + (storedOpening == 0 ? 2 : 0) + " " + (lName[4].ToString() + lName[5].ToString())
                + " " + kPos + " " + lPos + " "
                + spawnPoints[k].name + " " + lName);
        */
        //Debug.Log(Data.instance.ConvertToRotation(openings) + " " + openings[0] + " " + openings[1]
        //+ " " + kPos + " " + lPos);

        float yRotation = Data.instance.ConvertToRotation(openings);
        GameObject currCorridor1 = Instantiate(corridors[ChooseLCorridor(yRotation)], lPos, Quaternion.identity, Data.instance.mapGenHolderTransform); 
        currCorridor1.layer = 18;
        currCorridor1.GetComponentInChildren<CorridorNew>().rooms.Add(kParentPos);
        currCorridor1.GetComponentInChildren<CorridorNew>().rooms.Add(lParentPos);
        currCorridor1.transform.rotation = Quaternion.Euler(0, yRotation, 0);
        if (yRotation == 0)
        {
            //currCorridor1.GetComponentInChildren<BoxCollider>().enabled = false;
            currCorridor1.transform.localScale = new Vector3(-1, 1, 1);
            //currCorridor1.GetComponentInChildren<BoxCollider>().enabled = true;
            currCorridor1.transform.rotation = Quaternion.Euler(0, 90, 0);
        }


        // ------------------- Added parents position to List<Vector3> to avoid future doors of the room -------------------
        visitedRooms.Add(lParentPos);
        visitedRooms.Add(kParentPos);

        //CheckDuplicatesAndConnect(lParentPos, spawnPoints[k].transform.parent.transform.position);

        Data.instance.connectedRooms.Add(visitedRooms);

        /*
        //Debug.Log("VISITED ROOMS!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        foreach (var item in visitedRooms)
        {
            //Debug.Log(item);
        }
        //Debug.Log("CONNECTED ROOMS!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        foreach (var item in Data.instance.connectedRooms)
        {
            foreach (var item1 in item)
            {
                //Debug.Log(item1);
            }
        }
        */

        visitedRooms = new List<Vector3>();

        ////Debug.Log("Added a door of ____ " + spawnPoints[i].transform.parent.transform.position);
        ////Debug.Log("Added a door of ____ " + spawnPoints[k].transform.parent.transform.position);

        // ---------------------- Removes the used doors ----------------------

        if (!fromDataSingleton)
        {
            spawnPoints.RemoveAt(l);

            // -------------- decrease k if greater than i --------------                
            if (k > l)
            {
                k--;
            }
            l--;

            spawnPoints.RemoveAt(k);

            // -------------- decrease k if greater than i --------------                
            if (l > k)
            {
                l--;
            }
            k--;
        }
        isDoneConnectTwoRooms = true;
        yield return null;
    }

    // ---------------------- Spawns I corridors from "Vector3 From", to "Vector3 to" except start and finish (where L corridor is needed)----------------------
    private IEnumerator spawnHalf(Vector3 From, Vector3 to, bool isFirst, string kName, string lName, Vector3 kParentPos, Vector3 lParentPos)
    {
        // ----------- Variable for position to spawn at each for loop step -----------                
        spawnNowAt = From;
        Debug.Log("SPAWN HALF = " + From + " from and to " + to);
        // ----------- Variable for corridor to spawn at each for loop step -----------                
        GameObject corridorToSpawn = corridors[0];

        // -------------- Spawns corridors along z axis since x coord is constant --------------                
        if (From.x == to.x)
        {
            int increment = (From.z > to.z) ? -4 : 4;

            // ----------- Skips required corridors ----------- 
            int i = 1;

            //Instantiates L corridor in correct rotation at the door of a room
            if (isFirst)
            {
                List<int> openings = new List<int>();

                //Add opening according to the door type wuth the help of Data.instance.nearDoorL
                openings.Add(Data.instance.NeardoorLIndexSearch(kName[4].ToString() + kName[5].ToString()));

                //storedOpening is for the next L corridor in line
                storedOpening = (From.z > to.z) ? 0 : 2;

                //Add the opposite of storedOpening since this one will be facing the next L corridor
                openings.Add(storedOpening == 0 ? 2 : 0);

                //Debug.Log(Data.instance.ConvertToRotation(openings) + " " + (storedOpening == 0 ? 2 : 0) + " " + (kName[4].ToString() + kName[5].ToString())
                    //+ " " + spawnPoints[k].transform.position + " " + spawnPoints[l].transform.position + " "
                    //+ kName + " " + lName);

                float yRotation = Data.instance.ConvertToRotation(openings);

                GameObject currCorridor1 = Instantiate(corridors[ChooseLCorridor(yRotation)], spawnNowAt, Quaternion.identity, Data.instance.mapGenHolderTransform);
                currCorridor1.layer = 18;
                currCorridor1.GetComponentInChildren<CorridorNew>().rooms.Add(kParentPos);
                currCorridor1.GetComponentInChildren<CorridorNew>().rooms.Add(lParentPos);
                currCorridor1.transform.rotation = Quaternion.Euler(0, yRotation, 0);
                if (yRotation == 0)
                {
                    //currCorridor1.GetComponentInChildren<BoxCollider>().enabled = false;
                    currCorridor1.transform.localScale = new Vector3(-1, 1, 1);
                    //currCorridor1.GetComponentInChildren<BoxCollider>().enabled = true;
                    currCorridor1.transform.rotation = Quaternion.Euler(0, 90, 0);
                }

                isFirst = false;
            }
            //Instantiate L corridor in correct rotation at the join of two straight corridors (which are in L shape)
            else
            {
                List<int> openings = new List<int>();

                //Debug.Log(Data.instance.ConvertToRotation(openings) + " " + storedOpening + " " + ((From.z > to.z) ? 2 : 0)
                    //+ " " + spawnPoints[k].transform.position + " " + spawnPoints[l].transform.position + " "
                    //+ kName + " " + lName);

                //Add the previously stored storedOpening meant for this L corridor
                openings.Add(storedOpening);

                //storedOpening is for the next L corridor in line
                storedOpening = (From.z > to.z) ? 0 : 2;

                //Add the opposite of storedOpening since this one will be facing the next L corridor
                openings.Add(storedOpening == 0 ? 2 : 0);

                float yRotation = Data.instance.ConvertToRotation(openings);

                GameObject currCorridor1 = Instantiate(corridors[ChooseLCorridor(yRotation)], spawnNowAt, Quaternion.identity, Data.instance.mapGenHolderTransform);
                currCorridor1.layer = 18;
                currCorridor1.GetComponentInChildren<CorridorNew>().rooms.Add(kParentPos);
                currCorridor1.GetComponentInChildren<CorridorNew>().rooms.Add(lParentPos);
                currCorridor1.transform.rotation = Quaternion.Euler(0, yRotation, 0);
                if (yRotation == 0)
                {
                    //currCorridor1.GetComponentInChildren<BoxCollider>().enabled = false;
                    currCorridor1.transform.localScale = new Vector3(-1, 1, 1);
                    //currCorridor1.GetComponentInChildren<BoxCollider>().enabled = true;
                    currCorridor1.transform.rotation = Quaternion.Euler(0, 90, 0);
                }
            }
            spawnNowAt.z += increment;

            //Spawn I corridors
            for (; i < Mathf.Abs(From.z - to.z) / Data.instance.corridorSize; i++)
            {
                ////Debug.Log("Loop 1 = " + i);
                //yield return new WaitForSeconds(0.25f);
                GameObject currentCorridor = Instantiate(corridorToSpawn, spawnNowAt/*new Vector3(spawnNowAt.x + 0.15f/*- 0.25f, spawnNowAt.y, spawnNowAt.z)*/, Quaternion.identity, Data.instance.mapGenHolderTransform);
                currentCorridor.layer = 18;
                /*
                //Move CollisionDetector of corridor I by +0.25f in x axis to keep it in grid
                Transform collisionDetectorTransform = currentCorridor.transform.GetChild(1);
                collisionDetectorTransform.position = new Vector3(collisionDetectorTransform.position.x + 0.25f, collisionDetectorTransform.position.y, collisionDetectorTransform.position.z);
                */
                currentCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(kParentPos);
                currentCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(lParentPos);
                /*
                Data.instance.corridorCount++;
                if (Data.instance.isCollided)
                {
                    Data.instance.isCollided = false;
                    //check current corridor and rotation. check the already instantiated once type AND rotation (using other) ?????//check later

                }
                */
                currentCorridor.transform.GetChild(0).localPosition = new Vector3(0, 0, -0.08f);

                if (UnityEngine.Random.Range(0.0f, 1.0f) < ventCoverProbabilty)
                {
                    Instantiate(ventCover, spawnNowAt, Quaternion.Euler(0, UnityEngine.Random.Range(0, 3) * 90, 0), currentCorridor.transform);
                }

                // ----------- Item Gen -----------
                if(UnityEngine.Random.Range(0.0f, 1.0f) < 0.1f)
                {
                    itemGenScript.SpawnItems(spawnNowAt - new Vector3(1, 0, 1), spawnNowAt + new Vector3(1, 0, 1), 1, currentCorridor.transform);
                    Data.instance.ctr1++;
                }

                spawnNowAt.z += increment;
            }
        }

        // -------------- Spawns corridors along x axis since z coord is constant --------------                
        else if (From.z == to.z)
        {
            int increment = (From.x > to.x) ? -4 : 4;

            // ----------- Skips required corridors ----------- 
            int i = 1;

            //Instantiates L corridor in correct rotation at the door of a room
            if (isFirst)
            {
                List<int> openings = new List<int>();

                //Add opening according to the door type wuth the help of Data.instance.nearDoorL
                openings.Add(Data.instance.NeardoorLIndexSearch(kName[4].ToString() + kName[5].ToString()));

                //storedOpening is for the next L corridor in line
                storedOpening = (From.x > to.x) ? 1 : 3;

                //Add the opposite of storedOpening since this one will be facing the next L corridor
                openings.Add(storedOpening == 1 ? 3 : 1);

                //Debug.Log(Data.instance.ConvertToRotation(openings) + " " + ((From.x > to.x) ? 3 : 1) + " " + (kName[4].ToString() + kName[5].ToString())
                    //+ " " + spawnPoints[k].transform.position + " " + spawnPoints[l].transform.position + " "
                    //+ kName + " " + lName);

                float yRotation = Data.instance.ConvertToRotation(openings);

                GameObject currCorridor1 = Instantiate(corridors[ChooseLCorridor(yRotation)], spawnNowAt, Quaternion.identity, Data.instance.mapGenHolderTransform);
                currCorridor1.layer = 18;
                currCorridor1.GetComponentInChildren<CorridorNew>().rooms.Add(kParentPos);
                currCorridor1.GetComponentInChildren<CorridorNew>().rooms.Add(lParentPos);
                currCorridor1.transform.rotation = Quaternion.Euler(0, yRotation, 0);

                if (yRotation == 0)
                {
                    //currCorridor1.GetComponentInChildren<BoxCollider>().enabled = false;
                    currCorridor1.transform.localScale = new Vector3(-1, 1, 1);
                    //currCorridor1.GetComponentInChildren<BoxCollider>().enabled = true;
                    currCorridor1.transform.rotation = Quaternion.Euler(0, 90, 0);
                }

                isFirst = false;
            }
            //Instantiate L corridor in correct rotation at the join of two straight corridors (which are in L shape)
            else
            {
                List<int> openings = new List<int>();

                //Debug.Log(Data.instance.ConvertToRotation(openings) + " " + storedOpening + " " + ((From.x > to.x) ? 3 : 1)
                    //+ " " + spawnPoints[k].transform.position + " " + spawnPoints[l].transform.position + " "
                    //+ kName + " " + lName);

                //Add the previously stored storedOpening meant for this L corridor
                openings.Add(storedOpening);

                //storedOpening is for the next L corridor in line
                storedOpening = (From.x > to.x) ? 1 : 3;

                //Add the opposite of storedOpening since this one will be facing the next L corridor
                openings.Add(storedOpening == 1 ? 3 : 1);

                float yRotation = Data.instance.ConvertToRotation(openings);

                GameObject currCorridor1 = Instantiate(corridors[ChooseLCorridor(yRotation)], spawnNowAt, Quaternion.identity, Data.instance.mapGenHolderTransform);
                currCorridor1.layer = 18;
                currCorridor1.GetComponentInChildren<CorridorNew>().rooms.Add(kParentPos);
                currCorridor1.GetComponentInChildren<CorridorNew>().rooms.Add(lParentPos);
                currCorridor1.transform.rotation = Quaternion.Euler(0, yRotation, 0);

                if (yRotation == 0)
                {
                    //currCorridor1.GetComponentInChildren<BoxCollider>().enabled = false;
                    currCorridor1.transform.localScale = new Vector3(-1, 1, 1);
                    //currCorridor1.GetComponentInChildren<BoxCollider>().enabled = true;
                    currCorridor1.transform.rotation = Quaternion.Euler(0, 90, 0);
                }

            }
            spawnNowAt.x += increment;

            //Spawn I corridors
            for (; i < Mathf.Abs(From.x - to.x) / Data.instance.corridorSize; i++)
            {
                ////Debug.Log("Loop 2 = " + i);
                //yield return new WaitForSeconds(0.25f);
                GameObject currentCorridor = Instantiate(corridorToSpawn, spawnNowAt/*new Vector3(spawnNowAt.x + 0.4f/*0.25f, spawnNowAt.y, spawnNowAt.z)*/, Quaternion.identity, Data.instance.mapGenHolderTransform);
                currentCorridor.layer = 18;
                /*
                //Move CollisionDetector of corridor I by -0.25f in x axis to keep it in grid
                Transform collisionDetectorTransform = currentCorridor.transform.GetChild(1);
                collisionDetectorTransform.position = new Vector3(collisionDetectorTransform.position.x - 0.25f, collisionDetectorTransform.position.y, collisionDetectorTransform.position.z);
                */
                currentCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(kParentPos);
                currentCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(lParentPos);
                currentCorridor.transform.rotation = Quaternion.Euler(0, 90, 0);
                //Data.instance.corridorCount++;

                currentCorridor.transform.GetChild(0).localPosition = new Vector3(0, 0, 0.226f);

                if (UnityEngine.Random.Range(0.0f, 1.0f) < ventCoverProbabilty)
                {
                    Instantiate(ventCover, spawnNowAt, Quaternion.Euler(0, UnityEngine.Random.Range(0, 3) * 90, 0), currentCorridor.transform);
                }

                // ----------- Item Gen -----------
                if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.1f)
                {
                    itemGenScript.SpawnItems(spawnNowAt - new Vector3(1, 0, 1), spawnNowAt + new Vector3(1, 0, 1), 1, currentCorridor.transform);
                    Data.instance.ctr1++;
                }

                spawnNowAt.x += increment;

            }
        }
        isDoneSpawnHalf = true;
        yield return null;
    }

    private int ChooseLCorridor(float yRotation)
    {
        return (yRotation == 0 || yRotation == 180) ? 2 : ((yRotation == 90) ? 7 : 1);
    }

    // ---------------------- Checks if spawnPoints k and i belong to the same room or adjacent rooms ----------------------
    private bool CheckIfSameOrAdjacentRoom(int k, int i)
    {
        bool isDoorTypeX = spawnPoints[k].name.EndsWith("x") ? true : false;

        // ------------- Check x axis ------------- 
        if (Mathf.Abs(spawnPoints[k].transform.position.x - spawnPoints[i].transform.position.x) == Data.instance.xSize)
        {
            return true;
        }
        // ------------- Check z axis ------------- 
        else if (Mathf.Abs(spawnPoints[k].transform.position.z - spawnPoints[i].transform.position.z) == Data.instance.xSize)
        {
            return true;
        }

        // ------------- Check between x door and z door (in L shape) ------------- 
        if (Mathf.Abs(spawnPoints[k].transform.position.x - spawnPoints[i].transform.position.x) == Data.instance.xSize / 2 
            && spawnPoints[k].transform.parent.position == spawnPoints[i].transform.parent.position)
        {
            return true;
        }

        return false;
    }

    public int Compare(GameObject x, GameObject y)
    {
        return (int)(Vector3.Distance(x.transform.position, Data.instance.spawnPointsFirstPos)
                    - Vector3.Distance(y.transform.position, Data.instance.spawnPointsFirstPos));
    }


    private void CheckDuplicatesAndConnect(List<Vector3> rooms)
    {
        for (int i = 0; i < Data.instance.connectedRooms.Count; i++)
        {
            for (int j = 0; j < Data.instance.connectedRooms[i].Count; j++)
            {
                if(rooms[0] == Data.instance.connectedRooms[i][j])
                {
                    Data.instance.connectedRoomsThroughCollision.Add(new ConnectedComponent(rooms[0] / 2 + rooms[1] / 2, rooms));
                }
            }
        }
    }

    private void CompareAndAddAdjacent()
    {
        Vector3 collidedConnectedRoomToSearch;
        for (int i = 0; i < Data.instance.connectedRoomsThroughCollision.Count; i++)
        {
            for (int j = 0; j < Data.instance.connectedRoomsThroughCollision[i].rooms.Count; j++)
            {
                collidedConnectedRoomToSearch = Data.instance.connectedRoomsThroughCollision[i].rooms[j];
                for (int k = 0; k < Data.instance.connectedRooms.Count; k++)
                {
                    for (int q = 0; q < Data.instance.connectedRooms[k].Count; q++)
                    {
                        // -------------- Now we are taking an element of connectedRoomsThroughCollision (collidedConnectedRoomToSearch)  -------------- 
                        // -------------- and comparing it with every element of connectedRooms -------------- 
                        // -------------- and adding the req ones to connectedRoomsThroughCollision --------------
                        // -------------- and removing the same ones from connectedRooms --------------
                        if (collidedConnectedRoomToSearch == Data.instance.connectedRooms[k][q])
                        {
                            Data.instance.connectedRoomsThroughCollision.Add(new ConnectedComponent(/*Data.instance.connectedRoomsThroughCollision[i].corridorPos*/ new Vector3(1, 1, 1), Data.instance.connectedRooms[k]));
                            Data.instance.connectedRooms.RemoveAt(k);
                            k--;
                            break;
                        }
                    }
                }
            }
        }
    }

    private void MakeInitHallways()
    {
        ConnectTwoRooms(new Vector3(-16, 0, 0), new Vector3(4, 0, 0), "Door+x", "Door-x", new Vector3(-16 - 24, 0, 0), new Vector3(4 + 24, 0, 0), true);
        ConnectTwoRooms(new Vector3(-4, 0, 0), new Vector3(-4, 0, -8), "Door-z", "Door+z", new Vector3(-4, 0, -24), new Vector3(-4, 0, -8 + 24), true);
    }

}
    /*
    private string checkCollisions(Vector3 From, Vector3 to)
    {
        int ctr = 0;
        bool goToNext = false;
        Vector3 FromTemp = From;
        Vector3 targetPos = new Vector3(FromTemp.x, 0.5f, to.z);
        //From to targetPos, x constant
        for (int i = 0; i < Data.instance.allRooms.Count; i++)
        {
            ////Debug.Log(((int[])Data.instance.allRooms[i])[1]);
            if(Mathf.Abs(-((int[])Data.instance.allRooms[i])[1] - FromTemp.x) != Data.instance.xSize/2 + 1 && Mathf.Abs(-((int[])Data.instance.allRooms[i])[1] - FromTemp.x) < Data.instance.xSize)
            {
                ctr++;
                goToNext = true;
                break;
            }
        }
        if (goToNext)
        {
            //targetPos to to, z constant
            FromTemp = targetPos;
            targetPos = to;
            int i = 0;
            for (i = 0; i < Data.instance.allRooms.Count; i++)
            {
                if (Mathf.Abs(-((int[])Data.instance.allRooms[i])[0] - FromTemp.z) != Data.instance.zSize/2 + 1 && Mathf.Abs(-((int[])Data.instance.allRooms[i])[0] - FromTemp.z) < Data.instance.zSize)
                {
                    goToNext = true;
                    break;
                }
            }
            if (i == Data.instance.allRooms.Count)
                return "xz";
        }
        if (goToNext)
        {
            targetPos = new Vector3(to.x, 0.5f, FromTemp.z);
            FromTemp = From;
            //z const
            for (int i = 0; i < Data.instance.allRooms.Count; i++)
            {
                if (Mathf.Abs(-((int[])Data.instance.allRooms[i])[0] - FromTemp.z) != Data.instance.zSize/2 + 1 && Mathf.Abs(-((int[])Data.instance.allRooms[i])[0] - FromTemp.z) < Data.instance.zSize)
                {
                    goToNext = true;
                    break;
                }
            }
        }
        if (goToNext)
        {
            //x const
            FromTemp = targetPos;
            targetPos = to;
            int i = 0;
            for (i = 0; i < Data.instance.allRooms.Count; i++)
            {
                if (Mathf.Abs(-((int[])Data.instance.allRooms[i])[1] - FromTemp.x) != Data.instance.xSize/2 + 1 && Mathf.Abs(-((int[])Data.instance.allRooms[i])[1] - FromTemp.x) < Data.instance.xSize)
                {
                    goToNext = true;
                    break;
                }
            }
            if (i == Data.instance.allRooms.Count)
                return "zx";
        }
        if (goToNext)
            return "No";
        else
            return "No";
    }
    */
    // Update is called once per frame



/*
 * //Colour scheme
            if (k == 2)
            {
                spawnPoints[k].GetComponent<Renderer>().material.color = Color.green;
                spawnPoints[k + 1].GetComponent<Renderer>().material.color = Color.green;
            }
            else if (k == 4)
            {
                spawnPoints[k].GetComponent<Renderer>().material.color = Color.red;
                spawnPoints[k + 1].GetComponent<Renderer>().material.color = Color.red;
            }
            else if (k == 6)
            {
                spawnPoints[k].GetComponent<Renderer>().material.color = Color.white;
                spawnPoints[k + 1].GetComponent<Renderer>().material.color = Color.white;
            }
            else if (k == 8)
            {
                spawnPoints[k].GetComponent<Renderer>().material.color = Color.yellow;
                spawnPoints[k + 1].GetComponent<Renderer>().material.color = Color.yellow;
            }
            else if (k == 10)
            {
                spawnPoints[k].GetComponent<Renderer>().material.color = Color.cyan;
                spawnPoints[k + 1].GetComponent<Renderer>().material.color = Color.cyan;
            }
            */
