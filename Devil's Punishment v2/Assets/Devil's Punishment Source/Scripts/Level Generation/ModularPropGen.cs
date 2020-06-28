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

    [Header("Prototyping Only")]
    public GameObject[] barrelProps;
    public int barrelPropVar;

    public GameObject[] crateProps;
    public int cratePropVar;

    Props[] props;


    public void Generate()
    {
        Debug.Log("Starting prop gen...");
        props = new Props[]
    {
        new Props("Barrel", 40, barrelPropVar, barrelProps),
        new Props("Crate", 40, cratePropVar, crateProps),
    };
        GenerateSpawnPositions();
    }

    private void GenerateSpawnPositions()
    {
        foreach (Vector3 pos in roomReferencesModular.roomFloors)
        {
            int spawnChance = UnityEngine.Random.Range(0, 100);
            Props propToSpawn = props[UnityEngine.Random.Range(0, props.Length)];
            if(spawnChance < propToSpawn.SpawnProb)
            {
                CreateClusterHolder(propToSpawn, pos);
            }
        }
        StartCoroutine(DisableRigidbodiesOnDelay());
    }

    private void CreateClusterHolder(Props propToSpawn, Vector3 pos)
    {
        GameObject k = new GameObject("PropClusterHolder");
        k.transform.parent = roomReferencesModular.gameObject.transform;
        SpawnProps(propToSpawn, pos, k);
    }

    private void SpawnProps(Props propToSpawn, Vector3 pos, GameObject clusterHolder)
    {
        int numProps = UnityEngine.Random.Range(1, 8);


        int propCount = 0;

        for (int i = propCount; i < numProps; i++)
        {
            float randomDistance_x = UnityEngine.Random.Range(-maxSpawnDist, maxSpawnDist);
            float randomDistance_z = UnityEngine.Random.Range(-maxSpawnDist, maxSpawnDist);
            int versionToSpawn = UnityEngine.Random.Range(0, propToSpawn.Variants);
            Vector3 spawnPos = pos + GenerateOffset(randomDistance_x, 3.0f, randomDistance_z);
            //---------Testing purposes only------------//

            //--------Testing segment concluded--------//
            // RaycastHit hit;
            if (PreventOverlap(spawnPos, propToSpawn.Versions[versionToSpawn]))
            {

                GameObject prop = Instantiate(propToSpawn.Versions[versionToSpawn], pos + GenerateOffset(randomDistance_x, 2.25f, randomDistance_z), Quaternion.identity);
                prop.transform.parent = clusterHolder.transform;
                prop.tag = "Prop";

                spawnedProps.Add(prop);
                ++propCount;

           }
            else
            {
                GameObject prop = Instantiate(propToSpawn.Versions[versionToSpawn], pos + GenerateOffset(randomDistance_x, 5.0f, randomDistance_z), Quaternion.identity);
                prop.transform.parent = clusterHolder.transform;
                prop.AddComponent<Rigidbody>();
                prop.tag = "Prop";

                spawnedProps.Add(prop);
                ++propCount;
            }
        }
    }

    private bool PreventOverlap(Vector3 posToCheck, GameObject objBeingSpawned)
    {
        float sphereCastRadius = 0.6f;
        
        bool canSpawn = true;
            for (int i = 0; i < spawnedProps.Count; i++)
            {
                RaycastHit hit;
            Debug.Log("Collider extents = " + objBeingSpawned.GetComponent<Collider>().bounds.extents.x);

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

            if (Physics.SphereCast(posToCheck + GenerateOffset(0, 3.0f, 0), sphereCastRadius, Vector3.down, out hit, 2.5f))
                {
                    //if (posToCheck.z + xExtent > centerPoint.z - radius &&
                    //posToCheck.z + xExtent < centerPoint.z + radius)
                    //{
                    //Return that this object will overlap another and as such can not be spawned
                    canSpawn = false;
                renderer.material.SetColor("_Color", Color.red);
                //}
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
