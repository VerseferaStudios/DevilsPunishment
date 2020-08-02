using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    public static Constants sRef;
    private void Awake()
    {
        sRef = this;
    }

    public readonly string TAG_VENTSPAWNFLOOR1 = "Vent Spawn Points";
    public readonly string TAG_VENTSPAWNFLOOR2 = "Vent Spawn Points 2nd Floor";
    public readonly string GBNAME_ROOMNEWVENTS = "RoomNewVents";
    public readonly string GBNAME_ROOMNEWVENTS2NDFLOOR = "RoomNewVents2ndFloor";

}
