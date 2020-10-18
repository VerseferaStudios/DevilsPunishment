using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNew2ndFloor : RoomNew
{
    //private List<Transform> spawnPoints = new List<Transform>();
    private List<GameObject> spawnPoints = new List<GameObject>();
    //public GameObject[] corridors;
    //public GameObject[] vents;
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

    //public ArrayList allRooms = new ArrayList();
    //public Transform mapGenHolderTransform;
    //public float ventCoverProbabilty = 0.390f;
    //public GameObject ventCover;

    //public ItemGen itemGenScript;
    
    private int counter = 0;

    private bool isDoneSpawnHalf = false, isDoneConnectTwoRooms = false;

    protected override void SetCorridorSpawnPointTag()
    {
        corridorSpawnPointTag = "Corridor Spawn Points 2nd Floor";
        isSetCorridorSPawnPointTag = true;
    }

    protected override IEnumerator AddRoomNewVents()
    {
        yield return new WaitForSeconds(1f);
        RoomNewVents2ndFloor roomNewVents = new GameObject(Constants.sRef.GBNAME_ROOMNEWVENTS2NDFLOOR).AddComponent<RoomNewVents2ndFloor>(); //Dont change name, its used to not add the script multiple times
        roomNewVents.corridors = vents;


        //roomNewVents = AddRoomNewCorrectly();
        roomNewVents.vents = vents;
        roomNewVents.allRooms = allRooms;
        roomNewVents.ventCover = ventCover;

        roomNewVents.mapGenHolderTransform = new GameObject("VentsHolder").transform;
        //roomNewVents.itemGenScript = itemGenScript;
        roomNewVents.mapSizeX = mapSizeX;
        roomNewVents.mapSizeZ = mapSizeZ;
        roomNewVents.roomHeight = roomHeight;
        roomNewVents.corridorSpawnPointTag = ventCoverTag;
        roomNewVents.isDevMode = isDevMode;
        if (isDevMode)
        {
            roomNewVents.testGridCube = testGridCube;
        }



        roomNewVents.aStarVisualisationTime = aStarVisualisationTime;

        SquareGrid squareGrid = AStarSearch.InitialiseSquareGrid(xSize, zSize, mapSizeX, mapSizeZ, out int xOverall, out int zOverall);

        roomNewVents.squareGrid = squareGrid;
        if (isDevMode)
        {
            roomNewVents.testGridPlaneHolder = testGridPlaneHolder;
        }
        //roomNewVents.ventCoverProbabilty = ventCoverProbabilty;
        //Data.instance.roomNewVents = roomNewVents;

        //RoomNewVents Start Script is called here for getting proper timing
        //so that all vent covers and spawn and also given proper tag
        StartCoroutine(roomNewVents.StartScript());

        yield return null;
    }

    //protected override void Start()
    //{
    //    base.Start();

    //    //enabled = false;
    //    return;
    //    //mapGen3 = GameObject.FindGameObjectWithTag("Rooms(MapGen)").GetComponent<MapGen3>();

    //    // ------------------- Get array of doors / spawnPoints -------------------
    //    GameObject[] tempSpawnPoints = GameObject.FindGameObjectsWithTag("Corridor Spawn Points 2nd Floor");
    //    //string f = tempSpawnPoints[0].GetComponentsInChildren<Transform>()[0].gameObject.name;
    //    //GameObjects to transform
    //    /*
    //    for (int i = 0; i < tempSpawnPoints.Length; i++)
    //    {
    //        tempSpawnPoints[i] = tempSpawnPoints[i].transform.position;
    //    }
    //    */

    //    // ------------------- Convert array of doors / spawnPoints into list -------------------
    //    spawnPoints.AddRange(tempSpawnPoints);
    //    //Debug.Log("spawnPoints.Count = " + spawnPoints.Count);

    //    // ------------------- Find exactly overlapping doors/spawnPoints, spawn a corridor at thst position, and destroy both doors/spawnPoints -------------------
    //    for (int i = 0; i < spawnPoints.Count; i++)
    //    {
    //        bool isFound = false;
    //        int lastIdx = i;
    //        for (int j = 0; j < spawnPoints.Count; j++)
    //        {
    //            if (i == j)
    //            {
    //                continue;
    //            }
    //            ////Debug.Log("i = " + i + " & j = " + j);
    //            if (spawnPoints[i].transform.position == spawnPoints[j].transform.position)
    //            {
    //                isFound = true;
    //                lastIdx = j;
    //                break;
    //            }
    //        }
    //        if (isFound)
    //        {
    //            GameObject currentCorridor = Instantiate(corridors[0], spawnPoints[i].transform.position, Quaternion.identity, Data.instance.mapGenHolderTransform);
    //            currentCorridor.layer = 18;
    //            //currentCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(spawnPoints[i].transform.parent.position);
    //            //currentCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(spawnPoints[lastIdx].transform.parent.position);
    //            if (spawnPoints[i].name.EndsWith("x"))
    //            {
    //                currentCorridor.transform.rotation = Quaternion.Euler(0, 90, 0);
    //            }
    //            //Debug.Log("Spawn1");
    //            //Data2ndFloor.instance.corridorCount++;

    //            // ------------------- Added parents position to List<Vector3> to avoid future doors of the room -------------------
    //            visitedRooms.Add(spawnPoints[i].transform.parent.position);
    //            visitedRooms.Add(spawnPoints[lastIdx].transform.parent.position);

    //            //CheckDuplicatesAndConnect(spawnPoints[i].transform.parent.transform.position, spawnPoints[lastIdx].transform.parent.transform.position);

    //            Data2ndFloor.instance.connectedRooms.Add(visitedRooms);

    //            ////Debug.Log(spawnPoints[i].transform.position + "______________________________________________________");
    //            spawnPoints.RemoveAt(i);

    //            // -------------- decrease lastIdx if greater than i --------------
    //            if (lastIdx > i)
    //            {
    //                lastIdx--;
    //            }

    //            i--;

    //            spawnPoints.RemoveAt(lastIdx);

    //            // -------------- decrease i if greater than lastIdx --------------
    //            if (i > lastIdx)
    //            {
    //                i--;
    //            }
    //            lastIdx--;

    //            visitedRooms = new List<Vector3>();

    //            isFound = false;
    //        }
    //    }


    //    //put all adjacnt rooms in same component
    //    /*
    //    for (int i = 0; i < Data2ndFloor.instance.connectedRooms.Count; i++)
    //    {
    //        if (Mathf.Abs(Data2ndFloor.instance.connectedRooms[i][0].x - Data2ndFloor.instance.connectedRooms[i][1].x) == Data2ndFloor.instance.xSize 
    //            || Mathf.Abs(Data2ndFloor.instance.connectedRooms[i][0].z - Data2ndFloor.instance.connectedRooms[i][1].z) == Data2ndFloor.instance.zSize)
    //        {

    //        }

    //    }
    //    */

    //    //give data the first door according to which we r sorting
    //    if(spawnPoints.Count > 0)
    //    {
    //        Data2ndFloor.instance.spawnPointsFirstPos = spawnPoints[0].transform.position;
    //    }

    //    //sort according to the comparer (ie according to the distance from the first door)
    //    //spawnPoints.Sort(Compare);
    //    //instead of sorting you can put a var max in the inner loop (l) and find the least distance one and use that !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

    //    // ------------------- Connect two doors of different rooms with suitable corridor shapes -------------------
    //    StartCoroutine(CallConnectRooms());

    //    //corridorsParent = (GameObject.Find("Corridors") as GameObject).transform;
    //    //Debug.Log(Data2ndFloor.instance.corridorCount + "corridor count!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
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

    private int ChooseLCorridor(float yRotation)
    {
        return (yRotation == 0 || yRotation == 180) ? 2 : ((yRotation == 90) ? 7 : 1);
    }

    // ---------------------- Checks if spawnPoints k and i belong to the same room or adjacent rooms ----------------------
    private bool checkIfSameOrAdjacentRoom(int k, int i)
    {
        bool isDoorTypeX = spawnPoints[k].name.EndsWith("x") ? true : false;

        // ------------- Check x axis ------------- 
        if (Mathf.Abs(spawnPoints[k].transform.position.x - spawnPoints[i].transform.position.x) == Data2ndFloor.instance.xSize)
        {
            return true;
        }
        // ------------- Check z axis ------------- 
        else if (Mathf.Abs(spawnPoints[k].transform.position.z - spawnPoints[i].transform.position.z) == Data2ndFloor.instance.xSize)
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

    //public int Compare(GameObject x, GameObject y)
    //{
    //    return (int)(Vector3.Distance(x.transform.position, Data2ndFloor.instance.spawnPointsFirstPos)
    //                - Vector3.Distance(y.transform.position, Data2ndFloor.instance.spawnPointsFirstPos));
    //}


    private void CheckDuplicatesAndConnect(List<Vector3> rooms)
    {
        for (int i = 0; i < Data2ndFloor.instance.connectedRooms.Count; i++)
        {
            for (int j = 0; j < Data2ndFloor.instance.connectedRooms[i].Count; j++)
            {
                if(rooms[0] == Data2ndFloor.instance.connectedRooms[i][j])
                {
                    Data2ndFloor.instance.connectedRoomsThroughCollision.Add(new ConnectedComponent(rooms[0] / 2 + rooms[1] / 2, rooms));
                }
            }
        }
    }

    private void CompareAndAddAdjacent()
    {
        Vector3 collidedConnectedRoomToSearch;
        for (int i = 0; i < Data2ndFloor.instance.connectedRoomsThroughCollision.Count; i++)
        {
            for (int j = 0; j < Data2ndFloor.instance.connectedRoomsThroughCollision[i].rooms.Count; j++)
            {
                collidedConnectedRoomToSearch = Data2ndFloor.instance.connectedRoomsThroughCollision[i].rooms[j];
                for (int k = 0; k < Data2ndFloor.instance.connectedRooms.Count; k++)
                {
                    for (int q = 0; q < Data2ndFloor.instance.connectedRooms[k].Count; q++)
                    {
                        // -------------- Now we are taking an element of connectedRoomsThroughCollision (collidedConnectedRoomToSearch)  -------------- 
                        // -------------- and comparing it with every element of connectedRooms -------------- 
                        // -------------- and adding the req ones to connectedRoomsThroughCollision --------------
                        // -------------- and removing the same ones from connectedRooms --------------
                        if (collidedConnectedRoomToSearch == Data2ndFloor.instance.connectedRooms[k][q])
                        {
                            Data2ndFloor.instance.connectedRoomsThroughCollision.Add(new ConnectedComponent(/*Data2ndFloor.instance.connectedRoomsThroughCollision[i].corridorPos*/ new Vector3(1, 1, 1), Data2ndFloor.instance.connectedRooms[k]));
                            Data2ndFloor.instance.connectedRooms.RemoveAt(k);
                            k--;
                            break;
                        }
                    }
                }
            }
        }
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
        for (int i = 0; i < Data2ndFloor.instance.allRooms.Count; i++)
        {
            ////Debug.Log(((int[])Data2ndFloor.instance.allRooms[i])[1]);
            if(Mathf.Abs(-((int[])Data2ndFloor.instance.allRooms[i])[1] - FromTemp.x) != Data2ndFloor.instance.xSize/2 + 1 && Mathf.Abs(-((int[])Data2ndFloor.instance.allRooms[i])[1] - FromTemp.x) < Data2ndFloor.instance.xSize)
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
            for (i = 0; i < Data2ndFloor.instance.allRooms.Count; i++)
            {
                if (Mathf.Abs(-((int[])Data2ndFloor.instance.allRooms[i])[0] - FromTemp.z) != Data2ndFloor.instance.zSize/2 + 1 && Mathf.Abs(-((int[])Data2ndFloor.instance.allRooms[i])[0] - FromTemp.z) < Data2ndFloor.instance.zSize)
                {
                    goToNext = true;
                    break;
                }
            }
            if (i == Data2ndFloor.instance.allRooms.Count)
                return "xz";
        }
        if (goToNext)
        {
            targetPos = new Vector3(to.x, 0.5f, FromTemp.z);
            FromTemp = From;
            //z const
            for (int i = 0; i < Data2ndFloor.instance.allRooms.Count; i++)
            {
                if (Mathf.Abs(-((int[])Data2ndFloor.instance.allRooms[i])[0] - FromTemp.z) != Data2ndFloor.instance.zSize/2 + 1 && Mathf.Abs(-((int[])Data2ndFloor.instance.allRooms[i])[0] - FromTemp.z) < Data2ndFloor.instance.zSize)
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
            for (i = 0; i < Data2ndFloor.instance.allRooms.Count; i++)
            {
                if (Mathf.Abs(-((int[])Data2ndFloor.instance.allRooms[i])[1] - FromTemp.x) != Data2ndFloor.instance.xSize/2 + 1 && Mathf.Abs(-((int[])Data2ndFloor.instance.allRooms[i])[1] - FromTemp.x) < Data2ndFloor.instance.xSize)
                {
                    goToNext = true;
                    break;
                }
            }
            if (i == Data2ndFloor.instance.allRooms.Count)
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
