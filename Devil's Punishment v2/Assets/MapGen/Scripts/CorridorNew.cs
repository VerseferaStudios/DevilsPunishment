using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorNew : MonoBehaviour
{
    //[SerializeField]
    //private bool isPipeFromLeft, isPipeToLeft, isNeedL = false, isNeedT = false, isNeedX = false;

    //[SerializeField]
    //[Tooltip("Corridor openings in the order of +z, +x, -z, -x for 0, 1, 2, 3 indices respectively of the array")]
    //private List<int> corridorOpenings = new List<int>(4);
    /*
     * For example
     *      __L
     *      for such a junction, if we get collision at the junction point;
     *      corridorOpenings[0] = corridorOpenings[1] = corridorOpenings[3] = true; (+z, +x and -x respectively)
     *      corridorOpenings[2] = false; (-z)
     *      
     */

    public List<Vector3> rooms = new List<Vector3>();

    //public string KOrL;

    //public List<Vector3> theEqualOnes = new List<Vector3>();

    //public GameObject collisionDetector;

    private void Start()
    {
        ////Debug.Log("corridorOpenings.Length" + corridorOpenings.Count + "    " + corridorOpenings.Capacity);
        //GameObject.FindObjectOfType
    }
    private void OnTriggerEnter(Collider other)
    {
        string otherTag = other.transform.parent.tag;
        if (otherTag.Equals("CorridorI") || otherTag.Equals("CorridorL") || otherTag.Equals("CorridorT") || otherTag.Equals("CorridorX"))
        {
            Debug.Log("HII");
            Data.instance.isCollided = true;
            Data.instance.collisionCount++;
            
            if(!(transform.parent.tag.Equals("CorridorI") && otherTag.Equals("CorridorI")))
            {
                Data.instance.connectedRoomsThroughCollision.Add(new ConnectedComponent(transform.position, rooms));
            }

            //isNeedL = true;
            Data.instance.collidedCorridors.Add(gameObject);
            ////Debug.Log(Data.instance.collisionCount + "&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&");
            //startTime = Time.time;
            //Destroy(gameObject); //transform.parent.gameObject
            ////Debug.Log("!");
        }
        else if (otherTag.Equals("VentI") || otherTag.Equals("VentL") || otherTag.Equals("VentT") || otherTag.Equals("VentX"))
        {
            Data.instance.collidedVents.Add(gameObject);
        }
    }

}
