using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorCollidorScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Data.instance.isCollided = true;
        Data.instance.collisionCount++;
        Debug.Log(Data.instance.collisionCount);
        Debug.Log("&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&");
        Destroy(transform.parent.gameObject);
    }
}
