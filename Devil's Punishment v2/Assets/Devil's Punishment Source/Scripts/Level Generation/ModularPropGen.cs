using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Props
{
    string propName;
    int spawnProb;
    int variants;
    GameObject[] versions;

    public Props(string PropName, int SpawnProb, int Variants, GameObject[] Versions)
    {
        propName = PropName;
        spawnProb = SpawnProb;
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

    Props[] props;

    //Method to generate props randomly across the room in random positions, only for debugging purposes.
    public void GenerateTestScatterProps()
    {
        Debug.Log("Starting prop gen...");
        
        //Create a Props array to hold the props we are going to be spawning in.
        props = new Props[]
    {
        //You could actually just use the GameObject array you are using's Length value for the variants int variable
        new Props("Barrel", 40, barrelPropVar, barrelProps),
        new Props("Crate", 40, cratePropVar, crateProps),
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
    }

    private void SpawnProps(Props propToSpawn, Vector3 floorPos, GameObject clusterHolder, float xSpawnPos, float zSpawnPos)
    {
        
        int versionToSpawn = UnityEngine.Random.Range(0, propToSpawn.Variants);
        Vector3 spawnPos = floorPos + GenerateOffset(xSpawnPos, 3.0f, zSpawnPos);

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
    }

    private void GenerateProps(string room)
    {
        //Check which room type is being spawned and assign the props to be used accordingly.
        switch (room)
        {
            case "Hospital":

                props = new Props[] {
                    new Props("Hospital", 1, hospitalBeds.Length, hospitalBeds)
                };

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
            and its radius to the width of object being spawned. A green circle indicates that the spawn position is available, a red indicates
            that the position is taken. */

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

            //If there is an object overlapping the sphere check for the object's spawn position, then return canSpawn as false, otherwise, if the position is vacant, return canSpawn as true.
        if (Physics.SphereCast(posToCheck + GenerateOffset(0, 3.0f, 0), sphereCastRadius, Vector3.down, out hit, 2.5f))
        {
            canSpawn = false;
            renderer.material.SetColor("_Color", Color.red);
        }
        else
        {

            renderer.material.SetColor("_Color", Color.green);
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
