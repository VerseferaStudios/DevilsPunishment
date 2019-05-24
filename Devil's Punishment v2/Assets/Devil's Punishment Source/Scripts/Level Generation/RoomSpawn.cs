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
    private int randomiser;
    public int[] TakenCells;
    private bool Overlap = false;

    void Start()
    {
        int i = 0;
        NumOfRooms = Random.Range(20, 25);
        while (i<5)
        {
            randomiser = Random.Range(0, RoomSpawns.Length-1);
            for (int s = 0; s < TakenCells.Length-1; s++)
            {
                if (TakenCells[s] == randomiser)
                    Overlap = true;
            }
            if (!Overlap)
            {
                GameObject.Instantiate(StaticRooms[i], RoomSpawns[randomiser].position, RoomSpawns[randomiser].rotation);
                i++;
                TakenCells[i] = randomiser;
            }
            Overlap = false;
            
        }
        NumOfRooms -= 5;
        /*for (int i = 0; i <= 5; i++)
        {
            randomiser = Random.Range(0, RoomSpawns.Length - 1);
            if (prevrando != randomiser)
                GameObject.Instantiate(StaticRooms[i], RoomSpawns[randomiser].position, RoomSpawns[randomiser].rotation);
            else i--;
            prevrando = randomiser;
        }*/


    }
}
