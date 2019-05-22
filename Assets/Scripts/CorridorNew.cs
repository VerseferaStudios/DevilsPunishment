using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorNew : MonoBehaviour
{
    public bool isPipeFromLeft, isPipeToLeft, isNeedL = false, isNeedT = false, isNeedX = false;

    [Tooltip("Corridor openings in the order of +z, +x, -z, -x for 0, 1, 2, 3 indices respectively of the array")]
    private List<int> corridorOpenings = new List<int>(4);
    /*
     * For example
     *      __L
     *      for such a junction, if we get collision at the junction point;
     *      corridorOpenings[0] = corridorOpenings[1] = corridorOpenings[3] = true; (+z, +x and -x respectively)
     *      corridorOpenings[2] = false; (-z)
     *      
     */

    private void Start()
    {
        //Debug.Log("corridorOpenings.Length" + corridorOpenings.Count + "    " + corridorOpenings.Capacity);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.CompareTag("Corridor"))
        {
            Data.instance.isCollided = true;
            Data.instance.collisionCount++;

            //isNeedL = true;

            Data.instance.collidedCorridors.Add(gameObject);
            //Debug.Log(Data.instance.collisionCount + "&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&");
            //startTime = Time.time;
            //Destroy(gameObject); //transform.parent.gameObject
        }
    }

}
