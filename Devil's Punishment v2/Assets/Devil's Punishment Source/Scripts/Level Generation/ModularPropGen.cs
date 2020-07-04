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

    public RoomReferencesModular roomReferencesModular;
    Vector3 roomPos;

    int floor;
    public float maxSpawnDist;
    public List<int> floorXsize;
    public List<int> floorZsize;
    public List<Transform> floorHolder;

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
        StartCoroutine(DisableRigidbodiesOnDelay());

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

                props = new Props[] {
                    new Props("Hospital bed", 100, true, hospitalBeds.Length, hospitalBeds),
                    new Props("Hospital chair", 100, true, hospitalChairs.Length, hospitalChairs)
                };

                if (floors.Count < 12)
                {
                    Debug.LogError("Modular room size too small! Room must have at least 12 tiles to support hospital room type");
                    floors.RemoveRange(0, floors.Count);
                }
                else
                {
                    int floor0Area = floorXsize[0] * floorZsize[0];
                    int floor1Area = floorXsize[1] * floorZsize[1];

                    Vector3 startPos;
                    int floorToUseIndex;
                    int otherFloor;

                    //Get the room with the largest area to have the hospital beds generate there.
                    if (floor0Area > floor1Area)
                    {
                        Transform _floorHolder = floorHolder[0];
                        startPos = _floorHolder.localPosition + floors[0] + GenerateOffset(0, 2.5f, 0);
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
                        startPos = _floorHolder.localPosition + floors[floor0Area] + GenerateOffset(0, 2.5f, 0);
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

                    //Get which side of the room is the longest and spawn the beds there.
                    if (Vector3.Distance(startPos, startPos + GenerateOffset(floorXsize[floorToUseIndex] * 4 + 4f, 0, 0)) > Vector3.Distance(startPos, startPos + GenerateOffset(0, 0, floorZsize[floorToUseIndex] * 4 + 4f)))
                    {
                        Debug.DrawLine(startPos, startPos + GenerateOffset(floorXsize[floorToUseIndex] * 4 - 4f, 0, 0), Color.green, 10.0f);
                        Debug.Log("Floor " + floorToUseIndex + "'s wall along the X axis is longer");

                        //Spawn a bed at every 4 unit interval as that is the interval the floor tiles are placed at.
                        for (int i = 0; i < floorXsize[floorToUseIndex]; i++)
                        {
                            Vector3 k = startPos + GenerateOffset(i * 4, 0, 0);

                            GameObject t = GameObject.CreatePrimitive(PrimitiveType.Cube);

                            t.transform.position = GenerateOffset(k.x, k.y, k.z);

                            //Check if the bed is next to a wall, if not, destroy it. --this may be changed to simply moving the bed to the opposite wall
                            RaycastHit hit;
                            RaycastHit grillCheck;
                            RaycastHit leftDoorCheck;
                            RaycastHit rightDoorCheck;
                            if (Physics.Raycast(k, GenerateOffset(0, 0, 1), out hit, 3.0f))
                            {
                                if (hit.collider.gameObject.name == "Interactable")
                                {
                                    Debug.Log("Destroying " + t.name);
                                    Destroy(t);
                                }
                                if (Physics.Raycast(k, GenerateOffset(0, -1, 0), out grillCheck, 3.0f))
                                {
                                    if (grillCheck.collider.gameObject.name == "Interactable")
                                    {
                                        Debug.Log("Destroying " + t.name);
                                        Destroy(t);
                                    }
                                }
                                if (Physics.Raycast(k, GenerateOffset(-1, 0, 0), out leftDoorCheck, 3.0f))
                                {
                                    if (leftDoorCheck.collider.gameObject.name == "Interactable")
                                    {
                                        Debug.Log("Destroying " + t.name);
                                        Destroy(t);
                                    }
                                }
                                if (Physics.Raycast(k, GenerateOffset(1, 0, 0), out rightDoorCheck, 3.0f))
                                {
                                    if (rightDoorCheck.collider.gameObject.name == "Interactable")
                                    {
                                        Debug.Log("Destroying " + t.name);
                                        Destroy(t);
                                    }
                                }
                            Debug.Log(t.name + "is next to wall.");
                        }
                            if (Physics.Raycast(k, GenerateOffset(0, 0, -1), out hit, floorZsize[floorToUseIndex] * 4 + 1))
                            {
                            }
                            Debug.DrawLine(k, k + GenerateOffset(0, 0, 3), Color.blue, 10.0f);
                            Debug.DrawLine(k, k + GenerateOffset(-3.0f, 0, 0), Color.blue, 10.0f);
                            Debug.DrawLine(k, k + GenerateOffset(3.0f, 0, 0), Color.blue, 10.0f);
                            Debug.DrawLine(k, k + GenerateOffset(0, 0, 1 * -floorZsize[floorToUseIndex] * 4 + 1), Color.red, 10.0f);
                        }
                    } else
                    {
                        Debug.DrawLine(startPos, startPos + GenerateOffset(0, 0, -floorZsize[floorToUseIndex] * 4 + 4f), Color.green, 10.0f);
                        Debug.Log("Floor " + floorToUseIndex + "'s wall along the Z axis is longer");

                        for (int i = 0; i < floorZsize[floorToUseIndex]; i++)
                        {
                            Vector3 k = startPos + GenerateOffset(0, 0, -i * 4);

                            GameObject t = GameObject.CreatePrimitive(PrimitiveType.Cube);

                            t.transform.position = GenerateOffset(k.x, k.y, k.z);

                            RaycastHit hit;
                            RaycastHit grillCheck;
                            RaycastHit leftDoorCheck;
                            RaycastHit rightDoorCheck;
                            if (Physics.Raycast(k, GenerateOffset(-1, 0, 0), out hit, 3.0f)) {
                                if (hit.collider.gameObject.name == "Interactable")
                                {
                                    Destroy(t);
                                }
                            }
                            if (Physics.Raycast(k, GenerateOffset(0, -1, 0), out grillCheck, 3.0f)) {
                                if (grillCheck.collider.gameObject.name == "Interactable")
                                {
                                    Destroy(t);
                                }
                            }
                            if (Physics.Raycast(k, GenerateOffset(0, 0, -1), out leftDoorCheck, 3.0f)) {
                                if (leftDoorCheck.collider.gameObject.name == "Interactable") 
                                {
                                    Destroy(t);
                                }
                            }
                                if(Physics.Raycast(k, GenerateOffset(0, 0, 1), out rightDoorCheck, 3.0f)){
                                if(rightDoorCheck.collider.gameObject.name == "Interactable")
                                {
                                    Destroy(t);
                                }
                                Debug.Log(t.name + "is next to wall.");
                            }
                            if (Physics.Raycast(k, GenerateOffset(1, 0, 0), out hit, floorXsize[floorToUseIndex] * 4 - 1))
                            {
                            }
                            Debug.DrawLine(k, k + GenerateOffset(-3.0f, 0, 0), Color.blue, 10.0f);
                            Debug.DrawLine(k, k + GenerateOffset(0, 0, -3.0f), Color.blue, 10.0f);
                            Debug.DrawLine(k, k + GenerateOffset(0, 0, -3.0f), Color.blue, 10.0f);
                            Debug.DrawLine(k, k + GenerateOffset(1 * floorXsize[floorToUseIndex] * 4 - 1, 0, 0), Color.red, 10.0f);
                        }
                    }

                }
                break;
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
