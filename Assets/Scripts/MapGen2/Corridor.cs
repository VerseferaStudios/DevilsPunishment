using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corridor : Room
{
    private int countLeftToSpawn;
    private int corridorType;
    public GameObject corridor1;
    //public GameObject corridor;

    private void Start()
    {
        //countLeftToSpawn = (int)transform.position.x - ((int[])allRooms[0])[1];
        //Debug.Log(countLeftToSpawn);
        if(transform.position == GameObject.Find("Target").transform.position)
        {
            Destroy(gameObject);
        }
        spawnOne();
    }

    public void spawnOne()
    {
        //if(countLeftToSpawn > 0)
        //{
            /*
            Debug.Log("ijn");
            Corridor ccc = corridor.GetComponentInChildren<Corridor>();
            ccc.corridor = corridor;
            ccc.countLeftToSpawn = countLeftToSpawn - 1;
            */
            Instantiate(transform.parent, transform.position - new Vector3(0, 0.5f, 0), Quaternion.identity);
        //}
    }

    public void replaceOtherCorridor(Corridor other)
    {
        int newCorridoorType = 0;
        Vector3 targetPos = other.transform.position;
        Destroy(other.gameObject);
        //get new corridor using a fn or sth taking argument "corridorType"
        GameObject newCorridor = null;
        Instantiate(newCorridor, targetPos, Quaternion.identity);
    }
    
}
