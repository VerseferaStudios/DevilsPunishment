using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Data : MonoBehaviour
{
    public static Data instance = null;
    public ArrayList allRooms = new ArrayList();
    public float xSize, zSize, corridorSize = 4;
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

    public Dictionary<Vector3, int> corridorPosDict = new Dictionary<Vector3, int>();

    public List<ConnectedComponent> connectedRoomsThroughCollision = new List<ConnectedComponent>();

    public List<List<Vector3>> connectedRooms = new List<List<Vector3>>();

    public Vector3 spawnPointsFirstPos;

    public int count = 0;

    public GameObject roomIndicator;

    public int ctr = 0;

    public bool isOnce = true, isDonePrevFnCall = true;

    public int prevCount = 0;

    public int counter = 0;
    public int counter1 = 0;

    public List<ConnectedComponent> temp = new List<ConnectedComponent>();

    public bool isFinishedCheckCollisions = false, isFinishedAddAndRemoveConnectedRooms = false, isConnectedComponentsCheckDone = false;
    
    public RoomNew roomNewScript;

    private GameObject[] roomsArray;

    private bool isFirstPassDone = false;

    public bool isStartedVents = false;

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
        //StartCoroutine(DoCheckPerSecond());
        //StartCoroutine(DoConnectedComponents());
        //StartCoroutine(DoVents());
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
                    yRotation = -90;
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
    public List<int> ConvertToOpenings(string tag, float yRotation)
    {
        List<int> openings = new List<int>();
        if (tag.Equals("CorridorI"))
        {
            openings.Add((int)(yRotation / 90f));
            openings.Add(openings[0] + 2);
            ////Debug.Log(openings[0] + " " + openings[1]);
        }
        else if (tag.Equals("CorridorL"))
        {
            if(yRotation == 270 || yRotation == -90)
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
        else if (tag.Equals("CorridorT"))
        {
            List<int> oneToFour = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                oneToFour.Add(i);
            }
            oneToFour.Remove((int)(yRotation / 90f));
            openings.AddRange(oneToFour);
        }
        else if (tag.Equals("CorridorX"))
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

        /*
        //Debug.Log("Dictionary of duplicates!!!!!!!!!!!!!");
        foreach (KeyValuePair<Vector3, int> item in corridorPosDict)
        {
            //Debug.Log(item.Key + " " + item.Value);
        }
        */
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

    public IEnumerator DoCheckPerSecond()
    {
        //for putting corridors so that connected components does correctly
        for (int i = 0; i < 3; i++)
        {
            CheckForCollision();
            yield return new WaitUntil(() => isFinishedCheckCollisions = true);
            isFinishedCheckCollisions = false;
        }
        isFirstPassDone = true;

        yield return new WaitUntil(() => isConnectedComponentsCheckDone == true);
        yield return new WaitForSeconds(1f);
        float startTime1 = Time.time;
        while (true)
        {
            if(Time.time - startTime1 >= 10f)
            {
                break;
            }

            //Debug.Log(collidedCorridors.Count + " " + count + "#################################");
            if (count < 6 /*&& collidedCorridors.Count != 0*/)
            {
              //  Debug.Log("$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$4");

                if (count == -1) // Change to 0 to execute AddAndRemoveAdjacentRooms()
                {
                    AddAndRemoveAdjacentRooms();
                    count++;
                    yield return new WaitUntil(() => isFinishedAddAndRemoveConnectedRooms = true);
                    isFinishedAddAndRemoveConnectedRooms = false;
                    ctr++;
                }
                else
                {
                //    Debug.Log("innnnnnnnnnnnnn");
                    CheckForCollision();
                    count++;
                    yield return new WaitUntil(() => isFinishedCheckCollisions = true);
                    isFinishedCheckCollisions = false;
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private void AddAndRemoveAdjacentRooms()   // Dead Code xD
    {

        List<ConnectedComponent> temp = connectedRoomsThroughCollision;
        connectedRoomsThroughCollision = new List<ConnectedComponent>();

        bool isFoundK = false;
        int reqK = -1;

        for (int i = 0; i < connectedRooms.Count; i++)
        {

            if(Mathf.Abs(connectedRooms[i][0].x - connectedRooms[i][1].x) == 10 || Mathf.Abs(connectedRooms[i][0].z - connectedRooms[i][1].z) == 10)
            {

                connectedRoomsThroughCollision.Add(new ConnectedComponent(connectedRooms[i][0] / 2 + connectedRooms[i][1] / 2, connectedRooms[i]));//errors?

                for (int k = 0; k < connectedRoomsThroughCollision.Count; k++)
                {
                    for (int q = 0; q < connectedRoomsThroughCollision[k].rooms.Count; q++)///////////////////////
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
                                if (connectedRooms[l][m] == connectedRoomsThroughCollision[k].rooms[q])
                                {
                                    connectedRoomsThroughCollision.Add(new ConnectedComponent(connectedRooms[l][0] / 2 + connectedRooms[l][1] / 2, connectedRooms[l]));

                                    GameObject gb = Instantiate(roomIndicator, connectedRooms[l][0] / 2 + connectedRooms[l][1] / 2, Quaternion.identity);
                                    CorridorNew cn = gb.AddComponent<CorridorNew>();
                                    cn.rooms = connectedRooms[l];
                                    //cn.KOrL = "l";
                                    //cn.theEqualOnes.Add(connectedRoomsThroughCollision[k].rooms[0]);
                                    //cn.theEqualOnes.Add(connectedRoomsThroughCollision[k].rooms[1]);

                                    isFoundK = true;

                                    connectedRooms.RemoveAt(l);
                                    l--;
                                    if (k > l)
                                    {
                                        k--;
                                    }
                                    reqK = k;
                                    break;

                                    //no break keep looking
                                    //break;
                                }
                            }
                        }
                    }
                    if (/*k >= connectedRooms.Count - 1 &&*/ isFoundK && reqK != -1)
                    {
                        //connectedRoomsThroughCollision.Add(new ConnectedComponent(connectedRoomsThroughCollision[reqK].rooms[0] / 2 + connectedRoomsThroughCollision[reqK].rooms[1] / 2, connectedRoomsThroughCollision[reqK].rooms));

                        GameObject gb = Instantiate(roomIndicator, connectedRoomsThroughCollision[reqK].rooms[0] / 2 + connectedRoomsThroughCollision[reqK].rooms[1] / 2, Quaternion.identity);
                        CorridorNew cn = gb.AddComponent<CorridorNew>();
                        cn.rooms = connectedRoomsThroughCollision[reqK].rooms;
                        //cn.KOrL = "k";

                        connectedRooms.RemoveAt(reqK);
                        //k--;
                        reqK = -1;
                        isFoundK = false;

                        AddConnectedRoomsThroughCollisionToConnectedRooms();

                        //break;//YYYY>????
                    }

                }

            }


        }
        


        connectedRoomsThroughCollision = temp;

    } 

    private bool AddAndRemoveConnectedRooms()
    {
        isDonePrevFnCall = false;
        connectedRoomsThroughCollision = connectedRoomsThroughCollision.Distinct().ToList();
        //connectedRoomsThroughCollision = connectedRoomsThroughCollision.Distinct().ToList();  //NEEDED????????

        //check the below for loop with //Debug
        // -------------- In List connectedRoomsThroughCollision, Keep only the elements which are in the same collision as connectedRoomsThroughCollision[0] --------------
        temp = new List<ConnectedComponent>();
        for (int i = 1; i < connectedRoomsThroughCollision.Count; i++)
        {
            if(connectedRoomsThroughCollision[0].corridorPos != connectedRoomsThroughCollision[i].corridorPos)
            {
                temp.Add(connectedRoomsThroughCollision[i]);
                connectedRoomsThroughCollision.RemoveAt(i);
                i--;
            }
        }
        


        ////Debug.Log("AddAndRemoveConnectedRooms iteration number" + ctr);

        //TEST
        //Number of distinct components B4
        ////Debug.Log("connectedRooms B4 connecting process = " + connectedRooms.Count);
        //displayConnectedRooms();



        // -------------- Now we are taking an element of connectedRoomsThroughCollision (collidedConnectedRoomToSearch)  -------------- 
        // -------------- and comparing it with every element of connectedRooms -------------- 
        // -------------- and adding the req ones to connectedRoomsThroughCollision --------------
        // -------------- and removing the same ones from connectedRooms --------------

        CompareAndAdd();



        // Now add a list to end of connectedRooms (which has less elements now)
        // the new list is the whole of connectedRoomsThroughCollision (to which more elements have been added)


        ////Debug.Log("connectedRoomsThroughCollision B4 adding connectedRoomsThroughCollision to connectedRooms[connectedRooms.Count - i]");
        //displayConnectedRoomsThroughCollision();


        AddConnectedRoomsThroughCollisionToConnectedRooms();



        //Then append the rest to connectedRooms[connectedRooms.Count - 1]
        /*for (int i = 1; i < connectedRoomsThroughCollision.Count && connectedRooms.Count - i > 0; i++)
        {
            connectedRooms[connectedRooms.Count - i].AddRange(connectedRoomsThroughCollision[i].rooms);
        }
        */


        // -------------- Display -------------
        //Debug.Log("ctr = " + ctr);
        displayConnectedRoomsThroughCollision();


        //Display temp, the list being kept back to revisit after execution of this whole function
        int z = 1;
        foreach (var item in temp)
        {
            //Debug.Log("Tempppp No. " + z + "item.Count = " + item.rooms.Count);
            foreach (var item1 in item.rooms)
            {
                //Debug.Log(item1);
            }
            z++;
        }




        if (counter1 > 100)
        {
            counter1 = 0;
            isDonePrevFnCall = true;
            isFinishedAddAndRemoveConnectedRooms = true;
            return true;
        }
        counter1++;

        //Resetting the List
        connectedRoomsThroughCollision = temp;
        if(connectedRoomsThroughCollision.Count == 0)
        {
            isDonePrevFnCall = true;
            isFinishedAddAndRemoveConnectedRooms = true;
            return true;
        }
        else
        {
            ctr++;
            return false;
        }
        
        

        //Number of distinct components
        ////Debug.Log("ConnectedRooms after adding connectedRoomsThroughCollision to connectedRooms[connectedRooms.Count - i]" + connectedRooms.Count);
        //displayConnectedRooms();
        

    }

    private void CompareAndAdd()
    {
        List<int> indices = new List<int>();

        for (int i = 0; i < connectedRoomsThroughCollision.Count; i++)
        {
            for (int j = 0; j < connectedRoomsThroughCollision[i].rooms.Count; j++)
            {
                for (int k = 0; k < connectedRooms.Count; k++)
                {
                    for (int q = 0; q < connectedRooms[k].Count; q++)
                    {
                        // -------------- Now we are taking an element of connectedRoomsThroughCollision (collidedConnectedRoomToSearch)  -------------- 
                        // -------------- and comparing it with every element of connectedRooms -------------- 
                        // -------------- and adding the req ones to connectedRoomsThroughCollision --------------
                        // -------------- and removing the same ones from connectedRooms --------------
                        if (connectedRoomsThroughCollision[i].rooms[j] == connectedRooms[k][q])
                        {
                            indices.Add(k);
                            connectedRoomsThroughCollision.Add(new ConnectedComponent(/*connectedRoomsThroughCollision[i].corridorPos*/ new Vector3(1, 1, 1), connectedRooms[k]));
                            connectedRooms.RemoveAt(k);
                            k--;
                            break;
                        }
                    }
                }
            }
        }
    }

    private void AddConnectedRoomsThroughCollisionToConnectedRooms()
    {
        connectedRoomsThroughCollision = connectedRoomsThroughCollision.Distinct().ToList();  //NEEDED????????

        for (int i = 1; i < connectedRoomsThroughCollision.Count; i++)
        {
            connectedRoomsThroughCollision[0].rooms.AddRange(connectedRoomsThroughCollision[i].rooms);
        }

        //Removes duplicates in the only List object (of type ConnectedComponent) left
        if (connectedRoomsThroughCollision.Count > 0) // Check corner case later!!!!!!!!!
        {
            connectedRoomsThroughCollision[0].rooms = connectedRoomsThroughCollision[0].rooms.Distinct().ToList();
        }

        connectedRoomsThroughCollision.RemoveRange(1, connectedRoomsThroughCollision.Count - 1);

        //B4 adding

        ////Debug.Log("connectedRoomsThroughCollision B4 adding connectedRoomsThroughCollision to connectedRooms[connectedRooms.Count - i]");
        //displayConnectedRoomsThroughCollision();

        ////Debug.Log("connectedRooms B4 adding connectedRoomsThroughCollision to connectedRooms[connectedRooms.Count - i]");
        //displayConnectedRooms();

        //Add connectedRoomsThroughCollision[0] to end of connectedRooms
        connectedRooms.Add(connectedRoomsThroughCollision[0].rooms);

    }


    private void displayConnectedRoomsThroughCollision()
    {
        int z = 1;
        foreach (var item in connectedRoomsThroughCollision)
        {
            //Debug.Log("ConnectedRoomsThroughCollision No. " + z + "item.Count = " + item.rooms.Count);
            foreach (var item1 in item.rooms)
            {
                //Debug.Log(item1);
            }
            z++;
        }
    }

    private void displayConnectedRooms()
    {
        int z = 1;
        foreach (var item in connectedRooms)
        {
            //Debug.Log("ConnectedRooms No. " + z);
            foreach (var item1 in item)
            {
                //Debug.Log(item1);
            }
            z++;
        }
    }

    private void ResolveAdjacentRooms()
    {

    }


    private void CheckForCollision()
    {
        //Debug.Log("Count Olaf =" + collidedCorridors.Count);


        //FindDuplicates(); //use this to group corridors at the same place use ConvertToOpenings and Linq.Distinct and do the necessary


        //Debug.Log("----------------------wargarsg----------------------");
        for (int i = 0; i < collidedCorridors.Count; i++)
        {
            if (collidedCorridors[i] == null)
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

                ////Debug.Log(collidedCorridors[i].transform.position);
                ////Debug.Log(collidedCorridors[j].transform.position);
                bool isError = false;

                if (collidedCorridors[i].transform.position == collidedCorridors[j].transform.position)
                //if(Mathf.Abs(collidedCorridors[i].transform.position.x - collidedCorridors[j].transform.position.x) <= 0.6f
                //    && Mathf.Abs(collidedCorridors[i].transform.position.z - collidedCorridors[j].transform.position.z) <= 0.6f)
                {
                    //Make condition perfect er

                    if (collidedCorridors[i].transform.parent.name.Equals(collidedCorridors[j].transform.parent.name)
                        && (collidedCorridors[i].transform.rotation == collidedCorridors[j].transform.rotation))
                    {
                        ////Debug.Log("Leave");

                    }
                    else if (!isNotFirstTime)
                    {

                        ////Debug.Log("in at " + collidedCorridors[i].transform.position);
                        ////Debug.Log(collidedCorridors[i].transform.parent.name + " " + collidedCorridors[i].transform.rotation.eulerAngles);
                        ////Debug.Log(collidedCorridors[j].transform.parent.name + " " + collidedCorridors[j].transform.rotation.eulerAngles);
                        List<int> openings1 = new List<int>(), openings2 = new List<int>();
                        openings1 = ConvertToOpenings(collidedCorridors[i].transform.parent.tag, collidedCorridors[i].transform.rotation.eulerAngles.y);
                        openings2 = ConvertToOpenings(collidedCorridors[j].transform.parent.tag, collidedCorridors[j].transform.rotation.eulerAngles.y);
                        ////Debug.Log(openings1[0] + " " + openings1[1]);
                        ////Debug.Log(openings2[0] + " " + openings2[1]);
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
                        //Debug.Log("Before combining");
                        //Debug.Log("openings1");
                        /*
                        foreach (var item in openings1)
                        {
                            Debug.Log(item);
                        }
                        Debug.Log("openings2");
                        foreach (var item in openings2)
                        {
                            Debug.Log(item);
                        }
                        */
                        openings1.AddRange(openings2);


                        openings1 = openings1.Distinct().ToList();
                        /*
                        Debug.Log("After combining");
                        foreach (var item in openings1)
                        {
                            Debug.Log(item);
                        }
                        */
                        isError = false;

                        if (openings1.Count == 3)
                        {
                            float yRotation = ConvertToRotation(openings1);
                            GameObject currCorridor = Instantiate((yRotation == 0 || yRotation == 270 || yRotation == -90) ? corridorT2 : corridorT1, collidedCorridors[j].transform.parent.transform.position, Quaternion.identity);
                            if (yRotation == 270 || yRotation == -90 || yRotation == 180)
                            {
                                //MeshCollider bc = currCorridor.GetComponentInChildren<MeshCollider>();
                                //Destroy(bc);
                                currCorridor.transform.GetChild(0).localScale = new Vector3(-1, 1, 1);
                                //currCorridor.transform.Find("CollisionDetector").gameObject.AddComponent<MeshCollider>().size = new Vector3(1, 0.5f, 1);
                            }
                            currCorridor.transform.rotation = Quaternion.Euler(0, yRotation, 0);
                            //Debug.Log("added T at " + currCorridor.transform.position + " with yRot " + yRotation + " and scale " + currCorridor.transform.localScale);
                        }
                        else if (openings1.Count == 4)
                        {
                            Instantiate(corridorX, collidedCorridors[j].transform.parent.transform.position, Quaternion.identity);
                        }
                        else
                        {
                            //Debug.Log("Error!!!!!!!!!!!!!!!!!!!!!!!!!");
                            isError = true;
                            Debug.Log("Count = " + openings1.Count);
                            Debug.Log("Position = " + collidedCorridors[i].transform.position);
                            //Debug.Log("rotation = " + collidedCorridors[j].transform.rotation.eulerAngles.y);
                            //Debug.Log("parent name " + collidedCorridors[j].transform.parent.name);
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
                        //Debug.Log("Destroying " + collidedCorridors[i].transform.parent);
                        if(!isError)
                            Destroy(collidedCorridors[i].transform.parent.gameObject);
                    }

                    //Debug.Log("Destroying " + collidedCorridors[j].transform.parent);
                    if(!isError)
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

                    if (!isNotFirstTime && i > j)
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

        //Debug.Log("Count Olaf AFTER =" + collidedCorridors.Count);
        for (int q = 0; q < collidedCorridors.Count; q++)
        {
            ////Debug.Log(collidedCorridors[q].transform.position + " " + collidedCorridors[q].transform.parent.name);
        }

        if (isDonePrevFnCall && connectedRoomsThroughCollision.Count != prevCount)
        {
            //AddConnectedRooms(connectedRoomsThroughCollision, false);
            //AddAndRemoveConnectedRooms();
            //ctr++;
        }

        prevCount = connectedRoomsThroughCollision.Count;

        isFinishedCheckCollisions = true;
        
    }

    public IEnumerator DoConnectedComponents()
    {
        yield return new WaitUntil(() => isFirstPassDone == true);
        isFirstPassDone = false;
        yield return new WaitForSeconds(2f);
        while (true)
        {
            if (Time.time - startTime >= 3f && isOnce) //&& (count >= 5 || collidedCorridors.Count == 0)))
            {
                isOnce = false;
                //Debug.Log("B444444444444444444444444444444444444444");
                if (connectedRoomsThroughCollision.Count != 0)
                {

                    while (!AddAndRemoveConnectedRooms()) ;

                    yield return new WaitUntil(() => isFinishedAddAndRemoveConnectedRooms = true);
                    isFinishedAddAndRemoveConnectedRooms = false;
                    ctr++;
                }
                //Debug.Log("AFTERRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRR");


                roomsArray = GameObject.FindGameObjectsWithTag("Room");

                
                

                int z = 1;

                foreach (var item in connectedRooms)
                {
                    //Debug.Log("LLLLLAAAASTSSTTTTTTT ConnectedRooms No. " + z);
                    foreach (var item1 in item)
                    {
                        //Debug.Log(item1);
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

                yield return new WaitForSeconds(2.0f);

                for (int i = 0; i + 1 < connectedRooms.Count; i += 2)
                {
                    //Find doors with parent pos same as req and door pos also same in another fn to reduce GC
                    //Debug.Log("roomPos = " + connectedRooms[i][0] + " i = " + i);
                    if (connectedRooms[i].Count == 0)
                    {
                        Debug.Log("Error1");
                    }
                    if (connectedRooms[i + 1].Count == 0)
                    {
                        Debug.Log("Error2");
                    }
                    if (connectedRooms[i].Count != 0 && connectedRooms[i + 1].Count != 0)
                    {
                        Transform door0 = FindDoor(connectedRooms[i][0]);
                        //Debug.Log("roomPos = " + connectedRooms[i + 1][0] + " i + 1 = " + i);
                        Transform door1 = FindDoor(connectedRooms[i + 1][0]);
                        //Debug.Log(door0.parent.position + " " + door1.parent.position);
                        roomNewScript.ConnectTwoRooms(door0.position, door1.position, door0.name, door1.name, door0.parent.position, door1.parent.position, true, i + 1);
                    }

                }

                isConnectedComponentsCheckDone = true;
            }
            yield return new WaitForSeconds(2.0f);
        }
    }

    private Transform FindDoor(Vector3 roomPos)
    {
        for (int i = 0; i < roomsArray.Length; i++)
        {
            //Debug.Log(roomsArray[i].transform.position);
            if(roomsArray[i].transform.position == roomPos)
            {
                //Found the room
              //  Debug.Log("found = " + roomsArray[i].name);
                return roomsArray[i].transform.GetChild(1);
            }
        }
        return null;
    }

    public IEnumerator DoVents()
    {
        yield return new WaitUntil(() => isConnectedComponentsCheckDone == true);
        yield return new WaitForSeconds(1.0f);
        isStartedVents = true;
        //Do Vents here! Do it Morty!

        //Re initialising variables for using with vents!!! Morty stick it up your a**!!
        isCollided = false;
        collisionCount = 0;
        connectedRoomsThroughCollision = new List<ConnectedComponent>();
        collidedCorridors = new List<GameObject>();

        roomNewScript.StartItUp();

    }

}
