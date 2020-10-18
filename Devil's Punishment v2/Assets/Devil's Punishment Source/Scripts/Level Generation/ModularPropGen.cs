using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Props
{
    string propName;
    int spawnProb;
    bool isBoxShape;
    int variants;
    GameObject[] versions;

    public Props(string PropName, int SpawnProb, bool IsBoxShape, int Variants, GameObject[] Versions)
    {
        propName = PropName;
        spawnProb = SpawnProb;
        isBoxShape = IsBoxShape;
        variants = Variants;
        versions = Versions;
    }

    public string PropName
    {
        get { return propName; }
        set { propName = value; }
    }
    public int SpawnProb
    {
        get { return spawnProb; }
        set { spawnProb = value; }
    }

    public bool IsBoxShape
    {
        get { return IsBoxShape; }
        set { isBoxShape = value; }
    }

    public int Variants
    {
        get { return variants; }
        set { variants = value; }
    }

    public GameObject[] Versions
    {
        get {  return versions; }
        set { versions = value; }
    }
}

public class WallEdge
{
    Vector3 tilePos;
    Quaternion wallDirection;

    public WallEdge(Vector3 TilePos, Quaternion WallDirection)
    {
        tilePos = TilePos;
        wallDirection = WallDirection;
    }

    public Vector3 TilePos
    {
        get { return tilePos; }
        set { tilePos = value; }
    }

    public Quaternion WallDirection
    {
        get { return wallDirection; }
        set { wallDirection = value; }
    }
}

public class ModularPropGen : MonoBehaviour
{

    /*Todo: Fix rotation of objects spawning around beds        SpawnBeds()
     * Add spawning of other objects such as IV's to hospital room spawner
     */

    public RoomReferencesModular roomReferencesModular;
    Vector3 roomPos;

    int floor;
    public float maxSpawnDist;
    public List<int> floorXsize;
    public List<int> floorZsize;
    public List<Transform> floorHolder;
    private bool bigRoom = false;

    [Header("Spawning Positions")]
    List<GameObject> spawnedProps = new List<GameObject>();

    [Header("Test")]
    public GameObject testGridCube;
    public GameObject testGridPlane;

    /*-------Prototyping Only-------*/

    [Header("   Warehouse random props")]
    public GameObject[] barrelProps;
    public int barrelPropVar;

    public GameObject[] crateProps;
    public int cratePropVar;

    [Header("   Hospital props")]
    public GameObject[] hospitalBeds;
    public GameObject[] hospitalChairs;
    public GameObject[] hospitalMedCab;
    public GameObject[] hospitaltvChairs;
    public GameObject[] hospitaltv;
    public GameObject[] hospitalStretchers;
    public GameObject[] hospitalTrolleys;
    public GameObject[] hospitalLights; 

    [Header("   Common Room props")]
    public GameObject[] commonRoomSofas;
    public GameObject[] commonRoomTVs;
    public GameObject[] commonRoomTables;
    public GameObject[] commonRoomLockers;

    Props[] props;

    //Method to generate props randomly across the room in random positions, only for debugging purposes.
    public void GenerateTestScatterProps()
    {
        Debug.Log("Starting prop gen...");
        
        //Create a Props array to hold the props we are going to be spawning in.
        props = new Props[]
    {
        //You could actually just use the GameObject array you are using's Length value for the variants int variable
        new Props("Barrel", 30, false, barrelPropVar, barrelProps),
        new Props("Crate", 30, false, cratePropVar, crateProps),
    };
        foreach (Vector3 pos in roomReferencesModular.roomFloors)
        {
            //Generate a random chance of spawning objects on that floor section
            int spawnChance = UnityEngine.Random.Range(0, 100);
            //Get a random prop from the prop's variations
            Props propToSpawn = props[UnityEngine.Random.Range(0, props.Length)]; 
            if (spawnChance < propToSpawn.SpawnProb)
            {
                //Create an empty gameObject to act as the center piece for that cluster of props. All prop positions nearby will be spawned relative to this cluster holder.
                GameObject k = new GameObject("PropClusterHolder");
                k.transform.parent = roomReferencesModular.gameObject.transform;
                
                int numProps = UnityEngine.Random.Range(1, 8);

                int propCount = 0;

                for (int i = propCount; i < numProps; i++)
                {
                    //Generate a random distance from the cluster holder and store it.
                    float randomDistance_x = UnityEngine.Random.Range(-maxSpawnDist, maxSpawnDist);
                    float randomDistance_z = UnityEngine.Random.Range(-maxSpawnDist, maxSpawnDist);
                    //Use generated random point nearby cluster holder to send a spawnprop request to the SpawnProps method.
                    SpawnProps(propToSpawn, pos, k, randomDistance_x, randomDistance_z);
                    ++propCount;
                }
            }
        }
        //Any objects that have rigidbodies enabled, will have their physics disabled after 5 seconds in order to spare performance.
        //StartCoroutine(DisableRigidbodiesOnDelay()); moved to GenerateProps()

        //GenerateProps("Hospital");
    }

    private void SpawnProps(Props propToSpawn, Vector3 floorPos, GameObject clusterHolder, float xSpawnPos, float zSpawnPos)
    {
        int versionToSpawn = UnityEngine.Random.Range(0, propToSpawn.Variants);
        Vector3 spawnPos = floorPos + GenerateOffset(xSpawnPos, 3.0f, zSpawnPos);
        //if (Physics.Raycast(spawnPos, Vector3.down, out RaycastHit spot, 10.0f))
        //{
            //if (spot.collider.gameObject.name == "Grill")
            //{
                //Debug.Log("Obj attempted to spawn on vent. Aborting...");
            //} else
            if (PreventOverlap(spawnPos, propToSpawn.Versions[versionToSpawn]))
            {
                //Spawn the prop at the designated location.
                GameObject prop = Instantiate(propToSpawn.Versions[versionToSpawn], floorPos + GenerateOffset(xSpawnPos, 2.25f, zSpawnPos), Quaternion.identity);
                prop.transform.parent = clusterHolder.transform;
                prop.tag = "Prop";

                spawnedProps.Add(prop);

            }
            else
            {
                //Spawn the prop so that it is elevated above whichever prop is blocking its spawn point and add a rigidbody component to the spawned object.
                GameObject prop = Instantiate(propToSpawn.Versions[versionToSpawn], floorPos + GenerateOffset(xSpawnPos, 5.0f, zSpawnPos), Quaternion.identity);
                prop.transform.parent = clusterHolder.transform;
                prop.AddComponent<Rigidbody>();
                prop.tag = "Prop";

                spawnedProps.Add(prop);
                }
            //}
    }

