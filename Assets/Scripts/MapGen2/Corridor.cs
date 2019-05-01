using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corridor //: MonoBehaviour
{
    private int corridorType;
    public GameObject spawnPoints; //needed?

    public void replaceOtherCorridor(Corridor other)
    {
        int newCorridoorType = 0;
        other.corridorType = newCorridoorType;
        other.replaceSelf();
    }

    public void replaceSelf()
    {

    }
}
