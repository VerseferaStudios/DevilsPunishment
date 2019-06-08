using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{

    private int checker = 0;

    // Start is called before the first frame update
    void Start()
    {
        while (!fn());
        {
            Debug.Log(checker);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private bool fn()
    {
        Debug.Log("srgfsxv");
        checker++;
        return (checker >= 100) ? true : false;
    }
}
