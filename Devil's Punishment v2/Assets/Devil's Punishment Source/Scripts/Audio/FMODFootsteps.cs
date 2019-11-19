using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODFootsteps : MonoBehaviour
{

    [FMODUnity.EventRef]
    public string inputsound;
    bool playerismoving;
    bool playerisrunning;
    public float walkingspeed;
    public float runningspeed;



    void Update()
{
    if (Input.GetAxis("Vertical") >= 0.01f || Input.GetAxis("Horizontal") >= 0.01f || Input.GetAxis("Vertical") <= -0.01f || Input.GetAxis("Horizontal") <= -0.01f)
    {
        //Debug.Log ("Player is moving");
        playerismoving = true;
            print("Moving");
    }
    else if (Input.GetAxis("Vertical") == 0 || Input.GetAxis("Horizontal") == 0)
    {
        //Debug.Log ("Player is not moving");
        playerismoving = false;
            print("Not Moving");
        }

        if (Input.GetButton("Sprint"))
        {
            playerismoving = false;
            playerisrunning = true;
            print("Running");
        }

    }


void CallFootsteps()
{
    if (playerismoving == true)
    {
        //Debug.Log ("Player is moving");
        FMODUnity.RuntimeManager.PlayOneShot(inputsound);
    }

}



void Start()
{
    InvokeRepeating("CallFootsteps", 0, walkingspeed);
  //  InvokeRepeating("CallFootsteps", 0, runningspeed);
}


void OnDisable()
{
    playerismoving = false;
    playerisrunning = false;
}
}
