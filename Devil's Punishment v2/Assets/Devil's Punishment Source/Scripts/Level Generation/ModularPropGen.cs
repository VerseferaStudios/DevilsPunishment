using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModularPropGen : MonoBehaviour
{

    List<float> xRoomSizes = new List<float>();
    List<float> zRoomSizes = new List<float>();
    Vector3 roomPos;
    List<Vector3> floorPos = new List<Vector3>();
    int floor;

    [Header("Spawning Positions")]
    private List<Vector3> spawnPosVectors = new List<Vector3>();

    [Header("Test")]
    public GameObject testGridCube;
    public GameObject testGridPlane;

    public ModularPropGen(Vector3 FloorPos, float XRoomSize, float ZRoomSize)
    {
        floorPos.Add(FloorPos);
        xRoomSizes.Add(XRoomSize);
        zRoomSizes.Add(XRoomSize);
        GenerateSpawnPositions();
    }

    private void GenerateSpawnPositions()
    {
        
    }
    private void MapPosition(float x, float z)
    {
        Vector3 possibleSpawnPos = new Vector3(x, 0, z);
        Debug.Log("Pos in room: " + possibleSpawnPos);
        spawnPosVectors.Add(possibleSpawnPos);
    }
}
