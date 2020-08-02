using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BoundaryController : MonoBehaviour
{
    public Transform resetPos;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {

            other.gameObject.transform.position = resetPos.position;
        }
        else
        {
            other.gameObject.SetActive(false);
        }
       
    }
}
