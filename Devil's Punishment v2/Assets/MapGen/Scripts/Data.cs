using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Data : MonoBehaviour
{
    public GameObject roomsLoaderPrefab;
    public Transform mapGenHolderTransform;

    public static Data instance = null;
    public ArrayList allRooms = new ArrayList();
    public float xSize, zSize, corridorSize = 4;
    //public int collisionCount = 0, corridorCount = 0;
    //public bool isCollided = false;
    public List<GameObject> collidedCorridors = new List<GameObject>();
    public List<GameObject> collidedVents = new List<GameObject>();

    public float startTime;
    private bool isNotFirstTime = false;
    private bool isNotFirstTimeVents = false;

    ///<summary>
    ///The direction of the opening for the L corridor near door of type stored in this list
    ///</summary>
    public List<string> nearDoorL = new List<string>();
    public GameObject corridorT1, corridorT2, corridorX, ventT, ventX;

    public Dictionary<Vector3, int> corridorPosDict = new Dictionary<Vector3, int>();

    public List<ConnectedComponent> connectedRoomsThroughCollision = new List<ConnectedComponent>();

    public List<List<Vector3>> connectedRooms = new List<List<Vector3>>();
    public List<List<Vector3>> connectedRoomsVents = new List<List<Vector3>>();

    public Vector3 spawnPointsFirstPos;

    public int count = 0;

    public GameObject roomIndicator;

    public int ctr = 0;

    public bool isOnce = true, isDonePrevFnCall = true;

    public int prevCount = 0;

    public int counter = 0;
    public int counter1 = 0;

    public List<ConnectedComponent> temp = new List<ConnectedComponent>();

    public bool isFinishedCheckCollisions = false, isFinishedCheckCollisionsVents = false, isFinishedAddAndRemoveConnectedRooms = false, isConnectedComponentsCheckDone = false;
    
    public RoomNew roomNewScript;

    private GameObject[] roomsArray;

    private bool isFirstPassDone = false;

    public int ctr1 = 0;

    public PlayerController_Revamped playerController;

    public bool canStartCorridorTestSpawner = false;

    public List<string> doorRotationHelper = new List<string>();

    public ModularRoomAssembler modularRoomAssembler;
    public List<GameObject> roomsFloor1Modular;


    //public bool isPipeAtLeft = true;

    public bool isDoneConnectTwoRooms = false;

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
    }

    private void Start()
    {
        modularRoomAssembler = GetComponent<ModularRoomAssembler>();
        roomsFloor1Modular = new List<GameObject>();

        startTime = Time.time;
        nearDoorL.Add("-z");
        nearDoorL.Add("-x");
        nearDoorL.Add("+z");
        nearDoorL.Add("+x");
        prevCount = connectedRoomsThroughCollision.Count;

        doorRotationHelper.Add("+z");
        doorRotationHelper.Add("+x");
        doorRotationHelper.Add("-z");
        doorRotationHelper.Add("-x");

        //StartCoroutine(DoCheckPerSecond());
        //StartCoroutine(DoConnectedComponents());
    }

    public void StartInstantiateCo()
    {
        StartCoroutine(InstantiateRoomsLoader());
    }

    private IEnumerator InstantiateRoomsLoader()
    {
        Debug.Log("123456712345");
        yield return new WaitForSeconds(5f);
        Debug.Log("345");
        Instantiate(roomsLoaderPrefab);
        Debug.Log("123456712234567345");
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
    public List<int> ConvertToOpenings(string tag, float yRotation, bool isNegativeScale)
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

        //Adding scale -1 case
        if (isNegativeScale)
        {
            if ((int)(yRotation / 90) == 0 || (int)(yRotation / 90) == 2)
            {
                int onesToAdd = 0, threesToAdd = 0;
                for (int i = 0; i < openings.Count; i++)
                {
                    if (openings[i] == 1)
                    {
                        openings.RemoveAt(i);
                        i--;
                        threesToAdd++;
                        //openings.Add(3);
                    }
                    else if (openings[i] == 3)
                    {
                        openings.RemoveAt(i);
                        i--;
                        onesToAdd++;
                        //openings.Add(1);
                    }
                }
                for (int i = 0; i < onesToAdd; i++)
                {
                    openings.Add(1);
                }
                for (int i = 0; i < threesToAdd; i++)
                {
                    openings.Add(3);
                }
            }
            else if ((int)(yRotation / 90) == 1 || (int)(yRotation / 90) == 3)
            {
                int zerosToAdd = 0, twosToAdd = 0;
                for (int i = 0; i < openings.Count; i++)
                {
                    if (openings[i] == 0)
                    {
                        openings.RemoveAt(i);
                        i--;
                        twosToAdd++;
                        //openings.Add(2);
                    }
                    else if (openings[i] == 2)
                    {
                        openings.RemoveAt(i);
                        i--;
                        zerosToAdd++;
                        //openings.Add(0);
                    }
                }
                for (int i = 0; i < zerosToAdd; i++)
                {
                    openings.Add(0);
                }
                for (int i = 0; i < twosToAdd; i++)
                {
                    openings.Add(2);
                }
            }
        }

        return openings;
    }

    public List<int> ConvertToOpeningsVents(string tag, float yRotation, float holderZRotation, float holderXRotation)
    {
        List<int> openings = new List<int>();
        if (tag.Equals("VentI"))
        {
            openings.Add((int)(yRotation / 90f));
            openings.Add(openings[0] + 2);
            ////Debug.Log(openings[0] + " " + openings[1]);
        }
        else if (tag.Equals("VentL"))
        {
            if (yRotation == 270 || yRotation == -90)
            {
                if (holderZRotation == 0)
                {
                    openings.Add(0);
                }
                else
                {
                    openings.Add(-2);
                }
                openings.Add(3);
            }
            else
            {
                openings.Add((int)(yRotation / 90f));
                if (holderZRotation == 0)
                {
                    openings.Add(openings[0] + 1);
                }
                else
                {
                    openings.Add(-2);
                }
            }
        }
        else if (tag.Equals("VentT"))
        {
            List<int> oneToFour = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                oneToFour.Add(i);
            }
            int rot = (int)(yRotation / 90f);
            oneToFour.Remove(rot);
            if(holderXRotation != 0)
            {
                oneToFour.Remove((rot + 2) % 4);
                openings.Add(-2);
            }
            openings.AddRange(oneToFour);
        }
        else if (tag.Equals("VentX"))
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

    public IEnumerator DoCheckVentsPerSecond()
    {
        /*
        //for putting corridors so that connected components does correctly
        for (int i = 0; i < 3; i++)
        {
            CheckForCollisionVents();
            yield return new WaitUntil(() => isFinishedCheckCollisions = true);
            isFinishedCheckCollisions = false;
        }
        isFirstPassDone = true;

        yield return new WaitUntil(() => isConnectedComponentsCheckDone == true);
        */
        yield return new WaitForSeconds(5f);
        float startTime1 = Time.time;
        while (true)
        {
            if (Time.time - startTime1 >= 10f)
            {
                //RemoveStrayVentCovers();
                break;
            }

            Debug.Log(collidedVents.Count + " " + count + "#################################");
            if (count < 6 /*&& collidedCorridors.Count != 0*/)
            {
                Debug.Log("$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$4");
                
                {
                    Debug.Log("innnnnnnnnnnnnn");
                    CheckForCollisionVents();
                    count++;
                    yield return new WaitUntil(() => isFinishedCheckCollisionsVents = true);
                    isFinishedCheckCollisionsVents = false;
                }
            }
            else
            {
                break;
                //RemoveStrayVentCovers();
            }

            MapgenProgress.instance.addProgress(1);
            yield return new WaitForSeconds(1f);
        }
	MapgenProgress.instance.loadedMap(); // done!
    }

    public void NumberOfVentCoversInScene()
    {
        Debug.LogError("No of vent covers" + GameObject.FindGameObjectsWithTag("Vent Cover").Length);
    }

    public void RemoveStrayVentCovers()
    {
        NumberOfVentCoversInScene();
        Debug.LogError("removing stray vents" + collidedVents.Count);
        for (int i = 0; i < collidedVents.Count; i++)
        {
            Destroy(collidedVents[i].transform.parent.gameObject); //REMOVES VENTS NOT VENT COVERS xd
        }
        collidedVents.Clear();
        NumberOfVentCoversInScene();
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

            Debug.Log(collidedCorridors.Count + " " + count + "#################################");
            if (count < 6 /*&& collidedCorridors.Count != 0*/)
            {
                Debug.Log("$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$4");

                MapgenProgress.instance.addProgress(4);

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
                    MapgenProgress.instance.addProgress(1);
                    Debug.Log("innnnnnnnnnnnnn");
                    CheckForCollision();
                    count++;
                    yield return new WaitUntil(() => isFinishedCheckCollisions = true);
                    isFinishedCheckCollisions = false;
                }
            }
            else
            {
                break;
            }
            yield return new WaitForSeconds(1f);
        }
        MapgenProgress.instance.addProgress(3);
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
                                    /*
                                    GameObject gb = Instantiate(roomIndicator, connectedRooms[l][0] / 2 + connectedRooms[l][1] / 2, Quaternion.identity, mapGenHolderTransform);
                                    CorridorNew cn = gb.AddComponent<CorridorNew>();
                                    cn.rooms = connectedRooms[l];
                                    cn.KOrL = "l";
                                    cn.theEqualOnes.Add(connectedRoomsThroughCollision[k].rooms[0]);
                                    cn.theEqualOnes.Add(connectedRoomsThroughCollision[k].rooms[1]);
                                    */
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
                                MapgenProgress.instance.addProgress(1);
                            }
                        }
                    }
                    if (/*k >= connectedRooms.Count - 1 &&*/ isFoundK && reqK != -1)
                    {
                        //connectedRoomsThroughCollision.Add(new ConnectedComponent(connectedRoomsThroughCollision[reqK].rooms[0] / 2 + connectedRoomsThroughCollision[reqK].rooms[1] / 2, connectedRoomsThroughCollision[reqK].rooms));
                        /*
                        GameObject gb = Instantiate(roomIndicator, connectedRoomsThroughCollision[reqK].rooms[0] / 2 + connectedRoomsThroughCollision[reqK].rooms[1] / 2, Quaternion.identity, mapGenHolderTransform);
                        CorridorNew cn = gb.AddComponent<CorridorNew>();
                        cn.rooms = connectedRoomsThroughCollision[reqK].rooms;
                        cn.KOrL = "k";
                        */
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
        /*
        foreach (var item in temp)
        {
            //Debug.Log("Tempppp No. " + z + "item.Count = " + item.rooms.Count);
            foreach (var item1 in item.rooms)
            {
                Debug.Log(item1);
            }
            z++;
        }
        */




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
        /*
        foreach (var item in connectedRoomsThroughCollision)
        {
            //Debug.Log("ConnectedRoomsThroughCollision No. " + z + "item.Count = " + item.rooms.Count);
            foreach (var item1 in item.rooms)
            {
                Debug.Log(item1);
            }
            z++;
        }
        */
    }

    private void displayConnectedRooms()
    {
        int z = 1;
        /*
        foreach (var item in connectedRooms)
        {
            //Debug.Log("ConnectedRooms No. " + z);
            foreach (var item1 in item)
            {
                Debug.Log(item1);
            }
            z++;
        }
        */
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

                //if (collidedCorridors[i].transform.position == collidedCorridors[j].transform.position)
                if(Mathf.Abs(collidedCorridors[i].transform.position.x - collidedCorridors[j].transform.position.x) <= 0.6f
                    && Mathf.Abs(collidedCorridors[i].transform.position.z - collidedCorridors[j].transform.position.z) <= 0.6f)
                {

                    //bool isErroneousTCorr = false;
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
                        openings1 = ConvertToOpenings(collidedCorridors[i].transform.parent.tag, collidedCorridors[i].transform.rotation.eulerAngles.y,
                                                        (collidedCorridors[i].transform.parent.localScale.x == -1) ? true : false);
                        openings2 = ConvertToOpenings(collidedCorridors[j].transform.parent.tag, collidedCorridors[j].transform.rotation.eulerAngles.y,
                                                        (collidedCorridors[j].transform.parent.localScale.x == -1) ? true : false);
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

                        /*
                        Debug.Log("Before combining");
                        Debug.Log("openings1");

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
                            Vector3 spawnAtPos = collidedCorridors[j].transform.parent.transform.position;
                            spawnAtPos.x = Mathf.Round(spawnAtPos.x);
                            spawnAtPos.z = Mathf.Round(spawnAtPos.z);
                            GameObject currCorridor = Instantiate((yRotation == 0 || yRotation == 270 || yRotation == -90) ? corridorT2 : corridorT1, spawnAtPos, Quaternion.identity, mapGenHolderTransform);
                            MapgenProgress.instance.addProgress(1);
                            if (yRotation == 0)
                            {
                                //isErroneousTCorr = true;
                                currCorridor.transform.GetChild(0).localPosition = new Vector3(0.15f, 0, -0.155f);
                                //currCorridor.transform.localPosition += new Vector3(0, 5, 0);
                            }
                            if (yRotation == 270 || yRotation == -90 || yRotation == 180)
                            {

                                if (yRotation == -90 || yRotation == 270)
                                {
                                    currCorridor.transform.GetChild(0).localPosition = new Vector3(0.156f, 0, -0.156f);
                                }

                                //MeshCollider bc = currCorridor.GetComponentInChildren<MeshCollider>();
                                //Destroy(bc);
                                currCorridor.transform.localScale = new Vector3(-1, 1, 1);
                                //currCorridor.transform.Find("CollisionDetector").gameObject.AddComponent<MeshCollider>().size = new Vector3(1, 0.5f, 1);
                            }
                            
                            currCorridor.transform.rotation = Quaternion.Euler(0, yRotation, 0);
                            // TODO: Commented out for console clear 10/02/19
                           // Debug.Log("added T at " + currCorridor.transform.position + " with yRot " + yRotation + " and scale " + currCorridor.transform.localScale);
                        }
                        else if (openings1.Count == 4)
                        {

                            Vector3 spawnAtPos = collidedCorridors[j].transform.parent.transform.position;
                            spawnAtPos.x = Mathf.Round(spawnAtPos.x);
                            spawnAtPos.z = Mathf.Round(spawnAtPos.z);
                            Instantiate(corridorX, spawnAtPos, Quaternion.identity, mapGenHolderTransform);
                            MapgenProgress.instance.addProgress(2);
                        }
                        else
                        {
                            Debug.Log("Error!!!!!!!!!!!!!!!!!!!!!!!!!");
                            isError = true;
                            Debug.Log("Count = " + openings1.Count);
                            Debug.Log("Position = " + collidedCorridors[i].transform.position);
                            Debug.Log("Name i = " + collidedCorridors[i].transform.parent.name);
                            Debug.Log("Name j = " + collidedCorridors[j].transform.parent.name);

                            foreach (var item in openings1)
                            {
                                Debug.Log(item);
                            }
                            Debug.Log("openings2");
                            foreach (var item in openings2)
                            {
                                Debug.Log(item);
                            }

                            //Debug.Log("rotation = " + collidedCorridors[j].transform.rotation.eulerAngles.y);
                            //Debug.Log("parent name " + collidedCorridors[j].transform.parent.name);
                        }

                        /*
                        //If I and I collides with different rotations
                        if (collidedCorridors[i].transform.rotation != collidedCorridors[j].transform.rotation)
                        {
                            GameObject currCorridor1 = Instantiate(corridorX, collidedCorridors[j].transform.position, Quaternion.identity, mapGenHolderTransform);
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
                        if (!isError)// && !isErroneousTCorr)
                            Destroy(collidedCorridors[i].transform.parent.gameObject);
                    }

                    //Debug.Log("Destroying " + collidedCorridors[j].transform.parent);
                    if(!isError)// && !isErroneousTCorr)
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
        /*
        Debug.Log("Count Olaf AFTER =" + collidedCorridors.Count);
        for (int q = 0; q < collidedCorridors.Count; q++)
        {
            Debug.Log(collidedCorridors[q].transform.position + " " + collidedCorridors[q].transform.parent.name);
        }

        if (isDonePrevFnCall && connectedRoomsThroughCollision.Count != prevCount)
        {
            //AddConnectedRooms(connectedRoomsThroughCollision, false);
            //AddAndRemoveConnectedRooms();
            //ctr++;
        }

        prevCount = connectedRoomsThroughCollision.Count;
        */
        isFinishedCheckCollisions = true;
        
    }

    public IEnumerator DoConnectedComponents()
    {
        yield return new WaitUntil(() => isFirstPassDone == true);
        isFirstPassDone = false;
        yield return new WaitForSeconds(2f);
        while (isOnce)
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



                /*
                int z = 1;

                foreach (var item in connectedRooms)
                {
                    //Debug.Log("LLLLLAAAASTSSTTTTTTT ConnectedRooms No. " + z);
                    foreach (var item1 in item)
                    {
                        //Debug.Log(item1);
                        GameObject gb = Instantiate(roomIndicator, item1, Quaternion.identity, mapGenHolderTransform);
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
                */
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
                        int j = 0, k = 0;
                        while(j < connectedRooms[i].Count && j < connectedRooms[i + 1].Count 
                            && k < connectedRooms[i].Count && k < connectedRooms[i + 1].Count)
                        {
                            Transform door0 = FindDoor(connectedRooms[i][j]);
                            //Debug.Log("roomPos = " + connectedRooms[i + 1][0] + " i + 1 = " + i);
                            Transform door1 = FindDoor(connectedRooms[i + 1][k]);
                            if (door0 != null && door1 != null)
                            {
                                Vector3 door0Pos = door0.position;
                                door0Pos.x = Mathf.Round(door0Pos.x);
                                door0Pos.z = Mathf.Round(door0Pos.z);

                                Vector3 door1Pos = door1.position;
                                door1Pos.x = Mathf.Round(door1Pos.x);
                                door1Pos.z = Mathf.Round(door1Pos.z);

                                //Debug.Log(door0.parent.position + " " + door1.parent.position);
                                StartCoroutine(roomNewScript.ConnectTwoRooms(door0Pos, door1Pos, door0.name, door1.name, door0.parent.position, door1.parent.position, true));
                                break;
                            }
                            else
                            {
                                if(door0 == null)
                                {
                                    j++;
                                }
                                else
                                {
                                    k++;
                                }
                                /*
                                Debug.LogError("null" + ((door0 == null) ? " door0" : " door1"));
                                Debug.LogError(connectedRooms[i][j]);
                                Debug.LogError(connectedRooms[i + 1][k]);
                                Debug.LogError(roomsArray.Length);
                                for (int l = 0; l < roomsArray.Length; l++)
                                {
                                    Debug.LogError(roomsArray[l].transform.position);
                                }
                                */
                            }
                        }
                        
                    }

                }
                //Debug.LogError(ctr1);
                //Debug.LogError(ctr2);
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
            if(roomsArray[i].transform.position.x == roomPos.x 
                && roomsArray[i].transform.position.z == roomPos.z)
            {
                //Found the room
                Debug.Log("found = " + roomsArray[i].name + " childCount = " + roomsArray[i].transform.childCount + " " + roomsArray[i].transform.position);
                return roomsArray[i].transform.GetChild(1);
            }
        }
        Debug.Log("Didnt find");
        return null;
    }

    private void CheckForCollisionVents()
    {
        //Debug.Log("Count Olaf =" + collidedVents.Count);

        //FindDuplicates(); //use this to group corridors at the same place use ConvertToOpenings and Linq.Distinct and do the necessary

        //Debug.Log("----------------------wargarsg----------------------");
        for (int i = 0; i < collidedVents.Count; i++)
        {
            if (collidedVents[i] == null)
            {
                collidedVents.RemoveAt(i);
                i--;
                continue;
            }
            for (int j = 0; /*j < i + 4 &&*/ j < collidedVents.Count; j++)
            {
                if (i == j) continue;

                if (collidedVents[j] == null)
                {
                    collidedVents.RemoveAt(j);
                    if (i > j)
                    {
                        i--;
                    }
                    j--;
                    continue;
                }

                /*
                if (collidedVents[i] == null)
                {
                    collidedVents.RemoveAt(i);
                    i--;
                    continue;
                }
                */

                ////Debug.Log(collidedVents[i].transform.position);
                ////Debug.Log(collidedVents[j].transform.position);
                bool isError = false;

                //if (collidedVents[i].transform.position == collidedVents[j].transform.position)
                if (Mathf.Abs(collidedVents[i].transform.position.x - collidedVents[j].transform.position.x) <= 0.6f
                    && Mathf.Abs(collidedVents[i].transform.position.z - collidedVents[j].transform.position.z) <= 0.6f)
                {
                    //Make condition perfect er

                    if (collidedVents[i].transform.parent.name.Equals(collidedVents[j].transform.parent.name)
                        && (collidedVents[i].transform.rotation == collidedVents[j].transform.rotation))
                    {
                        ////Debug.Log("Leave");

                    }
                    else if (!isNotFirstTimeVents) // why the variable
                    {

                        //Debug.Log("QWERTY in at " + collidedVents[i].transform.position);
                        ////Debug.Log(collidedVents[i].transform.parent.name + " " + collidedVents[i].transform.rotation.eulerAngles);
                        ////Debug.Log(collidedVents[j].transform.parent.name + " " + collidedVents[j].transform.rotation.eulerAngles);
                        List<int> openings1 = new List<int>(), openings2 = new List<int>();

                        openings1 = ConvertToOpeningsVents(collidedVents[i].transform.parent.tag, collidedVents[i].transform.rotation.eulerAngles.y, 
                            collidedVents[i].transform.parent.GetChild(0).localEulerAngles.z, collidedVents[i].transform.parent.GetChild(0).localEulerAngles.x);

                        openings2 = ConvertToOpeningsVents(collidedVents[j].transform.parent.tag, collidedVents[j].transform.rotation.eulerAngles.y, 
                            collidedVents[j].transform.parent.GetChild(0).localEulerAngles.z, collidedVents[j].transform.parent.GetChild(0).localEulerAngles.x);
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

                        /*
                        Debug.Log("Before combining");
                        Debug.Log("openings1");

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
                            openings1.Sort();
                            bool isThereVentCoverAbove = false;
                            Debug.Log("========");
                            /*
                            foreach (var item in openings1)
                            {
                                Debug.Log(item);
                            }
                            */
                            if (openings1[0] == -2)
                            {
                                isThereVentCoverAbove = true;
                                openings1.RemoveAt(0);
                                openings1.Add((openings1[1] + 1) % 4);
                            }
                            /*
                            foreach (var item in openings1)
                            {
                                Debug.Log(item);
                            }
                            */
                            float yRotation = ConvertToRotation(openings1);
                            Vector3 spawnAtPos = collidedVents[j].transform.parent.transform.position;
                            spawnAtPos.x = Mathf.Round(spawnAtPos.x);
                            spawnAtPos.z = Mathf.Round(spawnAtPos.z);
                            GameObject currCorridor = Instantiate(ventT, spawnAtPos, Quaternion.identity, mapGenHolderTransform);
                            MapgenProgress.instance.addProgress(2);
                            /*
                            if (yRotation == 0)
                            {
                                currCorridor.transform.GetChild(0).localPosition = new Vector3(0.15f, 0, -0.155f);
                            }
                            if (yRotation == 270 || yRotation == -90 || yRotation == 180)
                            {

                                if (yRotation == -90 || yRotation == 270)
                                {
                                    currCorridor.transform.GetChild(0).localPosition = new Vector3(0.156f, 0, -0.156f);
                                }

                                //MeshCollider bc = currCorridor.GetComponentInChildren<MeshCollider>();
                                //Destroy(bc);
                                currCorridor.transform.localScale = new Vector3(-1, 1, 1);
                                //currCorridor.transform.Find("CollisionDetector").gameObject.AddComponent<MeshCollider>().size = new Vector3(1, 0.5f, 1);
                            }
                            */
                            if (isThereVentCoverAbove)
                            {
                                currCorridor.transform.GetChild(0).localEulerAngles = new Vector3(90, 0, 0);
                            }
                            currCorridor.transform.rotation = Quaternion.Euler(0, yRotation, 0);
                            //Debug.Log("added T VENT at " + currCorridor.transform.position + " with yRot " + yRotation + " and scale " + currCorridor.transform.localScale);
                        }
                        else if (openings1.Count == 4)
                        {
                            float yRotation = 0;
                            openings1.Sort();
                            bool isThereVentCoverAbove = false;
                            /*Debug.Log("========");
                            foreach (var item in openings1)
                            {
                                Debug.Log(item);
                            }
                            */
                            if (openings1[0] == -2)
                            {
                                isThereVentCoverAbove = true;
                                openings1.RemoveAt(0);
                                if (collidedVents[j].transform.parent.name.EndsWith("L"))
                                {
                                    Destroy(collidedVents[j].transform.parent.parent.gameObject); // DONT DESTROY "New Game Object" (put condition)
                                    collidedVents[j].transform.parent.transform.SetParent(mapGenHolderTransform);//but it will be destroyed anyway later... check performance(meh) THIS may not work.. but its fine for now
                                }
                                else if (collidedVents[i].transform.parent.name.EndsWith("L"))
                                {
                                    Destroy(collidedVents[i].transform.parent.parent.gameObject);
                                    collidedVents[i].transform.parent.transform.SetParent(mapGenHolderTransform);//but it will be destroyed anyway later... check performance(meh) THIS may not work.. but its fine for now
                                }
                                yRotation = ConvertToRotation(openings1);
                            }
                            /*
                            foreach (var item in openings1)
                            {
                                Debug.Log(item);
                            }*/
                            Vector3 spawnAtPos = collidedVents[j].transform.parent.transform.position;
                            spawnAtPos.x = Mathf.Round(spawnAtPos.x);
                            spawnAtPos.z = Mathf.Round(spawnAtPos.z);
                            Instantiate((isThereVentCoverAbove) ? ventT : ventX, spawnAtPos, Quaternion.Euler(0, yRotation, 0), mapGenHolderTransform);
                        }
                        else
                        {
                            Debug.Log("Error!!!!!!!!!!!!!!!!!!!!!!!!!");
                            isError = true;
                            Debug.Log("Count = " + openings1.Count);
                            Debug.Log("Position = " + collidedVents[i].transform.position);
                            //Debug.Log("rotation = " + collidedVents[j].transform.rotation.eulerAngles.y);
                            //Debug.Log("parent name " + collidedVents[j].transform.parent.name);
                        }
                        MapgenProgress.instance.addProgress(2);
                        /*
                        //If I and I collides with different rotations
                        if (collidedVents[i].transform.rotation != collidedVents[j].transform.rotation)
                        {
                            GameObject currCorridor1 = Instantiate(ventX, collidedVents[j].transform.position, Quaternion.identity, mapGenHolderTransform);
                        }
                        //If I and L collides
                        else if (!collidedVents[i].transform.parent.name.Equals(collidedVents[j].transform.parent.name))
                        {
                            //T
                        }
                        else
                        {

                        }
                        */
                        //Debug.Log("Destroying " + collidedVents[i].transform.parent);
                        if (!isError)
                            Destroy(collidedVents[i].transform.parent.gameObject);
                    }

                    //Debug.Log("Destroying " + collidedVents[j].transform.parent);
                    if (!isError)
                        Destroy(collidedVents[j].transform.parent.gameObject);

                    // !!!!!!!!!!!!!!!!! Take care of collisions that happen when the above corridors (T and X) are instantiated, if any !!!!!!!!!!!!!!!!!
                    if (!isNotFirstTimeVents)
                    {
                        collidedVents.RemoveAt(i);

                        if (j > i)
                        {
                            j--;
                        }

                        i--;
                    }

                    collidedVents.RemoveAt(j);

                    if (!isNotFirstTimeVents && i > j)
                    {
                        i--;
                    }
                    j--;

                    isNotFirstTimeVents = true;
                    break;
                }

            }

            if (isNotFirstTimeVents)
            {
                isNotFirstTimeVents = false;
            }
        }

        if (collidedVents.Count == 1)
        {
            Destroy(collidedVents[0].transform.parent.gameObject);
        }

        /*
        Debug.Log("Count Olaf AFTER =" + collidedVents.Count);
        for (int q = 0; q < collidedVents.Count; q++)
        {
            Debug.Log(collidedVents[q].transform.position + " " + collidedVents[q].transform.parent.name);
        }

        if (isDonePrevFnCall && connectedRoomsThroughCollision.Count != prevCount)
        {
            //AddConnectedRooms(connectedRoomsThroughCollision, false);
            //AddAndRemoveConnectedRooms();
            //ctr++;
        }

        prevCount = connectedRoomsThroughCollision.Count;
        */
        isFinishedCheckCollisionsVents = true;



    }

}
