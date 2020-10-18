using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : Test
{
    //private void Start()
    //{
    //    List<int> ar= new List<int>();
    //    ar.Add(1);
    //    Debug.Log(ar.Capacity);
    //}
    protected override void DebugCall()
    {
        int[,] arr = new int[20, 20];

        Debug.Log("Test1 Script");
        Debug.Log(arr.Length);
        Debug.Log(arr.Rank);
    }
}