    public void GenerateProps(string room)
    {
        Debug.Log(roomReferencesModular.roomFloors.Count + " total floor tiles");
        List<Vector3> floors = new List<Vector3>();
        floors.AddRange(roomReferencesModular.roomFloors);
        int floorCount = floors.Count;
        int lastFloor = floorCount-=1;

        Debug.Log("Last floor tile index = " + lastFloor);
        //Check which room type is being spawned and assign the props to be used accordingly.
        switch (room)
        {

            case "Hospital":

                List<GameObject> beds = new List<GameObject>();

                props = new Props[] {
                    new Props("Hospital bed", 100, true, hospitalBeds.Length, hospitalBeds),
                    new Props("Hospital chair", 100, true, hospitalChairs.Length, hospitalChairs),
                    new Props("Hospital medcab", 100, true, hospitalMedCab.Length, hospitalMedCab),
                    new Props("Hospital tvChair", 100, false, hospitaltvChairs.Length, hospitaltvChairs),
                    new Props("Hospital tv", 100, false, hospitaltv.Length, hospitaltv),
                    new Props("Hospital stretcher", 100, false, hospitalStretchers.Length, hospitalStretchers),
                    new Props("Hospital trolley", 100, true, hospitalTrolleys.Length, hospitalTrolleys),
                    new Props("Hospital lights", 100, false, hospitalLights.Length, hospitalLights)
                };

                Vector3 startPos;
                Vector3 otherstartPos;
                Transform _floorHolder;
                Transform _otherFloorHolder;
                int floorToUseIndex;
                int otherFloor;
                int floor0Area;
                int floor1Area;

                if (floors.Count < 10)
                {
                    Debug.LogError("Modular room size too small! Room must have at least 10 tiles to support hospital room type");
                    floors.RemoveRange(0, floors.Count);
                }
                else
                {
                    floor0Area = floorXsize[0] * floorZsize[0];
                    floor1Area = floorXsize[1] * floorZsize[1];

                    bool useBothFloors = false;

                    if(floors.Count > 21)
                    {
                        useBothFloors = true;
                        bigRoom = true;
                    }

                    int spawns = UnityEngine.Random.Range(0, 10) <= 8 ? spawns = 2 : spawns = UnityEngine.Random.Range(0, 2);

                    if (floors.Count > 24)
                    {
                        spawns = 2;
                    }

                    //Get the room with the largest area to have the hospital beds generate there.
                    if (floor0Area > floor1Area)
                    {
                        _floorHolder = floorHolder[0];
                        _otherFloorHolder = floorHolder[1];
                        startPos = _floorHolder.localPosition + floors[0] + GenerateOffset(0, 2.5f, 0);
                        otherstartPos = _otherFloorHolder.localPosition + floors[floor0Area] + GenerateOffset(0, 2.5f, 0);
                        if (Physics.Raycast(startPos, Vector3.down, out RaycastHit hit, 5.0f))
                        {
                            Debug.LogWarning(hit.collider.gameObject.name + " was hit by the grill check ray");
                            if (hit.collider.gameObject.name == "Interactable" || hit.collider.gameObject.name == "RenderPlane")
                            {
                                startPos.y -= 2.1f;
                                otherstartPos.y -= 2.1f;
                            }
                        }
                        Debug.DrawRay(startPos, Vector3.down * 5, Color.red, 10.0f);
                        floorToUseIndex = 0;
                        otherFloor = 1;
                        /*
                        for (int i = 0; i < floor0Area; i++)
                        {
                            GameObject t = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            t.transform.position = floors[i] + GenerateOffset(0, 2.5f, 0);
                        }
                        */

                    } else
                    {
                        _floorHolder = floorHolder[1];
                        _otherFloorHolder = floorHolder[0];
                        startPos = _floorHolder.localPosition + floors[floor0Area] + GenerateOffset(0, 2.5f, 0);
                        otherstartPos = _otherFloorHolder.localPosition + floors[0] + GenerateOffset(0, 2.5f, 0);
                        if (Physics.Raycast(startPos, Vector3.down, out RaycastHit hit, 5.0f))
                        {
                            Debug.LogWarning(hit.collider.gameObject.name + " was hit by the grill check ray");
                            if (hit.collider.gameObject.name == "Interactable" || hit.collider.gameObject.name == "RenderPlane")
                            {
                                startPos.y -= 2.1f;
                                otherstartPos.y -= 2.1f;
                            }
                        }
                        Debug.DrawRay(startPos, Vector3.down * 5, Color.red, 10.0f);
                        floorToUseIndex = 1;
                        otherFloor = 0;
                        /*
                        for (int i = floor0Area; i < floors.Count; i++)
                        {
                            GameObject t = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            t.transform.position = floors[i] + GenerateOffset(0, 2.5f, 0);
                        }
                        */
                    }

                    SpawnBeds(startPos, floorToUseIndex, spawns, beds, 0);

                    if (useBothFloors == true)
                    {

                        SpawnBeds(otherstartPos, otherFloor, spawns, beds, 0);

                    }
                    SpawnPropsAroundBeds(beds);

                    StartCoroutine(DisableRigidbodiesOnDelay());
                }
                break;

            case "Common Room":

                if (floors.Count >= 10)
                {
                    props = new Props[]
                    {
                    new Props("Sofa", 100, false, commonRoomSofas.Length, commonRoomSofas),
                    new Props("TV", 100, false, commonRoomTVs.Length, commonRoomTVs),
                    new Props("CoffeeTable", 100, false, commonRoomTables.Length, commonRoomTables),
                    new Props("Lockers", 100, false, commonRoomLockers.Length, commonRoomLockers)
                    };

                    bigRoom = false;

                    List<Vector3> WallTiles = new List<Vector3>();
                    List<Vector3> availTiles = new List<Vector3>();

                    _floorHolder = floorHolder[0];
                    _otherFloorHolder = floorHolder[1];
                    floor0Area = floorXsize[0] * floorZsize[0];
                    floor1Area = floorXsize[1] * floorZsize[1];
                    startPos = _floorHolder.localPosition + floors[0] + GenerateOffset(0, 2.5f, 0);
                    otherstartPos = _otherFloorHolder.localPosition + floors[floor0Area] + GenerateOffset(0, 2.5f, 0);

                    if (floors.Count > 14)
                        bigRoom = true;

                    foreach (Vector3 floor in floors)
                    {
                        Debug.Log("Checking floor pos: " + floor);
                        if (WallCheck(floor + GenerateOffset(0, 3, 0)).Item1 && WallCheck(floor + GenerateOffset(0, 3, 0)).Item2 < 2)
                        {
                            WallTiles.Add(floor);
                        } else if(!WallCheck(floor + GenerateOffset(0, 3, 0)).Item5 && WallCheck(floor + GenerateOffset(0, 3, 0)).Item2 < 1)
                        {
                            availTiles.Add(floor);
                        }
                    }
                    Debug.Log("Wall Tile Count: " + WallTiles.Count);
                    List<GameObject> tvs = new List<GameObject>();

                    GameObject tvProp = props[1].Versions[0];
                    Debug.Log("Wall Tile Count: " + WallTiles.Count);
                    int tvSpawnIndex = UnityEngine.Random.Range(0, WallTiles.Count - 1);

                    GameObject tv = SpawnProp(tvProp, WallTiles[tvSpawnIndex] + GenerateOffset(0, 5, 0), WallCheck(WallTiles[tvSpawnIndex] + GenerateOffset(0, 5, 0)).Item4, GenerateOffset(1.628875f, 0.9346116f, 1.16411f),
                        WallCheck(WallTiles[tvSpawnIndex] + GenerateOffset(0, 5, 0)).Item3, GenerateOffset(-90, 0, 0), null, tvs, false, false, null);

                    GameObject sofa = props[0].Versions[0];

                    GameObject _sofa = Instantiate(sofa, WallTiles[tvSpawnIndex] + GenerateOffset(0, 2.23f, 0), WallCheck(WallTiles[tvSpawnIndex] + GenerateOffset(0, 5, 0)).Item4);
                    _sofa.transform.Rotate(new Vector3(-90, 0, 90));
                    _sofa.transform.SetParent(tv.transform, true);
                    //Move back
                    _sofa.transform.localPosition -= new Vector3(0, 1, 0);
                    //Move to the left relatively
                    _sofa.transform.localPosition += new Vector3(0.78f, 0, 0);

                    WallTiles.RemoveAt(tvSpawnIndex);

                    if (bigRoom)
                    {
                        int tvSpawnIndex2 = UnityEngine.Random.Range(0, WallTiles.Count);

                        tv = SpawnProp(tvProp, WallTiles[tvSpawnIndex2] + GenerateOffset(0, 5, 0), WallCheck(WallTiles[tvSpawnIndex2] + GenerateOffset(0, 5, 0)).Item4, GenerateOffset(1.628875f, 0.9346116f, 1.16411f),
                            WallCheck(WallTiles[tvSpawnIndex2] + GenerateOffset(0, 5, 0)).Item3, GenerateOffset(-90, 0, 0), null, tvs, false, false, null);

                        _sofa = Instantiate(sofa, WallTiles[tvSpawnIndex2] + GenerateOffset(0, 2.23f, 0), WallCheck(WallTiles[tvSpawnIndex2] + GenerateOffset(0, 5, 0)).Item4);
                        _sofa.transform.Rotate(new Vector3(-90, 0, 90));
                        _sofa.transform.SetParent(tv.transform, true);
                        //Move back
                        _sofa.transform.localPosition -= new Vector3(0, 1, 0);
                        //Move to the left relatively
                        _sofa.transform.localPosition += new Vector3(0.78f, 0, 0);

                        WallTiles.RemoveAt(tvSpawnIndex2);
                    }

                    int lockersToSpawnNum;

                    if (bigRoom)
                        lockersToSpawnNum = 4;
                    else
                        lockersToSpawnNum = 2;


                    for (int i = 0; i < lockersToSpawnNum; i++)
                    {
                        int lockersSpawnIndex = UnityEngine.Random.Range(0, WallTiles.Count);
                        GameObject lockersPrefab = props[3].Versions[0];
                        GameObject lockers = Instantiate(lockersPrefab, WallTiles[lockersSpawnIndex] + GenerateOffset(0, 2.2f, 0), WallCheck(WallTiles[lockersSpawnIndex] + GenerateOffset(0, 2.4f, 0)).Item4);
                        //lockers.transform.SetParent(WallCheck(WallTiles[lockersSpawnIndex] + GenerateOffset(0, 2.4f, 0)).Item6, true);
                        lockers.transform.position = WallCheck(WallTiles[lockersSpawnIndex] + GenerateOffset(0, 2.25f, 0)).Item3;
                        lockers.transform.Rotate(GenerateOffset(0, -90, 0));
                        WallTiles.RemoveAt(lockersSpawnIndex);
                    }

                    //This is just here for debugging purposes, remove later.
                    foreach (Vector3 tile in availTiles)
                    {
                        GameObject k = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        k.SetActive(false);
                        k.transform.position = tile + GenerateOffset(0, 3, 0);
                    }

                    if(availTiles.Count > 0) 
                    {
                    GameObject coffeTableProp = props[2].Versions[0];
                    int sofaSpawnIndex = UnityEngine.Random.Range(0, availTiles.Count);

                    //I'm not using the spawn prop method here because it is a really long winded method and I don't really need it here
                    GameObject coffeeTable = Instantiate(coffeTableProp, availTiles[sofaSpawnIndex] + GenerateOffset(0, 2.228f, 0), Quaternion.identity);

                    //Using this variable here just makes it easier to spawn a sofa on either side of the coffee table by multiplying the localPosition change by 1 or -1 i.e x.
                    int x = -1;
                        for (int i = 0; i < 2; i++)
                        {
                            GameObject cSofa = Instantiate(sofa, coffeeTable.transform.position, Quaternion.identity);
                            cSofa.transform.SetParent(coffeeTable.transform, true);
                            cSofa.transform.rotation = Quaternion.Euler(GenerateOffset(-90, 0, 0));
                            cSofa.transform.localPosition += Vector3.left * -9.5f * x;
                            if (x == 1)
                            {
                                cSofa.transform.Rotate(GenerateOffset(0, 0, 180));
                                cSofa.transform.localPosition += Vector3.back * 4;
                            }
                            else
                            {
                                cSofa.transform.localPosition -= Vector3.back * 4;
                            }


                            x = 1;
                        }
                    }



                }
                break;

        }
    }

