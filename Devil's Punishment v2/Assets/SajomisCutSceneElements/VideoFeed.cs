using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoFeed : MonoBehaviour
{
    public GameObject feed;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void OnTriggerEnter(Collider other)
    {
        if (tag == "Player")
        {
            feed.SetActive(true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (tag == "Player")
        {
            feed.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
