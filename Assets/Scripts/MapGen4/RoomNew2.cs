using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNew2 : MonoBehaviour
{
    private GameObject[] spawnPoints;
    public GameObject corridor;
    private Transform corridorsParent;
    private MapGen3 mapGen3;
    private float nextTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        mapGen3 = GameObject.FindGameObjectWithTag("Rooms(MapGen)").GetComponent<MapGen3>();


        spawnPoints = GameObject.FindGameObjectsWithTag("Corridor Spawn Points");
        for (int k = 0; k + 1 < spawnPoints.Length; k += 2)
        {

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


            Vector3 targetPos = new Vector3(0, 3, 0);

            string chk = checkCollisions(spawnPoints[k].transform.position, spawnPoints[k + 1].transform.position);
            Debug.Log(chk);
            if (chk == "xz")
            {
                targetPos = new Vector3(spawnPoints[k].transform.position.x, 0.5f, spawnPoints[k + 1].transform.position.z);
            }
            else if (chk == "zx")
            {
                targetPos = new Vector3(spawnPoints[k + 1].transform.position.x, 0.5f, spawnPoints[k].transform.position.z);
            }
            else
                //Check AGAIN

            spawnHalf(spawnPoints[k].transform.position, targetPos, false); //false
            spawnHalf(targetPos, spawnPoints[k + 1].transform.position, true);


            Debug.Log(targetPos);
            
        }
        //corridorsParent = (GameObject.Find("Corridors") as GameObject).transform;
    }

    private void spawnHalf(Vector3 from, Vector3 to, bool skipFirst)
    {
        Debug.Log(from + " " + to);
        //Debug.Log("In Function");
        Vector3 spawnNowAt = from;
        if(from.x == to.x)
        {
            int increment = (from.z > to.z) ? -1 : 1;
            for (int i = 0; i < Mathf.Abs(from.z - to.z) + 1; i++)
            {
                //Debug.Log("Loop 1 = " + i);
                if (skipFirst)
                {
                    skipFirst = false;
                    continue;
                }
                Instantiate(corridor, spawnNowAt, Quaternion.identity);
                spawnNowAt.z += increment;
            }
        }
        else if(from.z == to.z)
        {
            int increment = (from.x > to.x) ? -1 : 1;
            for (int i = 0; i < Mathf.Abs(from.x - to.x) + 1; i++)
            {
                //Debug.Log("Loop 2 = " + i);
                if (skipFirst)
                {
                    skipFirst = false;
                    continue;
                }
                Instantiate(corridor, spawnNowAt, Quaternion.identity);
                spawnNowAt.x += increment;
            }
        }
    }

    private string checkCollisions(Vector3 from, Vector3 to)
    {
        int ctr = 0;
        bool goToNext = false;
        Vector3 fromTemp = from;
        Vector3 targetPos = new Vector3(fromTemp.x, 0.5f, to.z);
        //from to targetPos, x constant
        for (int i = 0; i < Data.instance.allRooms.Count; i++)
        {
            Debug.Log(((int[])Data.instance.allRooms[i])[1]);
            if(Mathf.Abs(-((int[])Data.instance.allRooms[i])[1] - fromTemp.x) != Data.instance.xSize/2 + 1 && Mathf.Abs(-((int[])Data.instance.allRooms[i])[1] - fromTemp.x) < Data.instance.xSize)
            {
                ctr++;
                goToNext = true;
                break;
            }
        }
        if (goToNext)
        {
            //targetPos to to, z constant
            fromTemp = targetPos;
            targetPos = to;
            int i = 0;
            for (i = 0; i < Data.instance.allRooms.Count; i++)
            {
                if (Mathf.Abs(-((int[])Data.instance.allRooms[i])[0] - fromTemp.z) != Data.instance.zSize/2 + 1 && Mathf.Abs(-((int[])Data.instance.allRooms[i])[0] - fromTemp.z) < Data.instance.zSize)
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
            targetPos = new Vector3(to.x, 0.5f, fromTemp.z);
            fromTemp = from;
            //z const
            for (int i = 0; i < Data.instance.allRooms.Count; i++)
            {
                if (Mathf.Abs(-((int[])Data.instance.allRooms[i])[0] - fromTemp.z) != Data.instance.zSize/2 + 1 && Mathf.Abs(-((int[])Data.instance.allRooms[i])[0] - fromTemp.z) < Data.instance.zSize)
                {
                    goToNext = true;
                    break;
                }
            }
        }
        if (goToNext)
        {
            //x const
            fromTemp = targetPos;
            targetPos = to;
            int i = 0;
            for (i = 0; i < Data.instance.allRooms.Count; i++)
            {
                if (Mathf.Abs(-((int[])Data.instance.allRooms[i])[1] - fromTemp.x) != Data.instance.xSize/2 + 1 && Mathf.Abs(-((int[])Data.instance.allRooms[i])[1] - fromTemp.x) < Data.instance.xSize)
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

    // Update is called once per frame
    void Update()
    {
        
    }


}


/*
 * //Colour scheme
            if (i == 2)
            {
                spawnPoints[i].GetComponent<Renderer>().material.color = Color.green;
                spawnPoints[i + 1].GetComponent<Renderer>().material.color = Color.green;
            }
            else if (i == 4)
            {
                spawnPoints[i].GetComponent<Renderer>().material.color = Color.red;
                spawnPoints[i + 1].GetComponent<Renderer>().material.color = Color.red;
            }
            else if (i == 6)
            {
                spawnPoints[i].GetComponent<Renderer>().material.color = Color.white;
                spawnPoints[i + 1].GetComponent<Renderer>().material.color = Color.white;
            }
            else if (i == 8)
            {
                spawnPoints[i].GetComponent<Renderer>().material.color = Color.yellow;
                spawnPoints[i + 1].GetComponent<Renderer>().material.color = Color.yellow;
            }
            else if (i == 10)
            {
                spawnPoints[i].GetComponent<Renderer>().material.color = Color.cyan;
                spawnPoints[i + 1].GetComponent<Renderer>().material.color = Color.cyan;
            }
            */
