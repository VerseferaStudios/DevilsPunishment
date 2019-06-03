using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataBackup : MonoBehaviour
{
    public static Data instance = null;
    public ArrayList allRooms = new ArrayList();
    public float xSize, zSize;
    public int collisionCount = 0, corridorCount = 0;
    public bool isCollided = false;
    public List<GameObject> collidedCorridors = new List<GameObject>();

    public float startTime;
    private bool isNotFirstTime = false;

    ///<summary>
    ///The direction of the opening for the L corridor near door of type stored in this list
    ///</summary>
    public List<string> nearDoorL = new List<string>();
    public GameObject corridorT1, corridorT2, corridorX;

    private float nextTime = 1f;

    public Dictionary<Vector3, int> corridorPosDict = new Dictionary<Vector3, int>();

    public List<ConnectedComponent> connectedRoomsThroughCollision = new List<ConnectedComponent>();

    public List<List<Vector3>> connectedRooms = new List<List<Vector3>>();

    public Vector3 spawnPointsFirstPos;

    public int count = 0;

    public GameObject roomIndicator;

    public int ctr = 0;

    public bool isOnce = true, isDonePrevFnCall = true;

    public int prevCount = 0;

    //public bool isPipeAtLeft = true;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        //Random.InitState(10);
        //Random.state = JsonUtility.FromJson<Random.State>("{\"s0\":1252075656,\"s1\":1756889672,\"s2\":100038291,\"s3\":-1311783885}");
    }

    private void Start()
    {
        startTime = Time.time;
        nearDoorL.Add("-z");
        nearDoorL.Add("-x");
        nearDoorL.Add("+z");
        nearDoorL.Add("+x");
        prevCount = connectedRoomsThroughCollision.Count;
    }

    private void Update()
    {

        if(count < 5 && collidedCorridors.Count != 0 && Time.time - startTime > nextTime)
        {
            count++;

            Debug.Log("Count Olaf =" + collidedCorridors.Count);


            //FindDuplicates(); //use this to group corridors at the same place use ConvertToOpenings and Linq.Distinct and do the necessary


            Debug.Log("----------------------wargarsg----------------------");
            for (int i = 0; i < collidedCorridors.Count; i++)
            {
                if(collidedCorridors[i] == null)
                {
                    collidedCorridors.RemoveAt(i);
                    i--;
                    continue;
                }
                for (int j = 0; /*j < i + 4 &&*/ j < collidedCorridors.Count; j++)
                {
                    if (i == j) continue;

                    if (collidedCorridors[j] == null)
                    {
                        collidedCorridors.RemoveAt(j);
                        if (i > j)
                        {
                            i--;
                        }
                        j--;
                        continue;
                    }

                    /*
                    if (collidedCorridors[i] == null)
                    {
                        collidedCorridors.RemoveAt(i);
                        i--;
                        continue;
                    }
                    */

                    //Debug.Log(collidedCorridors[i].transform.position);
                    //Debug.Log(collidedCorridors[j].transform.position);

                    if (collidedCorridors[i].transform.position == collidedCorridors[j].transform.position)
                    //if(Mathf.Abs(collidedCorridors[i].transform.position.x - collidedCorridors[j].transform.position.x) <= 0.6f
                    //    && Mathf.Abs(collidedCorridors[i].transform.position.z - collidedCorridors[j].transform.position.z) <= 0.6f)
                    {
                        //Make condition perfect er

                        if (collidedCorridors[i].transform.parent.name.Equals(collidedCorridors[j].transform.parent.name) 
                            && (collidedCorridors[i].transform.rotation == collidedCorridors[j].transform.rotation))
                        {
                            //Debug.Log("Leave");

                        }
                        else if(!isNotFirstTime)
                        {

                            //Debug.Log("in at " + collidedCorridors[i].transform.position);
                            //Debug.Log(collidedCorridors[i].transform.parent.name + " " + collidedCorridors[i].transform.rotation.eulerAngles);
                            //Debug.Log(collidedCorridors[j].transform.parent.name + " " + collidedCorridors[j].transform.rotation.eulerAngles);
                            List<int> openings1 = new List<int>(), openings2 = new List<int>();
                            openings1 = ConvertToOpenings(collidedCorridors[i].transform.parent.name, collidedCorridors[i].transform.rotation.eulerAngles.y);
                            openings2 = ConvertToOpenings(collidedCorridors[j].transform.parent.name, collidedCorridors[j].transform.rotation.eulerAngles.y);
                            //Debug.Log(openings1[0] + " " + openings1[1]);
                            //Debug.Log(openings2[0] + " " + openings2[1]);
                            /*
                            for (int i = 0; i < openings1.Count; i++)
                            {
                                for (int j = 0; j < openings2.Count; j++)
                                {
                                    if (i == j) continue;
                                    if(openings1[i] == openings2[j])
                                    {

                                    }
                                }
                            }
                            */
                            openings1.AddRange(openings2);


                            openings1 = openings1.Distinct<int>().ToList<int>();


                            if (openings1.Count == 3)
                            {
                                float yRotation = ConvertToRotation(openings1);
                                GameObject currCorridor = Instantiate((yRotation == 0 || yRotation == -90) ? corridorT2 : corridorT1 , collidedCorridors[j].transform.position, Quaternion.identity);
                                if(yRotation == 0 || yRotation == 90)
                                {
                                    currCorridor.GetComponentInChildren<BoxCollider>().enabled = false;
                                    currCorridor.transform.localScale = new Vector3(-1, 1, 1);
                                    currCorridor.GetComponentInChildren<BoxCollider>().enabled = true;
                                }
                                currCorridor.transform.rotation = Quaternion.Euler(0, yRotation, 0);
                            }
                            if(openings1.Count == 4)
                            {
                                Instantiate(corridorX, collidedCorridors[j].transform.position, Quaternion.identity);
                            }

                            /*
                            //If I and I collides with different rotations
                            if (collidedCorridors[i].transform.rotation != collidedCorridors[j].transform.rotation)
                            {
                                GameObject currCorridor1 = Instantiate(corridorX, collidedCorridors[j].transform.position, Quaternion.identity);
                            }
                            //If I and L collides
                            else if (!collidedCorridors[i].transform.parent.name.Equals(collidedCorridors[j].transform.parent.name))
                            {
                                //T
                            }
                            else
                            {

                            }
                            */

                            Destroy(collidedCorridors[i].transform.parent.gameObject);
                        }

                        Destroy(collidedCorridors[j].transform.parent.gameObject);

                        // !!!!!!!!!!!!!!!!! Take care of collisions that happen when the above corridors (T and X) are instantiated, if any !!!!!!!!!!!!!!!!!
                        if (!isNotFirstTime)
                        {
                            collidedCorridors.RemoveAt(i);

                            if (j > i)
                            {
                                j--;
                            }

                            i--;
                        }

                        collidedCorridors.RemoveAt(j);

                        if(!isNotFirstTime && i > j)
                        {
                            i--;
                        }
                        j--;

                        isNotFirstTime = true;
                        break;
                    }
                    
                }

                if (isNotFirstTime)
                {
                    isNotFirstTime = false;
                }
            }

            if (collidedCorridors.Count == 1)
            {
                Destroy(collidedCorridors[0].transform.parent.gameObject);
            }
            
            Debug.Log("Count Olaf AFTER =" + collidedCorridors.Count);
            for (int q = 0; q < collidedCorridors.Count; q++)
            {
                //Debug.Log(collidedCorridors[q].transform.position + " " + collidedCorridors[q].transform.parent.name);
            }

            if (isDonePrevFnCall && connectedRoomsThroughCollision.Count != prevCount)
            {
                //AddConnectedRooms(connectedRoomsThroughCollision, false);
                AddAndRemoveConnectedRooms();
                ctr++;
            }

            prevCount = connectedRoomsThroughCollision.Count;

            nextTime = Time.time + 1f;

        }
        if( Time.time - startTime >= 4f && isOnce ) //&& (count >= 5 || collidedCorridors.Count == 0))
        {
            if (connectedRoomsThroughCollision.Count != 0)
            {
                AddAndRemoveConnectedRooms();
                ctr++;
            }
                
            isOnce = false;
            int z = 1;
            foreach (var item in connectedRooms)
            {
                Debug.Log("LLLLLAAAASTSSTTTTTTT ConnectedRooms No. " + z);
                foreach (var item1 in item)
                {
                    Debug.Log(item1);
                    GameObject gb = Instantiate(roomIndicator, item1, Quaternion.identity);
                    //Colour scheme
                    if (z == 1)
                    {
                        gb.GetComponent<Renderer>().material.color = Color.green;
                    }
                    else if (z == 2)
                    {
                        gb.GetComponent<Renderer>().material.color = Color.red;
                    }
                    else if (z == 3)
                    {
                        gb.GetComponent<Renderer>().material.color = Color.white;
                    }
                    else if (z == 4)
                    {
                        gb.GetComponent<Renderer>().material.color = Color.yellow;
                    }
                    else if (z == 5)
                    {
                        gb.GetComponent<Renderer>().material.color = Color.cyan;
                    }
                    else if (z == 6)
                    {
                        gb.GetComponent<Renderer>().material.color = Color.blue;
                    }
                    else if (z == 7)
                    {
                        gb.GetComponent<Renderer>().material.color = Color.magenta;
                    }
                    else if (z == 8)
                    {
                        gb.GetComponent<Renderer>().material.color = Color.grey;
                    }
                }
                ++z;
            }
            Debug.Log("Random.seed = " + JsonUtility.ToJson(Random.state));
        }
    }

    // --------------------- Converts corridorOpening indices/numbers into yRotations for corridor prefabs ---------------------
    public float ConvertToRotation(List<int> corridorOpenings)
    {
        corridorOpenings.Sort();
        float yRotation = 0;
        //DeadEnd
        if (corridorOpenings.Count == 1)
        {
            yRotation = 90 * corridorOpenings[0];
        }
        // I or L
        else if(corridorOpenings.Count == 2)
        {
            // I
            if((corridorOpenings[0] == 0 && corridorOpenings[1] == 2))// || (corridorOpenings[1] == 0 && corridorOpenings[0] == 2))
            {
                yRotation = 0;
            }
            // I
            else if(corridorOpenings[0] == 1 && corridorOpenings[1] == 3)
            {
                yRotation = 90;
            }
            // L
            else
            {
                yRotation = 90 * corridorOpenings[0];
                if(corridorOpenings[0] == 0 && corridorOpenings[1] == 3)
                {
                    yRotation = 270;
                }
            }
        }
        //T
        else if (corridorOpenings.Count == 3)      //Check if it works-------------------------
        {
            List<int> oneToFour = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                oneToFour.Add(i);
            }
            for (int i = 0; i < corridorOpenings.Count; i++)
            {
                oneToFour.Remove(corridorOpenings[i]);
            }
            yRotation = oneToFour[0] * 90;
        }
        //X
        else if(corridorOpenings.Count == 4)
        {
            yRotation = 0;
        }
        return yRotation;
    }

    //Search index of the required door
    public int NeardoorLIndexSearch(string door)
    {
        for (int i = 0; i < nearDoorL.Count; i++)
        {
            if (nearDoorL[i].Equals(door))
            {
                return i;
            }
        }
        return -1;
    }

    //Check if it works-------------------------
    public List<int> ConvertToOpenings(string name, float yRotation)
    {
        List<int> openings = new List<int>();
        if (name.Equals("Corridor_I(Clone)"))
        {
            openings.Add((int)(yRotation / 90f));
            openings.Add(openings[0] + 2);
            //Debug.Log(openings[0] + " " + openings[1]);
        }
        else if (name.Equals("Corridor_L_1(Clone)"))
        {
            if(yRotation == 270)
            {
                openings.Add(0);
                openings.Add(3);
            }
            else
            {
                openings.Add((int)(yRotation / 90f));
                openings.Add(openings[0] + 1);
            }
        }
        else if (name.Equals("Corridor_T_.1(Clone)"))
        {
            List<int> oneToFour = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                oneToFour.Add(i);
            }
            oneToFour.Remove((int)(yRotation / 90f));
            openings.AddRange(oneToFour);
        }
        else if (name.Equals("Corridor_X_1(Clone)"))
        {
            List<int> oneToFour = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                oneToFour.Add(i);
            }
            openings.AddRange(oneToFour);
        }
        return openings;
    }

    private void FindDuplicates()
    {
        foreach (GameObject gb in collidedCorridors)
        {
            if (!corridorPosDict.ContainsKey(gb.transform.position))
            {
                corridorPosDict.Add(gb.transform.position, 1);
            }
            else
            {
                int count = 0;
                corridorPosDict.TryGetValue(gb.transform.position, out count);
                corridorPosDict.Remove(gb.transform.position);
                corridorPosDict.Add(gb.transform.position, count + 1);
            }
        }

        //Debug.Log("Dictionary of duplicates!!!!!!!!!!!!!");
        foreach (KeyValuePair<Vector3, int> item in corridorPosDict)
        {
            //Debug.Log(item.Key + " " + item.Value);
        }
    }

    public bool CheckIfVisited(Vector3 toCheck)
    {
        bool flag = false;
        foreach (List<Vector3> groupOfRooms in connectedRooms)
        {
            if (groupOfRooms.Contains(toCheck))
            {
                flag = true;
                break;
            }
        }
        return flag;
    }

    /*
    private void AddOrRemoveConnectedRooms11(ref List<List<Vector3>> connectedRoomsThroughCollision, bool isRemoveAfterAdding)
    {
        bool isFoundOne = false, isFoundBoth = false;
        int idxCollision = -1, idx = -1;
        for (int k = 0; k < connectedRoomsThroughCollision.Count; ++k)
        {
            for (int i = 0; i < connectedRooms.Count; i++)
            {
                for (int j = 0; j < connectedRooms[i].Count; j++)
                {
                    if (connectedRooms[i][j] == connectedRoomsThroughCollision[k][0])
                    {
                        isFoundOne = true;
                        idxCollision = k; //remove for isFoundBoth
                        idx = i;
                        break;
                    }
                }
                /*
                if (isFoundOne)
                {
                    for (int j = 0; j < connectedRooms[i].Count; j++)
                    {
                        if (connectedRooms[i][j] == connectedRoomsThroughCollision[k][1])
                        {
                            isFoundBoth = true;
                            idxCollision = k;
                            idx = i;
                            break;
                        }
                    }
                }
                *//*
            }
        }
        //if (!isFoundBoth)
        if (!isFoundOne)
        {
            if (!isRemoveAfterAdding)
            {
                List<Vector3> arr = new List<Vector3>();
                for (int i = 0; i < 2 * connectedRoomsThroughCollision.Count; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        arr.Add(connectedRoomsThroughCollision[i][j]);
                    }
                }
                connectedRooms.Add(arr);
            }
        }
        else
        {
            //Add the remaining connected rooms
            for (int i = 0; i < connectedRoomsThroughCollision.Count; i++)
            {
                if(i == idxCollision)
                {
                    continue;
                }
                for (int j = 0; j < 2; j++)
                {
                    //if()
                        connectedRooms[idx].Add(connectedRoomsThroughCollision[i][j]);
                }
            }

            //search for second set and remove
            SearchForOtherRooms(idx);
        }
    }

    private void SearchForOtherRooms(int idx)
    {
        //search for second set and remove
        for (int i = 0; i < connectedRooms.Count; i++)
        {
            if(i == idx)
            {
                continue;
            }
            for (int j = 0; j < 2; j++)
            {
                if (connectedRooms[i].Contains(connectedRoomsThroughCollision[i].rooms[j]))
                {
                    //AddOrRemoveConnectedRooms( ref connectedRooms.GetRange(i, 1) , true);
                }
            }
        }
    }
    */
    private void AddAndRemoveConnectedRooms()
    {
        isDonePrevFnCall = false;
        //connectedRoomsThroughCollision = connectedRoomsThroughCollision.Distinct().ToList();  //NEEDED????????

        //check the below for loop with debug
        List<ConnectedComponent> temp = new List<ConnectedComponent>();
        for (int i = 1; i < connectedRoomsThroughCollision.Count; i++)
        {
            if(connectedRoomsThroughCollision[0].corridorPos != connectedRoomsThroughCollision[i].corridorPos)
            {
                temp.Add(connectedRoomsThroughCollision[i]);
                connectedRoomsThroughCollision.RemoveAt(i);
                i--;
            }
        }

        int z = 1;
        foreach (var item in temp)
        {
            Debug.Log("Tempppp No. " + z + "item.Count = " + item.rooms.Count);
            foreach (var item1 in item.rooms)
            {
                Debug.Log(item1);
            }
            z++;
        }


        Debug.Log("AddAndRemoveConnectedRooms iteration number" + ctr);

        //TEST
        //Number of distinct components B4
        Debug.Log("connectedRooms B4 connecting process = " + connectedRooms.Count);
        displayConnectedRooms();




        bool isFoundK = false;
        for (int k = 0; k < connectedRooms.Count; k++)
        {
            for (int q = 0; q < connectedRooms[k].Count; q++)
            {
                for (int l = 0; l < connectedRooms.Count; l++)
                {
                    if (k == l)
                    {
                        continue;
                    }
                    for (int m = 0; m < connectedRooms[l].Count; m++)
                    {
                        //Now we are taking an element of connectedRoomsThroughCollision (collidedConnectedRoomToSearch) 
                        //and comparing it with every element of connectedRooms
                        if (connectedRooms[l][m] == connectedRooms[k][q])
                        {
                            connectedRoomsThroughCollision.Add(new ConnectedComponent(connectedRooms[k][0] / 2 + connectedRooms[k][1] / 2, connectedRooms[k]));
                            connectedRoomsThroughCollision.Add(new ConnectedComponent(connectedRooms[l][0] / 2 + connectedRooms[l][1] / 2, connectedRooms[l]));

                            GameObject gb = Instantiate(roomIndicator, connectedRooms[l][0] / 2 + connectedRooms[l][1] / 2, Quaternion.identity);
                            gb.AddComponent<CorridorNew>().rooms = connectedRooms[l];

                            isFoundK = true;

                            connectedRooms.RemoveAt(l);
                            l--;
                            if(k > l)
                            {
                                k--;
                            }
                            break;

                            //no break keep looking
                            //break;
                        }
                    }
                }
            }
            if (k >= connectedRooms.Count - 1 && isFoundK)
            {
                GameObject gb = Instantiate(roomIndicator, connectedRooms[k][0] / 2 + connectedRooms[k][1] / 2, Quaternion.identity);
                gb.AddComponent<CorridorNew>().rooms = connectedRooms[k];

                connectedRooms.RemoveAt(k);
                k--;
                isFoundK = false;
                break;//YYYY>????
            }
        }




        List<int> indices = new List<int>();

        Vector3 collidedConnectedRoomToSearch;
        for (int i = 0; i < connectedRoomsThroughCollision.Count; i++)
        {
            for (int j = 0; j < connectedRoomsThroughCollision[i].rooms.Count; j++)
            {
                collidedConnectedRoomToSearch = connectedRoomsThroughCollision[i].rooms[j];
                for (int k = 0; k < connectedRooms.Count; k++)
                {
                    for (int q = 0; q < connectedRooms[k].Count; q++)
                    {
                        //Now we are taking an element of connectedRoomsThroughCollision (collidedConnectedRoomToSearch) 
                        //and comparing it with every element of connectedRooms
                        if(collidedConnectedRoomToSearch == connectedRooms[k][q])
                        {
                            indices.Add(k);
                            connectedRoomsThroughCollision.Add(new ConnectedComponent(connectedRoomsThroughCollision[i].corridorPos, connectedRooms[k]));
                            connectedRooms.RemoveAt(k);
                            k--;
                            break;
                        }
                    }
                }
            }
        }

        //Now add a list to end of connectedRooms (which has less elements now)
        // the new list is the whole of connectedRoomsThroughCollision (to which more elements have been added)


        Debug.Log("connectedRoomsThroughCollision B4 adding connectedRoomsThroughCollision to connectedRooms[connectedRooms.Count - i]");
        displayConnectedRoomsThroughCollision();

        
        
        connectedRoomsThroughCollision = connectedRoomsThroughCollision.Distinct().ToList();  //NEEDED????????

        for (int i = 1; i < connectedRoomsThroughCollision.Count; i++)
        {
            connectedRoomsThroughCollision[0].rooms.AddRange(connectedRoomsThroughCollision[i].rooms); 
        }

        //Removes duplicates in the only List object (of type ConnectedComponent) left
        if(connectedRoomsThroughCollision.Count > 0)
        {
            connectedRoomsThroughCollision[0].rooms = connectedRoomsThroughCollision[0].rooms.Distinct().ToList();
        }

        connectedRoomsThroughCollision.RemoveRange(1, connectedRoomsThroughCollision.Count - 1);

        //B4 adding

        Debug.Log("connectedRoomsThroughCollision B4 adding connectedRoomsThroughCollision to connectedRooms[connectedRooms.Count - i]");
        displayConnectedRoomsThroughCollision();
        
        Debug.Log("connectedRooms B4 adding connectedRoomsThroughCollision to connectedRooms[connectedRooms.Count - i]");
        displayConnectedRooms();

        //Add connectedRoomsThroughCollision[0] to end of connectedRooms
        connectedRooms.Add(connectedRoomsThroughCollision[0].rooms);


        //Then append the rest to connectedRooms[connectedRooms.Count - 1]
        /*for (int i = 1; i < connectedRoomsThroughCollision.Count && connectedRooms.Count - i > 0; i++)
        {
            connectedRooms[connectedRooms.Count - i].AddRange(connectedRoomsThroughCollision[i].rooms);
        }
        */

        //Resetting the List
        connectedRoomsThroughCollision = temp;
        if(connectedRoomsThroughCollision.Count == 0)
        {
            isDonePrevFnCall = true;
        }
        else
        {
            AddAndRemoveConnectedRooms();
            ctr++;
        }
        


        //Number of distinct components
        Debug.Log("ConnectedRooms after adding connectedRoomsThroughCollision to connectedRooms[connectedRooms.Count - i]" + connectedRooms.Count);
        displayConnectedRooms();
        

        /*

        List<List<Vector3>> A = new List<List<Vector3>>();
        List<List<Vector3>> B = new List<List<Vector3>>();

        List<Vector3> temp = new List<Vector3>();

        temp.Add(new Vector3(1, 1, 1));
        temp.Add(new Vector3(11, 10, 10));

        B.Add(temp);

        temp = new List<Vector3>();

        temp.Add(new Vector3(-111, -10, -10));
        temp.Add(new Vector3(-222, -20, -20));

        A.Add(temp);
        A.Add(temp);
        Debug.Log("A");
        foreach (var item in A)
        {
            Debug.Log("----");
            foreach (var item1 in item)
            {
                Debug.Log(item1);
            }
            z++;
        }
        Debug.Log("B");
        foreach (var item in B)
        {
            Debug.Log("----");
            foreach (var item1 in item)
            {
                Debug.Log(item1);
            }
            z++;
        }

        B[B.Count - 1].AddRange(A[0]);
        B[B.Count - 1].AddRange(A[1]);

        Debug.Log("A");
        foreach (var item in A)
        {
            Debug.Log("----");
            foreach (var item1 in item)
            {
                Debug.Log(item1);
            }
            z++;
        }
        Debug.Log("B");
        foreach (var item in B)
        {
            Debug.Log("----");
            foreach (var item1 in item)
            {
                Debug.Log(item1);
            }
            z++;
        }
        */

    }

    private void displayConnectedRoomsThroughCollision()
    {
        int z = 1;
        foreach (var item in connectedRoomsThroughCollision)
        {
            Debug.Log("ConnectedRoomsThroughCollision No. " + z + "item.Count = " + item.rooms.Count);
            foreach (var item1 in item.rooms)
            {
                Debug.Log(item1);
            }
            z++;
        }
    }

    private void displayConnectedRooms()
    {
        int z = 1;
        foreach (var item in connectedRooms)
        {
            Debug.Log("ConnectedRooms No. " + z);
            foreach (var item1 in item)
            {
                Debug.Log(item1);
            }
            z++;
        }
    }

    private void ResolveAdjacentRooms()
    {

    }

}
