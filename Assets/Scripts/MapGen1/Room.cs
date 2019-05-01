using UnityEngine;
using UnityEditor;

public class Room // : ScriptableObject
{

    public int[] doorType { get; set; }
    //public string doorPos { get; set; }
    public int roomType { get; set; }

    public Room(int[] doorType, int roomType)
    {
        this.doorType = doorType;
        this.roomType = roomType;
    }

    public Room()
    {
        doorType = new int[4] { 0, 0, 0, 0 }; //needed?
    }
}