using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSync : MonoBehaviour
{
    public InteractableDoor[] doors;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            doors = (InteractableDoor[])GameObject.FindObjectsOfType<InteractableDoor>();

        }
    }
}
