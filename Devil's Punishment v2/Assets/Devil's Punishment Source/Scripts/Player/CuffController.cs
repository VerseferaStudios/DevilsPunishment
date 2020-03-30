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
    //public PlayerController playercontroller;
    public bool isCuffed;
    private bool pisLocked; //Pistol
    private bool risLocked; //Assault Rifle
    private bool sisLocked; //Shotgun

    private PlayerController_Revamped playerController;

    private void Start()
    {
        //StartCoroutine(StartAfterDelay());
        playerController = GetComponent<PlayerController_Revamped>();
      //  Cuff();
    }

    private IEnumerator StartAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        playerController = GetComponent<PlayerController_Revamped>();
      //  Cuff();
    }


    //Can that item get picked up?
    public void Cuff()
    {
        isCuffed = true;
        pisLocked = true;
        risLocked = true;
        sisLocked = true;
        playerController.CharacterAnimator.SetFloat("isCuffed_Normalized", 1.0f);
        //Add a on screen or audio cue for failure.
        //Debug.Log("Cuffed, find a way to get your cuffs off!");
    }

    public void Uncuff()
    {
        isCuffed = false;
        pisLocked = false;
        risLocked = false;
        sisLocked = false;
        playerController.CharacterAnimator.SetFloat("isCuffed_Normalized", 0.0f);
        //Debug.Log("You have picked up a gun.");
    }

    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Trigger for ladder & tag = " + other.tag);
        if (other.CompareTag("Ladder") && Input.GetKey(KeyCode.E))
        {
            //playerController = other.GetComponent<PlayerController_Revamped>();
            Debug.Log("Climbing Ladder");
            playerController.SetIsClimbing(true);
            playerController.VerticalLocomotion();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerController != null && playerController.GetIsClimbing() && other.CompareTag("Ladder"))// && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("EXITED LADDER");
            playerController.transform.position += playerController.transform.forward * 1;
            //playerController.transform.position += new Vector3(playerController.movementInputRaw.x, 0, playerController.movementInputRaw.y);
            playerController.SetIsClimbing(false);
        }
    }

    private void Update()
    {
        if (playerController.GetIsClimbing() && Input.GetKey(KeyCode.Space))
        {
           playerController.SetIsClimbing(false);
        }
    }

}