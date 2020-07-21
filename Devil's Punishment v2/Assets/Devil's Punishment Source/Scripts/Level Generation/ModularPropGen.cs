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
                    new Props("Hospital stretcher", 100, false, hospitalStretchers.Length, hospitalStretchers)
                };

                if (floors.Count < 10)
                {
                    Debug.LogError("Modular room size too small! Room must have at least 10 tiles to support hospital room type");
                    floors.RemoveRange(0, floors.Count);
                }
                else
                {
                    int floor0Area = floorXsize[0] * floorZsize[0];
                    int floor1Area = floorXsize[1] * floorZsize[1];

                    bool useBothFloors = false;

                    if(floors.Count > 18)
                    {
                        useBothFloors = true;
                        bigRoom = true;
                    }

                    int spawns = UnityEngine.Random.Range(0, 10) <= 8 ? spawns = 2 : spawns = UnityEngine.Random.Range(0, 2);

                    if (floors.Count > 24)
                    {
                        spawns = 2;
                    }

                    Vector3 startPos;
                    Vector3 otherstartPos;
                    int floorToUseIndex;
                    int otherFloor;

                    //Get the room with the largest area to have the hospital beds generate there.
                    if (floor0Area > floor1Area)
                    {
                        Transform _floorHolder = floorHolder[0];
                        Transform _otherFloorHolder = floorHolder[1];
                        startPos = _floorHolder.localPosition + floors[0] + GenerateOffset(0, 2.5f, 0);
                        otherstartPos = _otherFloorHolder.localPosition + floors[floor0Area] + GenerateOffset(0, 2.5f, 0);
                        if (Physics.Raycast(startPos, Vector3.down, out RaycastHit hit, 5.0f))
                        {
                            if (hit.collider.gameObject.name == "Interactable")
                            {
                                startPos.y -= 4;
                                otherstartPos.y -= 4;
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
                        Transform _floorHolder = floorHolder[1];
                        Transform _otherFloorHolder = floorHolder[0];
                        startPos = _floorHolder.localPosition + floors[floor0Area] + GenerateOffset(0, 2.5f, 0);
                        otherstartPos = _otherFloorHolder.localPosition + floors[0] + GenerateOffset(0, 2.5f, 0);
                        if (Physics.Raycast(startPos, Vector3.down, out RaycastHit hit, 5.0f))
                        {
                            if (hit.collider.gameObject.name == "Interactable")
                            {
                                startPos.y -= 4;
                                otherstartPos.y -= 4;
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

                    SpawnBeds(startPos, floorToUseIndex, spawns, beds);

                    if (useBothFloors == true)
                    {

                        SpawnBeds(otherstartPos, otherFloor, spawns, beds);

                    }
                    SpawnPropsAroundBeds(beds);
                    
                    StartCoroutine(DisableRigidbodiesOnDelay());
                }
                break;
        }
    }

    private void SpawnPropsAroundBeds(List<GameObject> beds)
    {
        int stretcherBedIndex = -1;
        int stretcherBedIndex2 = -1;

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
            if (bed == beds[stretcherBedIndex])
            {
                propToSpawn = props[5];

                GameObject st = Instantiate(propToSpawn.Versions[0], bed.transform.position, Quaternion.identity);
                st.transform.parent = bed.transform;
                st.transform.localPosition = new Vector3(0, 0.5f, 3.5f);
                st.transform.Rotate(new Vector3(UnityEngine.Random.Range(0, 45), UnityEngine.Random.Range(0, 180), 0));

                spawnedProps.Add(st);
            }
            else if (bigRoom && bed == beds[stretcherBedIndex2])
            {
                propToSpawn = props[5];

                GameObject st = Instantiate(propToSpawn.Versions[0], bed.transform.position, Quaternion.identity);
                st.transform.parent = bed.transform;
                st.transform.localPosition = new Vector3(0, 0.5f, 4.5f);
                st.transform.Rotate(new Vector3(UnityEngine.Random.Range(0, 45), UnityEngine.Random.Range(0, 180), 0));
            } else //Spawn other objects near the bed only if there is no stretcher.
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
                    zSpawn = UnityEngine.Random.Range(0.5f, 1.6f);
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

                    spawnedProps.Add(v);    spawnedProps.Add(tv);
                }
            }
        }
    }

    private void SpawnBeds(Vector3 startPos, int floorToUse, int spawns, List<GameObject> objList)
    {
        Props propToSpawn = props[0];
        int versionToSpawn = UnityEngine.Random.Range(0, propToSpawn.Variants);

        GameObject t = null;

        float bedSideoffset = 0.5f;
        float bedForwardoffset = 1.5f;
        float bedYoffset = 0.24f;

        //Get which side of the room is the longest and spawn the beds there.
        if (Vector3.Distance(startPos, startPos + GenerateOffset(floorXsize[floorToUse] * 4 + 4f, 0, 0)) > Vector3.Distance(startPos, startPos + GenerateOffset(0, 0, floorZsize[floorToUse] * 4 + 4f)))
        {
            //Run this if the X side wall is longer.
            Debug.DrawLine(startPos, startPos + GenerateOffset(floorXsize[floorToUse] * 4 - 4f, 0, 0), Color.green, 10.0f);
            Debug.Log("Floor " + floorToUse + "'s wall along the X axis is longer");

            //Spawn a bed at every 4 unit interval as that is the interval the floor tiles are placed at.
            for (int i = 0; i < floorXsize[floorToUse]; i++)
            {
                Vector3 k = startPos + GenerateOffset(i * 4, 0, 0);

                //Check if the bed is next to a wall, if not, destroy it. --this may be changed to simply moving the bed to the opposite wall
                RaycastHit hit;
                if (Physics.Raycast(k, GenerateOffset(0, 0, 1), out hit, 3.0f) && spawns != 1)
                {

                    Quaternion faceWall = Quaternion.LookRotation(k - hit.point);

                    t = GameObject.Instantiate(propToSpawn.Versions[versionToSpawn], k + GenerateOffset(-bedSideoffset, -bedYoffset, bedForwardoffset), faceWall);

                    t.transform.parent = roomReferencesModular.transform;
                     
                    if (hit.collider.gameObject.name == "Interactable")
                    {
                        Debug.Log("Destroying " + t.name);
                        Destroy(t);
                    }
                    else
                    if (!InteractableCheck(k, GenerateOffset(0, -1, 0)))
                    {
                        Debug.Log("Destroying " + t.name);
                        Destroy(t);
                    }
                    else
                    if (!InteractableCheck(k, GenerateOffset(-1, 0, 0)))
                    {
                        Debug.Log("Destroying " + t.name);
                        Destroy(t);
                    }
                    else
                    if (!InteractableCheck(k, GenerateOffset(1, 0, 0)))
                    {
                        Debug.Log("Destroying " + t.name);
                        Destroy(t);
                    }
                    else
                    {
                        objList.Add(t);
                    }
                    Debug.Log(t.name + "is next to wall.");
                }
                if (Physics.Raycast(k, GenerateOffset(0, 0, -1), out hit, floorZsize[floorToUse] * 4 + 1))
                {
                    Debug.Log("Walls to spawn index: " + spawns);
                    Vector3 oppPos = new Vector3(k.x, k.y, k.z - floorZsize[floorToUse] * 4 + 4);
                    Quaternion faceWall = Quaternion.LookRotation(k - hit.point);
                    switch (spawns)
                    {
                        case 1:
                            Destroy(t);
                            if (Physics.Raycast(oppPos, GenerateOffset(0, 0, -1), out hit, 3.0f))
                            {

                                GameObject p = GameObject.Instantiate(propToSpawn.Versions[versionToSpawn], oppPos + GenerateOffset(bedSideoffset, -bedYoffset, -bedForwardoffset - 0.45f), faceWall);

                                p.transform.parent = roomReferencesModular.transform;

                                if (hit.collider.gameObject.name == "Interactable")
                                {
                                    Debug.Log("Destroying " + p.name);
                                    Destroy(p);
                                }
                                else
                                if (!InteractableCheck(oppPos, GenerateOffset(0, -1, 0)))
                                {
                                    Debug.Log("Destroying " + p.name);
                                    Destroy(p);
                                }
                                else
                                if (!InteractableCheck(oppPos, GenerateOffset(-1, 0, 0)))
                                {
                                    Debug.Log("Destroying " + p.name);
                                    Destroy(p);
                                }
                                else
                                if (!InteractableCheck(oppPos, GenerateOffset(1, 0, 0)))
                                {
                                    Debug.Log("Destroying " + p.name);
                                    Destroy(p);
                                }
                                else
                                {
                                    objList.Add(p);
                                }
                            }
                            break;

                        case 2:
                            if (Physics.Raycast(oppPos, GenerateOffset(0, 0, -1), out hit, 3.0f))
                            {

                                GameObject p = GameObject.Instantiate(propToSpawn.Versions[versionToSpawn], oppPos + GenerateOffset(bedSideoffset, -bedYoffset, -bedForwardoffset - 0.45f), faceWall);

                                p.transform.parent = roomReferencesModular.transform;

                                if (hit.collider.gameObject.name == "Interactable")
                                {
                                    Debug.Log("Destroying " + p.name);
                                    Destroy(p);
                                }
                                else
                                if (!InteractableCheck(oppPos, GenerateOffset(0, -1, 0)))
                                {
                                    Debug.Log("Destroying " + p.name);
                                    Destroy(p);
                                }
                                else
                                if (!InteractableCheck(oppPos, GenerateOffset(-1, 0, 0)))
                                {
                                    Debug.Log("Destroying " + p.name);
                                    Destroy(p);
                                }
                                else
                                if (!InteractableCheck(oppPos, GenerateOffset(1, 0, 0)))
                                {
                                    Debug.Log("Destroying " + p.name);
                                    Destroy(p);
                                }
                                else
                                {
                                    objList.Add(p);
                                }
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
                Vector3 k = startPos + GenerateOffset(0, 0, -i * 4);

                RaycastHit hit;
                if (Physics.Raycast(k, GenerateOffset(-1, 0, 0), out hit, 3.0f) && spawns != 1)
                {

                    Quaternion faceWall = Quaternion.LookRotation(k - hit.point);

                    t = GameObject.Instantiate(propToSpawn.Versions[versionToSpawn], k + GenerateOffset(-bedForwardoffset, -bedYoffset, -bedSideoffset), faceWall);

                    t.transform.parent = roomReferencesModular.transform;

                    if (hit.collider.gameObject.name == "Interactable")
                    {
                        Destroy(t);
                    }
                    else
                    if (!InteractableCheck(k, GenerateOffset(0, -1, 0)))
                    {
                        Destroy(t);
                    }
                    else
                    if (!InteractableCheck(k, GenerateOffset(0, 0, -1)))
                    {
                        Destroy(t);
                    }
                    else
                    if (!InteractableCheck(k, GenerateOffset(0, 0, 1)))
                    {
                        Destroy(t);
                    }
                    else
                    {
                        objList.Add(t);
                    }
                    Debug.Log(t.name + "is next to wall.");
                }
                if (Physics.Raycast(k, GenerateOffset(1, 0, 0), out hit, floorXsize[floorToUse] * 4 - 1))
                {
                    Debug.Log("Walls to spawn index: " + spawns);
                    Vector3 oppPos = new Vector3(k.x + floorXsize[floorToUse] * 4 - 4, k.y, k.z);
                    Quaternion faceWall = Quaternion.LookRotation(k - hit.point);
                    switch (spawns)
                    {
                        case 1:
                            Destroy(t);
                            if (Physics.Raycast(oppPos, GenerateOffset(1, 0, 0), out hit, 3.0f))
                            {
                                GameObject p = Instantiate(propToSpawn.Versions[versionToSpawn], oppPos + GenerateOffset(bedForwardoffset + 0.45f, -bedYoffset, bedSideoffset), faceWall);

                                p.transform.parent = roomReferencesModular.transform;

                                if (hit.collider.gameObject.name == "Interactable")
                                {
                                    Destroy(p);
                                }
                                else
                                if (!InteractableCheck(oppPos, GenerateOffset(0, -1, 0)))
                                {
                                    Destroy(p);
                                }
                                else
                                if (!InteractableCheck(oppPos, GenerateOffset(0, 0, -1)))
                                {
                                    Destroy(p);
                                }
                                else
                                if (!InteractableCheck(oppPos, GenerateOffset(0, 0, 1)))
                                {
                                    Destroy(p);
                                }
                                else
                                {
                                    objList.Add(p);
                                }
                            }
                            break;

                        case 2:
                            if (Physics.Raycast(oppPos, GenerateOffset(1, 0, 0), out hit, 3.0f))
                            {
                                GameObject p = Instantiate(propToSpawn.Versions[versionToSpawn], oppPos + GenerateOffset(bedForwardoffset + 0.45f, -bedYoffset, bedSideoffset), faceWall);

                                p.transform.parent = roomReferencesModular.transform;

                                if (hit.collider.gameObject.name == "Interactable")
                                {
                                    Destroy(p);
                                }
                                else
                                if (!InteractableCheck(oppPos, GenerateOffset(0, -1, 0)))
                                {
                                    Destroy(p);
                                }
                                else
                                if (!InteractableCheck(oppPos, GenerateOffset(0, 0, -1)))
                                {
                                    Destroy(p);
                                }
                                else
                                if (!InteractableCheck(oppPos, GenerateOffset(0, 0, 1)))
                                {
                                    Destroy(p);
                                }
                                else
                                {
                                    objList.Add(p);
                                }
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

    private bool InteractableCheck(Vector3 origin, Vector3 direction)
    {
        RaycastHit rayHit;
        if(Physics.Raycast(origin, direction, out rayHit, 3.0f))
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
