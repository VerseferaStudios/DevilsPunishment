using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNew : MonoBehaviour
{
    //private List<Transform> spawnPoints = new List<Transform>();
    private List<GameObject> spawnPoints = new List<GameObject>();
    public GameObject[] corridors;
    private Transform corridorsParent;
    //private MapGen3 mapGen3;
    private float nextTime = 0f;
    private bool breakLoop = false;
    private List<Vector3> visitedRooms = new List<Vector3>();
    private Vector3 spawnNowAt;

    void Start()
    {
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
        Debug.Log("spawnPoints.Count = " + spawnPoints.Count);


        // ------------------- Find exactly overlapping doors/spawnPoints, spawn a corridor at thst position, and destroy both doors/spawnPoints -------------------
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            bool isFound = false;
            int lastIdx = i;
            for (int j = 0; j < spawnPoints.Count; j++)
            {
                if(i == j)
                {
                    continue;
                }
                //Debug.Log("i = " + i + " & j = " + j);
                if(spawnPoints[i].transform.position == spawnPoints[j].transform.position)
                {
                    isFound = true;
                    lastIdx = j;
                    break;
                }
            }
            if (isFound)
            {
                GameObject currentCorridor = Instantiate(corridors[1]/* L_1 */, spawnPoints[i].transform.position, Quaternion.identity);
                Debug.Log("Spawn1");
                Data.instance.corridorCount++;



                //Debug.Log(spawnPoints[i].transform.position + "______________________________________________________");
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

                isFound = false;
            }
        }

        for (int k = 0; k < spawnPoints.Count; k++)//or k+=2 does it matter?
        {

            // ------------------- Remove door/spawnPoint if its of the same room -------------------
            if (visitedRooms.Contains(spawnPoints[k].transform.parent.transform.position))
            {
                //Debug.Log("Removed a door of ____ " + spawnPoints[k].transform.parent.transform.position);
                spawnPoints.RemoveAt(k);
                k--;
                continue;
            }

            Vector3 targetPos = new Vector3(0, 3, 0);

            bool isKx, isIx;

            for (int i = 0; i < spawnPoints.Count; i++) //i = 0 makes no diff; some rooms are getting overlooked, y //EXPT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            {

                if (visitedRooms.Contains(spawnPoints[i].transform.parent.transform.position))
                {
                    //Debug.Log("Removed a door of ____ " + spawnPoints[i].transform.parent.transform.position);
                    spawnPoints.RemoveAt(i);
                    k--;
                    break;
                }

                if (k == i)
                {
                    continue;
                }

                // ------------------------ if k and i are not in the same room ------------------------
                if (!checkIfSameOrAdjacentRoom(k, i)) 
                {

                    Vector3 From = spawnPoints[k].transform.position;
                    GameObject lastCorridorType;

                    // ------------------- Connects x and z doors with L shape with no hindrance -------------------
                    if (spawnPoints[k].name.EndsWith("x") && spawnPoints[i].name.EndsWith("z"))
                    {
                        targetPos = new Vector3(From.x, 0.5f, spawnPoints[i].transform.position.z);
                        Debug.Log("Spawn2");
                        //lastCorridorType = 
                    }

                    // ------------------- Connects z and x doors with L shape with no hindrance -------------------
                    else if (spawnPoints[k].name.EndsWith("z") && spawnPoints[i].name.EndsWith("x"))
                    {
                        targetPos = new Vector3(spawnPoints[i].transform.position.x, 0.5f, From.z);
                        Debug.Log("Spawn3");
                    }

                    // ------------------- Doesnt (xD) Connects (x and x) or (z and z) doors in different rooms with I shape with no hindrance -------------------
                    else if (
                        // -------------- if x doors AND z differnce == 10 --------------
                        (spawnPoints[k].transform.position.x == spawnPoints[i].transform.position.x && Mathf.Abs(spawnPoints[k].transform.position.z - spawnPoints[i].transform.position.z) == 10)
                        ||
                        // -------------- if z doors AND x differnce == 10 --------------
                        (spawnPoints[k].transform.position.z == spawnPoints[i].transform.position.z && Mathf.Abs(spawnPoints[k].transform.position.x - spawnPoints[i].transform.position.x) == 10)
                        )
                    {
                        //targetPos = spawnPoints[i].transform.position;
                        Debug.Log("Spawn4--");
                    }

                    // ------------------- Connects x and x doors with `L shape to avoid hindrance -------------------
                    else if (spawnPoints[k].name.EndsWith("x") && spawnPoints[i].name.EndsWith("x"))
                    {
                        //check and go nearer to destination
                        Vector3 to = spawnPoints[k].transform.position;
                        to.z += 5f;     //Check 5 or 6 !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                        // ------------------- Calls the actual spawning function -------------------
                        spawnHalf(spawnPoints[k].transform.position, to, false);
                        From = to;
                        targetPos = new Vector3(spawnPoints[i].transform.position.x, 0.5f, From.z);
                        Debug.Log("Spawn5");
                        /*
                        Debug.Log("From = " + From);
                        Debug.Log(targetPos);
                        Debug.Log(spawnPoints[i].transform.position);
                        */
                    }

                    // ------------------- Connects z and z doors with `L shape to avoid hindrance -------------------
                    else if (spawnPoints[k].name.EndsWith("z") && spawnPoints[i].name.EndsWith("z"))
                    {
                        //check and go nearer to destination
                        Vector3 to = spawnPoints[k].transform.position;
                        to.x += 5f;     //Check 5 or 6 !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        
                        // ------------------- Calls the actual spawning function -------------------
                        spawnHalf(spawnPoints[k].transform.position, to, false);
                        From = to;
                        targetPos = new Vector3(From.x, 0.5f, spawnPoints[i].transform.position.z);
                        Debug.Log("Spawn6");
                        /*
                        Debug.Log("From = " + From);
                        Debug.Log(targetPos);
                        Debug.Log(spawnPoints[i].transform.position);
                        */
                    }


                    // ------------------- Calls the actual spawning function -------------------
                    spawnHalf(From, targetPos, false); //false
                    spawnHalf(targetPos, spawnPoints[i].transform.position, true);

                    // ------------------- Added parents position to List<Vector3> to avoid future doors of the room -------------------
                    visitedRooms.Add(spawnPoints[i].transform.parent.transform.position);
                    visitedRooms.Add(spawnPoints[k].transform.parent.transform.position);
                    //Debug.Log("Added a door of ____ " + spawnPoints[i].transform.parent.transform.position);
                    //Debug.Log("Added a door of ____ " + spawnPoints[k].transform.parent.transform.position);

                    // ---------------------- Removes the used doors ----------------------
                    spawnPoints.RemoveAt(i);
                    
                    // -------------- decrease k if greater than i --------------                
                    if (k > i)
                    {
                        k--;
                    }
                    i--;

                    spawnPoints.RemoveAt(k); 
                    
                    // -------------- decrease k if greater than i --------------                
                    if (i > k)
                    {
                        i--;
                    }
                    k--;

                    break;
                }
            }


            // ------------------- In case we missed a room -------------------
            if (k >= spawnPoints.Count && spawnPoints.Count != 0)
            {
                k = 0; 
            }

        }
        //corridorsParent = (GameObject.Find("Corridors") as GameObject).transform;
        Debug.Log(Data.instance.corridorCount + "corridor count!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
    }

    // ---------------------- Spawns corridors from "Vector3 From", to "Vector3 to" ----------------------
    private void spawnHalf(Vector3 From, Vector3 to, bool skipFirst)
    {
        // ----------- Variable for position to spawn at each for loop step -----------                
        spawnNowAt = From;
        // ----------- Variable for corridor to spawn at each for loop step -----------                
        GameObject corridorToSpawn = corridors[0];

        // -------------- Spawns corridors along z axis since x coord is constant --------------                
        if (From.x == to.x)
        {
            int increment = (From.z > to.z) ? -1 : 1;
            //if()
            int i = 1;
            // ----------- Currently skips last corridor ----------- 
            // ----------- Add "spawnNowAt.z += increment;" in if condition to skipFirst----------- 
            if (skipFirst)
            {
                skipFirst = false;
                i = 1;
            }
            for (; i < Mathf.Abs(From.z - to.z) + 1; i++)
            {
                //Debug.Log("Loop 1 = " + i);
                GameObject currentCorridor = Instantiate(corridorToSpawn, spawnNowAt, Quaternion.identity);
                currentCorridor.transform.rotation = Quaternion.Euler(0, 90, 0);
                Data.instance.corridorCount++;
                if (Data.instance.isCollided)
                {
                    Data.instance.isCollided = false;
                    //check current corridor and rotation. check the already instantiated once type AND rotation (using other) ?????//check later

                }
                spawnNowAt.z += increment;
            }
        }

        // -------------- Spawns corridors along x axis since z coord is constant --------------                
        else if (From.z == to.z)
        {
            int increment = (From.x > to.x) ? -1 : 1;
            int i = 0;
            // ----------- Currently skips last corridor ----------- 
            // ----------- Add "spawnNowAt.z += increment;" in if condition to skipFirst----------- 
            if (skipFirst)
            {
                skipFirst = false;
                i = 1;
            }
            for (; i < Mathf.Abs(From.x - to.x) + 1; i++)
            {
                //Debug.Log("Loop 2 = " + i);
                GameObject currentCorridor = Instantiate(corridorToSpawn, spawnNowAt, Quaternion.identity);
                Data.instance.corridorCount++;



                spawnNowAt.x += increment;
            }
        }
    }

    // ---------------------- Checks if spawnPoints k and i belong to the same room or adjacent rooms ----------------------
    private bool checkIfSameOrAdjacentRoom(int k, int i)
    {
        bool isDoorTypeX = spawnPoints[k].name.EndsWith("x") ? true : false ;

        // ------------- Check x axis ------------- 
        if (Mathf.Abs(spawnPoints[k].transform.position.x - spawnPoints[i].transform.position.x) == 10)
        {
            return true;
        }
        // ------------- Check z axis ------------- 
        else if (Mathf.Abs(spawnPoints[k].transform.position.z - spawnPoints[i].transform.position.z) == 10)
        {
            return true;
        }

        // ------------- Check between x door and z door (in L shape) ------------- 
        if (Mathf.Abs(spawnPoints[k].transform.position.x - spawnPoints[i].transform.position.x) == 5f)
        {
            return true;
        }

            return false;
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
            //Debug.Log(((int[])Data.instance.allRooms[i])[1]);
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


}


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
