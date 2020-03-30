using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public bool occupied;
    public bool visited;
    public int corridorIdx;
    public int corridorYRot;
    public float hCost;
    public float gCost;
    public Vector3 pos;

    public Cell()
    {
        occupied = true;
        visited = false;
        corridorIdx = -1;
        corridorYRot = -1;
        hCost = 99;
        gCost = 99;
    }

}
