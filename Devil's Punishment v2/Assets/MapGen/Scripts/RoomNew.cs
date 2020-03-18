using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    public List<List<Cell>> mapCells;
    //public List<List<bool>> occupiedCells;
    //public List<List<bool>> visitedCells;
    //public List<List<bool>> occupiedCells;

    public GameObject helper;
    public Transform helperTransform;

    private Vector3 chkPos = new Vector3(-160, 0, -28);
    private Vector3 chkPos1 = new Vector3(-156, 0, -28);
    private Vector3 chkPos2 = new Vector3(-152, 0, -28);

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

        mapCells = new List<List<Cell>>();
        for (int i = 0; i < Data.instance.mapSizeX * max / 4; i++)
        {
            List<Cell> occupiedCellsRow = new List<Cell>();
            for (int j = 0; j < Data.instance.mapSizeZ * max / 4; j++)
            {
                occupiedCellsRow.Add(new Cell());
            }
            mapCells.Add(occupiedCellsRow);
        }

        //ChkPoses();
        /*
        for (int i = 10; i < 40; i++)
        {
            for (int j = 10; j < 20; j++)
            {
                occupiedCells[i][j] = false;
            }
        }
        */

        //Debug.Log(occupiedCells.Count);
        //Debug.Log(occupiedCells[0].Count);
        /*
        for (int i = 0; i < occupiedCells.Count; i++)
        {
            for (int j = 0; j < occupiedCells[i].Count; j++)
            {
                //Debug.Log("occupiedCells[" + i + "][" + j + "] = " + occupiedCells[i][j]);
            }
        }
        */
        RoomReferences roomReferences;
        for (int i = 0; i < Data.instance.roomsFloor1.Count; i++)
        {
            roomReferences = Data.instance.roomsFloor1[i].GetComponent<RoomReferences>();
            
            //do topright and bottom left after seeing rotation and localPos etc and 4 units squares/ cells OcupIeDDCElls
            int[] idxTopRight = GetIdx(roomReferences.topRightCorner.position);
            //idxTopRight[0]++;
            //idxTopRight[1]++;
            int[] idxBottomLeft = GetIdx(roomReferences.bottomLeftCorner.position);
            //idxBottomLeft[0]--;
            //idxBottomLeft[1]--;

            int minX = Mathf.Min(idxTopRight[0], idxBottomLeft[0]);
            int maxX = Mathf.Max(idxTopRight[0], idxBottomLeft[0]);

            int minZ = Mathf.Min(idxTopRight[1], idxBottomLeft[1]);
            int maxZ = Mathf.Max(idxTopRight[1], idxBottomLeft[1]);


            Debug.Log("From minX " + minX + " to maxX " + maxX);
            Debug.Log("From minZ " + minZ + " to maxZ " + maxZ);

            for (int j = minX; j <= maxX; j++)
            {
                for (int k = minZ; k <= maxZ; k++)
                {
                    //Debug.Log("j = " + j + " & k = " + k);
                    int[] pos = new int[] { j, k };
                    Vector3 posV3 = GetPos(pos);
                    //Debug.Log(posV3);
                    //Instantiate(helper, posV3, Quaternion.identity, helperTransform).GetComponent<MeshRenderer>().sharedMaterial.color = Color.green;
                    mapCells[j][k].occupied = false;
                    mapCells[j][k].occupied = false;
                }
            }

            //Debug.Log(roomReferences.topRightCorner.position);
            //Debug.Log(roomReferences.bottomLeftCorner.position);
        }
        /*
        for (int i = 0; i < occupiedCells.Count; i++)
        {
            for (int j = 0; j < occupiedCells[i].Count; j++)
            {
                if(occupiedCells[i][j] == false)
                {
                    Debug.Log("i = " + i + " & j = " + j);
                    int[] pos = new int[] { i, j };
                    Vector3 posV3 = GetPos(pos);
                    posV3.y += 10;
                    Debug.Log(posV3);
                    Instantiate(helper, posV3, Quaternion.identity, helperTransform).GetComponent<MeshRenderer>().sharedMaterial.color = Color.blue;
                }
                else
                {
                    //Debug.Log("i = " + i + " & j = " + j);
                    int[] pos = new int[] { i, j };
                    Vector3 posV3 = GetPos(pos);
                    posV3.y += 15;
                    //Debug.Log(posV3);
                    Instantiate(helper, posV3, Quaternion.identity, helperTransform);//.GetComponent<MeshRenderer>().sharedMaterial.color = Color.white;
                }
            }
        }
        */
        //ChkPoses();

    }

    private void PopulateVisitedCells()
    {
        for (int i = 0; i < mapCells.Count; i++)
        {
            for (int j = 0; j < mapCells[i].Count; j++)
            {
                mapCells[i][j].visited = false;
            }
        }
    }

    private void ChkPoses()
    {
        int[] x = GetIdx(chkPos);
        int[] x1 = GetIdx(chkPos1);
        int[] x2 = GetIdx(chkPos2);
        Debug.Log("chkPos " + mapCells[x[0]][x[1]].occupied);
        Debug.Log("chkPos1 " + mapCells[x1[0]][x1[1]].occupied);
        Debug.Log("chkPos2 " + mapCells[x2[0]][x2[1]].occupied);
    }

    public void StartScript()
    {
        PopulateOccupiedCells();
        PopulateVisitedCells();
        //return;
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
                    /*
                    Debug.Log("spawn Points");
                    for (int i = 0; i < spawnPoints.Count; i++)
                    {
                        Debug.Log(spawnPoints[i].transform.position);
                        Debug.Log(spawnPoints[i].name);
                    }
                    Debug.Log("K = " + k + " && L = " + l);
                    */
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
                    //yield return new WaitForSeconds(2);

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
                /*
                Debug.Log("---------------------aesrdtfgyuhij0------------------------------------");
                MapgenProgress.instance.addProgress(2);
                */
		        StartCoroutine(Data.instance.DoConnectedComponents());
                Debug.Log("Time.time = " + Time.time);
                yield return new WaitForSeconds(5);
                yield return new WaitUntil(() => Data.instance.isConnectedComponentsCheckDone);
                /*
                StartCoroutine(Data.instance.DoCheckPerSecond());

                Data.instance.canStartCorridorTestSpawner = true;
                */
                InstantiateAllCorridors();
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
        int x = Mathf.RoundToInt(pos.x / -4);
        int z = Mathf.RoundToInt(pos.z / -4);

        return new int[]{ x, z};
    }

    private Vector3 GetPos(int[] idx)
    {
        return new Vector3(idx[0] * -4, 0, idx[1] * -4);
    }

    public IEnumerator ConnectTwoRooms(Vector3 kPos, Vector3 lPos, string kName, string lName, Vector3 kParentPos, Vector3 lParentPos, bool fromDataSingleton)
    {

        PopulateVisitedCells();

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

        //Debug.Log(kPos);
        //Debug.Log(lPos);
        int count = 0;
        bool reachedDestination = false;
        Debug.Log("B4 while");
        //ChkPoses();

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
            Debug.Log("During while");
            //ChkPoses();

            kIdx = GetIdx(currentSpawnPos);

            if (mapCells[kIdx[0]] != null && mapCells[kIdx[0]][kIdx[1] - 1] != null && 
                (!mapCells[kIdx[0]][kIdx[1] - 1].visited && kIdx[1] - 1 < mapCells[kIdx[0]].Count && mapCells[kIdx[0]][kIdx[1] - 1].occupied))  //0 or North // mirror image so add z
            {
                freeSpaces[0] = true;
            }
            else
            {
                //Debug.Log("North occupied or visited");
                freeSpaces[0] = false;
            }
            Vector3 gotPos = GetPos(new int[] { kIdx[0], kIdx[1] - 1 });
            //Debug.Log("pos = " + gotPos);
            //Debug.Log("kIdx[0] & kIdx[1] - 1 = " + kIdx[0] + " " + (int)(kIdx[1] - 1));
            //Debug.Log(GetIdx(gotPos)[0] + " " + GetIdx(gotPos)[1]);

            if (mapCells[kIdx[0] - 1] != null && mapCells[kIdx[0] - 1][kIdx[1]] != null &&
                (!mapCells[kIdx[0] - 1][kIdx[1]].visited && kIdx[0] - 1 < mapCells.Count && mapCells[kIdx[0] - 1][kIdx[1]].occupied))           //1 or East // mirror image so add x
            {
                freeSpaces[1] = true;
            }
            else
            {
                //Debug.Log("East occupied or visited");
                freeSpaces[1] = false;
            }
            //Debug.Log("pos = " + GetPos(new int[] { kIdx[0] - 1, kIdx[1] }));
            //Debug.Log("kIdx[0] - 1 & kIdx[1]= " + (int)(kIdx[0] - 1) + " " + kIdx[1]);

            if (mapCells[kIdx[0]] != null && mapCells[kIdx[0]][kIdx[1] + 1] != null &&
                (!mapCells[kIdx[0]][kIdx[1] + 1].visited && kIdx[1] + 1 < mapCells[kIdx[0]].Count && mapCells[kIdx[0]][kIdx[1] + 1].occupied))  //2 or South // mirror image so minus z
            {
                freeSpaces[2] = true;
            }
            else
            {
                //Debug.Log("South occupied or visited");
                freeSpaces[2] = false;
            }
            //Debug.Log("pos = " + GetPos(new int[] { kIdx[0], kIdx[1] + 1 }));
            //Debug.Log("kIdx[0] & kIdx[1] + 1 = " + kIdx[0] + " " + (int)(kIdx[1] + 1));

            if (mapCells[kIdx[0] + 1] != null & mapCells[kIdx[0] + 1][kIdx[1]] != null &&
                (!mapCells[kIdx[0] + 1][kIdx[1]].visited && kIdx[0] + 1 < mapCells.Count && mapCells[kIdx[0] + 1][kIdx[1]].occupied))           //1 or West // mirror image so minus x
            {
                freeSpaces[3] = true;
            }
            else
            {
                //Debug.Log("West occupied or visited");
                freeSpaces[3] = false;
            }
                //Debug.Log("pos = " + GetPos(new int[] { kIdx[0] + 1, kIdx[1]}));
                //Debug.Log("kIdx[0] + 1 & kIdx[1] = " + (int)(kIdx[0] + 1 )+ " " + kIdx[1]);

            int ctr = 0;
            for (int i = 0; i < 4; i++)
            {
                //newDistance = initDistance;
                if (freeSpaces[i])
                {
                    ctr++;
                    newSpawnPos = prevSpawnPos;
                    DistanceHelper(i, out newSpawnPos, newSpawnPos);
                    newDistance = Mathf.Abs(lPos.x - newSpawnPos.x) + Mathf.Abs(lPos.z - newSpawnPos.z);
                    manhattanDist.Add(newDistance);
                    //Debug.Log("newDistance = " + newDistance);
                }
                else
                {
                    //Debug.Log("newDistance NNN = " + 500);
                    manhattanDist.Add(500);
                }
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
            /*
            for (int i = 0; i < manhattanDist.Count; i++)
            {
                Debug.Log(" => " + manhattanDist[i]);
            }
            Debug.Log("min is " + initDistance);
            Debug.Log("newSpawnPos = " + newSpawnPos + "& thisMove = " + thisMove);
            */
            thisMove = manhattanDist.IndexOf(initDistance);
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



            freeSpaces.Clear();
            manhattanDist.Clear();
            for (int i = 0; i < 4; i++)
            {
                freeSpaces.Add(false);
            }

            reachedDestination = (currentSpawnPos.x == lPos.x) && (currentSpawnPos.z == lPos.z);

            mapCells[kIdx[0]][kIdx[1]].visited = true;

            //yield return new WaitForSeconds(0.1f);
            if (reachedDestination)
            {
                break;
            }
            count++;
            Debug.Log("count = " + count);
            if (count > 100) break;
            //Use recursion?
        }
        Debug.Log("After while");
        //ChkPoses();

        // --------------- Add L (or I) corridor to door at begining room ---------------
        List<int> openings = new List<int>();

        //Add opening according to the door type wuth the help of Data.instance.nearDoorL
        openings.Add(Data.instance.NeardoorLIndexSearch(kName[4].ToString() + kName[5].ToString()));
        int nextCorridorMove = moves[0];
        nextCorridorMove += 2;
        nextCorridorMove %= 4;
        openings.Add(moves[0]);
        if(nextCorridorMove == openings[0])
        {
            Debug.Log("I Corridor");
            AddICorridorSpawnInfo(kPos, moves[0], false);
        }
        else
        {
            Debug.Log("L Corridor");
            AddLCorridorSpawnInfo(openings, kPos);
        }

        //In between I Corridors
        for (int i = 0; i < moves.Count - 1; i++)
        {
            //GameObject currentCorridor;

            //Debug.Log("moves[i] = " + moves[i]);
            //if (i + 1 < moves.Count) Debug.Log("moves[i + 1] = " + moves[i + 1]);
            //Debug.Log("positions[i] = " + positions[i]);

            //if (initDistance == Mathf.Abs(kPos.x - lPos.x) || initDistance == Mathf.Abs(kPos.z - lPos.z))
            if (i + 1 < moves.Count && 
                ((moves[i] == 0 || moves[i] == 2) && (moves[i + 1] == 1 || moves[i + 1] == 3) ||
                 (moves[i] == 1 || moves[i] == 3) && (moves[i + 1] == 0 || moves[i + 1] == 2)))
            {
                //L Corridor!!!!!!!
                //Debug.Log("L Corridor in above pos");
                List<int> openings1 = new List<int>();
                int nextCorridorMove1 = moves[i];
                nextCorridorMove1 += 2;
                nextCorridorMove1 %= 4;
                openings1.Add(nextCorridorMove1);
                openings1.Add(moves[i + 1]);
                AddLCorridorSpawnInfo(openings1, positions[i]);
                //InstantiateLCorridor(openings1, positions[i], kParentPos, lParentPos);
            }
            else
            {
                AddICorridorSpawnInfo(positions[i], moves[i], false);
                //InstantiateICorridor(positions[i], kParentPos, lParentPos, moves[i], false);
            }
        }



        // --------------- Add L (or I) corridor to door at destination room ---------------
        openings = new List<int>();

        int doorMove = moves[moves.Count - 2];
        doorMove += 2;
        doorMove %= 4;
        openings.Add(doorMove);

        //Add opening according to the door type wuth the help of Data.instance.nearDoorL
        openings.Add(Data.instance.NeardoorLIndexSearch(lName[4].ToString() + lName[5].ToString()));
        //Debug.Log("lName = " + lName + "&& openings = " + openings[0] + " " + openings[1]);
        //Debug.Log("doorMove = " + doorMove);
        if(moves[moves.Count - 2] == openings[1])
        {
            Debug.Log("I Corridor");
            AddICorridorSpawnInfo(lPos, doorMove, false);
        }
        else
        {
            Debug.Log("L Corridor");
            AddLCorridorSpawnInfo(openings, lPos);
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

    private void InstantiateAllCorridors()//Vector3 kParentPos, Vector3 lParentPos)
    {
        Vector3 kParentPos = Vector3.zero;//REMOVE LATER!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        Vector3 lParentPos = Vector3.zero;
        for (int i = 0; i < mapCells.Count; i++)
        {
            for (int j = 0; j < mapCells[i].Count; j++)
            {
                if (mapCells[i][j].corridorIdx != -1)
                {
                    string corridorName = CorridorsListIdxToCorridorName(mapCells[i][j].corridorIdx);
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
                }
            }
        }
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

    private void AddICorridorSpawnInfo(Vector3 posI, int movesI, bool isOverride)
    {
        Debug.Log("I info");
        int[] kIdx = GetIdx(posI);
        int yRotation = (movesI == 0 || movesI == 2) ? 0 : 90;
        /*
        if(Vector3.Distance(posI, new Vector3(-124, 0, -20)) < 6)
        {
            Debug.Log("I Corr collision check => ");
            Debug.Log("mapCells[kIdx[0]][kIdx[1]].corridorIdx = " + mapCells[kIdx[0]][kIdx[1]].corridorIdx);
            Debug.Log("mapCells[kIdx[0]][kIdx[1]].corridorYRot = " + mapCells[kIdx[0]][kIdx[1]].corridorYRot);
        }
        */

        if (mapCells[kIdx[0]][kIdx[1]].corridorIdx == -1
           && mapCells[kIdx[0]][kIdx[1]].corridorYRot == -1)
        {
            mapCells[kIdx[0]][kIdx[1]].corridorIdx = 0;
            mapCells[kIdx[0]][kIdx[1]].corridorYRot = yRotation;
        }
        else if (!(mapCells[kIdx[0]][kIdx[1]].corridorIdx == 0
                && mapCells[kIdx[0]][kIdx[1]].corridorYRot == (int)yRotation))
        /*(mapCells[kIdx[0]][kIdx[1]].corridorIdx == 0 
        && (mapCells[kIdx[0]][kIdx[1]].corridorYRot == yRotation + 90
        || mapCells[kIdx[0]][kIdx[1]].corridorYRot == yRotation - 90)
        && !isOverride)*/
        {
            AddCollisionInfoHelper(kIdx, yRotation, "CorridorI");
        }
    }

    private void AddLCorridorSpawnInfo(List<int> openings, Vector3 posToSpawn)
    {
        Debug.Log("L info");
        float yRotation = Data.instance.ConvertToRotation(openings);
        int corridorIdx = ChooseLCorridor(yRotation);
        int[] kIdx = GetIdx(posToSpawn);

        if (mapCells[kIdx[0]][kIdx[1]].corridorIdx == -1
           && mapCells[kIdx[0]][kIdx[1]].corridorYRot == -1)
        {
            mapCells[kIdx[0]][kIdx[1]].corridorIdx = corridorIdx;
            mapCells[kIdx[0]][kIdx[1]].corridorYRot = (int)yRotation;
        }
        else if (!(mapCells[kIdx[0]][kIdx[1]].corridorIdx == corridorIdx
            && mapCells[kIdx[0]][kIdx[1]].corridorYRot == (int)yRotation))
            //&& !isOverride)
        {
            AddCollisionInfoHelper(kIdx, (int)yRotation, "CorridorL");
        }
    }

    private void AddTCorridorSpawnInfo(int[] kIdx, int yRotationNew)
    {
        Debug.Log("T info");
        if(mapCells[kIdx[0]][kIdx[1]].corridorIdx == -1 
           && mapCells[kIdx[0]][kIdx[1]].corridorYRot == -1)
        {
            mapCells[kIdx[0]][kIdx[1]].corridorIdx = 3; //or 4
            mapCells[kIdx[0]][kIdx[1]].corridorYRot = yRotationNew;
        }
        else if (!(mapCells[kIdx[0]][kIdx[1]].corridorIdx == 3
            && mapCells[kIdx[0]][kIdx[1]].corridorYRot == yRotationNew))
            //&& !isOverride)
        {
            AddCollisionInfoHelper(kIdx, yRotationNew, "CorridorT");
        }
    }

    private void AddXCorridorSpawnInfo(int [] kIdx)//, Vector3 kParentPos, Vector3 lParentPos, List<int> openings)
    {
        Debug.Log("X info");
        mapCells[kIdx[0]][kIdx[1]].corridorIdx = 5;
        mapCells[kIdx[0]][kIdx[1]].corridorYRot = 0;
    }
    
    private void AddCollisionInfoHelper(int[] kIdx, int yRotation, string corridorName)
    {
        List<int> openings1, openings2;
        openings1 = Data.instance.ConvertToOpenings(corridorName, yRotation, false);
        openings2 = Data.instance.ConvertToOpenings(CorridorsListIdxToCorridorName(mapCells[kIdx[0]][kIdx[1]].corridorIdx),
                                                    mapCells[kIdx[0]][kIdx[1]].corridorYRot, false);
        openings1.AddRange(openings2);

        openings1 = openings1.Distinct().ToList();

        if (openings1.Count == 3)
        {
            if (corridorName.EndsWith("T"))
            {
                mapCells[kIdx[0]][kIdx[1]].corridorIdx = 3; //or 4
                mapCells[kIdx[0]][kIdx[1]].corridorYRot = yRotation;
                return;
            }
            float yRotationNew = Data.instance.ConvertToRotation(openings1);
            AddTCorridorSpawnInfo(kIdx, (int)yRotationNew);
        }
        else if (openings1.Count == 4)
        {
            AddXCorridorSpawnInfo(kIdx);
        }
    }

    private void InstantiateICorridor(Vector3 posI, Vector3 kParentPos, Vector3 lParentPos)
    {
        int[] kIdx = GetIdx(posI);
        GameObject currentCorridor;
        currentCorridor = Instantiate(corridors[0]/*corridors[mapCells[kIdx[0]][kIdx[1]].corridorIdx]*/, posI, Quaternion.identity);

        currentCorridor.layer = 18;
        /*
        //Move CollisionDetector of corridor I by -0.25f in x axis to keep it in grid
        Transform collisionDetectorTransform = currentCorridor.transform.GetChild(1);
        collisionDetectorTransform.position = new Vector3(collisionDetectorTransform.position.x - 0.25f, collisionDetectorTransform.position.y, collisionDetectorTransform.position.z);
        */
        currentCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(kParentPos);
        currentCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(lParentPos);

        currentCorridor.transform.rotation = Quaternion.Euler(0, mapCells[kIdx[0]][kIdx[1]].corridorYRot, 0);
        //Data.instance.corridorCount++;
        currentCorridor.transform.GetChild(0).localPosition = new Vector3(0, 0, (mapCells[kIdx[0]][kIdx[1]].corridorYRot == 0) ? -0.08f : 0.226f);

        //For now, later remove and put outside this else block

        if (UnityEngine.Random.Range(0.0f, 1.0f) < ventCoverProbabilty)
        {
            Instantiate(ventCover, posI, Quaternion.Euler(0, UnityEngine.Random.Range(0, 3) * 90, 0), currentCorridor.transform);
        }

        // ----------- Item Gen -----------
        if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.1f)
        {
            itemGenScript.SpawnItems(posI - new Vector3(1, 0, 1), posI + new Vector3(1, 0, 1), 1, currentCorridor.transform);
            Data.instance.ctr1++;
        }
    }

    private void InstantiateLCorridor(int[] kIdx, Vector3 posToSpawn, Vector3 kParentPos, Vector3 lParentPos)
    {
        GameObject currCorridor1 = Instantiate(corridors[mapCells[kIdx[0]][kIdx[1]].corridorIdx], posToSpawn, Quaternion.identity, Data.instance.mapGenHolderTransform);
        currCorridor1.layer = 18;
        currCorridor1.GetComponentInChildren<CorridorNew>().rooms.Add(kParentPos);
        currCorridor1.GetComponentInChildren<CorridorNew>().rooms.Add(lParentPos);
        currCorridor1.transform.rotation = Quaternion.Euler(0, mapCells[kIdx[0]][kIdx[1]].corridorYRot, 0);
        if (mapCells[kIdx[0]][kIdx[1]].corridorYRot == 0)
        {
            //currCorridor1.GetComponentInChildren<BoxCollider>().enabled = false;
            currCorridor1.transform.localScale = new Vector3(-1, 1, 1);
            //currCorridor1.GetComponentInChildren<BoxCollider>().enabled = true;
            currCorridor1.transform.rotation = Quaternion.Euler(0, 90, 0);
        }
    }

    private void InstantiateTCorridor(int[] kIdx, Vector3 posToSpawn, Vector3 kParentPos, Vector3 lParentPos)
    {
        float yRotation = mapCells[kIdx[0]][kIdx[1]].corridorYRot;
        GameObject currCorridor = Instantiate((yRotation == 0 || yRotation == 270 || yRotation == -90) ? corridors[4] : corridors[3], posToSpawn, Quaternion.identity, mapGenHolderTransform);
        //MapgenProgress.instance.addProgress(1);
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

        currCorridor.transform.rotation = Quaternion.Euler(0, yRotation, 0);

        currCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(kParentPos);
        currCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(lParentPos);

        // TODO: Commented out for console clear 10/02/19
        // Debug.Log("added T at " + currCorridor.transform.position + " with yRot " + yRotation + " and scale " + currCorridor.transform.localScale);
    }

    private void InstantiateXCorridor(Vector3 posToSpawn, Vector3 kParentPos, Vector3 lParentPos)
    {
        //spawnAtPos.x = Mathf.Round(posToSpawn.x);
        //spawnAtPos.z = Mathf.Round(posToSpawn.z);
        CorridorNew corridorNew = Instantiate(corridors[5], posToSpawn, Quaternion.identity, mapGenHolderTransform).GetComponentInChildren<CorridorNew>();

        corridorNew.rooms.Add(kParentPos);
        corridorNew.rooms.Add(lParentPos);

        //MapgenProgress.instance.addProgress(2);
    }

    private string CorridorsListIdxToCorridorName(int idx)
    {
        if(idx == 0)
        {
            return "CorridorI";
        }
        else if(idx == 3 || idx == 4)
        {
            return "CorridorT";
        }
        else if(idx == 5)
        {
            return "CorridorX";
        }
        else if(idx == 1 || idx == 2 || idx == 7)
        {
            return "CorridorL";
        }
        else //if(idx == 6)
        {
            return "CorridorDeadEnd";
        }
    }

    /*
    private void InstantiateTorXCorridor(List<int> openings, Vector3 posToSpawn, Vector3 kParentPos, Vector3 lParentPos)
    {
        List<int> openings1 = new List<int>(), openings2 = new List<int>();
        openings1 = ConvertToOpenings(collidedCorridors[i].transform.parent.tag, collidedCorridors[i].transform.rotation.eulerAngles.y,
                                        (collidedCorridors[i].transform.parent.localScale.x == -1) ? true : false);
        openings2 = ConvertToOpenings(collidedCorridors[j].transform.parent.tag, collidedCorridors[j].transform.rotation.eulerAngles.y,
                                        (collidedCorridors[j].transform.parent.localScale.x == -1) ? true : false);
        openings1.AddRange(openings2);


        openings1 = openings1.Distinct().ToList();

        isError = false;

        if (openings1.Count == 3)
        {
            float yRotation = ConvertToRotation(openings1);
            Vector3 spawnAtPos = collidedCorridors[j].transform.parent.transform.position;
            spawnAtPos.x = Mathf.Round(spawnAtPos.x);
            spawnAtPos.z = Mathf.Round(spawnAtPos.z);
            GameObject currCorridor = Instantiate((yRotation == 0 || yRotation == 270 || yRotation == -90) ? corridorT2 : corridorT1, spawnAtPos, Quaternion.identity, mapGenHolderTransform);
            MapgenProgress.instance.addProgress(1);
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

            currCorridor.transform.rotation = Quaternion.Euler(0, yRotation, 0);
            // TODO: Commented out for console clear 10/02/19
            // Debug.Log("added T at " + currCorridor.transform.position + " with yRot " + yRotation + " and scale " + currCorridor.transform.localScale);
        }
        else if (openings1.Count == 4)//done separate
        {
        /*
            Vector3 spawnAtPos = collidedCorridors[j].transform.parent.transform.position;
            spawnAtPos.x = Mathf.Round(spawnAtPos.x);
            spawnAtPos.z = Mathf.Round(spawnAtPos.z);
            Instantiate(corridorX, spawnAtPos, Quaternion.identity, mapGenHolderTransform);
            MapgenProgress.instance.addProgress(2);
            *//*
        }
        else
        {
            Debug.Log("Error!!!!!!!!!!!!!!!!!!!!!!!!!");
            isError = true;
            Debug.Log("Count = " + openings1.Count);
            Debug.Log("Position = " + collidedCorridors[i].transform.position);
            Debug.Log("Name i = " + collidedCorridors[i].transform.parent.name);
            Debug.Log("Name j = " + collidedCorridors[j].transform.parent.name);

            foreach (var item in openings1)
            {
                Debug.Log(item);
            }
            Debug.Log("openings2");
            foreach (var item in openings2)
            {
                Debug.Log(item);
            }

            //Debug.Log("rotation = " + collidedCorridors[j].transform.rotation.eulerAngles.y);
            //Debug.Log("parent name " + collidedCorridors[j].transform.parent.name);
        }
    }
    */

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
