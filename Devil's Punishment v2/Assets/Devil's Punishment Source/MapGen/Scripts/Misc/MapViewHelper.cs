using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapViewHelper : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float[] arr = new float[2];

        //parent
        Transform parentTransform = new GameObject("MapViewHelper").transform;
        
        // ------------------- BOUNDS or SIZE of the grid -------------------
        for (int i = 0; i < 5; i++)
        {
            arr[0] = 48 * i + 28;
            for (int j = 0; j < 5; j++)
            {
                arr[1] = 48 * j + 28;
                Transform gb = new GameObject(i + ", " + j).transform;
                gb.position = new Vector3(-arr[1], 0, -arr[0]);
                gb.parent = parentTransform;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
