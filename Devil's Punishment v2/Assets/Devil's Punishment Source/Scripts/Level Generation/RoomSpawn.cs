using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawn : MonoBehaviour
{
    public Transform[] RoomSpawns;
    public GameObject[] StaticRooms;
    public GameObject[] RandoRooms;
    public float[] RandoRoomIndices;
    public int NumOfRooms;
    private int randomiser, prevrando=0;

    void Start()
    {
        NumOfRooms = Random.Range(20, 25);
        for (int i = 0; i <= 5; i++)
        {
            randomiser = Random.Range(0, RoomSpawns.Length);
            if (prevrando != randomiser)
                GameObject.Instantiate(StaticRooms[i], RoomSpawns[randomiser].position, RoomSpawns[randomiser].rotation);
            else i--;
            prevrando = randomiser;
        }

        
    }
}
