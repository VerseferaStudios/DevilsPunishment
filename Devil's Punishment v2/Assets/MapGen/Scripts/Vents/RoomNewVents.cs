using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
enum FloorNo
{
    _1stFloor,
    _2ndFloor
}
public class RoomNewVents : RoomNew
{
    //private List<Transform> spawnPoints = new List<Transform>();
    private List<GameObject> spawnPoints = new List<GameObject>();
    //public GameObject[] corridors;
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


    [SerializeField] private FloorNo floorNo;

    protected override void Start()
    {
        corridorSpawnPointTag = floorNo == FloorNo._1stFloor ? Constants.sRef.TAG_VENTSPAWNFLOOR1 : Constants.sRef.TAG_VENTSPAWNFLOOR2;
        base.Start();

        StartCoroutine(StartScript());

    }

    protected override int ChooseLCorridor(float yRotation)
    {
        return 1;
    }

    protected override Vector3 GetPos(int[] idx)
    {
        return new Vector3(idx[0] * -4, 0.5f + roomHeight - 2, idx[1] * -4);
    }

    protected override void VentPos(ref Vector3 currPos)
    {
        currPos.y -= 2;
    }

    protected override void HelperIInstantiate(GameObject currentCorridor, int[] kIdx, Vector3 posI) { return; }

    protected override void HelperTInstantiate(float yRotation, GameObject currCorridor) { return; }

    protected override void SquareGridWallPopulate() { return; }

    protected override void LRotScaleHelper(int yRotation, GameObject currCorridor1, int childEulerZ)
    {
        currCorridor1.transform.rotation = Quaternion.Euler(0, yRotation, 0);
        Debug.Log("childEulerZ = " + childEulerZ);
        currCorridor1.transform.GetChild(0).localEulerAngles = new Vector3(0, 0, childEulerZ);
    }

    protected override void TRotationScaleHelper(float yRotation, GameObject currCorridor, int childEulerX)
    {
        base.TRotationScaleHelper(yRotation, currCorridor, childEulerX);
        currCorridor.transform.GetChild(0).localEulerAngles = new Vector3(childEulerX, 0, 0);
    }

