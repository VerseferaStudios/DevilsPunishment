using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODFootsteps : MonoBehaviour
{

    [FMODUnity.EventRef]
    public string walkingsound;
    [FMODUnity.EventRef]
    public string runningsound;
    [FMODUnity.EventRef]
    public string crouchingsound;
    bool playeriswalking;
    bool playerisrunning;
    bool playeriscrouching;
    public float walkingspeed;
    public float runningspeed;
    public float crouchingspeed;
    bool canmove;

    void Update()
{
        
            if (Input.GetAxis("Vertical") >= 0.001f || Input.GetAxis("Horizontal") >= 0.001f || Input.GetAxis("Vertical") <= -0.001f || Input.GetAxis("Horizontal") <= -0.001f)
            {
                //Debug.Log ("Player is moving");
                playeriswalking = true;
                canmove = true;
                print("Moving");
            }


            else if (Input.GetAxis("Vertical") == 0 || Input.GetAxis("Horizontal") == 0)
            {
                //Debug.Log ("Player is not moving");
                playeriswalking = false;
                playerisrunning = false;
                playeriscrouching = false;
                canmove = false;
                print("Not Moving");
            }

        
        if (Input.GetButton("Sprint"))
        {
            if (canmove == true)
            {
                playeriswalking = false;
                playerisrunning = true;
                playeriscrouching = false;
                print("Running");
            }
        }


        if (Input.GetButton("Crouch"))
        {
            if (canmove == true)
            {
                playeriswalking = false;
                playerisrunning = false;
                playeriscrouching = true;
                print("Crouching");
            }
        }

    }


    void CallFootstepsWalking()
{
    if (playeriswalking == true)
    {
        playerisrunning = false;
        playeriscrouching = false;
        Debug.Log ("Player is walking");
        FMODUnity.RuntimeManager.PlayOneShot(walkingsound);
    }

}

    void CallFootstepsRunning()
    {
        if (playerisrunning == true)
        {
            playeriswalking = false;
            playeriscrouching = false;
            Debug.Log ("Player is Running");
            FMODUnity.RuntimeManager.PlayOneShot(runningsound);
        }
    }

    void CallFootstepsCrouching()
    {
        if (playeriscrouching == true)
        {
            playeriswalking = false;
            playerisrunning = false;
            Debug.Log("Player is crouching");
            FMODUnity.RuntimeManager.PlayOneShot(crouchingsound);
        }
    }



    void Start()
{
    InvokeRepeating("CallFootstepsWalking", 0, walkingspeed);
    InvokeRepeating("CallFootstepsRunning", 0, runningspeed);
    InvokeRepeating("CallFootstepsCrouching", 0, runningspeed);

    }


void OnDisable()
{
    playeriswalking = false;
    playerisrunning = false;
    playeriscrouching = false;
}
}
