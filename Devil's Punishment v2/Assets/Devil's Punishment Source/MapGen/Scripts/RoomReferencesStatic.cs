using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomReferencesBase : MonoBehaviour
{
    public GameObject[] doors;
    public Transform ventParent;
    //public RoomDoorsInfo roomDoorsInfo;
}

public class RoomReferencesStatic : RoomReferencesBase
{
    public GameObject[] existingCorridors;
    public Transform topRightItemGen;
    public Transform bottomLeftItemGen;
    public Transform topRightMapGen;
    public Transform bottomLeftMapGen;
}
