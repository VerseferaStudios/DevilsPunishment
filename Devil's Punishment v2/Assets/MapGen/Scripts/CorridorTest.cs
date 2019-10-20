using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorTest : MonoBehaviour
{
    private void Start()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        string otherTag = other.transform.parent.tag;
        if (otherTag.Equals("CorridorI") || otherTag.Equals("CorridorL") || otherTag.Equals("CorridorT") || otherTag.Equals("CorridorX"))
        {
            if(!(transform.parent.tag.Equals("CorridorI") && otherTag.Equals("CorridorI")))
            {

            }
            Data.instance.collidedCorridors.Add(gameObject);
            ////Debug.Log(Data.instance.collisionCount + "&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&");
        }
    }

}
