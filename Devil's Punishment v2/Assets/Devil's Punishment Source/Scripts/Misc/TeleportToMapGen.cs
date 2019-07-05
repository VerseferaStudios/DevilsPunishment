using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportToMapGen : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger...");
        if (other.CompareTag("Player"))
        {
            Debug.Log("teleporting...");
            Vector3 playerPos = other.transform.position;
            //if (Data.instance.allRooms.Count > 0)
            {
                //other.transform.position = new Vector3(-((float[])Data.instance.allRooms[0])[1], 0, -((float[])Data.instance.allRooms[0])[0]);
                other.transform.position = new Vector3(playerPos.x - 220, playerPos.y + 3, playerPos.z - 2);
                other.transform.eulerAngles = new Vector3(0, 180, 0);
            }
            //else
            {
                //Debug.Log("No rooms to teleport to");
            }
        }
    }
}
