using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawn : MonoBehaviour
{
    public Transform[] RoomSpawns; //Individual spawnpoints for the rooms
    public GameObject[] StaticRooms; //Static room prefabs go here
    public GameObject[] RandoRooms; //Random room prefabs go here
    public int NumOfRooms; //Number of rooms to generate
    private int randomiser;//For calling Random.Range()
    public int[] TakenCells;//List of preoccupied cells so that two rooms don't spawn in the same spot
    private bool Overlap = false;//flag for search logic for checking taken cells

    void Start()
    {
        int i = 0;
        NumOfRooms = Random.Range(20, 25); //pick a random number between 20 and 25
        while (i<5) //loop to spawn static rooms
        {
            randomiser = Random.Range(0, RoomSpawns.Length-1); //randomly choose a cell
            for (int s = 0; s < TakenCells.Length-1; s++)//check if cell is not taken
            {
                if (TakenCells[s] == randomiser)
                    Overlap = true;
            }
            if (!Overlap) //if cell is not taken by another room, spawn a room here
            {
                GameObject.Instantiate(StaticRooms[i], RoomSpawns[randomiser].position, RoomSpawns[randomiser].rotation);
                i++;
                TakenCells[i] = randomiser;
            }
            Overlap = false;
            
        }
        NumOfRooms -= 5;//5 rooms have already been taken, so subtract them from the number of rooms left to spawn
        while (i < NumOfRooms)
        {
            //same code as above but now spawns random cells
            randomiser = Random.Range(0, RoomSpawns.Length - 1);
            for (int s = 0; s < TakenCells.Length - 1; s++)
            {
                if (TakenCells[s] == randomiser)
                    Overlap = true;
            }
            if (!Overlap)
            {
                int randompicker = Random.Range(0, RandoRooms.Length - 1);
                GameObject.Instantiate(RandoRooms[randompicker], RoomSpawns[randomiser].position, RoomSpawns[randomiser].rotation);
                i++;
                TakenCells[i] = randomiser;
            }
            Overlap = false;
        }
    }//end
}
