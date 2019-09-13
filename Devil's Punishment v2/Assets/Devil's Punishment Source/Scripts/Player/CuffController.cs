﻿//Author: Dave Bird
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
    private bool pisLocked; //Pistol
    private bool risLocked; //Assault Rifle
    private bool sisLocked; //Shotgun


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

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger for ladder & tag = " + other.tag);
        if (other.CompareTag("Player") && Input.GetKey(KeyCode.E))
        {
            Debug.Log("Climbing Ladder");
            playercontroller.SetIsClimbing(true);
            playercontroller.VerticalLocomotion();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))// && Input.GetKeyDown(KeyCode.E))
        {
            playercontroller.SetIsClimbing(false);
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            playercontroller.SetIsClimbing(false);
        }
    }

}