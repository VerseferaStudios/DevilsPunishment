using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNew3 : MonoBehaviour
{
    private List<GameObject> spawnPoints = new List<GameObject>();
    public GameObject corridor;
    private Transform corridorsParent;
    private MapGen3 mapGen3;
    private float nextTime = 0f;
    private bool breakLoop = false;

    // Start is called before the first frame update
    void Start()
    {
        mapGen3 = GameObject.FindGameObjectWithTag("Rooms(MapGen)").GetComponent<MapGen3>();


        GameObject[] tempSpawnPoints = GameObject.FindGameObjectsWithTag("Corridor Spawn Points");
        string f = tempSpawnPoints[0].GetComponentsInChildren<Transform>()[0].gameObject.name;
        Debug.Log(f);
        spawnPoints.AddRange(tempSpawnPoints);
        Debug.Log(spawnPoints.Count);
        for (int k = 0; k < spawnPoints.Count; k++)//or k+=2 does it matter?
        {

            Vector3 targetPos = new Vector3(0, 3, 0);

            //string chk = checkCollisions(spawnPoints[k].transform.position, spawnPoints[k + 1].transform.position);
            //Debug.Log(chk);
            //if (chk == "xz")
            //{
            //            targetPos = new Vector3(spawnPoints[k].transform.position.x, 0.5f, spawnPoints[k + 1].transform.position.z); 
            //}
            /*
            else if (chk == "zx")
            {
                targetPos = new Vector3(spawnPoints[k + 1].transform.position.x, 0.5f, spawnPoints[k].transform.position.z);
            }
            else
                //Check AGAIN
            */

            bool isKx, isIx;

            for (int i = 0; i < spawnPoints.Count; i++) //i = 0 makes no diff; some rooms are getting overlooked, y //EXPT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            {
                if (k == i)
                {
                    continue;
                }

                if (!checkIfSameRoom(k, i))
                {

                    Vector3 From = spawnPoints[k].transform.position;

                    if (spawnPoints[k].name.EndsWith("x") && spawnPoints[i].name.EndsWith("z"))
                    {
                        isKx = true;
                        targetPos = new Vector3(spawnPoints[k].transform.position.x, 0.5f, spawnPoints[i].transform.position.z);
                    }
                    else if (spawnPoints[k].name.EndsWith("z") && spawnPoints[i].name.EndsWith("x"))
                    {
                        targetPos = new Vector3(spawnPoints[i].transform.position.x, 0.5f, spawnPoints[k].transform.position.z);
                    }
                    else if (spawnPoints[k].name.EndsWith("x") && spawnPoints[i].name.EndsWith("x"))
                    {
                        //check and go nearer to destination
                        Vector3 to = spawnPoints[k].transform.position;
                        to.z += 5.5f;
                        spawnHalf(spawnPoints[k].transform.position, to, false);
                        From = to;
                        targetPos = new Vector3(spawnPoints[i].transform.position.x, 0.5f, From.z);
                        Debug.Log("From = " + From);
                        Debug.Log(targetPos);
                        Debug.Log(spawnPoints[i].transform.position);
                    }
                    else if (spawnPoints[k].name.EndsWith("z") && spawnPoints[i].name.EndsWith("z"))
                    {
                        //check and go nearer to destination
                        Vector3 to = spawnPoints[k].transform.position;
                        to.x += 5.5f;
                        spawnHalf(spawnPoints[k].transform.position, to, false);
                        From = to;
                        targetPos = new Vector3(From.x, 0.5f, spawnPoints[i].transform.position.z);
                        Debug.Log("From = " + From);
                        Debug.Log(targetPos);
                        Debug.Log(spawnPoints[i].transform.position);
                    }


                    //Debug.Log("1___________");
                    //Debug.Log(From);
                    //Debug.Log(spawnPoints[i].transform.position);
                    spawnHalf(From, targetPos, false); //false
                    spawnHalf(targetPos, spawnPoints[i].transform.position, true);
                    spawnPoints.RemoveAt(i);
                    spawnPoints.RemoveAt(k); //EXPT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                                             //i-=2; //EXPT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                                             //k -=2; //EXPT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                    if (k < i)
                    {
                        k--;
                    }
                    else
                    {
                        k -= 2;
                    }
                    break;
                }
                else if (breakLoop)
                {
                    breakLoop = false;
                    break;  //or continue?   //EXPT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                }
            }





            //Debug.Log(targetPos);

            if (k >= spawnPoints.Count && spawnPoints.Count != 0)
            {
                k = 0; // makes it worse y????????????????????????//////
            }

        }
        //corridorsParent = (GameObject.Find("Corridors") as GameObject).transform;
    }

    private void spawnHalf(Vector3 From, Vector3 to, bool skipFirst)
    {
        //Debug.Log(From + " " + to);
        //Debug.Log("In Function");
        Vector3 spawnNowAt = From;
        if (From.x == to.x)
        {
            int increment = (From.z > to.z) ? -1 : 1;
            for (int i = 0; i < Mathf.Abs(From.z - to.z) + 1; i++)
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
        else if (From.z == to.z)
        {
            int increment = (From.x > to.x) ? -1 : 1;
            for (int i = 0; i < Mathf.Abs(From.x - to.x) + 1; i++)
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

    private bool checkIfSameRoom(int k, int i)
    {
        bool isDoorTypeX = spawnPoints[k].name.EndsWith("x") ? true : false;

        if (Mathf.Abs(spawnPoints[k].transform.position.x - spawnPoints[i].transform.position.x) == 11)
        {
            if (!isDoorTypeX)
            {
                //Debug.Log("2___________");
                //Debug.Log(spawnPoints[k].transform.position);
                //Debug.Log(spawnPoints[i].transform.position);
                spawnHalf(spawnPoints[k].transform.position, spawnPoints[i].transform.position, false);
                breakLoop = true;
            }
            return true;
        }
        else if (Mathf.Abs(spawnPoints[k].transform.position.z - spawnPoints[i].transform.position.z) == 11)
        {
            if (isDoorTypeX)
            {
                //Debug.Log("3___________");
                //Debug.Log(spawnPoints[k].transform.position);
                //Debug.Log(spawnPoints[i].transform.position);
                spawnHalf(spawnPoints[k].transform.position, spawnPoints[i].transform.position, false);
                breakLoop = true;
            }
            return true;
        }
        if (Mathf.Abs(spawnPoints[k].transform.position.x - spawnPoints[i].transform.position.x) == 5.5f)
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
