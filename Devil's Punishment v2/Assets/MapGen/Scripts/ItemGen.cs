//What this script is SUPPOSED to do is the following...
//-Map Generator spawns in 1 of each of the static rooms (forced) and a "x" amount of the non-static rooms and of them non-static rooms there could be none of a certain type of room or it could be nothing BUT that room type (ie Dormatory, Hospital etc.)
//-As the map generator room spawns in a room it tells this script that it spawned in what ever that room was 
//-There are only certain rooms that have loot spawn locations, and of them rooms they have a range of 1-8 spawn spots depending on the room.
//-As the map generator spawns in a room it will tell my script ok there is a armory. for every armory there are 8 spawn spots so it multiplies the number of the rooms of that type by the number of spawn spots. Once the map generator is done spawning in rooms it will then take all the answers for rooms*spawn spots and add them together (32 spawns in armory, 10 in hospital, 5 in cave... etc.) 
//-What ever that spawn is now the number of POSSIBLE locations for loot to spawn. It takes that number and creates the list to be that long. (ie if its 55 loot spots big the list will be 55 spaces big)
//-Once it has the loot spots figured out it then is supposed to randomly choose 3 locations that are below the transform location of y <= 8 (because world creator wants all the generator pieces on the first floor) and they have 100% spawn rate that is why they go first (ideally i would like to then remove that loot spot from the spawn list.
//- This will leave all possible spots for the other loot (ie guns, ammo, health, pills, and light upgrade) each one has its own spawn rate.
//-After the generator part I need it to go down the list as its going calculate ok will it spawn loot yes/no (50% chance to spawn in each spot in rooms 20% chance in corridor spots)... yes ok calculate the drop rate and place that item there, then romove the spot from the list.... can it spawn here... no .. ok remove from list and move on to the next one.
//-Once all places are either filled or deleted from the list then it shuts off the script permanantly until the next game.

//My issue is coming into the generator looting, its saying that it cant convert a int to a transform which i know, but how do i get it to move down the list and place it in that transform spot. perhaps i have the wrong set up... any insight would be a great help ty