    private GameObject SpawnProp(GameObject spawnObj, Vector3 spawnPos, Quaternion spawnRot, Vector3 objScale, Vector3 movePos, Vector3 postRot, Transform parentObj, List<GameObject> propList, bool moveLocally, bool lookAtObj, GameObject lookObj)
    {
        spawnObj = Instantiate(spawnObj, spawnPos, spawnRot);
        spawnObj.transform.SetParent(parentObj, true);
        if (moveLocally)
        {
            spawnObj.transform.localPosition = movePos;
            //spawnObj.transform.localScale = objScale;
        } else
        {
            spawnObj.transform.position = movePos;
        }
        if (lookAtObj)
        {
            spawnObj.transform.rotation = Quaternion.LookRotation(GenerateOffset(spawnObj.transform.position.x - lookObj.transform.position.x, lookObj.transform.localPosition.y, spawnObj.transform.position.z - lookObj.transform.position.z));
        }
        spawnObj.transform.Rotate(postRot);
        propList.Add(spawnObj);
        return spawnObj;
    }

    private void SpawnPropsAroundBeds(List<GameObject> beds)
    {
        int stretcherBedIndex = -1;
        int stretcherBedIndex2 = -1;
        int trolleySpot = UnityEngine.Random.Range(0, beds.Count);

        if (bigRoom)
        {
            int halfBeds = Mathf.RoundToInt(beds.Count / 2);
            stretcherBedIndex = UnityEngine.Random.Range(0, halfBeds);
            stretcherBedIndex2 = UnityEngine.Random.Range(halfBeds, beds.Count);
        } else
        {
            stretcherBedIndex = UnityEngine.Random.Range(0, beds.Count);
        }

        foreach (GameObject bed in beds)
        {
            Props propToSpawn;

            int spawnSide;
            float zSpawn;
            bool stretcherBed1 = false;
            bool stretcherBed2 = false;

            if(bed == beds[stretcherBedIndex])
            {
                stretcherBed1 = true;
            } else if (bigRoom)
            {
                if(bed == beds[stretcherBedIndex2])
                {
                    stretcherBed2 = true;
                }
            }

            if (stretcherBed1 || stretcherBed2)
            {
                propToSpawn = props[5];

                GameObject st = Instantiate(propToSpawn.Versions[0], bed.transform.position, Quaternion.identity);
                st.transform.parent = bed.transform;
                st.transform.localPosition = new Vector3(0, 0.5f, 3.0f);
                st.transform.Rotate(new Vector3(UnityEngine.Random.Range(0, 45), UnityEngine.Random.Range(-70, -110), 0));

                spawnedProps.Add(st);
            }
            
            if (bed == beds[trolleySpot])
            {
                SpawnMedicineTrolleys(bed.transform);
            } else {

                //Spawn a medicine cabinet next to the bed.
                propToSpawn = props[2];

                spawnSide = 1;
                zSpawn = 0.3f;
                GameObject s = Instantiate(propToSpawn.Versions[0], bed.transform.position, Quaternion.identity);
                s.transform.parent = bed.transform;
                s.transform.localPosition = new Vector3(0.5f * spawnSide, 0.7f, zSpawn);
                s.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, -90));
                s.transform.localScale = new Vector3(0.193f, 0.193f, 0.193f);

                spawnedProps.Add(s);

                //Spawn a stretcher in front of the bed if it is the designated stretcher bed.
                if(!stretcherBed1 && !stretcherBed2) //Spawn other objects near the bed only if there is no stretcher.
                {
                    //Spawn 0, 1 or 2 chairs on either side of the bed.
                    propToSpawn = props[1];

                    bool chairSpawned = false;

                    int objsToSpawn = UnityEngine.Random.Range(2, 10) <= 5 ? objsToSpawn = 0 : objsToSpawn =
                        UnityEngine.Random.Range(0, 10) <= 6 ? objsToSpawn = 1 : objsToSpawn = 2;
                    if (objsToSpawn > 0)
                    {
                        chairSpawned = true;
                    }

                    int k = 0;

                    for (int i = 1; i <= objsToSpawn; i++)
                    {
                        zSpawn = UnityEngine.Random.Range(0.86f, 1.6f);
                        spawnSide = UnityEngine.Random.Range(0, 2) == 0 ? spawnSide = 1 : spawnSide = -2;

                        if (i == 1)
                        {
                            _ = spawnSide == 1 ? k = -2 : k = 1;
                        }

                        if (i == 2)
                        {
                            spawnSide = k;
                        }

                        GameObject t = Instantiate(propToSpawn.Versions[0], bed.transform.position, Quaternion.identity);

                        if (PreventOverlap(bed.transform.position + new Vector3(0.8f * spawnSide, 0, zSpawn), propToSpawn.Versions[0]))
                        {
                            t.transform.parent = bed.transform;
                            t.transform.localPosition = new Vector3(0.8f * spawnSide, 0, zSpawn);
                            t.transform.rotation = Quaternion.LookRotation(new Vector3(t.transform.position.x - bed.transform.position.x,
                                0, t.transform.position.z - bed.transform.position.z));
                            t.transform.Rotate(new Vector3(0, UnityEngine.Random.Range(160, 200), 0));
                            t.AddComponent<Rigidbody>();
                        }
                        else
                        {
                            t.transform.parent = bed.transform;
                            t.transform.localPosition = new Vector3(0.8f * spawnSide, 2.0f, zSpawn);
                            t.transform.rotation = Quaternion.LookRotation(new Vector3(t.transform.position.x - bed.transform.position.x,
                                0, t.transform.position.z - bed.transform.position.z));
                            t.transform.Rotate(new Vector3(0, UnityEngine.Random.Range(160, 200), 0));
                            t.AddComponent<Rigidbody>();
                        }
                        spawnedProps.Add(t);
                        Debug.DrawLine(t.transform.position, new Vector3(bed.transform.position.x,
                            t.transform.position.y, bed.transform.position.z), Color.cyan, 10.0f);
                    }


                    //Spawn a chair with a television on it in front of beds that have chairs.
                    if (chairSpawned)
                    {
                        propToSpawn = props[3];
                        GameObject v = Instantiate(propToSpawn.Versions[0], bed.transform.position, Quaternion.identity);
                        v.transform.parent = bed.transform;
                        v.transform.localPosition = new Vector3(-0.42f, 0, 2.8f);
                        v.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, UnityEngine.Random.Range(-120, -100)));
                        v.transform.localScale = new Vector3(1, 1, 1);

                        propToSpawn = props[4];

                        GameObject tv = Instantiate(propToSpawn.Versions[0], bed.transform.position, Quaternion.identity);
                        tv.transform.parent = v.transform;
                        tv.transform.localPosition = new Vector3(-0.011f, -0.075f, 0.512f);
                        tv.transform.localRotation = Quaternion.Euler(new Vector3(18.425f, -90, -90));
                        tv.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);

