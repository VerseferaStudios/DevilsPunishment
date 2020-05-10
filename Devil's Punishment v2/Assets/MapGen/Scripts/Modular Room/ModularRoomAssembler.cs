using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Rectangular / Cuboidal room for now
public class ModularRoomAssembler : MonoBehaviour
{

    public Transform door_corridor_Transform;
    public Vector3 door_corridor_Pos;
    public Vector3 roomCentrePos;
    public Vector3 wall_roomdoor_Pos;
    public GameObject[] doors;
    public Material mat;
    //public Texture2D packedTexture;

    public Transform roomHolderTransform;
    private List<Transform> partHolderTransforms;
    private List<Transform> walls_holder;
    private List<Transform> walls_holder2;
    private List<Transform> floor_holder;
    private List<Transform> ceiling_holder;
    private Transform extra_walls_holder;

    private int height = 2;

    public GameObject side_wall;
    public GameObject wall_with_door;
    public GameObject floor;
    public GameObject ceiling;
    public GameObject grill;
    [SerializeField]
    private List<int> size_x, size_z;
    private int noOfParts = 2;
    private List<Vector3> pos;
    private List<Vector3> startFloorPosList;

    private List<int> nswe_helper;

    private bool[] door_done;

    public RoomReferencesModular roomReferencesModular;

    // Start is called before the first frame update
    void Start()
    {
        //StartScript();
    }

    public void StartScript()
    {
        doors = new GameObject[noOfParts];
        Debug.Log(Random.Range(1, 1));
        doors[0] = door_corridor_Transform.gameObject;
        door_corridor_Pos = door_corridor_Transform.position;
        door_done = new bool[noOfParts];
        door_done[0] = true;

        roomHolderTransform = new GameObject("Modular Room 1").transform;
        roomCentrePos = door_corridor_Pos - new Vector3(0, 0, 20);
        roomHolderTransform.position = roomCentrePos;
        roomHolderTransform.tag = "Modular Room stuff";

        size_x = new List<int>();
        size_z = new List<int>();

        partHolderTransforms = new List<Transform>();
        walls_holder = new List<Transform>();
        walls_holder2 = new List<Transform>();
        floor_holder = new List<Transform>();
        ceiling_holder = new List<Transform>();

        pos = new List<Vector3>();
        startFloorPosList = new List<Vector3>();

        nswe_helper = new List<int>();
        nswe_helper.Add(-1); //Since main room part doesnt have nswe //To keep PartNo intact

        for (int i = 0; i < noOfParts; i++)
        {
            ChooseSize(i);
            if (i == 0)
            {
                pos.Add(Vector3.zero);
            }
            else
            {
                pos.Add(TranslatePartToChosenDirection(i));
            }
            DecideRoomOriginOrCorner_CreateHolders(i, pos[i]);
            wall_roomdoor_Pos = door_corridor_Pos + new Vector3(0, 2, -2) - roomHolderTransform.position; // doesnt work yet
            if(i == 0)
            {
                doors[0].transform.SetParent(partHolderTransforms[0]);
                doors[0].tag = "Corridor Spawn Points";
            }

        }


        PlaceFloor_Ceiling(0);
        PlaceWall(0);

        Swap_NSWE();

        for (int i = 1; i < noOfParts; i++)
        {
            PlaceFloor_Ceiling(i);
            PlaceWall(i);
        }
/*
        for (int i = 0; i < noOfParts; i++)
            CombineMeshes(i);
            */
        //StartCoroutine(CombineAfterDelay());
        //PlaceRemainingWalls();
        Debug.Log(Random.Range(1, 1));

    }

    private IEnumerator CombineAfterDelay()
    {
        yield return new WaitForSeconds(4);
        GameObject[] gbs = GameObject.FindGameObjectsWithTag("Modular Room Collision Detector");
        for (int i = 0; i < gbs.Length; i++)
        {
            Destroy(gbs[i]);
        }
        yield return new WaitForSeconds(4);
        for (int i = 0; i < noOfParts; i++)
            CombineMeshes(i);
    }

