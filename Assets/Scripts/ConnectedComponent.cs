using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectedComponent
{
    public Vector3 corridorPos;
    public List<Vector3> rooms;

    public ConnectedComponent(Vector3 corridorPos, List<Vector3> rooms)
    {
        this.corridorPos = corridorPos;
        this.rooms = rooms;
    }
}
