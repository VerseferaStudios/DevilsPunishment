using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNew2ndFloor : MonoBehaviour, IComparer<GameObject>
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
    public float ventCoverProbabilty = 0.090f;
    public GameObject ventCover;

    public ItemGen itemGenScript;

    void Start()
    {
        //mapGen3 = GameObject.FindGameObjectWithTag("Rooms(MapGen)").GetComponent<MapGen3>();

        // ------------------- Get array of doors / spawnPoints -------------------
        GameObject[] tempSpawnPoints = GameObject.FindGameObjectsWithTag("Corridor Spawn Points 2nd Floor");
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
                GameObject currentCorridor = Instantiate(corridors[0], spawnPoints[i].transform.position, Quaternion.identity, Data2ndFloor.instance.mapGenHolderTransform);
                currentCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(spawnPoints[i].transform.parent.transform.position);
                currentCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(spawnPoints[lastIdx].transform.parent.transform.position);
                if (spawnPoints[i].name.EndsWith("x"))
                {
                    currentCorridor.transform.rotation = Quaternion.Euler(0, 90, 0);
                }
                //Debug.Log("Spawn1");
                //Data2ndFloor.instance.corridorCount++;

                // ------------------- Added parents position to List<Vector3> to avoid future doors of the room -------------------
                visitedRooms.Add(spawnPoints[i].transform.parent.transform.position);
                visitedRooms.Add(spawnPoints[lastIdx].transform.parent.transform.position);

                //CheckDuplicatesAndConnect(spawnPoints[i].transform.parent.transform.position, spawnPoints[lastIdx].transform.parent.transform.position);

                Data2ndFloor.instance.connectedRooms.Add(visitedRooms);

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
        for (int i = 0; i < Data2ndFloor.instance.connectedRooms.Count; i++)
        {
            if (Mathf.Abs(Data2ndFloor.instance.connectedRooms[i][0].x - Data2ndFloor.instance.connectedRooms[i][1].x) == Data2ndFloor.instance.xSize 
                || Mathf.Abs(Data2ndFloor.instance.connectedRooms[i][0].z - Data2ndFloor.instance.connectedRooms[i][1].z) == Data2ndFloor.instance.zSize)
            {
                    
            }

        }
        */

        //give data the first door according to which we r sorting
        if(spawnPoints.Count > 0)
        {
            Data2ndFloor.instance.spawnPointsFirstPos = spawnPoints[0].transform.position;
        }

        //sort according to the comparer (ie according to the distance from the first door)
        //spawnPoints.Sort(Compare);
        //instead of sorting you can put a var max in the inner loop (l) and find the least distance one and use that !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        // ------------------- Connect two doors of different rooms with suitable corridor shapes -------------------
        int times = 0;
        for (k = 0; k < spawnPoints.Count; k++)//or k+=2 does it matter?
        {

            // ------------------- Remove door/spawnPoint if its of the same room -------------------
            if (/*times == 0 && spawnPoints.Count >= 9 && */Data2ndFloor.instance.CheckIfVisited(spawnPoints[k].transform.parent.transform.position))
            {
                ////Debug.Log("Removed a door of ____ " + spawnPoints[k].transform.parent.transform.position);
                spawnPoints.RemoveAt(k);
                k--;
                continue;
            }

            for (l = 0; l < spawnPoints.Count; l++) //i = 0 makes no diff; some rooms are getting overlooked, y //EXPT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            {

                if (k == l)
                {
                    continue;
                }

                if (/*times == 0 && spawnPoints.Count >= 9 && */Data2ndFloor.instance.CheckIfVisited(spawnPoints[l].transform.parent.transform.position))
                {
                    ////Debug.Log("Removed a door of ____ " + spawnPoints[i].transform.parent.transform.position);
                    spawnPoints.RemoveAt(l);
                    k--;
                    break;
                }

                if (k == l)
                {
                    continue;
                }

                // ------------------------ if k and i are not in the same room ------------------------
                if (!checkIfSameOrAdjacentRoom(k, l))
                {
                    ConnectTwoRooms(spawnPoints[k].transform.position, spawnPoints[l].transform.position,
                                    spawnPoints[k].name, spawnPoints[l].name, 
                                    spawnPoints[k].transform.parent.position, spawnPoints[l].transform.parent.position, false);
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
            if(k == spawnPoints.Count - 1)
            {

                //MakeInitHallways();

                Debug.Log("---------------------aesrdtfgyuhij0------------------------------------");
                MapgenProgress.instance.addProgress(2);

                StartCoroutine(Data2ndFloor.instance.DoConnectedComponents());
                StartCoroutine(Data2ndFloor.instance.DoCheckPerSecond());
            }

        }

        //corridorsParent = (GameObject.Find("Corridors") as GameObject).transform;
        //Debug.Log(Data2ndFloor.instance.corridorCount + "corridor count!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
    }

    public void ConnectTwoRooms(Vector3 kPos, Vector3 lPos, string kName, string lName, Vector3 kParentPos, Vector3 lParentPos, bool fromDataSingleton)
    {
        //making all y coordinates of all corridors equal to 0.5f
        kPos.y = lPos.y = 0.5f + Data2ndFloor.instance.floor2Height;


        Vector3 targetPos = new Vector3(0, 3, 0);

        Vector3 From = kPos;

        // ------------------- Connects x and z doors with L shape with no hindrance -------------------
        if (kName.EndsWith("x") && lName.EndsWith("z"))
        {
            targetPos = new Vector3(From.x, 0.5f + Data2ndFloor.instance.floor2Height, lPos.z);
            //Debug.Log("Spawn2");
        }

        // ------------------- Connects z and x doors with L shape with no hindrance -------------------
        else if (kName.EndsWith("z") && lName.EndsWith("x"))
        {
            targetPos = new Vector3(lPos.x, 0.5f + Data2ndFloor.instance.floor2Height, From.z);
            //Debug.Log("Spawn3");
        }

        // ------------------- Doesnt (xD) Connects (x and x) or (z and z) doors in different rooms with I shape with no hindrance -------------------
        else if (
            // -------------- if x doors AND z differnce == xSize --------------
            (kPos.x == lPos.x && Mathf.Abs(kPos.z - lPos.z) == Data2ndFloor.instance.xSize)
            ||
            // -------------- if z doors AND x differnce == 10 --------------
            (kPos.z == lPos.z && Mathf.Abs(kPos.x - lPos.x) == Data2ndFloor.instance.xSize)
            )
        {
            //targetPos = spawnPoints[i].transform.position;
            //Debug.Log("Spawn4--");
        }

        // --------------------- Connects x and x doors ---------------------
        else if (kName.EndsWith("x") && lName.EndsWith("x"))
        {
            //-------------- Connects x and x doors with `L shape to avoid hindrance --------------
            if (kPos.x != lPos.x)
            {
                //check and go nearer to destination
                Vector3 to = kPos;
                to.z += Data2ndFloor.instance.xSize / 2;     //Check 5 or 6 !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                // ------------------- Calls the actual spawning function -------------------
                spawnHalf(kPos, to, true, kName, lName, kParentPos, lParentPos);
                isExtraTurn = true;
                From = to;
                targetPos = new Vector3(lPos.x, 0.5f + Data2ndFloor.instance.floor2Height, From.z);
                //Debug.Log("Spawn5");
                /*
                //Debug.Log("From = " + From);
                //Debug.Log(targetPos);
                //Debug.Log(spawnPoints[i].transform.position);
                */
            }
            //-------------- Connects x and x doors with I shape since there's no hindrance --------------
            else
            {
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
                to.x += Data2ndFloor.instance.xSize / 2;     //Check 5 or 6 !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                // ------------------- Calls the actual spawning function -------------------
                spawnHalf(kPos, to, true, kName, lName, kParentPos, lParentPos);
                isExtraTurn = true;
                From = to;
                targetPos = new Vector3(From.x, 0.5f + Data2ndFloor.instance.floor2Height, lPos.z);
                //Debug.Log("Spawn6");
                /*
                //Debug.Log("From = " + From);
                //Debug.Log(targetPos);
                //Debug.Log(spawnPoints[i].transform.position);
                */
            }
            //-------------- Connects z and z doors with I shape since there's no hindrance --------------
            else
            {
                targetPos = lPos;
            }
        }


        // ------------------- Calls the actual spawning function -------------------
        spawnHalf(From, targetPos, !isExtraTurn, kName, lName, kParentPos, lParentPos);

        isExtraTurn = false;

        if (targetPos != lPos)
        {
            spawnHalf(targetPos, lPos, false, kName, lName, kParentPos, lParentPos);
        }

        //GameObject currCorridor1 = Instantiate(corridors[ChooseLCorridor(yRotation)], spawnNowAt, Quaternion.identity, Data2ndFloor.instance.mapGenHolderTransform);

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

        //Add opening according to the door type wuth the help of Data2ndFloor.instance.nearDoorL
        openings.Add(Data2ndFloor.instance.NeardoorLIndexSearch(lName[4].ToString() + lName[5].ToString()));
        /*
        //Debug.Log(Data2ndFloor.instance.ConvertToRotation(openings) + " " + (storedOpening == 0 ? 2 : 0) + " " + (lName[4].ToString() + lName[5].ToString())
                + " " + kPos + " " + lPos + " "
                + spawnPoints[k].name + " " + lName);
        */
        //Debug.Log(Data2ndFloor.instance.ConvertToRotation(openings) + " " + openings[0] + " " + openings[1]
        //+ " " + kPos + " " + lPos);

        float yRotation = Data2ndFloor.instance.ConvertToRotation(openings);
        GameObject currCorridor1 = Instantiate(corridors[ChooseLCorridor(yRotation)], lPos, Quaternion.identity, Data2ndFloor.instance.mapGenHolderTransform); 
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

        Data2ndFloor.instance.connectedRooms.Add(visitedRooms);

        /*
        //Debug.Log("VISITED ROOMS!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        foreach (var item in visitedRooms)
        {
            //Debug.Log(item);
        }
        //Debug.Log("CONNECTED ROOMS!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        foreach (var item in Data2ndFloor.instance.connectedRooms)
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
        
    }

    // ---------------------- Spawns I corridors from "Vector3 From", to "Vector3 to" except start and finish (where L corridor is needed)----------------------
    private void spawnHalf(Vector3 From, Vector3 to, bool isFirst, string kName, string lName, Vector3 kParentPos, Vector3 lParentPos)
    {
        // ----------- Variable for position to spawn at each for loop step -----------                
        spawnNowAt = From;
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

                //Add opening according to the door type wuth the help of Data2ndFloor.instance.nearDoorL
                openings.Add(Data2ndFloor.instance.NeardoorLIndexSearch(kName[4].ToString() + kName[5].ToString()));

                //storedOpening is for the next L corridor in line
                storedOpening = (From.z > to.z) ? 0 : 2;

                //Add the opposite of storedOpening since this one will be facing the next L corridor
                openings.Add(storedOpening == 0 ? 2 : 0);

                //Debug.Log(Data2ndFloor.instance.ConvertToRotation(openings) + " " + (storedOpening == 0 ? 2 : 0) + " " + (kName[4].ToString() + kName[5].ToString())
                    //+ " " + spawnPoints[k].transform.position + " " + spawnPoints[l].transform.position + " "
                    //+ kName + " " + lName);

                float yRotation = Data2ndFloor.instance.ConvertToRotation(openings);

                GameObject currCorridor1 = Instantiate(corridors[ChooseLCorridor(yRotation)], spawnNowAt, Quaternion.identity, Data2ndFloor.instance.mapGenHolderTransform);
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

                //Debug.Log(Data2ndFloor.instance.ConvertToRotation(openings) + " " + storedOpening + " " + ((From.z > to.z) ? 2 : 0)
                    //+ " " + spawnPoints[k].transform.position + " " + spawnPoints[l].transform.position + " "
                    //+ kName + " " + lName);

                //Add the previously stored storedOpening meant for this L corridor
                openings.Add(storedOpening);

                //storedOpening is for the next L corridor in line
                storedOpening = (From.z > to.z) ? 0 : 2;

                //Add the opposite of storedOpening since this one will be facing the next L corridor
                openings.Add(storedOpening == 0 ? 2 : 0);

                float yRotation = Data2ndFloor.instance.ConvertToRotation(openings);

                GameObject currCorridor1 = Instantiate(corridors[ChooseLCorridor(yRotation)], spawnNowAt, Quaternion.identity, Data2ndFloor.instance.mapGenHolderTransform);
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
            for (; i < Mathf.Abs(From.z - to.z) / Data2ndFloor.instance.corridorSize + 1 - 1; i++)
            {
                ////Debug.Log("Loop 1 = " + i);
                GameObject currentCorridor = Instantiate(corridorToSpawn, spawnNowAt/*new Vector3(spawnNowAt.x + 0.15f/*- 0.25f, spawnNowAt.y, spawnNowAt.z)*/, Quaternion.identity, Data2ndFloor.instance.mapGenHolderTransform);
                /*
                //Move CollisionDetector of corridor I by +0.25f in x axis to keep it in grid
                Transform collisionDetectorTransform = currentCorridor.transform.GetChild(1);
                collisionDetectorTransform.position = new Vector3(collisionDetectorTransform.position.x + 0.25f, collisionDetectorTransform.position.y, collisionDetectorTransform.position.z);
                */
                currentCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(kParentPos);
                currentCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(lParentPos);
                /*
                Data2ndFloor.instance.corridorCount++;
                if (Data2ndFloor.instance.isCollided)
                {
                    Data2ndFloor.instance.isCollided = false;
                    //check current corridor and rotation. check the already instantiated once type AND rotation (using other) ?????//check later

                }
                */
                currentCorridor.transform.GetChild(0).localPosition = new Vector3(0, 0, -0.08f);

                if (UnityEngine.Random.Range(0.0f, 1.0f) < ventCoverProbabilty)
                {
                    Instantiate(ventCover, spawnNowAt, Quaternion.Euler(0, UnityEngine.Random.Range(0, 3) * 90, 0), currentCorridor.transform).transform.GetChild(1).tag = "Vent Spawn Points 2nd Floor";
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

                //Add opening according to the door type wuth the help of Data2ndFloor.instance.nearDoorL
                openings.Add(Data2ndFloor.instance.NeardoorLIndexSearch(kName[4].ToString() + kName[5].ToString()));

                //storedOpening is for the next L corridor in line
                storedOpening = (From.x > to.x) ? 1 : 3;

                //Add the opposite of storedOpening since this one will be facing the next L corridor
                openings.Add(storedOpening == 1 ? 3 : 1);

                //Debug.Log(Data2ndFloor.instance.ConvertToRotation(openings) + " " + ((From.x > to.x) ? 3 : 1) + " " + (kName[4].ToString() + kName[5].ToString())
                    //+ " " + spawnPoints[k].transform.position + " " + spawnPoints[l].transform.position + " "
                    //+ kName + " " + lName);

                float yRotation = Data2ndFloor.instance.ConvertToRotation(openings);

                GameObject currCorridor1 = Instantiate(corridors[ChooseLCorridor(yRotation)], spawnNowAt, Quaternion.identity, Data2ndFloor.instance.mapGenHolderTransform);
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

                //Debug.Log(Data2ndFloor.instance.ConvertToRotation(openings) + " " + storedOpening + " " + ((From.x > to.x) ? 3 : 1)
                    //+ " " + spawnPoints[k].transform.position + " " + spawnPoints[l].transform.position + " "
                    //+ kName + " " + lName);

                //Add the previously stored storedOpening meant for this L corridor
                openings.Add(storedOpening);

                //storedOpening is for the next L corridor in line
                storedOpening = (From.x > to.x) ? 1 : 3;

                //Add the opposite of storedOpening since this one will be facing the next L corridor
                openings.Add(storedOpening == 1 ? 3 : 1);

                float yRotation = Data2ndFloor.instance.ConvertToRotation(openings);

                GameObject currCorridor1 = Instantiate(corridors[ChooseLCorridor(yRotation)], spawnNowAt, Quaternion.identity, Data2ndFloor.instance.mapGenHolderTransform);
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
            for (; i < Mathf.Abs(From.x - to.x) / Data2ndFloor.instance.corridorSize + 1 - 1; i++)
            {
                ////Debug.Log("Loop 2 = " + i);
                GameObject currentCorridor = Instantiate(corridorToSpawn, spawnNowAt/*new Vector3(spawnNowAt.x + 0.4f/*0.25f, spawnNowAt.y, spawnNowAt.z)*/, Quaternion.identity, Data2ndFloor.instance.mapGenHolderTransform);
                /*
                //Move CollisionDetector of corridor I by -0.25f in x axis to keep it in grid
                Transform collisionDetectorTransform = currentCorridor.transform.GetChild(1);
                collisionDetectorTransform.position = new Vector3(collisionDetectorTransform.position.x - 0.25f, collisionDetectorTransform.position.y, collisionDetectorTransform.position.z);
                */
                currentCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(kParentPos);
                currentCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(lParentPos);
                currentCorridor.transform.rotation = Quaternion.Euler(0, 90, 0);
                //Data2ndFloor.instance.corridorCount++;

                currentCorridor.transform.GetChild(0).localPosition = new Vector3(0, 0, 0.226f);

                if (UnityEngine.Random.Range(0.0f, 1.0f) < ventCoverProbabilty)
                {
                    Instantiate(ventCover, spawnNowAt, Quaternion.Euler(0, UnityEngine.Random.Range(0, 3) * 90, 0), currentCorridor.transform).transform.GetChild(1).tag = "Vent Cover 2nd Floor";
                }

                spawnNowAt.x += increment;

            }
        }
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
        if (Mathf.Abs(spawnPoints[k].transform.position.x - spawnPoints[i].transform.position.x) == Data2ndFloor.instance.xSize / 2)
        {
            return true;
        }

        return false;
    }

    public int Compare(GameObject x, GameObject y)
    {
        return (int)(Vector3.Distance(x.transform.position, Data2ndFloor.instance.spawnPointsFirstPos)
                    - Vector3.Distance(y.transform.position, Data2ndFloor.instance.spawnPointsFirstPos));
    }


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
