using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorGenerator : MonoBehaviour
{
    public Texture2D mapTexture;
    public int mapWidth = 32;
    public int mapHeight = 32;
    public int depthValue = 4;

    [Range(0, 1)]
    public float startSeedMin = 0.25f;

    [Range(0, 1)]
    public float startSeedMax = 0.75f;

    private int minLength;
    private int maxLength;

    /// <summary>
    /// Generates a corridor mapping
    /// </summary>
    /// <returns> Returns true if successful </returns>
    public bool GenerateMap()
    {
        mapTexture = new Texture2D(mapWidth, mapHeight, TextureFormat.ARGB32, false);
        minLength = mapWidth / 4;
        maxLength = mapWidth * 3 / 4;
        return GenerateMapRecursive(Random.Range((int)(mapWidth * startSeedMin), (int)(mapWidth * startSeedMax)),
            Random.Range((int)(mapHeight * startSeedMin), (int)(mapHeight * startSeedMax)), 1, 0, 0);
    }

    /// <summary>
    /// Recursive function that procedurely generate the map.
    /// </summary>
    /// <param name="x"> x-position of target pixel</param>
    /// <param name="y"> y-position of target pixel</param>
    /// <param name="depth"> Value that represents the depth of the procedural tree </param>
    /// <returns></returns>
    private bool GenerateMapRecursive(int x, int y, int depth, int lastDirectionX, int LastDirectionY)
    {
        if (depth > depthValue) { return false; }

        mapTexture.SetPixel(x, y, Color.black);

        int numberOfBranches = Random.Range(2, 4);

        for (int i = 0; i <= numberOfBranches; i++)
        {
            int direction = Random.Range(0, 2);

            int xIncrement = 0, yIncrement = 0;
            int distance = Random.Range(minLength / depth, maxLength / depth);
            distance = Mathf.Clamp(distance, 4, mapWidth - 3);

            if (direction == 0)
            {
                xIncrement = (Random.Range(0, 2) == 0) ? 1 : -1;
                if (lastDirectionX == xIncrement * distance) { distance = Random.Range(minLength, maxLength); }
                yIncrement = 0;
            }
            else
            {
                xIncrement = 0;
                yIncrement = (Random.Range(0, 2) == 0) ? 1 : -1;
                if (LastDirectionY == yIncrement * distance) { distance = Random.Range(minLength, maxLength); }
            }

            for (int j = 1; j <= distance; j++)
            {
                int xInLoop = x + xIncrement * j;
                int yInLoop = y + yIncrement * j;
                if (xInLoop >= 0 && xInLoop < mapWidth && yInLoop >= 0 && yInLoop < mapHeight)
                {
                    mapTexture.SetPixel(x + xIncrement * j, y + yIncrement * j, Color.black);
                }
            }

            mapTexture.Apply();

            int xToPass = x + xIncrement * distance;
            int yToPass = y + yIncrement * distance;

            if (xToPass >= 0 && xToPass < mapWidth && yToPass >= 0 && yToPass < mapHeight)
            {
                GenerateMapRecursive(xToPass, yToPass, depth + 1, xIncrement * distance, yIncrement * distance);
            }
        }
        return true;
    }
}
