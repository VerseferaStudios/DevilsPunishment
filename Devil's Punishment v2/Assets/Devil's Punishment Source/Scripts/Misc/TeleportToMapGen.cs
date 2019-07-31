using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportToMapGen : MonoBehaviour
{

    public Transform player;

    private void Start()
    {
        //StartCoroutine(TeleportCo());
    }
    /*
    private IEnumerator TeleportCo()
    {
        //yield return new WaitForSeconds(15f);
        Debug.Log("trigger...");
        if(Mathf.Abs(player.position.x - transform.position.x) <= 1.5f && Mathf.Abs(player.position.z - transform.position.z) <= 1.5f)
        {
            Debug.Log("teleporting...");
            player.parent.parent.position = new Vector3(player.parent.parent.position.x - 220, player.parent.parent.position.y + 3, player.parent.parent.position.z - 2);
            //player.parent.parent.eulerAngles = new Vector3(0, 180, 0);
            yield return null;
        }
        else
        {
            Debug.Log("Keep Waiting...");
            yield return new WaitForSeconds(0.5f);
            //yield return new WaitForSeconds(( Mathf.Abs(player.position.x - transform.position.x) 
                                           // + Mathf.Abs(player.position.z - transform.position.z) ) / 20);
        }
    }
    */
    
    private void OnTriggerEnter(Collider other)
    {
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
