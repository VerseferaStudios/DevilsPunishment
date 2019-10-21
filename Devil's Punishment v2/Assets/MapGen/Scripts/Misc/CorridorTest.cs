using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorTest : MonoBehaviour
{
    public GameObject prevRoom;
    [SerializeField]
    private bool isCollided = false;

    private void Start()
    {
        StartCoroutine(RoomRemove());
    }
    private void OnTriggerEnter(Collider other)
    {
        string otherTag = other.transform.parent.tag;
        if (otherTag.Equals("CorridorI") || otherTag.Equals("CorridorL") || otherTag.Equals("CorridorT") || otherTag.Equals("CorridorX"))
        {
            isCollided = true;
        }
    }

    private IEnumerator RoomRemove()
    {
        yield return new WaitForSeconds(0.1f);
        if (!isCollided)
        {
            prevRoom.SetActive(false);
            Debug.Log("Room Removed!");
            gameObject.SetActive(false);
        }
    }

}
