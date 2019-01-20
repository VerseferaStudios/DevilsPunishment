using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorGenerator : MonoBehaviour
{
    public Texture2D MapTexture;
    public int MapWidth = 32;
    public int MapHeight = 32;
    public int depthValue = 4;

    [Range(0, 1)]
    public float StartSeedMin = 0.25f;

    [Range(0, 1)]
    public float StartSeedMax = 0.75f;

    private int minLength;
    private int maxLength;

    /// <summary>
    /// Generates a corridor mapping
    /// </summary>
    /// <returns> Returns true if successful </returns>
    public bool generateMap()
    {
        MapTexture = new Texture2D(MapWidth, MapHeight, TextureFormat.ARGB32, false);
        minLength = MapWidth / 4;
        maxLength = MapWidth * 3 / 4;
        return generateMapRecursive(Random.Range((int)(MapWidth * StartSeedMin), (int)(MapWidth * StartSeedMax)),
            Random.Range((int)(MapHeight * StartSeedMin), (int)(MapHeight * StartSeedMax)), 1, 0, 0);
    }

    /// <summary>
    /// Recursive function that procedurely generate the map.
    /// </summary>
    /// <param name="x"> x-position of target pixel</param>
    /// <param name="y"> y-position of target pixel</param>
    /// <param name="depth"> Value that represents the depth of the procedural tree </param>
    /// <returns></returns>
    private bool generateMapRecursive(int x, int y, int depth, int lastDirectionX, int LastDirectionY)
    {
        if (depth > depthValue) { return false; }

        MapTexture.SetPixel(x, y, Color.black);

        int numberOfBranches = Random.Range(2, 4);

        for (int i = 0; i <= numberOfBranches; i++)
        {
            int direction = Random.Range(0, 2);

            int xIncrement = 0, yIncrement = 0;
            int distance = Random.Range(minLength / depth, maxLength / depth);
            distance = Mathf.Clamp(distance, 4, MapWidth - 3);

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
                if (xInLoop >= 0 && xInLoop < MapWidth && yInLoop >= 0 && yInLoop < MapHeight)
                {
                    MapTexture.SetPixel(x + xIncrement * j, y + yIncrement * j, Color.black);
                }
            }

            MapTexture.Apply();

            int xToPass = x + xIncrement * distance;
            int yToPass = y + yIncrement * distance;

            if (xToPass >= 0 && xToPass < MapWidth && yToPass >= 0 && yToPass < MapHeight)
            {
                generateMapRecursive(xToPass, yToPass, depth + 1, xIncrement * distance, yIncrement * distance);
            }
        }
        return true;
    }
}
