using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Rectangular / Cuboidal room for now
public class ModularRoomAssembler : MonoBehaviour
{

    public Transform door_corridor_Transform;
    public Vector3 door_corridor_Pos;
    public Vector3 door_room_Pos;
    public Material mat;
    //public Texture2D packedTexture;

    private Transform roomHolderTransform;
    private List<Transform> partHolderTransforms;
    private Transform walls_holder;
    private Transform walls_holder2;
    private Transform floor_holder;
    private Transform ceiling_holder;

    private int height = 2;

    public GameObject side_wall;
    public GameObject wall_with_door;
    public GameObject floor;
    public GameObject ceiling;
    public GameObject grill;
    [SerializeField]
    private List<int> size_x, size_z;
    private int noOfParts = 2;

    // Start is called before the first frame update
    void Start()
    {
        StartScript();
    }

    public void StartScript()
    {
        Debug.Log(Random.Range(1, 1));
        door_corridor_Pos = door_corridor_Transform.position;

        roomHolderTransform = new GameObject("Modular Room 1").transform;
        roomHolderTransform.tag = "Modular Room stuff";

        size_x = new List<int>();
        size_z = new List<int>();

        partHolderTransforms = new List<Transform>();

        for (int i = 0; i < noOfParts; i++)
        {
            ChooseSize(i);
            Vector3 pos;
            if(i == 0)
            {
                pos = Vector3.zero;
            }
            else
            {
                pos = TranslatePartToChosenDirection(i);
            }
            DecideRoomOriginOrCorner_CreateHolders(i, pos);

            door_room_Pos = door_corridor_Pos + new Vector3(0, 2, -2) - roomHolderTransform.position;

            PlaceFloor_Ceiling(i);
            PlaceWall(i);

            CombineMeshes(i);
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private int SumOfList(char list)
    {
        int sum = 0;
        if (list == 'z')
        {
            for (int i = 0; i < size_z.Count; i++)
            {
                sum += size_z[i];
            }
            return sum;
        }
        else if (list == 'x')
        {
            for (int i = 0; i < size_x.Count; i++)
            {
                sum += size_x[i];
            }
            return sum;
        }
        Debug.LogError("Wrong List identifier as parameter '" + list + "'");
        return -1;
    }

    private void ChooseSize(int partNo)
    {
        int sum_x = SumOfList('x');
        int sum_z = SumOfList('z');
        if(40 - sum_x < 8 / 4)
        {
            Debug.LogError("Ouch! Room part size less than 2");
            this.enabled = false;
        }
        if (40 - sum_z < 8 / 4)
        {
            Debug.LogError("Ouch! Room part size less than 2");
            this.enabled = false;
        }
        size_x.Add(Random.Range(8 / 4, 40 / 4)); //Change to 36 units max size if needed
        size_z.Add(Random.Range(8 / 4, 40 / 4));
        Debug.Log("partno " + partNo + "; size_x =" + size_x[partNo] + "; size_z =" + size_z[partNo]);
        size_x[partNo]--;
        size_z[partNo]--;
        if(size_x[partNo] < 0 || size_z[partNo] < 0)
        {
            Debug.LogError("Ouch! Negative room part size");
            this.enabled = false;
        }
    }

    private int NSWEPartChoose(int partNo)
    {
        return Random.Range(1, 4); //0, 4 if coveering corridor door is solved
    }

    private int ZRandomPartTranslationHelper(int size1, int size2)
    {
        if(size1 + size2 > 4)
        {
            int res = Random.Range(-(size1 - 2), size2 - 2);
            if(res >= size1 || res >= size2)
            {
                res = size1 - 1;
                Debug.Log("PHEW");
            }
            Debug.Log("res = " + res);
            return res;
        }
        Debug.Log(" :( in Z");
        return 0;
    }

    private int XRandomPartTranslationHelper(int size1, int size2)
    {
        if (size1 + size2 > 4)
        {
            int res = Random.Range(-(size1 - 2), size2 - 2);
            if (res >= size1 || res >= size2)
            {
                res = size1 - 1;
                Debug.Log("PHEW");
            }
            Debug.Log("res = " + res);
            return res;
        }
        Debug.Log(" :( in X");
        return 0;
    }

    private Vector3 TranslatePartToChosenDirection(int partNo)
    {
        int x = NSWEPartChoose(partNo);
        Debug.Log("NSWEPartChoose(partNo) = " + x);
        Vector3 partHolderPartNoPos = Vector3.zero;
        switch (x)
        {
            case 0:
                partHolderPartNoPos = new Vector3(partHolderTransforms[0].position.x + 4 * XRandomPartTranslationHelper(size_x[partNo], size_x[0]),
                                                                partHolderTransforms[0].position.y,
                                                                partHolderTransforms[0].position.z + 4 * (size_z[partNo] + 1));
                break;
            case 1:
                partHolderPartNoPos = new Vector3(partHolderTransforms[0].position.x + 4 * (size_x[0] + 1),
                                                                partHolderTransforms[0].position.y,
                                                                partHolderTransforms[0].position.z + 4 * ZRandomPartTranslationHelper(size_z[partNo], size_z[0]));
                break;
            case 2:
                partHolderPartNoPos = new Vector3(partHolderTransforms[0].position.x + 4 * XRandomPartTranslationHelper(size_x[partNo], size_x[0]),
                                                                partHolderTransforms[0].position.y,
                                                                partHolderTransforms[0].position.z - 4 * (size_z[0] + 1));
                break;
            case 3:
                partHolderPartNoPos = new Vector3(partHolderTransforms[0].position.x - 4 * (size_x[partNo] + 1),
                                                                partHolderTransforms[0].position.y,
                                                                partHolderTransforms[0].position.z + 4 * ZRandomPartTranslationHelper(size_z[partNo], size_z[0]));
                break;
        }
        return partHolderPartNoPos;
    }

    private void DecideRoomOriginOrCorner_CreateHolders(int partNo, Vector3 startFloorPos)
    {
        if(partNo == 0)
        {
            startFloorPos = new Vector3(door_corridor_Pos.x + Random.Range(-size_x[partNo], 0) * 4, door_corridor_Pos.y, door_corridor_Pos.z - 4);
        }
        GameObject sfp = new GameObject("startFloorPos");
        sfp.transform.position = startFloorPos;
        sfp.tag = "Modular Room stuff";
        partHolderTransforms.Add(new GameObject("Modular Room Part " + partNo).transform);
        partHolderTransforms[partNo].position = startFloorPos;
        partHolderTransforms[partNo].parent = roomHolderTransform;
        walls_holder = new GameObject("walls_holder").transform;
        walls_holder2 = new GameObject("walls_holder").transform;
        floor_holder = new GameObject("floor_holder").transform;
        ceiling_holder = new GameObject("ceiling_holder").transform;
        walls_holder.parent = walls_holder2.parent = floor_holder.parent = ceiling_holder.parent = partHolderTransforms[partNo];
        walls_holder.position = floor_holder.position = ceiling_holder.position = startFloorPos;
        walls_holder2.position = startFloorPos + new Vector3(4 * size_x[partNo], 0, 0);
    }

    private void PlaceFloor_Ceiling(int partNo)
    {
        for (int i = 0; i <= size_x[partNo]; i++)
        {
            for (int j = 0; j <= size_z[partNo]; j++)
            {
                //Floor
                Transform t = Instantiate(floor).transform;
                t.parent = floor_holder;
                t.localPosition = new Vector3(i * 4, 0, -j * 4);
                if(j == 2 && i == 2)
                {
                    t = Instantiate(grill).transform;
                    t.parent = floor_holder;
                    t.localPosition = new Vector3(i * 4, .2f, -j * 4);
                }

                //Ceilng
                t = Instantiate(ceiling).transform;
                t.parent = ceiling_holder;
                t.localPosition = new Vector3(i * 4, height * 4, -j * 4);
            }
        }
    }

    private void PlaceWall(int partNo)
    {
        Vector3 startWallPos = new Vector3(roomHolderTransform.position.x, roomHolderTransform.position.y, roomHolderTransform.position.z - 4 / 2);
        for (int i = 0; i <= size_x[partNo]; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject toSpawn = side_wall;
                Vector3 pos = new Vector3(i * 4, 2 + j * 4, 4 / 2);
                if(pos == door_room_Pos)
                {
                    toSpawn = wall_with_door;
                }
                Transform t = Instantiate(toSpawn).transform;
                t.parent = walls_holder;
                t.localPosition = pos;
                t.localEulerAngles = new Vector3(t.localEulerAngles.x, t.localEulerAngles.y + 90, t.localEulerAngles.z);

                toSpawn = side_wall;
                pos = new Vector3(i * 4, 2 + j * 4, -4 / 2 - size_z[partNo] * 4);
                if (pos == door_room_Pos)
                {
                    toSpawn = wall_with_door;
                }
                t = Instantiate(toSpawn).transform;
                t.parent = walls_holder;
                t.localPosition = pos;
                t.localEulerAngles = new Vector3(t.localEulerAngles.x, t.localEulerAngles.y + 90, t.localEulerAngles.z);
            }
        }

        startWallPos += new Vector3(-4 * size_x[partNo], 0, -4 * size_z[partNo]);
        for (int i = 0; i <= size_z[partNo]; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Transform t = Instantiate(side_wall).transform;
                t.parent = walls_holder2;
                t.localPosition = new Vector3(4 / 2, 2 + j * 4, -i * 4);
                //t.localEulerAngles = new Vector3(t.localEulerAngles.x, t.localEulerAngles.y + 90, t.localEulerAngles.z);

                t = Instantiate(side_wall).transform;
                t.parent = walls_holder2;
                t.localPosition = new Vector3(- size_x[partNo] * 4 - 4 / 2, 2 + j * 4, -i * 4);
                //t.localEulerAngles = new Vector3(t.localEulerAngles.x, t.localEulerAngles.y + 90, t.localEulerAngles.z);
            }
        }
    }

