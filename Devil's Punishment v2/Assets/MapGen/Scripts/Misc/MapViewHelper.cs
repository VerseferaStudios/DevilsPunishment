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
        Transform parentTransform = Instantiate(new GameObject("MapViewHelper")).transform;
        
        // ------------------- BOUNDS or SIZE of the grid -------------------
        for (int i = 0; i < 4; i++)
        {
            arr[0] = 48 * i + 28;
            for (int j = 0; j < 4; j++)
            {
                arr[1] = 48 * j + 28;
                Instantiate(new GameObject(), new Vector3(-arr[1], 0, -arr[0]), Quaternion.identity, parentTransform);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
