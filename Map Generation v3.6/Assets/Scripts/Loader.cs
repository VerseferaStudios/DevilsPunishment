using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    public GameObject data;

    private void Awake()
    {
        if(Data.instance == null)
        {
            Instantiate(data);
        }
    }
}
