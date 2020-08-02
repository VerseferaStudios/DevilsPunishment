using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testmergemeshes : MonoBehaviour
{

    // Source textures.
    Texture2D[] atlasTextures;

    // Rectangles for individual atlas textures.
    Rect[] rects;

    public Renderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        atlasTextures = new Texture2D[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            Debug.Log("meshfilters = " + meshFilters[i].gameObject.name);
            atlasTextures[i] = (Texture2D)meshFilters[i].gameObject.GetComponent<MeshRenderer>().sharedMaterial.mainTexture;
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);

            i++;
        }
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        transform.gameObject.SetActive(true);


        // Pack the individual textures into the smallest possible space,
        // while leaving a two pixel gap between their edges.
        Texture2D atlas = new Texture2D(8192, 8192);
        rects = atlas.PackTextures(atlasTextures, 2, 8192);


        renderer.material.mainTexture = atlas;
        renderer.material.mainTextureOffset = new Vector2(rects[2].x, rects[2].y);
        renderer.material.mainTextureScale = new Vector2(rects[2].width, rects[2].height);


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
