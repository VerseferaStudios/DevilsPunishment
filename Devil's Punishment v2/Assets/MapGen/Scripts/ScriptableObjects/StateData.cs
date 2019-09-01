using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StateData", menuName = "ScriptableObjects/StateData", order = 1)]
public class StateData : ScriptableObject
{
    public List<Random.State> states = new List<Random.State>();
    public List<int> noOfRooms, mapGenSize;
}