    //Code from Unity Scripting API
    private void CombineMeshes(int partNo)
    {
        Rect[] rects;
        Texture2D[] atlasTextures;

        MeshFilter[] meshFilters = partHolderTransforms[partNo].GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        atlasTextures = new Texture2D[meshFilters.Length];
        Material matNew = new Material(Shader.Find("Standard"));

        int i = 0;
        while (i < meshFilters.Length)
        {
            if (!(meshFilters[i].gameObject.name.StartsWith("Vent Covers")))
            {
                //Debug.Log(meshFilters[i].gameObject.GetComponent<MeshRenderer>());//.sharedMaterial.mainTexture.isReadable);
                //Debug.Log("meshFilters[i].gameObject.GetComponent<MeshRenderer>().sharedMaterial.mainTexture");
                atlasTextures[i] = (Texture2D)meshFilters[i].gameObject.GetComponent<MeshRenderer>().sharedMaterial.mainTexture;
            }
            //meshFilters[i].gameObject.GetComponent<Renderer>().material = matNew;

            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            //Debug.Log("meshfilters = " + meshFilters[i].gameObject.name);
            //meshFilters[i].gameObject.SetActive(false);

            i++;
        }
        roomHolderTransform.gameObject.SetActive(false);
        GameObject room = new GameObject("Combined Room");
        room.tag = "Modular Room stuff";
        MeshFilter meshFilter = room.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = room.AddComponent<MeshRenderer>();
        //meshRenderer.sharedMaterial = mat;
        meshFilter.mesh = new Mesh();
        meshFilter.mesh.CombineMeshes(combine);


        Texture2D packedTexture = new Texture2D(2048, 2048);
        // Pack the individual textures into the smallest possible space,
        // while leaving a two pixel gap between their edges.
        rects = packedTexture.PackTextures(atlasTextures, 0, 2048);

        Vector2[] uva, uvb;
        for (int j = 0; j < meshFilters.Length; j++)
        {
            uva = (Vector2[])(((MeshFilter)meshFilters[j]).mesh.uv);
            uvb = new Vector2[uva.Length];
            for (int k = 0; k < uva.Length; k++)
            {
                uvb[k] = new Vector2((uva[k].x * rects[j].width) + rects[j].x, (uva[k].y * rects[j].height) + rects[j].y);
            }
            meshFilters[j].mesh.uv = uvb;
        }
        
        //matNew.mainTexture = packedTexture;
        meshRenderer.sharedMaterial = matNew;
        meshRenderer.sharedMaterial.mainTextureOffset = new Vector2(rects[2].x, rects[2].y);
        meshRenderer.sharedMaterial.mainTextureScale = new Vector2(rects[2].width, rects[2].height);
        Debug.Log(rects.Length);
        
        matNew.mainTexture = packedTexture;
        meshFilter.gameObject.GetComponent<MeshRenderer>().sharedMaterial = matNew;
    }
}