    protected override void AddLCorridorSpawnInfo(List<int> openings, Vector3 posToSpawn, int zOverall, int childEulerZ, int childEulerX)
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
        Debug.Log("lcorr childEulerZ = " + childEulerZ);
            squareGrid.tiles[kIdx[0], kIdx[1]].childEulerZ = childEulerZ;
            squareGrid.tiles[kIdx[0], kIdx[1]].childEulerX = childEulerX;
        }
        else if (!(squareGrid.tiles[kIdx[0], kIdx[1]].corridorIdx == corridorIdx
            && squareGrid.tiles[kIdx[0], kIdx[1]].corridorYRot == (int)yRotation))
        //&& !isOverride)
        {
            Debug.LogWarning("123");
            AddCollisionInfoHelper(kIdx, (int)yRotation, "CorridorL", zOverall, childEulerZ, childEulerX);
        }
    }

    protected override void AddCollisionInfoHelper(int[] kIdx, int yRotation, string corridorName, int zOverall, int childEulerZ, int childEulerX)
    {
        List<int> openings1, openings2;
        openings1 = Data.instance.ConvertToOpeningsVents(corridorName, yRotation, childEulerZ, childEulerX);
        openings2 = Data.instance.ConvertToOpeningsVents(CorridorsListIdxToCorridorName(squareGrid.tiles[kIdx[0], kIdx[1]].corridorIdx),
                                                    squareGrid.tiles[kIdx[0], kIdx[1]].corridorYRot, childEulerZ, childEulerX);



        //openings1 = ConvertToOpeningsVents(collidedVents[i].transform.parent.tag, collidedVents[i].transform.rotation.eulerAngles.y,
        //    collidedVents[i].transform.parent.GetChild(0).localEulerAngles.z, collidedVents[i].transform.parent.GetChild(0).localEulerAngles.x);

        //openings2 = ConvertToOpeningsVents(collidedVents[j].transform.parent.tag, collidedVents[j].transform.rotation.eulerAngles.y,
        //    collidedVents[j].transform.parent.GetChild(0).localEulerAngles.z, collidedVents[j].transform.parent.GetChild(0).localEulerAngles.x);

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

        Vector3 currLoc = GetPos(kIdx);
        if (currLoc.x == -112 && currLoc.z == -36)
        {
            Debug.LogWarning("1");
        }

        float yRotationNew = Data.instance.ConvertToRotation(openings1);
        if(openings1.Count == 2)
        {
            if (currLoc.x == -112 && currLoc.z == -36)
            {
                Debug.LogWarning("1");
            }
            if(squareGrid.tiles[kIdx[0], kIdx[1]].childEulerZ != 0)
            {
                //T corridor with X 90
                AddTCorridorSpawnInfo(kIdx, (int)yRotationNew, zOverall, 0, 90, true);
            }

            if (squareGrid.tiles[kIdx[0], kIdx[1]].childEulerX != 0)
            {
                //Already T see if issue is there, if yea delete above grill, else continue
            }
        }
        else if (openings1.Count == 3)
        {
            if (corridorName.EndsWith("T"))
                //&& squareGrid.tiles[kIdx[0], kIdx[1]].corridorYRot == yRotation) //no Need
            {
                squareGrid.tiles[kIdx[0], kIdx[1]].corridorIdx = 3; //or 4
                squareGrid.tiles[kIdx[0], kIdx[1]].corridorYRot = yRotation;
                return;
            }
            bool isMinusTwo = false;
            for (int i = 0; i < openings1.Count; i++)
            {
                if(openings1[i] == -2)
                {
                    isMinusTwo = true;
                }
            }
            if (isMinusTwo)
            {
                AddTCorridorSpawnInfo(kIdx, (int)yRotationNew, zOverall, 0, 90, true);
            }
            else
            {
                AddTCorridorSpawnInfo(kIdx, (int)yRotationNew, zOverall, childEulerZ, childEulerX, true);
            }

            if (currLoc.x == -112 && currLoc.z == -36)
            {
                Debug.LogWarning("1");
            }

            //openings1.Sort();
            //bool isThereVentCoverAbove = false;
            //Debug.Log("========");
            ///*
            //foreach (var item in openings1)
            //{
            //    Debug.Log(item);
            //}
            //*/
            //if (openings1[0] == -2)
            //{
            //    isThereVentCoverAbove = true;
            //    openings1.RemoveAt(0);
            //    openings1.Add((openings1[1] + 1) % 4);
            //}
            ///*
            //foreach (var item in openings1)
            //{
            //    Debug.Log(item);
            //}
            //*/
            //GameObject currCorridor = Instantiate(ventT, spawnAtPos, Quaternion.identity, mapGenHolderTransform);
            //MapgenProgress.instance.addProgress(2);
            ///*
            //if (yRotation == 0)
            //{
            //    currCorridor.transform.GetChild(0).localPosition = new Vector3(0.15f, 0, -0.155f);
            //}
            //if (yRotation == 270 || yRotation == -90 || yRotation == 180)
            //{

            //    if (yRotation == -90 || yRotation == 270)
            //    {
            //        currCorridor.transform.GetChild(0).localPosition = new Vector3(0.156f, 0, -0.156f);
            //    }

            //    //MeshCollider bc = currCorridor.GetComponentInChildren<MeshCollider>();
            //    //Destroy(bc);
            //    currCorridor.transform.localScale = new Vector3(-1, 1, 1);
            //    //currCorridor.transform.Find("CollisionDetector").gameObject.AddComponent<MeshCollider>().size = new Vector3(1, 0.5f, 1);
            //}
            //*/
            //if (isThereVentCoverAbove)
            //{
            //    currCorridor.transform.GetChild(0).localEulerAngles = new Vector3(90, 0, 0);
            //}
            //currCorridor.transform.rotation = Quaternion.Euler(0, yRotation, 0);
            ////Debug.Log("added T VENT at " + currCorridor.transform.position + " with yRot " + yRotation + " and scale " + currCorridor.transform.localScale);

        }
        else if (openings1.Count == 4)
        {
            openings1.Sort();
            bool isThereVentCoverAbove = false;
            /*Debug.Log("========");
            foreach (var item in openings1)
            {
                Debug.Log(item);
            }
            */
            if (openings1[0] == -2)
            {
                isThereVentCoverAbove = true;
                openings1.RemoveAt(0);
                //if (collidedVents[j].transform.parent.name.EndsWith("L"))
                //{
                //    Destroy(collidedVents[j].transform.parent.parent.gameObject); // DONT DESTROY "New Game Object" (put condition)
                //    collidedVents[j].transform.parent.transform.SetParent(mapGenHolderTransform);//but it will be destroyed anyway later... check performance(meh) THIS may not work.. but its fine for now
                //}
                //else if (collidedVents[i].transform.parent.name.EndsWith("L"))
                //{
                //    Destroy(collidedVents[i].transform.parent.parent.gameObject);
                //    collidedVents[i].transform.parent.transform.SetParent(mapGenHolderTransform);//but it will be destroyed anyway later... check performance(meh) THIS may not work.. but its fine for now
                //}
            }
            else
            {
                AddXCorridorSpawnInfo(kIdx, zOverall);

                if (currLoc.x == -112 && currLoc.z == -36)
                {
                    Debug.LogWarning("1");
                }
            }

        }
        else
        {
            Debug.Log("Error!!!!!!!!!!!!!!!!!!!!!!!!!");
            //isError = true;
            Debug.Log("Count = " + openings1.Count);
            //Debug.Log("Position = " + collidedVents[i].transform.position);
            //Debug.Log("rotation = " + collidedVents[j].transform.rotation.eulerAngles.y);
            //Debug.Log("parent name " + collidedVents[j].transform.parent.name);
        }




        //if (openings1.Count == 3)
        //{
        //}
        //else if (openings1.Count == 4)
        //{
        //}
        //else
        //{
        //    Debug.Log("Error!!!!!!!!!!!!!!!!!!!!!!!!!");
        //    isError = true;
        //    Debug.Log("Count = " + openings1.Count);
        //    Debug.Log("Position = " + collidedVents[i].transform.position);
        //    //Debug.Log("rotation = " + collidedVents[j].transform.rotation.eulerAngles.y);
        //    //Debug.Log("parent name " + collidedVents[j].transform.parent.name);
        //}




    }

    public List<int> ConvertToOpeningsVents(string tag, float yRotation, float holderZRotation, float holderXRotation)
    {
        List<int> openings = new List<int>();
        if (tag.EndsWith("I"))
        {
            openings.Add((int)(yRotation / 90f));
            openings.Add(openings[0] + 2);
            ////Debug.Log(openings[0] + " " + openings[1]);
        }
        else if (tag.EndsWith("L"))
        {
            if (yRotation == 270 || yRotation == -90)
            {
                if (holderZRotation == 0)
                {
                    openings.Add(0);
                }
                else
                {
                    openings.Add(-2);
                }
                openings.Add(3);
            }
            else
            {
                openings.Add((int)(yRotation / 90f));
                if (holderZRotation == 0)
                {
                    openings.Add(openings[0] + 1);
                }
                else
                {
                    openings.Add(-2);
                }
            }
        }
        else if (tag.EndsWith("T"))
        {
            List<int> oneToFour = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                oneToFour.Add(i);
            }
            int rot = (int)(yRotation / 90f);
            oneToFour.Remove(rot);
            if (holderXRotation != 0)
            {
                oneToFour.Remove((rot + 2) % 4);
                openings.Add(-2);
            }
            openings.AddRange(oneToFour);
        }
        else if (tag.EndsWith("X"))
        {
            List<int> oneToFour = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                oneToFour.Add(i);
            }
            openings.AddRange(oneToFour);
        }
        return openings;
    }


    private void startscriptold() 
    {
        //mapGen3 = GameObject.FindGameObjectWithTag("Rooms(MapGen)").GetComponent<MapGen3>();

        // ------------------- Get array of doors / spawnPoints -------------------
        GameObject[] tempSpawnPoints = GameObject.FindGameObjectsWithTag(corridorSpawnPointTag);
        Debug.Log("DEBUG VENTS COVER NO = " + tempSpawnPoints.Length);

        // ------------------- Convert array of doors / spawnPoints into list -------------------
        spawnPoints.AddRange(tempSpawnPoints);

        //give data the first door according to which we r sorting
        //if (spawnPoints.Count > 0)
        //{
        //    Data.instance.spawnPointsFirstPos = spawnPoints[0].transform.position;
        //}

        //sort according to the comparer (ie according to the distance from the first door)
        //spawnPoints.Sort(Compare);
        //instead of sorting you can put a var max in the inner loop (l) and find the least distance one and use that !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        // ------------------- Connect two doors of different rooms with suitable corridor shapes -------------------
        int times = 0;
        for (k = 0; k < spawnPoints.Count; k++)//or k+=2 does it matter?
        {

            // ------------------- Remove door/spawnPoint if its of the same room -------------------
            if (/*times == 0 && spawnPoints.Count >= 9 && */Data.instance.CheckIfVisited(spawnPoints[k].transform.parent.transform.position))
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

                if (/*times == 0 && spawnPoints.Count >= 9 && */Data.instance.CheckIfVisited(spawnPoints[l].transform.parent.transform.position))
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
                    ConnectTwoRooms(spawnPoints[k].transform, spawnPoints[l].transform);
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
                //StartCoroutine(Data.instance.DoConnectedComponents());
                //StartCoroutine(Data.instance.DoCheckVentsPerSecond()); // EDIT FOR VENTS!!!
            }

        }

        //corridorsParent = (GameObject.Find("Corridors") as GameObject).transform;
        //Debug.Log(Data.instance.corridorCount + "corridor count!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
    }
    //Start EDITING vents from somewhere here
    public void ConnectTwoRooms(Transform spawnPointk, Transform spawnPointl)
    {
        Vector3 kPos = spawnPointk.position;
        Vector3 lPos = spawnPointl.position;
        //making all y coordinates of all corridors equal to 0.5f
        kPos.y = lPos.y = 0.5f - 2f;


        Vector3 targetPos = new Vector3(0, 3, 0);

        Vector3 From = kPos;

        if(From.x == lPos.x)
        {
            targetPos = new Vector3(From.x, 0.5f - 2f, lPos.z);
        }
        else
        {
            targetPos = new Vector3(lPos.x, 0.5f - 2f, From.z);
        }

        /*
        // ------------------- Connects x and z doors with L shape with no hindrance -------------------
        if (kName.EndsWith("x") && lName.EndsWith("z"))
        {
            targetPos = new Vector3(From.x, 0.5f, lPos.z);
            //Debug.Log("Spawn2");
        }

        // ------------------- Connects z and x doors with L shape with no hindrance -------------------
        else if (kName.EndsWith("z") && lName.EndsWith("x"))
        {
            targetPos = new Vector3(lPos.x, 0.5f, From.z);
            //Debug.Log("Spawn3");
        }

        // ------------------- Doesnt (xD) Connects (x and x) or (z and z) doors in different rooms with I shape with no hindrance -------------------
        else if (
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
                spawnHalf(kPos, to, true, kName, lName, kParentPos, lParentPos);
                isExtraTurn = true;
                From = to;
                targetPos = new Vector3(lPos.x, 0.5f, From.z);
                //Debug.Log("Spawn5");
                /*
                //Debug.Log("From = " + From);
                //Debug.Log(targetPos);
                //Debug.Log(spawnPoints[i].transform.position);
                */
        /*
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
        to.x += Data.instance.xSize / 2;     //Check 5 or 6 !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        // ------------------- Calls the actual spawning function -------------------
        spawnHalf(kPos, to, true, kName, lName, kParentPos, lParentPos);
        isExtraTurn = true;
        From = to;
        targetPos = new Vector3(From.x, 0.5f, lPos.z);
        //Debug.Log("Spawn6");
        /*
        //Debug.Log("From = " + From);
        //Debug.Log(targetPos);
        //Debug.Log(spawnPoints[i].transform.position);
        */
        /*
    }
    //-------------- Connects z and z doors with I shape since there's no hindrance --------------
    else
    {
        targetPos = lPos;
    }
}
*/
        //Spawn first vent near vent cover 
        SpawnFirstVent(From, targetPos, spawnPointk);
        // ------------------- Calls the actual spawning function -------------------
        spawnHalf(From, targetPos, true);//, kName, lName, kParentPos, lParentPos);

        if (targetPos != lPos)
        {
            spawnHalf(targetPos, lPos, false);//, kName, lName, kParentPos, lParentPos);
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

        // -------------- Add opening according to the door type wuth the help of Data.instance.nearDoorL --------------
        //openings.Add(Data.instance.NeardoorLIndexSearch(lName[4].ToString() + lName[5].ToString()));
        /*
        //Debug.Log(Data.instance.ConvertToRotation(openings) + " " + (storedOpening == 0 ? 2 : 0) + " " + (lName[4].ToString() + lName[5].ToString())
                + " " + kPos + " " + lPos + " "
                + spawnPoints[k].name + " " + lName);
        */
        //Debug.Log(Data.instance.ConvertToRotation(openings) + " " + openings[0] + " " + openings[1]
        //+ " " + kPos + " " + lPos);

        float yRotation = Data.instance.ConvertToRotation(openings);
        GameObject currCorridor1 = Instantiate(corridors[8], lPos, Quaternion.identity);//, Data.instance.mapGenHolderTransform);
        //currCorridor1.GetComponentInChildren<CorridorNew>().rooms.Add(kParentPos);
        //currCorridor1.GetComponentInChildren<CorridorNew>().rooms.Add(lParentPos);

        currCorridor1.transform.SetParent(spawnPointl);

        currCorridor1.transform.GetChild(0).localEulerAngles = new Vector3(0, 0, 90);
        currCorridor1.transform.rotation = Quaternion.Euler(0, yRotation, 0);
        if (yRotation == 0)
        {
            //currCorridor1.GetComponentInChildren<BoxCollider>().enabled = false;
            //currCorridor1.transform.localScale = new Vector3(-1, 1, 1);
            //currCorridor1.GetComponentInChildren<BoxCollider>().enabled = true;
            //currCorridor1.transform.rotation = Quaternion.Euler(0, 90, 0);
        }


        // ------------------- Added parents position to List<Vector3> to avoid future doors of the room -------------------
        //visitedRooms.Add(lParentPos);
        //visitedRooms.Add(kParentPos);

        //CheckDuplicatesAndConnect(lParentPos, spawnPoints[k].transform.parent.transform.position);

        Data.instance.connectedRoomsVents.Add(visitedRooms);

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

        //if (!fromDataSingleton)
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

    // ---------------- Instantiates L vent in correct rotation at the start vent cover ----------------
    public void SpawnFirstVent(Vector3 From, Vector3 to, Transform spawnPointk)
    {
        spawnNowAt = From;
        if (From.x == to.x)
        {
            List<int> openings = new List<int>();

            //Add opening according to the door type wuth the help of Data.instance.nearDoorL
            //openings.Add(Data.instance.NeardoorLIndexSearch(kName[4].ToString() + kName[5].ToString()));

            //storedOpening is for the next L corridor in line
            storedOpening = (From.z > to.z) ? 0 : 2;

            //Add the opposite of storedOpening since this one will be facing the next L corridor
            openings.Add(storedOpening == 0 ? 2 : 0);

            //Debug.Log(Data.instance.ConvertToRotation(openings) + " " + (storedOpening == 0 ? 2 : 0) + " " + (kName[4].ToString() + kName[5].ToString())
            //+ " " + spawnPoints[k].transform.position + " " + spawnPoints[l].transform.position + " "
            //+ kName + " " + lName);

            float yRotation = Data.instance.ConvertToRotation(openings);

            GameObject currCorridor1 = Instantiate(corridors[8], spawnNowAt, Quaternion.identity);//, Data.instance.mapGenHolderTransform);
            //currCorridor1.GetComponentInChildren<CorridorNew>().rooms.Add(kParentPos);
            //currCorridor1.GetComponentInChildren<CorridorNew>().rooms.Add(lParentPos);

            currCorridor1.transform.SetParent(spawnPointk);

            currCorridor1.transform.GetChild(0).localEulerAngles = new Vector3(0, 0, 90);
            currCorridor1.transform.rotation = Quaternion.Euler(0, yRotation, 0);
            if (yRotation == 0)
            {
                //currCorridor1.GetComponentInChildren<BoxCollider>().enabled = false;
                //currCorridor1.transform.localScale = new Vector3(-1, 1, 1);
                //currCorridor1.GetComponentInChildren<BoxCollider>().enabled = true;
                //currCorridor1.transform.rotation = Quaternion.Euler(0, 90, 0);
            }
        }        
        else if (From.z == to.z)
        {
            List<int> openings = new List<int>();

            //Add opening according to the door type wuth the help of Data.instance.nearDoorL
            //openings.Add(Data.instance.NeardoorLIndexSearch(kName[4].ToString() + kName[5].ToString()));

            //storedOpening is for the next L corridor in line
            storedOpening = (From.x > to.x) ? 1 : 3;

            //Add the opposite of storedOpening since this one will be facing the next L corridor
            openings.Add(storedOpening == 1 ? 3 : 1);

            //Debug.Log(Data.instance.ConvertToRotation(openings) + " " + ((From.x > to.x) ? 3 : 1) + " " + (kName[4].ToString() + kName[5].ToString())
            //+ " " + spawnPoints[k].transform.position + " " + spawnPoints[l].transform.position + " "
            //+ kName + " " + lName);

            float yRotation = Data.instance.ConvertToRotation(openings);

            GameObject currCorridor1 = Instantiate(corridors[8], spawnNowAt, Quaternion.identity);//, Data.instance.mapGenHolderTransform);
            //currCorridor1.GetComponentInChildren<CorridorNew>().rooms.Add(kParentPos);
            //currCorridor1.GetComponentInChildren<CorridorNew>().rooms.Add(lParentPos);

            currCorridor1.transform.SetParent(spawnPointk);

            currCorridor1.transform.GetChild(0).localEulerAngles = new Vector3(0, 0, 90);
            currCorridor1.transform.rotation = Quaternion.Euler(0, yRotation, 0);

            if (yRotation == 0)
            {
                //currCorridor1.GetComponentInChildren<BoxCollider>().enabled = false;
                //currCorridor1.transform.localScale = new Vector3(-1, 1, 1);
                //currCorridor1.GetComponentInChildren<BoxCollider>().enabled = true;
                //currCorridor1.transform.rotation = Quaternion.Euler(0, 90, 0);
            }
        }
    }

    // ---------------------- Spawns I corridors from "Vector3 From", to "Vector3 to" except start and finish (where L corridor is needed)----------------------
    private void spawnHalf(Vector3 From, Vector3 to, bool isFirst)//, string kName, string lName, Vector3 kParentPos, Vector3 lParentPos)
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

            //Instantiate L corridor in correct rotation at the join of two straight corridors (which are in L shape)
            if(!isFirst)
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
                //currCorridor1.GetComponentInChildren<CorridorNew>().rooms.Add(kParentPos);
                //currCorridor1.GetComponentInChildren<CorridorNew>().rooms.Add(lParentPos);
                currCorridor1.transform.rotation = Quaternion.Euler(0, yRotation, 0);
                if (yRotation == 0)
                {
                    //currCorridor1.GetComponentInChildren<BoxCollider>().enabled = false;
                    //currCorridor1.transform.localScale = new Vector3(-1, 1, 1);
                    //currCorridor1.GetComponentInChildren<BoxCollider>().enabled = true;
                    //currCorridor1.transform.rotation = Quaternion.Euler(0, 90, 0);
                }
            }
            spawnNowAt.z += increment;

            //Spawn I corridors
            for (; i < Mathf.Abs(From.z - to.z) / Data.instance.corridorSize + 1 - 1; i++)
            {
                ////Debug.Log("Loop 1 = " + i);
                GameObject currentCorridor = Instantiate(corridorToSpawn, spawnNowAt/*new Vector3(spawnNowAt.x + 0.15f/*- 0.25f, spawnNowAt.y, spawnNowAt.z)*/, Quaternion.identity, Data.instance.mapGenHolderTransform);
                /*
                //Move CollisionDetector of corridor I by +0.25f in x axis to keep it in grid
                Transform collisionDetectorTransform = currentCorridor.transform.GetChild(1);
                collisionDetectorTransform.position = new Vector3(collisionDetectorTransform.position.x + 0.25f, collisionDetectorTransform.position.y, collisionDetectorTransform.position.z);
                */
                //currentCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(kParentPos);
                //currentCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(lParentPos);
                /*
                Data.instance.corridorCount++;
                if (Data.instance.isCollided)
                {
                    Data.instance.isCollided = false;
                    //check current corridor and rotation. check the already instantiated once type AND rotation (using other) ?????//check later

                }
                */
                //currentCorridor.transform.GetChild(0).localPosition = new Vector3(0, 0, -0.08f);
                spawnNowAt.z += increment;
            }
        }

        // -------------- Spawns corridors along x axis since z coord is constant --------------                
        else if (From.z == to.z)
        {
            int increment = (From.x > to.x) ? -4 : 4;

            // ----------- Skips required corridors ----------- 
            int i = 1;

            //Instantiate L corridor in correct rotation at the join of two straight corridors (which are in L shape)
            if(!isFirst)
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
                //currCorridor1.GetComponentInChildren<CorridorNew>().rooms.Add(kParentPos);
                //currCorridor1.GetComponentInChildren<CorridorNew>().rooms.Add(lParentPos);
                currCorridor1.transform.rotation = Quaternion.Euler(0, yRotation, 0);

                if (yRotation == 0)
                {
                    //currCorridor1.GetComponentInChildren<BoxCollider>().enabled = false;
                    //currCorridor1.transform.localScale = new Vector3(-1, 1, 1);
                    //currCorridor1.GetComponentInChildren<BoxCollider>().enabled = true;
                    //currCorridor1.transform.rotation = Quaternion.Euler(0, 90, 0);
                }

            }
            spawnNowAt.x += increment;

            //Debug.Log(From.z + " " + to.z + "!!!!!");
            //Spawn I corridors
            for (; i < Mathf.Abs(From.x - to.x) / Data.instance.corridorSize + 1 - 1; i++)
            {
                ////Debug.Log("Loop 2 = " + i);
                GameObject currentCorridor = Instantiate(corridorToSpawn, spawnNowAt/*new Vector3(spawnNowAt.x + 0.4f/*0.25f, spawnNowAt.y, spawnNowAt.z)*/, Quaternion.identity, Data.instance.mapGenHolderTransform);
                /*
                //Move CollisionDetector of corridor I by -0.25f in x axis to keep it in grid
                Transform collisionDetectorTransform = currentCorridor.transform.GetChild(1);
                collisionDetectorTransform.position = new Vector3(collisionDetectorTransform.position.x - 0.25f, collisionDetectorTransform.position.y, collisionDetectorTransform.position.z);
                */
                //currentCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(kParentPos);
                //currentCorridor.GetComponentInChildren<CorridorNew>().rooms.Add(lParentPos);
                currentCorridor.transform.rotation = Quaternion.Euler(0, 90, 0);
                //Data.instance.corridorCount++;

                //currentCorridor.transform.GetChild(0).localPosition = new Vector3(0, 0, 0.226f);

                spawnNowAt.x += increment;
            }
        }
    }

    //private int ChooseLCorridor(float yRotation)
    //{
    //    return (yRotation == 0 || yRotation == 180) ? 2 : ((yRotation == 90) ? 7 : 1);
    //}

    // ---------------------- Checks if spawnPoints k and i belong to the same room or adjacent rooms ----------------------
    private bool checkIfSameOrAdjacentRoom(int k, int i)
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
        if (Mathf.Abs(spawnPoints[k].transform.position.x - spawnPoints[i].transform.position.x) == Data.instance.xSize / 2)
        {
            return true;
        }

        return false;
    }

    //public int Compare(GameObject x, GameObject y)
    //{
    //    return (int)(Vector3.Distance(x.transform.position, Data.instance.spawnPointsFirstPos)
    //                - Vector3.Distance(y.transform.position, Data.instance.spawnPointsFirstPos));
    //}


    private void CheckDuplicatesAndConnect(List<Vector3> rooms)
    {
        for (int i = 0; i < Data.instance.connectedRoomsVents.Count; i++)
        {
            for (int j = 0; j < Data.instance.connectedRoomsVents[i].Count; j++)
            {
                if (rooms[0] == Data.instance.connectedRoomsVents[i][j])
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
                for (int k = 0; k < Data.instance.connectedRoomsVents.Count; k++)
                {
                    for (int q = 0; q < Data.instance.connectedRoomsVents[k].Count; q++)
                    {
                        // -------------- Now we are taking an element of connectedRoomsThroughCollision (collidedConnectedRoomToSearch)  -------------- 
                        // -------------- and comparing it with every element of connectedRooms -------------- 
                        // -------------- and adding the req ones to connectedRoomsThroughCollision --------------
                        // -------------- and removing the same ones from connectedRooms --------------
                        if (collidedConnectedRoomToSearch == Data.instance.connectedRoomsVents[k][q])
                        {
                            Data.instance.connectedRoomsThroughCollision.Add(new ConnectedComponent(/*Data.instance.connectedRoomsThroughCollision[i].corridorPos*/ new Vector3(1, 1, 1), Data.instance.connectedRoomsVents[k]));
                            Data.instance.connectedRoomsVents.RemoveAt(k);
                            k--;
                            break;
                        }
                    }
                }
            }
        }
    }
    /*
    private void MakeInitHallways()
    {
        ConnectTwoRooms(new Vector3(-16, 0, 0), new Vector3(4, 0, 0), "Door+x", "Door-x", new Vector3(-16 - 24, 0, 0), new Vector3(4 + 24, 0, 0), true);
        ConnectTwoRooms(new Vector3(-4, 0, 0), new Vector3(-4, 0, -8), "Door-z", "Door+z", new Vector3(-4, 0, -24), new Vector3(-4, 0, -8 + 24), true);
    }
    */
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
