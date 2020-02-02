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
    private int size_x, size_z;

    // Start is called before the first frame update
    void Start()
    {

        door_corridor_Pos = door_corridor_Transform.position;

        ChooseSize();
        DecideRoomOriginOrCorner_CreateHolders();

        door_room_Pos = door_corridor_Pos + new Vector3(0, 2, -2) - roomHolderTransform.position;

        PlaceFloor_Ceiling();
        PlaceWall();

        CombineMeshes();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ChooseSize()
    {
        size_x = Random.Range(20 / 4, 40 / 4);
        size_z = Random.Range(20 / 4, 40 / 4);
    }

    private void DecideRoomOriginOrCorner_CreateHolders()
    {
        Vector3 startFloorPos = new Vector3(door_corridor_Pos.x + Random.Range(-size_x, 0) * 4, door_corridor_Pos.y, door_corridor_Pos.z - 4);
        new GameObject("startFloorPos").transform.position = startFloorPos;
        roomHolderTransform = new GameObject("Modular Room 1").transform;
        roomHolderTransform.position = startFloorPos;
        walls_holder = new GameObject("walls_holder").transform;
        walls_holder2 = new GameObject("walls_holder").transform;
        floor_holder = new GameObject("floor_holder").transform;
        ceiling_holder = new GameObject("ceiling_holder").transform;
        walls_holder.parent = walls_holder2.parent = floor_holder.parent = ceiling_holder.parent = roomHolderTransform;
        walls_holder.position = floor_holder.position = ceiling_holder.position = startFloorPos;
        walls_holder2.position = startFloorPos + new Vector3(4 * size_x, 0, 0);
    }

    private void PlaceFloor_Ceiling()
    {
        for (int i = 0; i <= size_x; i++)
        {
            for (int j = 0; j <= size_z; j++)
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

    private void PlaceWall()
    {
        Vector3 startWallPos = new Vector3(roomHolderTransform.position.x, roomHolderTransform.position.y, roomHolderTransform.position.z - 4 / 2);
        for (int i = 0; i <= size_x; i++)
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
                pos = new Vector3(i * 4, 2 + j * 4, -4 / 2 - size_z * 4);
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

        startWallPos += new Vector3(-4 * size_x, 0, -4 * size_z);
        for (int i = 0; i <= size_z; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Transform t = Instantiate(side_wall).transform;
                t.parent = walls_holder2;
                t.localPosition = new Vector3(4 / 2, 2 + j * 4, -i * 4);
                //t.localEulerAngles = new Vector3(t.localEulerAngles.x, t.localEulerAngles.y + 90, t.localEulerAngles.z);

                t = Instantiate(side_wall).transform;
                t.parent = walls_holder2;
                t.localPosition = new Vector3(- size_x * 4 - 4 / 2, 2 + j * 4, -i * 4);
                //t.localEulerAngles = new Vector3(t.localEulerAngles.x, t.localEulerAngles.y + 90, t.localEulerAngles.z);
            }
        }
    }

    //Code from Unity Scripting API
    private void CombineMeshes()
    {
        Rect[] rects;
        Texture2D[] atlasTextures;

        MeshFilter[] meshFilters = roomHolderTransform.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        atlasTextures = new Texture2D[meshFilters.Length];
        Material matNew = new Material(Shader.Find("Standard"));

        int i = 0;
        while (i < meshFilters.Length)
        {
            if (!(meshFilters[i].gameObject.name.StartsWith("Vent Covers")))
            {
                Debug.Log(meshFilters[i].gameObject.GetComponent<MeshRenderer>());//.sharedMaterial.mainTexture.isReadable);
                //Debug.Log("meshFilters[i].gameObject.GetComponent<MeshRenderer>().sharedMaterial.mainTexture");
                atlasTextures[i] = (Texture2D)meshFilters[i].gameObject.GetComponent<MeshRenderer>().sharedMaterial.mainTexture;
            }
            //meshFilters[i].gameObject.GetComponent<Renderer>().material = matNew;

            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            Debug.Log("meshfilters = " + meshFilters[i].gameObject.name);
            //meshFilters[i].gameObject.SetActive(false);

            i++;
        }
        roomHolderTransform.gameObject.SetActive(false);
        GameObject room = new GameObject("Combined Room");
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
