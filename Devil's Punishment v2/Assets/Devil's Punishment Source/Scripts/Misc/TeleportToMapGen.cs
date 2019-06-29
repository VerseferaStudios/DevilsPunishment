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
            other.transform.position = new Vector3(playerPos.x - 220, playerPos.y, playerPos.z);
            other.transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }
}
