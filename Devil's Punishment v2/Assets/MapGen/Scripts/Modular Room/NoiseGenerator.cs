using UnityEngine;
using System.Collections;

// Create a texture and fill it with Perlin noise.
// Try varying the xOrg, yOrg and scale values in the inspector
// while in Play mode to see the effect they have on the noise.

public class NoiseGenerator : MonoBehaviour
{

    // The origin of the sampled area in the plane.
    public float xOrg;
    public float yOrg;

    // The number of cycles of the basic noise pattern that are repeated
    // over the width and height of the texture.
    public float scale = 1.0F;

    private Texture2D noiseTex;
    private Color[] pix;
    public MeshRenderer rend;

    private float x, y, xCoord, yCoord, sample;

    void Start()
    {
        //// Set up the texture and a Color array to hold pixels during processing.
        //noiseTex = (Texture2D)rend.material.mainTexture;
        //Debug.Log("wall mat = " + rend.material.name);
        //Debug.Log("wall mat = " + rend.material.mainTexture.name);
        //Debug.Log("noiseTex => " + noiseTex.width);
        //Debug.Log("noiseTex => " + noiseTex.height);
        //pix = new Color[noiseTex.width * noiseTex.height];
        
        ////CalcNoise();

        //rend.sharedMaterial.mainTexture = noiseTex;
        //Debug.Log("wall mat = " + rend.sharedMaterial.mainTexture.name);
    }

    void CalcNoise()
    {
        // For each pixel in the texture...
        y = 0.0F;

        while (y < noiseTex.height)
        {
            x = 0.0F;
            while (x < noiseTex.width)
            {
                xCoord = xOrg + x / noiseTex.width * scale;
                yCoord = yOrg + y / noiseTex.height * scale;
                sample = Mathf.PerlinNoise(xCoord, yCoord);
                pix[(int)y * noiseTex.width + (int)x] = new Color(sample, sample, sample);
                x++;
            }
            y++;
        }

        // Copy the pixel data to the texture and load it into the GPU.
        noiseTex.SetPixels(pix);
        noiseTex.Apply();
    }

    void Update()
    {
        //CalcNoise();
    }
}