    private void Swap_NSWE()
    {
        for (int i = 0; i < nswe_helper.Count; i++)
        {
            if(nswe_helper[i] == 0)
            {
                nswe_helper[i] = 2;
            }
            else if (nswe_helper[i] == 1)
            {
                nswe_helper[i] = 3;
            }
            else if (nswe_helper[i] == 2)
            {
                nswe_helper[i] = 0;
            }
            else if (nswe_helper[i] == 3)
            {
                nswe_helper[i] = 1;
            }
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
        size_x.Add(Random.Range((8 / 4) / 2, (40 / 4) / 2)); //Change to 36 units max size if needed
        size_z.Add(Random.Range((8 / 4) / 2, (40 / 4) / 2));
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
        int x = Random.Range(1, 4);
        Debug.Log("Choosing NSWE = " + x);
        nswe_helper.Add(x);
        return x; //0, 4 if coveering corridor door is solved
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
        startFloorPosList.Add(startFloorPos);
        GameObject sfp = new GameObject("startFloorPos");
        sfp.transform.position = startFloorPos;
        sfp.tag = "Modular Room stuff";
        partHolderTransforms.Add(new GameObject("Modular Room Part " + partNo).transform);
        partHolderTransforms[partNo].position = startFloorPos;
        partHolderTransforms[partNo].parent = roomHolderTransform;
        walls_holder.Add(new GameObject("walls_holder").transform);
        walls_holder2.Add(new GameObject("walls_holder").transform);
        floor_holder.Add(new GameObject("floor_holder").transform);
        ceiling_holder.Add(new GameObject("ceiling_holder").transform);
        walls_holder[partNo].parent = walls_holder2[partNo].parent = floor_holder[partNo].parent = ceiling_holder[partNo].parent = partHolderTransforms[partNo];
        walls_holder[partNo].position = floor_holder[partNo].position = ceiling_holder[partNo].position = startFloorPos;
        walls_holder2[partNo].position = startFloorPos + new Vector3(4 * size_x[partNo], 0, 0);
    }

    private void PlaceFloor_Ceiling(int partNo)
    {
        for (int i = 0; i <= size_x[partNo]; i++)
        {
            for (int j = 0; j <= size_z[partNo]; j++)
            {
                //Floor
                Transform t = Instantiate(floor).transform;
                t.parent = floor_holder[partNo];
                t.localPosition = new Vector3(i * 4, 0, -j * 4);

                roomReferencesModular.roomFloors.Add(t.position);

                if (j == 2 && i == 2)
                {
                    t = Instantiate(grill).transform;
                    t.parent = floor_holder[partNo];
                    t.localPosition = new Vector3(i * 4, .2f, -j * 4);
                }

                //Ceilng
                t = Instantiate(ceiling).transform;
                t.parent = ceiling_holder[partNo];
                t.localPosition = new Vector3(i * 4, height * 4, -j * 4);
            }
        }
    }
    /*
    private bool CheckWallOverlap(int partNo, Vector3 pos_to_check, char x_or_z, int nswe)
    {
        float startFloorPos0;
        float startFloorPos0Normal;
        float size0;
        float size0Normal;
        float startFloorPosPartNo;
        float startFloorPosPartNoNormal;
        float sizePartNo;
        float sizePartNoNormal;
        float posToCheck;
        float posToCheckNormal;
        if(x_or_z == 'Z')
        {
            startFloorPos0 = startFloorPosList[0].z;
            startFloorPos0Normal = startFloorPosList[0].x;
            size0 = size_z[0] * 4;
            size0Normal = size_x[0] * 4;
            startFloorPosPartNo = startFloorPosList[partNo].z;
            startFloorPosPartNoNormal = startFloorPosList[partNo].x;
            sizePartNo = size_z[partNo] * 4;
            sizePartNoNormal = size_x[partNo] * 4;
            posToCheck = pos_to_check.z + pos[partNo].z;
            posToCheckNormal = pos_to_check.x + pos[partNo].x;
        }
        else
        {
            startFloorPos0 = startFloorPosList[0].x;
            startFloorPos0Normal = startFloorPosList[0].z;
            size0 = size_x[0] * 4;
            size0Normal = size_z[0] * 4;
            startFloorPosPartNo = startFloorPosList[partNo].x;
            startFloorPosPartNoNormal = startFloorPosList[partNo].z;
            sizePartNo = size_x[partNo] * 4;
            sizePartNoNormal = size_z[partNo] * 4;
            posToCheck = pos_to_check.x + pos[partNo].x;
            posToCheckNormal = pos_to_check.z + pos[partNo].z;
        }

        if ((startFloorPos0 <= posToCheck && posToCheck <= startFloorPos0 - size0 ||
            startFloorPos0 >= posToCheck && posToCheck >= startFloorPos0 - size0) &&
            (startFloorPosPartNo <= posToCheck && posToCheck <= startFloorPosPartNo - sizePartNo ||
            startFloorPosPartNo >= posToCheck && posToCheck >= startFloorPosPartNo - sizePartNo) &&
            (startFloorPos0Normal <= posToCheckNormal && posToCheckNormal <= startFloorPosPartNoNormal ||
            startFloorPos0Normal >= posToCheckNormal && posToCheckNormal >= startFloorPosPartNoNormal))// &&
            //startFloorPosPartNoNormal <= posToCheckNormal && posToCheckNormal <= startFloorPosPartNoNormal)
        {
            return true;
        }
        return false;

        //if(startfloorposlist[0].z >= startfloorposlist[partno].z)
        //{
        //    return startfloorposlist[0].z - startfloorposlist[partno].z >= size_z[0];
        //}
        //else
        //{
        //    return startfloorposlist[0].z - startfloorposlist[partno].z >= size_z[0];
        //}
    }
    */
    private void PlaceWall(int partNo)
    {

        Debug.Log("Placing WALL for partNo = " + partNo);

        Vector3 startWallPos = new Vector3(roomHolderTransform.position.x, roomHolderTransform.position.y, roomHolderTransform.position.z - 4 / 2);
        Transform t;
        Vector3 posCurr = Vector3.zero;
        GameObject toSpawn;

        //if (!CheckWallOverlap(partNo, pos, 'X', nswe_helper[partNo]))
        //if (!nswe_helper.Contains(0))
        //if (!nswe_helper.Contains(0))
        {
            for (int i = 0; i <= size_x[partNo]; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    toSpawn = side_wall;
                    posCurr = new Vector3(i * 4, 2 + j * 4, 4 / 2);
                    if (posCurr == wall_roomdoor_Pos)
                    {
                        toSpawn = wall_with_door;
                    }
                    else if (j == 0 && !door_done[partNo] && nswe_helper[partNo] != 0 && Random.Range(0f, 1f) < .1f)
                    {
                        DoorSpawnHelper("Door+z", partNo, false, posCurr, 0, 2);
                    }
                    t = Instantiate(toSpawn).transform;
                    t.parent = walls_holder[partNo];
                    t.localPosition = posCurr;
                    t.localEulerAngles = new Vector3(t.localEulerAngles.x, t.localEulerAngles.y + 90, t.localEulerAngles.z);
                }
            }
            if (!door_done[partNo])
            {
                DoorSpawnHelper("Door+z", partNo, false, posCurr, 0, 2);
            }
        }


        //if (!CheckWallOverlap(partNo, pos, 'X', nswe_helper[partNo]))
        //if (!nswe_helper.Contains(2))
        //if (!nswe_helper.Contains(2))
        {
            for (int i = 0; i <= size_x[partNo]; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    toSpawn = side_wall;
                    posCurr = new Vector3(i * 4, 2 + j * 4, -4 / 2 - size_z[partNo] * 4);
                    if (posCurr == wall_roomdoor_Pos)
                    {
                        toSpawn = wall_with_door;
                    }
                    else if (j == 0 && !door_done[partNo] && nswe_helper[partNo] != 2 && Random.Range(0f, 1f) < .1f)
                    {
                        DoorSpawnHelper("Door-z", partNo, false, posCurr, 0, -2);
                    }
                    t = Instantiate(toSpawn).transform;
                    t.parent = walls_holder[partNo];
                    t.localPosition = posCurr;
                    t.localEulerAngles = new Vector3(t.localEulerAngles.x, t.localEulerAngles.y + 90, t.localEulerAngles.z);
                }
            }
            if (!door_done[partNo])
            {
                DoorSpawnHelper("Door-z", partNo, false, posCurr, 0, -2);
            }
        }

