using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuffController : MonoBehaviour
{
    public static CuffController instance;

    public bool cuffed = false;

    void Awake()
    {
        instance = this;
    }

    public void ToggleCuffs()
    {
        cuffed = !cuffed;
    }

    bool IsCuffed()
    {
        return cuffed;
    }
}
