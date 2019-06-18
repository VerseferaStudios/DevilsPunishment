using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class MapGen3 : MonoBehaviour
{
    //first we'll see the ground floor
    //10 x 10 cube
    //later//private int gridSize;
    [Header("Rooms")]
    private int n = 15;
    public ArrayList allRooms = new ArrayList();
    private ArrayList gameObjectDetails = new ArrayList();

    public GameObject mainRoomIndicator, generatorRoom, startRoom, endRoom;
    
    private float xSize = 48f, zSize = 48f;

    public GameObject[] corridors;

    //For Vents
    [Header("Vents")]
    public float ventCoverProbabilty;
    public GameObject ventCover;

    public ItemGen itemGen;
    private List<Vector3> itemPositions;

    private void Start()
    {
        ventCoverProbabilty = 6f / n;
        rooms();
        Data.instance.corridorT1 = corridors[3];
        Data.instance.corridorT2 = corridors[4];
        Data.instance.corridorX = corridors[5];
        Data.instance.xSize = xSize;
        Data.instance.zSize = zSize;
    }

    public void rooms()
    {
        
        //Make this while and next loop into one? will Collisions be a prob?
        int k = 0, l = 0;
        while (k < n && l < 1000)
        {
            float[] arr = new float[2];

            // ------------------- BOUNDS or SIZE of the grid -------------------



            // 400 - 20 = 380 (MAX)
            // 0 + 20 = 20 (MIN)
            //Increments of 40

            //arr[0] = 40 * Random.Range(0, 9) + 20;  //9 coz -> 9 * 40 + 20 = 380
            //arr[1] = 40 * Random.Range(0, 9) + 20;


            // 480 - 20 = 460 (MAX)
            // 0 + 28 = 28 (MIN)
            //Increments of 40

            arr[0] = 48 * Random.Range(0, 9) + 28;  //9 coz -> 9 * 48 + 28 = 460
            arr[1] = 48 * Random.Range(0, 9) + 28;


            //arr[0] = Random.Range(/*11*/ + 1 + (int)(zSize/2), /*-11*/ -1 + 399 - (int)(xSize / 2)); //0,0 is the top left cell
            //arr[1] = Random.Range(/*11*/ + 1 + (int)(zSize / 2), /*-11*/ -1 + 399 - (int)(xSize / 2)); //0,0 is the top left cell


            /*
            if(k == 0)
            {
                arr[1] = 50;
                arr[0] = 80;
            }
            else
            {
                arr[1] = 50;
                arr[0] = 10;
            }
            */
            // ------------------- Integer positions in GRID / positions according to sizes of rooms in GRID fashion -------------------
            //arr[0] = Mathf.Round(((((int)arr[0])/zSize) * zSize) / zSize) * zSize;
            //arr[1] = Mathf.Round(((((int)arr[1])/xSize) * xSize) / zSize) * zSize;

            // ------------------- Checks for collisions between rooms  -------------------
            if (NoCollisions(arr))
            {
                allRooms.Add(arr);
                ++k;
            }
            ++l;
        }

        // ------------------- RANDOMLY choosing ROOMS to spawn  -------------------

        for (int i = 0; i < k; i++)
        {
            GameObject roomToSpawn = generatorRoom;
            switch (Random.Range(0, 2))
            {
                case 0 :
                    roomToSpawn = startRoom;
                    break;
                case 1:
                    roomToSpawn = endRoom;
                    break;/*
                case 2:
                    roomToSpawn = roomL;
                    break;
                case 3:
                    roomToSpawn = roomT;
                    break;
                case 4:
                    roomToSpawn = room4;
                    break;*/
            }
            Room roomScript = roomToSpawn.GetComponent<Room>();
            float yRotation = Random.Range(0, 3) * 90;
            GameObject spawnedRoom = Instantiate(roomToSpawn, new Vector3(-((float[])allRooms[i])[1], 0, -((float[])allRooms[i])[0]), Quaternion.Euler(0, yRotation, 0));




            //List to check item overlap
















            itemGen.SpawnItems(new Vector3(0, 0, 0), new Vector3(10, 0, 10), 4);


            if (Random.Range(0.0f, 1.0f) <= ventCoverProbabilty)
            {
                Instantiate(ventCover, new Vector3(-((float[])allRooms[i])[1], 0, -((float[])allRooms[i])[0]), Quaternion.Euler(0, Random.Range(0, 3) * 90, 0));
            }

            if(yRotation == 90)
            {
                spawnedRoom.GetComponent<RoomReferences>().doors[0].name = "Door+x";
            }
            else if(yRotation == 180 || yRotation == 270 || yRotation == -90)
            {
                float reqYRotationForCorridor = 0; 
                if (yRotation == 180)
                {
                    spawnedRoom.GetComponent<RoomReferences>().doors[0].name = "Door-z";
                    reqYRotationForCorridor = 0;
                }
                else if (yRotation == 270 || yRotation == -90)
                {
                    spawnedRoom.GetComponent<RoomReferences>().doors[0].name = "Door-x";
                    reqYRotationForCorridor = 90;
                }

                for (int j = 0; j < spawnedRoom.transform.GetChild(2).childCount; j++)
                {
                    spawnedRoom.transform.GetChild(2).GetChild(j).rotation = Quaternion.Euler(0, reqYRotationForCorridor, 0);
                }

            }
            


            // ------------------- Attaches RoomNew Script to last spawned Room and passes the corridors array (all types,I,4,T,L,etc) -------------------
            if (i == k - 1)
            {
                
                RoomNew roomNewScript = spawnedRoom.AddComponent<RoomNew>();
                roomNewScript.corridors = corridors;
                roomNewScript.ventCover = ventCover;
                roomNewScript.ventCoverProbabilty = ventCoverProbabilty / n / 3;
                Data.instance.roomNewScript = roomNewScript;
            }

            //gameObjectDetails.Add(roomScript);

        }
        

        /*
        ////Debug ALL ROOM POSITIONS

        for (int i = 0; i < n; i++)
        {
            //Debug.Log("_________________" + i + "___________________");
            float[] ddd = ((float[])allRooms[i]);
            //Debug.Log(ddd[0]);
            //Debug.Log(ddd[1]);
        }
        //Debug.Log("_________________DONE___________________");
        Data.instance.allRooms = this.allRooms;
        Data.instance.xSize = xSize;
        Data.instance.zSize = zSize;
        */
    }

    // --------------------------------- Checks for collisions between ROOMS ---------------------------------
    private bool NoCollisions(float[] arr)
    {
        for (int i = 0; i < allRooms.Count; i++)
        {
            if ((Mathf.Abs(arr[0] - ((float[])allRooms[i])[0]) < xSize) && (Mathf.Abs(arr[1] - ((float[])allRooms[i])[1]) < zSize))
            {
                return false;
            }
        }
        return true;
    }

    /*
    private GameObject corridorTypes(int corridorCode)
    {
        if (corridorCode == 0)
        {
            return noDoor;
        }
        else if (corridorCode == 1)
        {
            return narrowDoor;
        }
        else //if (corridorCode == 2)
        {
            return wideDoor;
        }
    }
    */
}