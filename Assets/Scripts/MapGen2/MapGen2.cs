using UnityEngine;
using UnityEditor;
using System.Collections;

public class MapGen2 : MonoBehaviour
{
    //first we'll see the ground floor
    //10 x 10 cube
    private int n = 10;
    private ArrayList allRooms = new ArrayList();
    private ArrayList roomsInARow = new ArrayList();
    private int allRoomsLength;
    private Room room;

    public GameObject mainRoom;




    private float xSize = 10f, zSize = 10f;




    private void Start()
    {
        Debug.Log("1");
        rooms();
        Debug.Log("5");
    }

    public void rooms()
    {



        int k = 0, l = 0;
        while (k < n && l < 1000)
        {
            Debug.Log("2");
            int[] arr = new int[2];
            arr[0] = (int)Mathf.Round(Random.Range(-0.49f + zSize, 99.49f - xSize)); //0,0 is the top left cell
            arr[1] = (int)Mathf.Round(Random.Range(-0.49f + zSize, 99.49f - xSize)); //0,0 is the top left cell

            arr[0] = (int)(Mathf.Round(((float)arr[0])/10f) * 10);
            arr[1] = (int)(Mathf.Round(((float)arr[1])/10f) * 10);
            

            if (noCollisions(arr))
            {
                Debug.Log("3----------------------------");
                allRooms.Add(arr);
                ++k;
            }
            ++l;
        }

       

        for (int i = 0; i < k; i++)
        {
            //roomsInARow = new ArrayList();
            Debug.Log("4");
            Instantiate(mainRoom, new Vector3(-((int[])allRooms[i])[1], 0, -((int[])allRooms[i])[0]), Quaternion.identity);

            //allRooms.Add(roomsInARow);

        }
        

        

        for (int i = 0; i < n; i++)
        {
            Debug.Log("_________________" + i + "___________________");
            int[] ddd = ((int[])allRooms[i]);
            Debug.Log(ddd[0]);
            Debug.Log(ddd[1]);
        }
    }

    private bool noCollisions(int[] arr)
    {
        for (int i = 0; i < allRooms.Count; i++)
        {
            if ((Mathf.Abs(arr[0] - ((int[])allRooms[i])[0]) < xSize) && (Mathf.Abs(arr[1] - ((int[])allRooms[i])[1]) < zSize))
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