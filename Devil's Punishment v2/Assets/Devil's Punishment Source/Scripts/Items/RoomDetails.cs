//Author: Dave Bird
//Date: Thursday, May 30, 2019
    //Last Edited: Thursday, May 30, 2019
        //By: Dave Bird
            //Purpose: Write the script
//Written For: Devil's Punishment v2
//Purpose: This script is placed as a "global" script upon each room type prefab that deals only with that room. It talks to the "LootDropper C# script.
//Notes: Further information can be found at http://www.nolocationyet.com (Dead link)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDetails : MonoBehaviour
{
    public LootDropper lootScript;
    public string roomType;
    public int spawnLocations; 
    
    // Start is called before the first frame update
    void Awake()
    {
        //lootScript.all.Add("spawnLocations");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
