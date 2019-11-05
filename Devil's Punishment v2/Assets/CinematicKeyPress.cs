using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicKeyPress : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    bool trigger = false;
    public Light light;

    public Vector3 movedir = Vector3.down;
    public KeyCode movement = KeyCode.J;
    public KeyCode lightKey = KeyCode.L;
    
    // Update is called once per frame
    void Update()
    {
        if(trigger)
        {
            
            transform.position += movedir * Time.deltaTime;
        }

        if(Input.GetKeyDown(movement))
        {
            trigger = !trigger;

            if(trigger)
            {
                GetComponent<Animator>().SetBool("walk", true);
            }
            else
            {
                GetComponent<Animator>().SetBool("walk", false);
            }
        }
        else if(Input.GetKeyDown(lightKey))
        {
            light.enabled = !light.enabled; //switch
        }
    }
}
