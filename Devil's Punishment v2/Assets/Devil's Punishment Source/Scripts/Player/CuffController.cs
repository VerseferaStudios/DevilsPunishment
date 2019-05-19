//Author: Dave Bird
//Date: Sunday, May 12, 2019
    //Last Edited: Sunday, May 19, 2019
        //By: Dave Bird
            //Purpose: Write the script
 //Written For: Devil's Punishment v2
 //Purpose: This script details all things that has to do with the limitations of the cuffed mechanic.
 //Notes: Further information can be found at http://www.nolocationyet.com (Dead link)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CuffController : MonoBehaviour
{
    public static CuffController instance;
    public PlayerController playercontroller;
    public bool isCuffed;
    public bool pisLocked; //Pistol
    public bool risLocked; //Assault Rifle
    public bool sisLocked; //Shotgun


    void Awake()
    {
        Cuff();
    }


    //Can that item get picked up?
    public void Cuff()
    {
        isCuffed = true;
        pisLocked = true;
        risLocked = true;
        sisLocked = true;
        //Add a on screen or audio cue for failure.
        Debug.Log("Cuffed, find a way to get your cuffs off!");
    }

    public void Uncuff()
    {
        isCuffed = false;
        pisLocked = false;
        risLocked = false;
        sisLocked = false;
        Debug.Log("You have picked up a gun.");
    }

    public void OnTriggerEnter(Collider Ladder)
    {
        if (tag == "Player" && Input.GetKeyDown(KeyCode.E))
        {
            playercontroller.VerticalLocomotion();
        }
    }
}