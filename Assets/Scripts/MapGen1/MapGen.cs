using UnityEngine;
using UnityEditor;
using System.Collections;

public class MapGen : MonoBehaviour
{
    //first we'll see the ground floor
    //10 x 10 cube
    public int n = 10;
    private ArrayList mainRooms = new ArrayList();
    public ArrayList allRooms = new ArrayList();
    public ArrayList roomsInARow = new ArrayList();
    private int allRoomsLength;
    private Room1 room;

    public GameObject roomIndicator, wideDoor, narrowDoor, noDoor, mainRoom;

    private void Start()
    {
        rooms();
    }

    public void rooms()
    {
        
        
        

        for (int i = 0; i < 10; i++)
        {
            int[] arr = new int[2];
            arr[0] = (int)Mathf.Round(Random.Range(-0.49f, 9.49f)); //0,0 is the top left cell
            arr[1] = (int)Mathf.Round(Random.Range(-0.49f, 9.49f)); //0,0 is the top left cell

            if (noCollisions(arr))
            {
                Debug.Log("Sdfghjksdfghj");
                mainRooms.Add(arr);
            }
        }
        
        

        for (int i = 0; i < n; i++)
        {
            roomsInARow = new ArrayList();
            for (int j = 0; j < n; j++)
            {
                room = new Room1();
                allRoomsLength = allRooms.Count;



                Debug.Log("-----Room No " + i + " " + j + " ----------");

                if (j == 0)
                {
                    room.doorType[3] = 0; // (int)Mathf.Round(Random.Range(-0.49f, 2.49f)); // 3 door types

                }
                /*
                else if (i != 0 && ((Room)((ArrayList)allRooms[i - 1])[j]).doorType[3] == 0)
                {
                    //Debug.Log("1");
                    room.doorType[3] = (int)Mathf.Round(Random.Range(+0.51f, 2.49f)); // 1 or 2 , no redundant walls in corridors
                }
                */
                else
                {
                    room.doorType[3] = ((Room1)roomsInARow[j - 1]).doorType[1];
                    
                }

                if (i == 0)
                {
                    room.doorType[0] = 0; // (int)Mathf.Round(Random.Range(-0.49f, 2.49f));
                    
                }
                /*
                else if (j != 0 && ((Room)roomsInARow[j - 1]).doorType[0] == 0)
                {
                    //Debug.Log("3");
                    room.doorType[0] = (int)Mathf.Round(Random.Range(+0.51f, 2.49f)); // 1 or 2 , no redundant walls in corridors
                }
                */
                else
                {
                    room.doorType[0] = ((Room1)((ArrayList)allRooms[i - 1])[j]).doorType[2];
                    
                }


                if (j == n - 1)
                {
                    room.doorType[1] = 0;
                }
                /*
                else if (i != 0 && ((Room)((ArrayList)allRooms[i - 1])[j]).doorType[1] == 0)
                {
                    //Debug.Log("1");
                    room.doorType[1] = (int)Mathf.Round(Random.Range(+0.51f, 2.49f)); // 1 or 2 , no redundant walls in corridors
                }
                */
                else
                {
                    //Debug.Log("2");
                    room.doorType[1] = (int)Mathf.Round(Random.Range(-0.49f, 2.49f));
                }

                if (i == n - 1)
                {
                    room.doorType[2] = 0;
                }
                /*
                else if (j != 0 && ((Room)roomsInARow[j - 1]).doorType[2] == 0)
                {
                    //Debug.Log("3");
                    room.doorType[2] = (int)Mathf.Round(Random.Range(+0.51f, 2.49f)); // 1 or 2 , no redundant walls in corridors
                }
                */
                else
                {
                    //Debug.Log("4");
                    room.doorType[2] = (int)Mathf.Round(Random.Range(-0.49f, 2.49f));
                }
                


                Debug.Log("&&&&&&&&&&&");
                //Debug.Log("Door type 3 = " + room.doorType[3]);
                Debug.Log(room.doorType[0]);
                Debug.Log(room.doorType[1]);
                Debug.Log(room.doorType[2]);
                Debug.Log(room.doorType[3]);

               

                //take care of room type here


                //Instantiating room
                GameObject parent = Instantiate(roomIndicator, new Vector3(-i * 9, 0, -j * 9), Quaternion.identity);//.transform;
                //Door +x

                GameObject gb = Instantiate(doorTypes(room.doorType[0]), parent.transform.position + new Vector3(4, 0, 0), Quaternion.identity, parent.transform);
                //a.tag = "+x";
                gb = Instantiate(doorTypes(room.doorType[1]), parent.transform.position + new Vector3(0, 0, -4), Quaternion.Euler(0, 90, 0), parent.transform);
                //a.tag = "-z";
                gb = Instantiate(doorTypes(room.doorType[2]), parent.transform.position + new Vector3(-4, 0, 0), Quaternion.identity, parent.transform);
                //a.tag = "-x";
                gb = Instantiate(doorTypes(room.doorType[3]), parent.transform.position + new Vector3(0, 0, 4), Quaternion.Euler(0, 90, 0), parent.transform);
                //a.tag = "+z";


                roomsInARow.Add(room);

            }


            allRooms.Add(roomsInARow);
            
        }



        int ctr = 0;
        bool a = false, b = false, c = false, d = false;
        for (int k = 0; k < allRooms.Count - 1; k++)
        {
            for (int l = 0; l < n - 1; l++)
            {
                if (((Room1)((ArrayList)allRooms[k])[l]).doorType[1] == 0)
                {
                    ctr++;
                    a = true;
                }
                if(((Room1)((ArrayList)allRooms[k])[l]).doorType[2] == 0)
                {
                    ctr++;
                    b = true;
                }

                if (((Room1)((ArrayList)allRooms[k + 1])[l + 1]).doorType[0] == 0)
                {
                    ctr++;
                    c = true;
                }
                if (((Room1)((ArrayList)allRooms[k + 1])[l + 1]).doorType[3] == 0)
                {
                    ctr++;
                    d = true;
                }

                //You can map a + b + c + d as a string and that is the key which maps to the value which is the gameObject to be instantiated.
                //That is instead of 4 if conditions
                if(ctr == 3)
                {
                    if (a)
                    {

                    }
                }
            }

        }




        Instantiate(mainRoom, new Vector3(-((int[])mainRooms[0])[1] * 9, 0, -((int[])mainRooms[0])[0] * 9), Quaternion.identity);
        Instantiate(mainRoom, new Vector3(-((int[])mainRooms[1])[1] * 9, 0, -((int[])mainRooms[1])[0] * 9), Quaternion.identity);
        Instantiate(mainRoom, new Vector3(-((int[])mainRooms[2])[1] * 9, 0, -((int[])mainRooms[2])[0] * 9), Quaternion.identity);

        for (int i = 0; i < n; i++)
        {
            Debug.Log("_________________" + i + "___________________");
            for (int j = 0; j < n; j++)
            {
                Debug.Log("+++++++++++++++" + j + "+++++++++++++++");
                int[] ddd = ((Room1)((ArrayList)allRooms[i])[j]).doorType;
                Debug.Log(ddd[0]);
                Debug.Log(ddd[1]);
                Debug.Log(ddd[2]);
                Debug.Log(ddd[3]);
            }

        }
    }

    private bool noCollisions(int[] arr)
    {
        for (int i = 0; i < mainRooms.Count; i++)
        {
            if (arr.Equals(mainRooms[i]))
            {
                return false;
            }
        }
        return true;
    }

    private GameObject doorTypes(int doorCode)
    {
        if(doorCode == 0)
        {
            return noDoor; 
        }
        else if (doorCode == 1)
        {
            return narrowDoor;
        }
        else //if (doorCode == 2)
        {
            return wideDoor;
        }
    }
}