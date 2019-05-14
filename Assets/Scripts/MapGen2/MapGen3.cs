using UnityEngine;
using UnityEditor;
using System.Collections;

public class MapGen3 : MonoBehaviour
{
    //first we'll see the ground floor
    //10 x 10 cube
    private int n = 10;
    public ArrayList allRooms = new ArrayList();
    private ArrayList gameObjectDetails = new ArrayList();
    public Transform target;
    //private Room room;

    public GameObject mainRoomIndicator, room4, room1, roomI, roomL, roomT;




    private float xSize = 11f, zSize = 11f;

    public GameObject corridor;


    private void Start()
    {
        //Debug.Log("1");
        rooms();
        //Debug.Log("5");
    }

    public void rooms()
    {
        

        int k = 0, l = 0;
        while (k < n && l < 1000)
        {
            float[] arr = new float[2];
            arr[0] = Mathf.Round(Random.Range(1 - 0.49f + zSize/2, -1 + 99.49f - zSize/2)); //0,0 is the top left cell
            arr[1] = Mathf.Round(Random.Range(1 - 0.49f + xSize/2, -1 + 99.49f - xSize/2)); //0,0 is the top left cell

            arr[0] = (Mathf.Round(((int)arr[0])/zSize) * zSize);
            arr[1] = (Mathf.Round(((int)arr[1])/xSize) * xSize);
            

            if (noCollisions(arr))
            {
                //Debug.Log("3----------------------------");
                allRooms.Add(arr);
                ++k;
            }
            ++l;
        }
        

        for (int i = 0; i < k; i++)
        {
            //roomsInARow = new ArrayList();
            //Debug.Log("4");
            GameObject roomToSpawn = room4;
            switch ((int)Mathf.Round(Random.Range(-0.49f, 4.49f)))
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
            //roomScript.index = i;
            GameObject spawnedRoom = Instantiate(roomToSpawn, new Vector3(-((float[])allRooms[i])[1], 0, -((float[])allRooms[i])[0]), Quaternion.identity);
            if(i == k - 1)
            {
                spawnedRoom.AddComponent<RoomNew>().corridor = corridor;
                //spawnedRoom.GetComponent<RoomNew>().corridor = corridor;
            }

            //gameObjectDetails.Add(roomScript);
            //allRooms.Add(roomsInARow);

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

    private bool noCollisions(float[] arr)
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