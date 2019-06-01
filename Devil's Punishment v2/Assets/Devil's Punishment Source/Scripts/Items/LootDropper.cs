using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootDropper : MonoBehaviour
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

    public void Start()
    {
        List <int> all = new List<int>();
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
        all.Add(locationSet);

    }


    public void Awake()
    {
        //Generator Parts are spawning, must be on 1st floor set parameter to be between 1 and ? on the y axis...
        randomLoc = Random.Range(0, locate.Length);
        //if (randomLoc.position.y <=8 randomLoc.Length)
        //    {
        //    Instantiate(gen1, locate[randomLoc].position, locate[randomLoc].rotation);
        //    //Delete or deactivate the spawn location
        //    }

        randomLoc = Random.Range(0, locate.Length);
        Instantiate(gen2, locate[randomLoc].position, locate[randomLoc].rotation);
        //Delete or deactivate the spawn location
        randomLoc = Random.Range(0, locate.Length);
        Instantiate(gen3, locate[randomLoc].position, locate[randomLoc].rotation);
        //Delete or deactivate the spawn location
        Debug.Log("All generator pieces have been placed!");
        Debug.Log("Spawning all other items now!");

        //Now going to spawn in the rest of the loot, once the loot is spawned in the spawn location is dead

    }
}