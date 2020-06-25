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
    List<Vector3> possibleSpawnPos = new List<Vector3>();

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
        new Props("Barrel", 20, barrelPropVar, barrelProps),
        new Props("Crate", 20, cratePropVar, crateProps),
    };
        GenerateSpawnPositions();
    }

    private void GenerateSpawnPositions()
    {
        foreach (Vector3 pos in roomReferencesModular.roomFloors)
        {
            int spawnChance = Random.Range(0, 100);
            Props propToSpawn = props[Random.Range(0, props.Length)];
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
        int numProps = Random.Range(1, 6);

        List<GameObject> propsInCluster = new List<GameObject>();

        int propCount = 0;

        for (int i = propCount; i < numProps; i++)
        {
            float randomDistance_x = Random.Range(-maxSpawnDist, maxSpawnDist);
            float randomDistance_z = Random.Range(-maxSpawnDist, maxSpawnDist);
            //Vector3 rayOrigin = pos + GenerateOffset(randomDistance_x, 6, randomDistance_z);
           // RaycastHit hit;
            //if (Physics.Raycast(rayOrigin, Vector3.down, out hit) && !hit.collider.gameObject.CompareTag("Prop"))
            //{
                int versionToSpawn = Random.Range(0, propToSpawn.Variants);
                GameObject prop = Instantiate(propToSpawn.Versions[versionToSpawn], pos + GenerateOffset(randomDistance_x, 2.25f, randomDistance_z), Quaternion.identity);
                prop.transform.parent = clusterHolder.transform;
                propsInCluster.Add(prop);
                ++propCount;
            //} else
           // {
             //   Debug.Log("Intersected another prop...");
            //}
        }
    }

    private Vector3 GenerateOffset(float x, float y, float z)
    {
        return new Vector3 (x, y, z);
    }
}
