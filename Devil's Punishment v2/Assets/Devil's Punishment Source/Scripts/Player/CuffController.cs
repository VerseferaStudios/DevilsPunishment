//Author: Dave Bird
//Date: Sunday, May 12, 2019
    //Last Edited: Sunday, May 14, 2019
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
    public bool pAisLocked; //Pistol Ammo
    public bool rAisLocked; //Assault Rifle Ammo
    public bool sAisLocked;  // Shotgun Ammo
    public bool medkitisLocked; //Medic Kit
    public bool pillisLocked;   //Pills
    public bool gPart1isLocked;  //Generator Part A
    public bool gPart2isLocked;  //Generator Part B
    public bool gPart3isLocked;  //Generator Part C
    public bool flUpgradeisLocked;  //Flashlight Upgrade
    public bool inventoryhasSpace; 


    void Awake()
    {
        isCuffed = true;
        inventoryhasSpace = true;
    }


    //Can that item get picked up?
    public void Cuffed()
    {
        if (isCuffed == true)
        {
            pisLocked = true;
            risLocked = true;
            sisLocked = true;
            pAisLocked = false;
            rAisLocked = false;
            sAisLocked = false;
            medkitisLocked = false;
            pillisLocked = false;
            gPart1isLocked = false;
            gPart2isLocked = false;
            gPart3isLocked = false;
            flUpgradeisLocked = false;

        }
        else if (!isCuffed)
        {
            pisLocked = false;
            risLocked = false;
            sisLocked = false;
            pAisLocked = false;
            rAisLocked = false;
            sAisLocked = false;
            medkitisLocked = false;
            pillisLocked = false;
            gPart1isLocked = false;
            gPart2isLocked = false;
            gPart3isLocked = false;
            flUpgradeisLocked = false;
        }
    }

    //public void OnTriggerEnter(Collider Ladder)
    //{
    //    if (tag.player == true && isCuffed == true && Input.GetKeyDown(KeyCode.E))
    //    {
    //        playercontroller.VerticalLocomotion();
    //    }
    //    else if (tag.player == true && !isCuffed && Input.GetKeyDown(KeyCode.E))
    //    {
    //        playercontroller.VerticalLocomotion();
    //    }
    //}
}

