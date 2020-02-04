using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenCombineMesh : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Base Code from Unity Scripting API
    public void CombineMeshesCorridors()
    {
        Rect[] rects;
        Texture2D[] atlasTextures;

        MeshFilter[] meshFilters = GameObject.FindGameObjectWithTag("CorridorI").GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        atlasTextures = new Texture2D[meshFilters.Length];
        Material matNew = new Material(Shader.Find("Standard"));

        int i = 0;
        while (i < meshFilters.Length)
        {
            string gbName = meshFilters[i].gameObject.name;
            if (meshFilters[i].CompareTag("CorridorChild"))// &&
                //meshFilters[i].gameObject.GetComponent<MeshRenderer>().sharedMaterial.mainTexture.isReadable)//!(gbName.StartsWith("Pick") || gbName.StartsWith("flash")))
            {
                Debug.Log(gbName);
                Debug.Log(meshFilters[i].gameObject.GetComponent<MeshRenderer>());//.sharedMaterial.mainTexture.isReadable);
                //Debug.Log("meshFilters[i].gameObject.GetComponent<MeshRenderer>().sharedMaterial.mainTexture");
                atlasTextures[i] = (Texture2D)meshFilters[i].gameObject.GetComponent<MeshRenderer>().sharedMaterial.mainTexture;

                //meshFilters[i].gameObject.GetComponent<Renderer>().material = matNew;

                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                //Debug.Log("meshfilters = " + meshFilters[i].gameObject.name);
                meshFilters[i].gameObject.SetActive(false);
            }

            i++;
        }
        //Data.instance.mapGenHolderTransform.gameObject.SetActive(false);
        GameObject room = new GameObject("Combined Room");
        MeshFilter meshFilter = room.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = room.AddComponent<MeshRenderer>();
        //meshRenderer.sharedMaterial = mat;
        meshFilter.mesh = new Mesh();
        meshFilter.mesh.CombineMeshes(combine);


        Texture2D packedTexture = new Texture2D(1024, 1024);
        // Pack the individual textures into the smallest possible space,
        // while leaving a two pixel gap between their edges.
        rects = packedTexture.PackTextures(atlasTextures, 0, 1024);

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