//Author: David K Bird
//Date: Thursday, May 23, 2019
//Last Edited: Tuesday, June 4, 2019
//By: David Bird
//Purpose: Writing the script
//Written For: Devil's Punishment v2
//Purpose: This script details all things that has to do with the loot generation at the beginning of the game.
//Notes: Further information can be found at http://www.nolocationyet.com (Dead link)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGen : MonoBehaviour
{
    //Define All possible loot locations 
    //Define the possibilty for if its a green light to spawn (50% 20%)
    //Define the Weight of rarity. The lower the number the rarer it is.
    //Define the need of 100% having some items drop (ie generator parts, set bool if it spawns to not spawnable again)
    //Turn off the spawn point once the item spawns.
    //Define if drops a gun it drops ammo as well. 

    [Space]
    [Header("How many?")]
    public int noArmory; //Number of Armory this will get set in the room mapgen script
    public int noBathroom; //Number of Bathrooms this will get set in the room mapgen script
    public int noDorms; //Number of Dorms this will get set in the room mapgen script
    public int noHospital; //Number of Hospitals this will get set in the room mapgen script
    public int noCorridorT; //Number of Corridor T this will get set in the room mapgen script
    public int noCorridorL; //Number of Corridor L this will get set in the room mapgen script
    public int noKitchen; //Number of Kitchens this will get set in the room mapgen script
    public int noRadio; //Number of Radio Rooms this will get set in the room mapgen script
    public int noCave; //Number of Caves this will get set in the room mapgen script
    public int noStartingRoom; //Number of Starting Rooms this will get set in the room mapgen script

    [Space]
    [Header("Loot Locations")]
    public Transform[] locate;
    int randomLoc;
    int locationSet;

    [Space]
    [Header("Lootables")]
    //Setting the items
    public GameObject gen1; //------------
    public GameObject gen2; // 100% spawn rate
    public GameObject gen3; //------------
    [Space]
    public GameObject med; //10% Spawnrate
    public GameObject pills; //5% spawnrate
    public GameObject pAmmo; //30% spawnrate
    public GameObject sAmmo; //15% spawnrate
    public GameObject rAmmo; //10% spawnrate
    public GameObject pistol; //30% spawnrate
    public GameObject shotgun; //15% spawnrate
    public GameObject rifle; //10% spawnrate
    public GameObject flashlight; //30% spawnrate

    [Space]
    [Header("Number of said room * number of spawnspots")]
    public int ArmorySpots = 8;
    public int BathroomSpots = 3;
    public int DormSpots = 3;
    public int HospitalSpots = 5;
    public int CorridorTSpots = 1;
    public int CorridorLSpots = 1;
    public int KitchenSpots = 3;
    public int RadioSpots = 3;
    public int CaveSpots = 1;
    public int StartingRoomSpots = 5;
    [Space]
    int armorySpawns;
    int bathroomSpawns;
    int dormSpawns;
    int hospitalSpawns;
    int corridorTSpawns;
    int corridorLSpawns;
    int kitchenSpawns;
    int radioSpawns;
    int caveSpawns;
    int startingRoomSpawns;

    private List<Vector3> itemPositions = new List<Vector3>();

    public void FindLocations()
    {
        List<Transform> all = new List<Transform>();
        //Finding all possible spots.
        noArmory = 0;
        armorySpawns = noArmory * ArmorySpots;
        noBathroom = 0;
        bathroomSpawns = noBathroom * BathroomSpots;
        noDorms = 0;
        dormSpawns = noDorms * DormSpots;
        noHospital = 0;
        hospitalSpawns = noHospital * HospitalSpots;
        noCorridorT = 0;
        corridorTSpawns = noCorridorT * CorridorTSpots;
        noCorridorL = 0;
        corridorLSpawns = noCorridorL * CorridorLSpots;
        noKitchen = 0;
        kitchenSpawns = noKitchen * KitchenSpots;
        noRadio = 0;
        radioSpawns = noRadio * RadioSpots;
        noCave = 0;
        caveSpawns = noCave * CaveSpots;
        noStartingRoom = 0;
        startingRoomSpawns = noStartingRoom * StartingRoomSpots;
        Debug.Log("Have found the locations!");
        locationSet = armorySpawns + noBathroom + noDorms + noHospital + noCorridorT + noCorridorL + noKitchen + noKitchen + noRadio + noCave + noStartingRoom;

        while (locationSet > 0)
        {
            all.Add(gameObject.transform); // Problem
            locationSet--;
        }
        Debug.Log("Locations are now in the list, ready for spawning!");
        SetSpawns();



        //SpawnItems(new Vector3(0, 0, 0), new Vector3(10, 0, 10), 4);
    }


    // 1. Item overlap - 
    // 2. Inside room - Done
    //Map gen finishes a room

    //Spawn "noOfSpawns" number of items in the given room indicated by topLeftCorner and bottomRightCorner
    public void SpawnItems(Vector3 topLeftCorner, Vector3 bottomRightCorner, int noOfSpawns)
    {

        bool isOverlapping = false;
        float maxSizeOfItem = 3;

        int k = 0;
        for (int i = 0; k < noOfSpawns && i < 1000; i++) //Run for more than noOfSpawns times /*while (k < n && l < 1000)*/
        {
            float randomPositionX = Random.Range(topLeftCorner.x, bottomRightCorner.x);
            float randomPositionZ = Random.Range(topLeftCorner.z, bottomRightCorner.z);

            Vector3 currentItemPos = new Vector3(randomPositionX, 0, randomPositionZ);


            //Get the size of the itemToSpawn

            //Make sure previous items in the same room do not overlap

            isOverlapping = false;
            for (int j = 0; j < itemPositions.Count; j++)
            {
                if(Vector3.Distance(currentItemPos, itemPositions[j]) < maxSizeOfItem)
                {
                    isOverlapping = true;
                }
            }

            if (isOverlapping)
            {
                continue;
            }
            k++;

            // Call a function to spawn the items according to the chance
            GameObject itemToSpawn = SpawnCorrectItemHelper();

            //Get bounds from script on each item
            //float bounds = itemToSpawn.GetComponent<ScriptName>().bounds;
            

            if(itemToSpawn != null)
            {
                GameObject gb = Instantiate(itemToSpawn, currentItemPos, Quaternion.identity);
                gb.GetComponent<Rigidbody>().useGravity = false;
                gb.GetComponent<Rigidbody>().isKinematic = true;
                itemPositions.Add(currentItemPos);
            }


        }



    }

    //Take the chances into account and spawn the right item
    private GameObject SpawnCorrectItemHelper()
    {
        /*   public GameObject gen1; //------------
       public GameObject gen2; // 100% spawn rate
       public GameObject gen3; //------------
       [Space]
       public GameObject med; //10% Spawnrate
       public GameObject pills; //5% spawnrate
       public GameObject pAmmo; //30% spawnrate
       public GameObject sAmmo; //15% spawnrate
       public GameObject rAmmo; //10% spawnrate
       public GameObject pistol; //30% spawnrate
       public GameObject shotgun; //15% spawnrate
       public GameObject rifle; //10% spawnrate
       public GameObject flashlight; //30% spawnrate
       */
        float rand = Random.Range(0f, 1.55f);
        if(rand < .10f)
        {
            return med;
        }
        else if (rand < .15f)
        {
            return pills;
        }
        else if (rand < .45f)
        {
            return pAmmo;
        }
        else if (rand < .60f)
        {
            return sAmmo;
        }
        else if (rand < .70f)
        {
            return rAmmo;
        }
        else if (rand < 1.00f)
        {
            return pistol;
        }
        else if (rand < 1.15f)
        {
            return shotgun;
        }
        else if (rand < 1.25f)
        {
            return rifle;
        }
        else
        {
            return flashlight;
        }
    }

    public void SetSpawns()
    {
        //Generator parts need to spawn first must be on 1st floor.
        //randomLoc = Random.Range(0, locate.Length);
        /*
        if (locate[].position.y <= 8)
        {

        }
        */
    }
    //public void Awake()
    //{
    //    //Generator Parts are spawning, must be on 1st floor set parameter to be between 1 and ? on the y axis...
    //    randomLoc = Random.Range(0, locate.Length);
    //    //if (randomLoc.position.y <=8 randomLoc.Length)
    //    //    {
    //    //    Instantiate(gen1, locate[randomLoc].position, locate[randomLoc].rotation);
    //    //    //Delete or deactivate the spawn location
    //    //    }

    //    randomLoc = Random.Range(0, locate.Length);
    //    Instantiate(gen2, locate[randomLoc].position, locate[randomLoc].rotation);
    //    //Delete or deactivate the spawn location
    //    randomLoc = Random.Range(0, locate.Length);
    //    Instantiate(gen3, locate[randomLoc].position, locate[randomLoc].rotation);
    //    //Delete or deactivate the spawn location
    //    Debug.Log("All generator pieces have been placed!");
    //    Debug.Log("Spawning all other items now!");

    //    //Now going to spawn in the rest of the loot, once the loot is spawned in the spawn location is dead

    //}
}