using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestChild : Test
{
    public GameObject door;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(door.name.EndsWith("x") ? true : false );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