                        spawnedProps.Add(v); spawnedProps.Add(tv);
                    }
                }
            }
        }
    }

    private void SpawnBeds(Vector3 startPos, int floorToUse, int spawns, List<GameObject> objList, int bedPropIndex)
    {
        Props propToSpawn = props[bedPropIndex];
        int versionToSpawn = UnityEngine.Random.Range(0, propToSpawn.Variants);

        GameObject t = null;

        float bedSideoffset = 0.5f;
        float bedForwardoffset = 1.5f;
        float bedYoffset = 0.24f;

        //List<WallEdge> bedSpawns = new List<WallEdge>();

        //bedSpawns = EdgeFloors(startPos, floorToUse, spawns);
        /*
        foreach(WallEdge pos in bedSpawns)
        {
            Quaternion faceWall = pos.WallDirection;
            t = GameObject.Instantiate(propToSpawn.Versions[versionToSpawn], pos.TilePos, faceWall);
            t.transform.parent = roomReferencesModular.transform;
            t.transform.localPosition = new Vector3(t.transform.localPosition.x - 4, t.transform.localPosition.y, t.transform.localPosition.z);

            objList.Add(t);
        }
        */
        
        //Get which side of the room is the longest and spawn the beds there. --THIS CODE WILL BE REPLACED WITH AN EDGE WALL MAPPER WITH ONLY INSTANTIATION HANDLED IN THIS FUNCTION--
        if (Vector3.Distance(startPos, startPos + GenerateOffset(floorXsize[floorToUse] * 4 + 4f, 0, 0)) > Vector3.Distance(startPos, startPos + GenerateOffset(0, 0, floorZsize[floorToUse] * 4 + 4f)))
        {
            //Run this if the X side wall is longer.
            Debug.DrawLine(startPos, startPos + GenerateOffset(floorXsize[floorToUse] * 4 - 4f, 0, 0), Color.green, 10.0f);
            Debug.Log("Floor " + floorToUse + "'s wall along the X axis is longer");

            //Spawn a bed at every 4 unit interval as that is the interval the floor tiles are placed at.
            for (int i = 0; i < floorXsize[floorToUse]; i++)
            {
                Vector3 lightPos = startPos + GenerateOffset(i * 4, 3.5f, (-floorZsize[floorToUse] * 4 + 4) /2);

                GameObject light = Instantiate(hospitalLights[0], lightPos, Quaternion.identity);
                light.transform.rotation = Quaternion.Euler(-90, 0, -90);

                Vector3 k = startPos + GenerateOffset(i * 4, 0, 0);

                //Check if the bed is next to a wall, if not, destroy it. --this may be changed to simply moving the bed to the opposite wall
                if (GoodSpawn(k, GenerateOffset(0, 0, 1), GenerateOffset(-1, 0, 0), GenerateOffset(1, 0, 0), GenerateOffset(0, -1, 0), spawns, 1).Item1)
                {
                    Quaternion faceWall = GoodSpawn(k, GenerateOffset(0, 0, 1), GenerateOffset(-1, 0, 0), GenerateOffset(1, 0, 0), GenerateOffset(0, -1, 0), spawns, 1).Item2;

                    t = GameObject.Instantiate(propToSpawn.Versions[versionToSpawn], k + GenerateOffset(-bedSideoffset, -bedYoffset, bedForwardoffset), faceWall);

                    t.transform.parent = roomReferencesModular.transform;

                    objList.Add(t);
                }
                
                RaycastHit hit;
                if (Physics.Raycast(k, GenerateOffset(0, 0, -1), out hit, floorZsize[floorToUse] * 4 + 1))
                {
                    Debug.Log("Walls to spawn index: " + spawns);
                    Vector3 oppPos = new Vector3(k.x, k.y, k.z - floorZsize[floorToUse] * 4 + 4);
                    Quaternion faceWall = Quaternion.LookRotation(k - hit.point);
                    switch (spawns)
                    {
                        case 1:
                            Destroy(t);

                            if(GoodSpawn(oppPos, GenerateOffset(0, 0, -1), GenerateOffset(-1, 0, 0), GenerateOffset(1, 0, 0), GenerateOffset(0, -1, 0), spawns, 3).Item1)
                            {
                                GameObject p = GameObject.Instantiate(propToSpawn.Versions[versionToSpawn], oppPos + GenerateOffset(bedSideoffset, -bedYoffset, -bedForwardoffset - 0.45f), faceWall);

                                p.transform.parent = roomReferencesModular.transform;

                                objList.Add(p);
                            }
                            
                            break;

                        case 2:

                            if(GoodSpawn(oppPos, GenerateOffset(0, 0, -1), GenerateOffset(-1, 0, 0), GenerateOffset(1, 0, 0), GenerateOffset(0, -1, 0), spawns, 3).Item1)
                            {
                                GameObject p = GameObject.Instantiate(propToSpawn.Versions[versionToSpawn], oppPos + GenerateOffset(bedSideoffset, -bedYoffset, -bedForwardoffset - 0.45f), faceWall);

                                p.transform.parent = roomReferencesModular.transform;

                                objList.Add(p);
                            }
                            
                            break;
                    }
                    Debug.DrawLine(oppPos, oppPos + GenerateOffset(0, 0, -3.0f), Color.black, 10.0f);
                    Debug.DrawLine(oppPos, oppPos + GenerateOffset(-3.0f, 0, 0), Color.black, 10.0f);
                    Debug.DrawLine(oppPos, oppPos + GenerateOffset(3.0f, 0, 0), Color.black, 10.0f);
                }
                Debug.DrawLine(k, k + GenerateOffset(0, 0, 3), Color.blue, 10.0f);
                Debug.DrawLine(k, k + GenerateOffset(-3.0f, 0, 0), Color.blue, 10.0f);
                Debug.DrawLine(k, k + GenerateOffset(3.0f, 0, 0), Color.blue, 10.0f);
                Debug.DrawLine(k, k + GenerateOffset(0, 0, 1 * -floorZsize[floorToUse] * 4 + 1), Color.red, 10.0f);
            }
        }
        else
        {
            //Run this if the Z side wall is longer.
            Debug.DrawLine(startPos, startPos + GenerateOffset(0, 0, -floorZsize[floorToUse] * 4 + 4f), Color.green, 10.0f);
            Debug.Log("Floor " + floorToUse + "'s wall along the Z axis is longer");

            for (int i = 0; i < floorZsize[floorToUse]; i++)
            {
                Vector3 lightPos = startPos + GenerateOffset((floorXsize[floorToUse] * 4 - 4) / 2, 3.5f, -i * 4);
                   
                GameObject light = Instantiate(hospitalLights[0], lightPos, Quaternion.identity);

                light.transform.rotation = Quaternion.Euler(-90, 0, 0);


                Vector3 k = startPos + GenerateOffset(0, 0, -i * 4);

                if (GoodSpawn(k, GenerateOffset(-1, 0, 0), GenerateOffset(0, 0, -1), GenerateOffset(0, 0, 1), GenerateOffset(0, -1, 0), spawns, 1).Item1)
                {
                    Quaternion facewall = GoodSpawn(k, GenerateOffset(-1, 0, 0), GenerateOffset(0, 0, -1), GenerateOffset(0, 0, 1), GenerateOffset(0, -1, 0), spawns, 1).Item2;

                    t = GameObject.Instantiate(propToSpawn.Versions[versionToSpawn], k + GenerateOffset(-bedForwardoffset, -bedYoffset, -bedSideoffset), facewall);

                    t.transform.parent = roomReferencesModular.transform;

                    objList.Add(t);
                }

                RaycastHit hit;
                if (Physics.Raycast(k, GenerateOffset(1, 0, 0), out hit, floorXsize[floorToUse] * 4 - 1))
                {
                    Debug.Log("Walls to spawn index: " + spawns);
                    Vector3 oppPos = new Vector3(k.x + floorXsize[floorToUse] * 4 - 4, k.y, k.z);
                    Quaternion faceWall = Quaternion.LookRotation(k - hit.point);
                    switch (spawns)
                    {
                        case 1:
                            Destroy(t);

                            if (GoodSpawn(oppPos, GenerateOffset(1, 0, 0), GenerateOffset(0, 0, -1), GenerateOffset(0, 0, 1), GenerateOffset(0, -1, 0), spawns, 3).Item1)
                            {
                                GameObject p = Instantiate(propToSpawn.Versions[versionToSpawn], oppPos + GenerateOffset(bedForwardoffset + 0.45f, -bedYoffset, bedSideoffset), faceWall);

                                p.transform.parent = roomReferencesModular.transform;

                                objList.Add(p);
                            }

                            break;

                        case 2:

                            if(GoodSpawn(oppPos, GenerateOffset(1, 0, 0), GenerateOffset(0, 0, -1), GenerateOffset(0, 0, 1), GenerateOffset(0, -1, 0), spawns, 3).Item1)
                            {
                                GameObject p = Instantiate(propToSpawn.Versions[versionToSpawn], oppPos + GenerateOffset(bedForwardoffset + 0.45f, -bedYoffset, bedSideoffset), faceWall);

                                p.transform.parent = roomReferencesModular.transform;

                                objList.Add(p);
                            }
                            break;
                    }
                    Debug.DrawLine(oppPos, oppPos + GenerateOffset(3.0f, 0, 0), Color.black, 10.0f);
                    Debug.DrawLine(oppPos, oppPos + GenerateOffset(0, 0, -3.0f), Color.black, 10.0f);
                    Debug.DrawLine(oppPos, oppPos + GenerateOffset(0, 0, 3.0f), Color.black, 10.0f);
                }
                Debug.DrawLine(k, k + GenerateOffset(-3.0f, 0, 0), Color.blue, 10.0f);
                Debug.DrawLine(k, k + GenerateOffset(0, 0, -3.0f), Color.blue, 10.0f);
                Debug.DrawLine(k, k + GenerateOffset(0, 0, -3.0f), Color.blue, 10.0f);
                //Debug.DrawLine(k, k + GenerateOffset(1 * floorXsize[floorToUseIndex] * 4 - 1, 0, 0), Color.red, 10.0f);
            }
        }
    }
    
    //This function will check if there is a wall on either side of the object and return that walls position and a rotation needed for the object to face the wall
    private (bool, int, Vector3, Quaternion, bool, Transform) WallCheck(Vector3 posToCheck)
    {
        bool x1 = false;
        bool x2 = false;
        bool z1 = false;
        bool z2 = false;
        bool doorPresent = false;
        int adjacentWalls = 0;
        Vector3 wallPos = posToCheck;
        Quaternion faceWall = Quaternion.identity;
        Transform wallT = null;
        bool canSpawn = false;
        //GameObject testCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //testCube.transform.position = posToCheck;
        //Bitshift to only hit game objects on the default layer;
        int layerMask = 1 << 0;

        RaycastHit hit;
        if(Physics.Raycast(posToCheck, Vector3.right, out hit, 4.0f, layerMask) && InteractableCheck(posToCheck, Vector3.right))
        {
            x1 = true;
            adjacentWalls++;
            wallPos = hit.point;
            wallT = hit.collider.gameObject.transform;
            faceWall = Quaternion.LookRotation(posToCheck - hit.point);
            Debug.Log("collider hit " + hit.collider.gameObject.name);
        } else if(!InteractableCheck(posToCheck, Vector3.right))
        {
            doorPresent = true;
        }
        Debug.Log("x1 returns: " + x1);
        if (Physics.Raycast(posToCheck, Vector3.left, out hit, 4.0f, layerMask) && InteractableCheck(posToCheck, Vector3.left))
        {
            x2 = true;
            adjacentWalls++;
            wallPos = hit.point;
            wallT = hit.collider.gameObject.transform;
            faceWall = Quaternion.LookRotation(posToCheck - hit.point);
            Debug.Log("collider hit " + hit.collider.gameObject.name);
        } else if (!InteractableCheck(posToCheck, Vector3.left))
        {
            doorPresent = true;
        }
        Debug.Log("x2 returns: " + x2);
        if (Physics.Raycast(posToCheck, Vector3.forward, out hit, 4.0f, layerMask) && InteractableCheck(posToCheck, Vector3.forward))
        {
            z1 = true;
            adjacentWalls++;
            wallPos = hit.point;
            wallT = hit.collider.gameObject.transform;
            faceWall = Quaternion.LookRotation(posToCheck - hit.point);
            Debug.Log("collider hit " + hit.collider.gameObject.name);
        } else if (!InteractableCheck(posToCheck, Vector3.forward))
        {
            doorPresent = true;
        }
        Debug.Log("z1 returns: " + z1);
        if (Physics.Raycast(posToCheck, Vector3.back, out hit, 4.0f, layerMask) && InteractableCheck(posToCheck, Vector3.back))
        {
            z2 = true;
            adjacentWalls++;
            wallPos = hit.point;
            wallT = hit.collider.gameObject.transform;
            faceWall = Quaternion.LookRotation(posToCheck - hit.point);
            Debug.Log("collider hit " + hit.collider.gameObject.name);
        } else if (!InteractableCheck(posToCheck, Vector3.back))
        {
            doorPresent = true;
        }
        
        if(Physics.Raycast(posToCheck, Vector3.down, out hit, 6.0f))
        {
            if (hit.collider.gameObject.name == "Interactable" || hit.collider.gameObject.name == "RenderPlane")
                doorPresent = true;
        }

        Debug.Log("z2 returns: " + z2);
        Debug.Log("doorPresent returns: " + doorPresent);
        /*  Debug lines
        Debug.DrawLine(posToCheck, posToCheck + (Vector3.back * 4.0f), Color.red, 10.0f);
        Debug.DrawLine(posToCheck, posToCheck + (Vector3.forward * 4.0f), Color.red, 10.0f);
        Debug.DrawLine(posToCheck, posToCheck + (Vector3.left * 4.0f), Color.red, 10.0f);
        Debug.DrawLine(posToCheck, posToCheck + (Vector3.right * 4.0f), Color.red, 10.0f);
        */
        switch (doorPresent)
        {
            case false:
                if (x1 == true || x2 == true || z1 == true || z2 == true)
                {
                    //Destroy(testCube);
                    canSpawn = true;
                }
                else canSpawn = false;
                break;

            case true:
                canSpawn = false;
                break;
        }
        Debug.Log("canSpawn returns: " + canSpawn);
        Debug.Log("Num of adjacent walls: " + adjacentWalls);
        return (canSpawn, adjacentWalls, wallPos, faceWall, doorPresent, wallT);
    }

    private void SpawnMedicineTrolleys(Transform bedT)
    {
        Props propToSpawn = props[6];

        GameObject tr = Instantiate(propToSpawn.Versions[0], bedT.position, bedT.rotation);
        tr.transform.parent = bedT;
        tr.transform.Rotate(new Vector3(0, -90, 0));
        tr.transform.localPosition = new Vector3(tr.transform.localPosition.x - 0.5f, tr.transform.localPosition.y, tr.transform.localPosition.z + 0.5f);
        tr.transform.parent = null;
        Destroy(bedT.gameObject);
    }


    //This function will check if the object spawn position is in front of a wall, and not next to or above any interactable objects.
    private (bool, Quaternion) GoodSpawn(Vector3 objPos, Vector3 objBack, Vector3 objLeft, Vector3 objRight, Vector3 objDown, int spawns, int spawnCheck)
    {
        RaycastHit hit;
        if (Physics.Raycast(objPos, objBack, out hit, 3.0f) && spawns != spawnCheck)
        {
            Quaternion faceWall = Quaternion.LookRotation(objPos - hit.point);

            if (hit.collider.gameObject.name == "Interactable")
            {
                return (false, faceWall);
                //Destroy(t);
            }
            else
            if (!InteractableCheck(objPos, objDown))
            {
                return (false, faceWall);
                // Destroy(t);
            }
            else
            if (!InteractableCheck(objPos, objLeft))
            {
                return (false, faceWall);
                //Destroy(t);
            }
            else
            if (!InteractableCheck(objPos, objRight))
            {
                return (false, faceWall);
                //Destroy(t);
            }
            else
            {
                return (true, faceWall);
                //objList.Add(t);
            }
        } else
        {
            return (false, Quaternion.identity);
        }

    }

    /* This function will map the edges of the room(s). startPos is the position of the floorholder obj, floorToUse is the int value of the floor to use, the edges list will hold all the vector positions of the edges of the room,
    spawns dictates which walls will be used to map the edges, 0 means the longest wall, 1 means the wall opposite to the longest and 2 means both, mapBothRooms is a bool where true will map the edges of both the smaller and bigger room
    */

    private List<WallEdge> EdgeFloors(Vector3 startPos, int floorToUse, int spawns)
    {
        List<WallEdge> edges = new List<WallEdge>();
        WallEdge z = null;

        if (Vector3.Distance(startPos, startPos + GenerateOffset(floorXsize[floorToUse] * 4 + 4f, 0, 0)) > Vector3.Distance(startPos, startPos + GenerateOffset(0, 0, floorZsize[floorToUse] * 4 + 4f)))
        {
            for (int i = 0; i < floorXsize[floorToUse]; i++)
            {
                Vector3 k = startPos + GenerateOffset(i * 4, 0, 0);

                if (GoodSpawn(k, GenerateOffset(0, 0, 1), GenerateOffset(-1, 0, 0), GenerateOffset(1, 0, 0), GenerateOffset(0, -1, 0), 1, 3).Item1)
                {
                    z = new WallEdge(k, GoodSpawn(k, GenerateOffset(0, 0, 1), GenerateOffset(-1, 0, 0), GenerateOffset(1, 0, 0), GenerateOffset(0, -1, 0), 1, 3).Item2);
                    edges.Add(z);
                }

                RaycastHit hit;
                if (Physics.Raycast(k, GenerateOffset(0, 0, -1), out hit, floorZsize[floorToUse] * 4 + 1))
                {
                    Debug.Log("Walls to spawn index: " + spawns);
                    Vector3 oppPos = new Vector3(k.x, k.y, k.z - floorZsize[floorToUse] * 4 + 4);
                    Quaternion faceWall = Quaternion.LookRotation(k - hit.point);
                    switch (spawns)
                    {
                        case 1:
                            edges.Remove(z);
                            if (GoodSpawn(oppPos, GenerateOffset(0, 0, -1), GenerateOffset(-1, 0, 0), GenerateOffset(1, 0, 0), GenerateOffset(0, -1, 0), spawns, 3).Item1)
                            {
                                z = new WallEdge(oppPos, GoodSpawn(oppPos, GenerateOffset(0, 0, -1), GenerateOffset(-1, 0, 0), GenerateOffset(1, 0, 0), GenerateOffset(0, -1, 0), spawns, 3).Item2);
                                edges.Add(z);
                            }

                            break;

                        case 2:

                            if (GoodSpawn(oppPos, GenerateOffset(0, 0, -1), GenerateOffset(-1, 0, 0), GenerateOffset(1, 0, 0), GenerateOffset(0, -1, 0), spawns, 3).Item1)
                            {
                                z = new WallEdge(oppPos, GoodSpawn(oppPos, GenerateOffset(0, 0, -1), GenerateOffset(-1, 0, 0), GenerateOffset(1, 0, 0), GenerateOffset(0, -1, 0), spawns, 3).Item2);
                                edges.Add(z);
                            }

                            break;
                    }
                }
            }
        } else
        {
            for (int i = 0; i < floorZsize[floorToUse]; i++)
            {
                Vector3 k = startPos + GenerateOffset(0, 0, -i * 4);

                if (GoodSpawn(k, GenerateOffset(-1, 0, 0), GenerateOffset(0, 0, -1), GenerateOffset(0, 0, 1), GenerateOffset(0, -1, 0), spawns, 1).Item1)
                {
                    z = new WallEdge(k, GoodSpawn(k, GenerateOffset(-1, 0, 0), GenerateOffset(0, 0, -1), GenerateOffset(0, 0, 1), GenerateOffset(0, -1, 0), spawns, 1).Item2);
                    edges.Add(z);
                }

                RaycastHit hit;
                if (Physics.Raycast(k, GenerateOffset(1, 0, 0), out hit, floorXsize[floorToUse] * 4 - 1))
                {
                    Debug.Log("Walls to spawn index: " + spawns);
                    Vector3 oppPos = new Vector3(k.x + floorXsize[floorToUse] * 4 - 4, k.y, k.z);
                    Quaternion faceWall = Quaternion.LookRotation(k - hit.point);
                    switch (spawns)
                    {
                        case 1:
                            edges.Remove(z);

                            if (GoodSpawn(oppPos, GenerateOffset(1, 0, 0), GenerateOffset(0, 0, -1), GenerateOffset(0, 0, 1), GenerateOffset(0, -1, 0), spawns, 3).Item1)
                            {
                                z = new WallEdge(oppPos, GoodSpawn(oppPos, GenerateOffset(1, 0, 0), GenerateOffset(0, 0, -1), GenerateOffset(0, 0, 1), GenerateOffset(0, -1, 0), spawns, 3).Item2);
                                edges.Add(z);
                            }

                            break;

                        case 2:

                            if (GoodSpawn(oppPos, GenerateOffset(1, 0, 0), GenerateOffset(0, 0, -1), GenerateOffset(0, 0, 1), GenerateOffset(0, -1, 0), spawns, 3).Item1)
                            {
                                z = new WallEdge(oppPos, GoodSpawn(oppPos, GenerateOffset(1, 0, 0), GenerateOffset(0, 0, -1), GenerateOffset(0, 0, 1), GenerateOffset(0, -1, 0), spawns, 3).Item2);
                                edges.Add(z);
                            }
                            break;
                    }
                    //Debug.DrawLine(oppPos, oppPos + GenerateOffset(3.0f, 0, 0), Color.black, 10.0f);
                    //Debug.DrawLine(oppPos, oppPos + GenerateOffset(0, 0, -3.0f), Color.black, 10.0f);
                    //Debug.DrawLine(oppPos, oppPos + GenerateOffset(0, 0, 3.0f), Color.black, 10.0f);
                }
                //Debug.DrawLine(k, k + GenerateOffset(-3.0f, 0, 0), Color.blue, 10.0f);
                //Debug.DrawLine(k, k + GenerateOffset(0, 0, -3.0f), Color.blue, 10.0f);
                //Debug.DrawLine(k, k + GenerateOffset(0, 0, -3.0f), Color.blue, 10.0f);
            }
        }
        return edges;
    }

    private bool InteractableCheck(Vector3 origin, Vector3 direction)
    {
        RaycastHit rayHit;
        int mapGenLayer = 1 << 18;
        mapGenLayer = ~mapGenLayer;

        if(Physics.Raycast(origin, direction, out rayHit, 3.0f, mapGenLayer))
        {
            if (rayHit.collider.gameObject.name == "Interactable")
            {
                return false;
            } else
            {
                return true;
            }
        } else
        {
            return true;
        }
    }

    private bool PreventOverlap(Vector3 posToCheck, GameObject objBeingSpawned)
    {
        float sphereCastRadius = 0.6f;
        
        bool canSpawn = true;
        for (int i = 0; i < spawnedProps.Count; i++)
        {
            RaycastHit hit;

            /*  Generate a sphere object for debugging purposes, setting its position to the position of the object to be spawned (posToCheck)
            and its radius to the width of object being spawned. A green sphere indicates that the spawn position is available, a red sphere indicates
            that the position is taken. */
            /*
            GameObject testSphere1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            testSphere1.transform.localScale = new Vector3(sphereCastRadius,
                sphereCastRadius,
                sphereCastRadius);
            testSphere1.transform.position = posToCheck + GenerateOffset(0, 3.0f, 0);

            GameObject testSphere2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            var renderer = testSphere2.GetComponent<MeshRenderer>();

            testSphere2.transform.localScale = new Vector3(sphereCastRadius,
                sphereCastRadius,
                sphereCastRadius);
            testSphere2.transform.position = posToCheck;

            testSphere1.GetComponent<SphereCollider>().enabled = false;
            testSphere2.GetComponent<SphereCollider>().enabled = false;
            */
            //If there is an object overlapping the sphere check for the object's spawn position, then return canSpawn as false, otherwise, if the position is vacant, return canSpawn as true.
        if (Physics.SphereCast(posToCheck + GenerateOffset(0, 3.0f, 0), sphereCastRadius, Vector3.down, out hit, 2.5f))
        {
            canSpawn = false;
            //renderer.material.SetColor("_Color", Color.red);
        }
        else
        {

            //renderer.material.SetColor("_Color", Color.green);
            canSpawn = true;
        }

            Debug.Log(objBeingSpawned + " checking for intersection with " + spawnedProps[i].name + "returns: " + canSpawn);
        }
        return canSpawn;
    }
        
    private IEnumerator DisableRigidbodiesOnDelay()
    {
        float waitTime = 5.0f;

        yield return new WaitForSeconds(waitTime);

        Debug.Log("Disabling rigidbody physics...");

        foreach(GameObject prop in spawnedProps)
        {
            if (prop.GetComponent<Rigidbody>() != null)
            {
                prop.GetComponent<Rigidbody>().isKinematic = true;
            }
        }

    }

    private Vector3 GenerateOffset(float x, float y, float z)
    {
        return new Vector3 (x, y, z);
    }
}
