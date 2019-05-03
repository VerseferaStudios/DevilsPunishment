using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNew : MonoBehaviour
{
    private GameObject[] spawnPoints;
    public GameObject corridor;
    private Transform corridorsParent;

    // Start is called before the first frame update
    void Start()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("Corridor Spawn Points");
        for (int i = 0; i + 1 < spawnPoints.Length; i += 2)
        {
            Vector3 targetPos = new Vector3(spawnPoints[i].transform.position.x, 0.5f, spawnPoints[i + 1].transform.position.z);
            spawn(spawnPoints[i].transform.position, targetPos, true); //false
            spawn(targetPos, spawnPoints[i + 1].transform.position, true);
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
        }
        corridorsParent = (GameObject.Find("Corridors") as GameObject).transform;
    }

    private void spawn(Vector3 from, Vector3 to, bool skipFirst)
    {
        Debug.Log("In Function");
        Vector3 spawnNowAt = from;
        if(from.x == to.x)
        {
            int increment = (from.z > to.z) ? -1 : 1;
            for (int i = 0; i < Mathf.Abs(from.z - to.z) + 1; i++)
            {
                Debug.Log("Loop 1 = " + i);
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
                Debug.Log("Loop 2 = " + i);
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
