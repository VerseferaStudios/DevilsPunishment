using UnityEngine;
using UnityEditor;
using System.Collections;

public class MapGen3 : MonoBehaviour
{
    //first we'll see the ground floor
    //10 x 10 cube
    //later//private int gridSize;
    [Header("Rooms")]
    public GameObject doorPrefab;
    private int n = 10;
    public ArrayList allRooms = new ArrayList();
    private ArrayList gameObjectDetails = new ArrayList();

    public GameObject mainRoomIndicator, generatorRoom, startRoom, endRoom;
    
    private float xSize = 48f, zSize = 48f;

    public GameObject[] corridors;

    //For Vents
    [Header("Vents")]
    public float ventCoverProbabilty = 0.050f;
    public GameObject ventCover;


    private void Start()
    {
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

            arr[0] = 48 * Random.Range(0, 5) + 28;  //9 coz -> 9 * 48 + 28 = 460
            arr[1] = 48 * Random.Range(0, 5) + 28;


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
            float yCoord = 1f, xCoord = 0, zCoord = 0;
            switch (Random.Range(0, 3))
            {
                case 0 :
                    roomToSpawn = startRoom;
                    yCoord = 0.064f;
                    xCoord = 0.06f;
                    zCoord = -0.075f;
                    break;
                case 1:
                    roomToSpawn = endRoom;
                    yCoord = 0.5f;
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
            GameObject spawnedRoom = Instantiate(roomToSpawn, new Vector3(-((float[])allRooms[i])[1] + xCoord, yCoord, -((float[])allRooms[i])[0] + zCoord), Quaternion.Euler(0, yRotation, 0));

            if(Random.Range(0.0f, 1.0f) < ventCoverProbabilty)
            {
                Instantiate(ventCover, new Vector3(-((float[])allRooms[i])[1], 0, -((float[])allRooms[i])[0]), Quaternion.Euler(0, Random.Range(0, 3) * 90, 0));
            }

            if(yRotation == 90)
            {
                spawnedRoom.GetComponent<RoomReferences>().doors[0].name = "Door+x";
                //spawnedRoom.transform.localPosition = new Vector3(spawnedRoom.transform.localPosition.x + 0.226f,  //*
                //                                                  spawnedRoom.transform.localPosition.y,           //* This is for Start Room
                //                                                  spawnedRoom.transform.localPosition.z + 0.065f); //*

                /*
                Transform corridorsOfRoom = spawnedRoom.transform.GetChild(2);
                for (int z = 0; z < corridorsOfRoom.childCount; z++)
                {
                    corridorsOfRoom.GetChild(z).localPosition = new Vector3(0, 0, 0.226f);
                }
                */

                GameObject door = Instantiate(doorPrefab/*door*/, spawnedRoom.transform.position + new Vector3(24, 0, 0), Quaternion.identity, spawnedRoom.transform);
                door.transform.SetParent(spawnedRoom.transform);
                door.transform.SetSiblingIndex(1);


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
                    //spawnedRoom.transform.localPosition = new Vector3(spawnedRoom.transform.localPosition.x + 0.226f,  //*
                    //                                                  spawnedRoom.transform.localPosition.y,           //* This is for Start Room
                    //                                                  spawnedRoom.transform.localPosition.z - 0.065f); //*

                    /*
                    Transform corridorsOfRoom = spawnedRoom.transform.GetChild(2);
                    for (int z = 0; z < corridorsOfRoom.childCount; z++)
                    {
                        corridorsOfRoom.GetChild(z).localPosition = new Vector3(0, 0, 0.226f);
                    }
                    */

                    reqYRotationForCorridor = 90;
                }

                for (int j = 0; j < spawnedRoom.transform.GetChild(2).childCount; j++)
                {
                    spawnedRoom.transform.GetChild(2).GetChild(j).rotation = Quaternion.Euler(0, reqYRotationForCorridor, 0);
                }

            }
            //probably +z....
            else
            {

            }
            


            // ------------------- Attaches RoomNew Script to last spawned Room and passes the corridors array (all types,I,4,T,L,etc) -------------------
            if (i == k - 1)
            {
                RoomNew roomNewScript = spawnedRoom.AddComponent<RoomNew>();
                roomNewScript.corridors = corridors;
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