using UnityEngine;
using UnityEditor;

public class Room : MapGen3
{
    //public int index;
    public int[] doorPos;
    public int roomType;
    public bool visited = false;
    private int[] from, to;
    public GameObject corridor;

    private void Start()
    {
        Instantiate(corridor, gameObject.GetComponentInChildren<Corridor>().transform.position, Quaternion.identity);
    }

    public void spawn()
    {
        //corridorType = (int)Mathf.Round(Random.Range(-0.49f, 3.49f));
        /*
        for (int i = index; i < allRooms.Count; ++i)
        {
            if (!visited)
            {
                from[0] = (int)transform.position.x;
                from[1] = (int)transform.position.z;
                to[0] = ((int[])allRooms[i])[1];
                to[1] = (int)transform.position.z;
                draw();

                from = to;
                to = (int[])allRooms[i];
                draw();

            }
        }
        */
    }

    public void draw()
    {

    }

    /*
    public Room2(int[] doorType, int roomType)
    {
        this.doorType = doorType;
        this.roomType = roomType;
    }

    public Room2()
    {
        doorType = new int[4] { 0, 0, 0, 0 }; //needed?
    }
    */

    public void spawnCorridor()
    {

    }

}