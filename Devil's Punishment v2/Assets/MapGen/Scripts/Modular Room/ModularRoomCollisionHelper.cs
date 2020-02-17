using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModularRoomCollisionHelper : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.CompareTag("Modular Room Wall") && other.transform.parent.position == transform.parent.position)
        {
            Destroy(other.transform.parent.gameObject);
        }
    }

}
