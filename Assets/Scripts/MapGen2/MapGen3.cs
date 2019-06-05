using UnityEngine;
using UnityEditor;
using System.Collections;

public class MapGen3 : MonoBehaviour
{
    //first we'll see the ground floor
    //10 x 10 cube
    //later//private int gridSize;
    private int n = 15;
    public ArrayList allRooms = new ArrayList();
    private ArrayList gameObjectDetails = new ArrayList();
    public Transform target;

    public GameObject mainRoomIndicator, room4, room1, roomI, roomL, roomT;
    
    private float xSize = 48f, zSize = 48f;

    public GameObject[] corridors;


    private void Start()
    {
        rooms();
        Data.instance.corridorT1 = corridors[3];
        Data.instance.corridorT2 = corridors[6];
        Data.instance.corridorX = corridors[4];
        Data.instance.xSize = xSize;
        Data.instance.zSize = zSize;
    }

    public void rooms()
    {
        

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
            GameObject roomToSpawn = room4;
            switch (4)//Random.Range(0, 4))
            {
                case 0 :
                    roomToSpawn = room1;
                    break;
                case 1:
                    roomToSpawn = roomI;
                    break;
                case 2:
                    roomToSpawn = roomL;
                    break;
                case 3:
                    roomToSpawn = roomT;
                    break;
                case 4:
                    roomToSpawn = room4;
                    break;
            }
            Room roomScript = roomToSpawn.GetComponent<Room>();
            GameObject spawnedRoom = Instantiate(roomToSpawn, new Vector3(-((float[])allRooms[i])[1], 0, -((float[])allRooms[i])[0]), Quaternion.identity);

            // ------------------- Attaches RoomNew Script to last spawned Room and passes the corridors array (all types,I,4,T,L,etc) -------------------
            if (i == k - 1)
            {
                spawnedRoom.AddComponent<RoomNew>().corridors = corridors;
            }

            //gameObjectDetails.Add(roomScript);

        }
        

        /*
        //Debug ALL ROOM POSITIONS

        for (int i = 0; i < n; i++)
        {
            Debug.Log("_________________" + i + "___________________");
            float[] ddd = ((float[])allRooms[i]);
            Debug.Log(ddd[0]);
            Debug.Log(ddd[1]);
        }
        Debug.Log("_________________DONE___________________");
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