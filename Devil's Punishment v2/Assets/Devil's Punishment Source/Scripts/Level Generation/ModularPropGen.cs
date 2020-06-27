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
    }

    private void CreateClusterHolder(Props propToSpawn, Vector3 pos)
    {
        GameObject k = new GameObject("PropClusterHolder");
        k.transform.parent = roomReferencesModular.gameObject.transform;
        SpawnProps(propToSpawn, pos, k);
    }

    private void SpawnProps(Props propToSpawn, Vector3 pos, GameObject clusterHolder)
    {
        int numProps = UnityEngine.Random.Range(1, 6);


        int propCount = 0;

        for (int i = propCount; i < numProps; i++)
        {
            float randomDistance_x = UnityEngine.Random.Range(-maxSpawnDist, maxSpawnDist);
            float randomDistance_z = UnityEngine.Random.Range(-maxSpawnDist, maxSpawnDist);
            int versionToSpawn = UnityEngine.Random.Range(0, propToSpawn.Variants);
            Vector3 spawnPos = pos + GenerateOffset(randomDistance_x, 2.25f, randomDistance_z);
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
              Debug.Log("Intersected another prop...");
           }
        }
    }

    private bool PreventOverlap(Vector3 posToCheck, GameObject objBeingSpawned)
    {
        bool canSpawn = false;
        if (spawnedProps.Count == 0)
        {
            canSpawn = true;
        }
        else {
            for (int i = 0; i < spawnedProps.Count; i++)
            {
                Vector3 centerPoint = spawnedProps[i].transform.position;
                float radius = spawnedProps[i].transform.GetChild(0).GetComponent<MeshRenderer>().bounds.extents.x;
                //float height = spawnedProps[i].transform.GetChild(0).GetComponent<MeshRenderer>().bounds.extents.y;   wip

                float xExtent = objBeingSpawned.transform.GetChild(0).GetComponent<MeshRenderer>().bounds.extents.x;
                //float yExtent = objBeingSpawned.GetComponent<LODGroup>().GetLODs()[0].renderers[0].bounds.extents.y;  wip

                if (posToCheck.x + xExtent > centerPoint.x - radius &&
                    posToCheck.x + xExtent < centerPoint.x + radius)
                {
                    //if (posToCheck.z + xExtent > centerPoint.z - radius &&
                    //posToCheck.z + xExtent < centerPoint.z + radius)
                    //{
                    //Return that this object will overlap another and as such can not be spawned
                    canSpawn = false;
                    //}
                }
                else if (posToCheck.x - xExtent > centerPoint.x - radius &&
                  posToCheck.x - xExtent < centerPoint.x + radius)
                {
                    //if (posToCheck.z - xExtent > centerPoint.z - radius &&
                    //posToCheck.z - xExtent < centerPoint.z + radius)
                    //{
                        //Return that this object will overlap another and as such can not be spawned
                        canSpawn = false;
                    //}
                }
                else
                {
                    canSpawn = true;
                }
                Color lineColor;
                if (canSpawn)
                {
                    lineColor = Color.green;
                }
                else
                {
                    lineColor = Color.red;
                }
                Debug.DrawLine(posToCheck, new Vector3(posToCheck.x + xExtent, posToCheck.y, posToCheck.z), lineColor, 10.0f);
                Debug.DrawLine(posToCheck, new Vector3(posToCheck.x - xExtent, posToCheck.y, posToCheck.z), lineColor, 10.0f);
                Debug.DrawLine(posToCheck, new Vector3(posToCheck.x, posToCheck.y, posToCheck.z + xExtent), lineColor, 10.0f);
                Debug.DrawLine(posToCheck, new Vector3(posToCheck.x, posToCheck.y, posToCheck.z - xExtent), lineColor, 10.0f);
                Debug.Log(objBeingSpawned + " parent of " + objBeingSpawned.transform.GetChild(0).name + " checking for intersection with "
    + spawnedProps[i].name + " parent of " + spawnedProps[i].transform.GetChild(0).name + "returns: " + canSpawn);
            }
        }
        return canSpawn;
    }

    private Vector3 GenerateOffset(float x, float y, float z)
    {
        return new Vector3 (x, y, z);
    }
}
