//Author: Dave Bird
//Date: Sunday, June 23, 2019
    //Last Edited: Tuesday, July 16, 2019
        //By: Dave Bird
            //Purpose: Cleanup Script
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

    public void Start()
    {
        glowStick.SetActive(true);
        glow.enabled = true;
        flashLight.SetActive(false);
        beam.enabled = false;
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
        if (Input.GetButtonDown("Toggle Flashlight"))
        //if (Input.GetKeyDown(KeyCode.G))
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
    }
}