//Author: Dave Bird
//Date: Sunday, June 23, 2019
    //Last Edited: Tuesday, July 9, 2019
        //By: Dave Bird
            //Purpose: Include ability to turn the glowstick on and off even without the lsdupgrade
//Written For: Devil's Punishment v2
//Purpose: This script details all things that has to do with the limitations of the cuffed mechanic.
//Notes: Further information can be found at http://www.nolocationyet.com (Dead link)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightScript : MonoBehaviour
{
    public GameObject glowStick;
    public GameObject flashLight;
    public GameObject pickUp;
    public Light glow;
    public Light beam;
    public bool isUpgraded;
    public int countno;

    [Header("Player controlling LSD angle")]
    [Tooltip("Spotlight GameObject goes here.")]
    public GameObject tilt;


    public void Start()
    {
        glowStick.SetActive(true);
        glow.enabled = true;
        flashLight.SetActive(false);
        beam.enabled = false;
        isUpgraded = false;
    }

    public void ToggleUpgraded()
    {
        if (glow.enabled && !beam.enabled)
        {
            glow.enabled = false;
            beam.enabled = true;
        }
        else if (!glow.enabled && beam.enabled)
        {
            glow.enabled = beam.enabled = false;
        }
        else
        {
            glow.enabled = true;
            beam.enabled = false;
        }
    }

    public void Upgraded()
    {
        isUpgraded = true;
        flashLight.SetActive(true);
    }

    //Swtich levels of light - DONE
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (isUpgraded)
            {
                flashLight.SetActive(true);
                ToggleUpgraded();
            }
            else
            {
                if (countno == 0)
                {
                    glow.enabled = false;
                    countno++;
                    Debug.Log("You have covered the glowstick.");
                }
                else if (countno == 1)
                {
                    glow.enabled = true;
                    countno--;
                    Debug.Log("You have uncovered the glowstick.");
                }
            }
        }

    //    //Making it so that the player can move the light.
    //    // if the wheel scrolled up light rotates up max 10 degrees (+); if wheel scrolled down light rotates down max 10 degrees (-).
    //    if (Input.mouseScrollDelta)
    //    {
    //        //moving in positive, (scroll away from you) rotation goes up
    //        return;
    //    }
    //    else if (Input.mouseScrollDelta)
    //    {
    //        //moving in negative, (scroll towards you) rotation goes down
    //        return;
    //    }

    //    //Toggling the other function.
    //    //Scroll Up widens the beam of light on flashlight. Scroll Down narrows the beam of light (Max of 50% either direction.
    //    if (Input.GetKeyDown(KeyCode.Q))
    //    {
    //        if (Input.mouseScrollDelta)
    //        {
    //            //widen beam (+) (spotlight angle wider) (up to 63)
    //            return;
    //        }
    //        else if (Input.mouseScrollDelta)
    //        {
    //            //narrow beam (-) (spotlight angle narrow) (down to 21)
    //            return;
    //        }
    //    }

    //    //Snapping it back to the normal postion
    //    if (Input.GetKeyDown(KeyCode.mouseScrollDelta)) //Clicking down the mouse wheel snaps rotation back to normal
    //    {
    //        //normal position is x0, y0, z0
    //        //normal rotation is x7.113, y-0.708, z0.5560001
    //        //normal angle is 42 
    //        return;
    //    }
    }
}