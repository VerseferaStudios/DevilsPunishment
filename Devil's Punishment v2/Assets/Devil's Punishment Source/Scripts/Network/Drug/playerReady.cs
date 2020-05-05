using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class playerReady : MonoBehaviour
{
    public PostProcessLayer layer;
    // Start is called before the first frame update
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            layer.enabled = true;
        }
    }

}