        startWallPos += new Vector3(-4 * size_x[partNo], 0, -4 * size_z[partNo]);
        //if (!CheckWallOverlap(partNo, pos, 'Z'))
        //if (!nswe_helper.Contains(1))
        //if (!nswe_helper.Contains(1))
        {
            for (int i = 0; i <= size_z[partNo]; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    posCurr = new Vector3(4 / 2, 2 + j * 4, -i * 4);
                    if (j == 0 && !door_done[partNo] && nswe_helper[partNo] != 1 && Random.Range(0f, 1f) < .1f)
                    {
                        DoorSpawnHelper("Door+x", partNo, true, posCurr, 2, 0);
                    }
                    t = Instantiate(side_wall).transform;
                    t.parent = walls_holder2[partNo];
                    t.localPosition = posCurr;
                    //t.localEulerAngles = new Vector3(t.localEulerAngles.x, t.localEulerAngles.y + 90, t.localEulerAngles.z);
                }
            }
            if (!door_done[partNo])
            {
                DoorSpawnHelper("Door+x", partNo, true, posCurr, 2, 0);
            }
        }

        //if (!CheckWallOverlap(partNo, pos, 'Z'))
        //if (!nswe_helper.Contains(3))
        //if (!nswe_helper.Contains(3))
        {
            for (int i = 0; i <= size_z[partNo]; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    posCurr = new Vector3(-size_x[partNo] * 4 - 4 / 2, 2 + j * 4, -i * 4);
                    if (j == 0 && !door_done[partNo] && nswe_helper[partNo] != 3 && Random.Range(0f, 1f) < .1f)
                    {
                        DoorSpawnHelper("Door-x", partNo, true, posCurr, -2, 0);
                    }
                    t = Instantiate(side_wall).transform;
                    t.parent = walls_holder2[partNo];
                    t.localPosition = posCurr;
                    //t.localEulerAngles = new Vector3(t.localEulerAngles.x, t.localEulerAngles.y + 90, t.localEulerAngles.z);
                }
            }
        }
        if (!door_done[partNo])
        {
            DoorSpawnHelper("Door-x", partNo, true, posCurr, -2, 0);
        }
    }

    private void DoorSpawnHelper(string doorName, int partNo, bool isWallHolder2, Vector3 posCurr, int xAdd, int zAdd)
    {
        doors[partNo] = new GameObject(doorName);
        doors[partNo].tag = "Corridor Spawn Points";
        doors[partNo].transform.SetParent(partHolderTransforms[partNo]);
        Vector3 startPos = ((isWallHolder2) ? walls_holder2 : walls_holder)[partNo].position + posCurr + new Vector3(xAdd, -2, zAdd);
        Vector3 doorPos = startPos;

        if (doorName[5] == 'z')
        {
            doorPos.z = 20 * (doorName[4] == '-' ? -1 : 1) + roomCentrePos.z;
        }
        else
        {
            doorPos.x = 20 * (doorName[4] == '-' ? -1 : 1) + roomCentrePos.x;
        }

        doors[partNo].transform.position = doorPos;

        string doorName2 = doorName.Substring(0, 4);
        doorName2 += doorName[4] == '+' ? '-' : '+';
        doorName2 += doorName[5];

        Debug.Log("startPos = " + startPos + " && doorPos = " +  doorPos);
        Debug.Log("doorName = " + doorName + " && doorName2 = " + doorName2);
        //Debug.Log(Data.instance.roomNewScript.gameObject.name);
        //Data.instance.roomNewScript.ConnectTwoRooms(startPos, doors[partNo].transform.position, doorName, doorName2, Vector3.zero, Vector3.one, true);
        door_done[partNo] = true;
    }

    private void PlaceRemainingWalls()
    {
        Swap_NSWE();
        extra_walls_holder = new GameObject("Extra Walls Holder").transform;
        extra_walls_holder.parent = roomHolderTransform;
        Debug.Log(nswe_helper.Contains(1));
        if (nswe_helper.Contains(1))
        {
            int idx = nswe_helper.IndexOf(1);
            float pos1 = startFloorPosList[idx].z;
            Debug.Log(pos1);
            float posx = startFloorPosList[0].z;
            float pos2 = startFloorPosList[0].z + size_z[0];
            float pos3 = startFloorPosList[idx].z + size_z[idx];
            float posMax = Mathf.Max(pos2, pos3);
            float posMin = Mathf.Min(pos2, pos3);
            Debug.Log(pos2);
            int diff = (int)(pos1 - pos2);
            int sign = (diff >= 0) ? 1 : -1;
            float posToSpawn = pos1;
            Transform t;
            Vector3 pos;
            Debug.Log(sign * posToSpawn > sign * posMax);
            bool once = true;
            while(sign * posToSpawn > sign * posMax)
            {
                for (int j = 0; j < height; j++)
                {
                    pos = new Vector3( startFloorPosList[0].x + size_x[0], 2 + j * 4, posToSpawn);
                    Debug.Log(pos);
                    t = Instantiate(side_wall).transform;
                    t.parent = extra_walls_holder;
                    t.localPosition = pos;
                    //t.localEulerAngles = new Vector3(t.localEulerAngles.x, t.localEulerAngles.y + 90, t.localEulerAngles.z);
                }
                posToSpawn -= sign * 4;
                if(once && !(sign * posToSpawn > sign * posx))
                {
                    posToSpawn = posMin;
                    once = false;
                }
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
                //if (meshFilters[i].gameObject.GetComponent<MeshRenderer>())
                    atlasTextures[i] = (Texture2D)meshFilters[i].gameObject.GetComponent<MeshRenderer>().sharedMaterial.mainTexture;
                //else
                  //  Debug.Log(meshFilters[i].gameObject.name);
            }
            //meshFilters[i].gameObject.GetComponent<Renderer>().material = matNew;
            //Debug.Log("i = " + i + " && meshFilters[i].gameObject.name = " + meshFilters[i].gameObject.name);
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
        meshFilter.mesh.CombineMeshes(combine, true, true);


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
