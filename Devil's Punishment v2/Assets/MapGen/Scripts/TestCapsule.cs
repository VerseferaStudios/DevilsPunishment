using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCapsule : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Collider[] c = GetComponentsInChildren<Collider>();
        Debug.Log("--------COLLIDORS INCOMING---------");
        for (int i = 0; i < c.Length; i++)
        {
            Debug.Log(i + "th name = " + c[i].gameObject.name);
            Debug.Log(i + "th type = " + c[i].name);
        }
        Debug.Log("--------COLLIDORS DONE---------");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
