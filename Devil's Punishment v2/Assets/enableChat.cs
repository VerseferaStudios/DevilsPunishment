using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class enableChat : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(isLocalPlayer)
        {
            transform.Find("Scroll View").gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
