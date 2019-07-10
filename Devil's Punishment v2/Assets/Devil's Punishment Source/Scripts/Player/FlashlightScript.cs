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
    public bool dark;
    public bool isUpgraded;
    public int count;
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
        dark = false;
        count = 0;
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
        ////Switching the different levels of light.

        ///////////////////////////////////////////////////////////////////////////
        ////FOR TESTING ONLY REMOVE AFTER TESTING
        //if (isUpgraded == false && Input.GetKeyDown(KeyCode.Z))
        //{
        //    Upgraded();
        //    Debug.Log("You have set the upgrade to being true temporarily.");
        //}
        //////////////////////////////////////////////////////////////////////////
        
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
                    dark = true;
                    countno++;
                    Debug.Log("You have covered the glowstick.");
                }
                else if (countno == 1)
                {
                    glow.enabled = true;
                    dark = false;
                    countno--;
                    Debug.Log("You have uncovered the glowstick.");
                }
            }
        }









        //if (Input.GetKeyDown(KeyCode.G) && isUpgraded == false && count == 0)
        //{
        //    glow.enabled = false;
        //    dark = true;
        //    count++;
        //    Debug.Log("You have covered the glowstick.");
        //}
        //else if (Input.GetKeyDown(KeyCode.G) && !isUpgraded && dark && count == 1)
        //{
        //    glow.enabled = true;
        //    dark = false;
        //    count--;
        //    Debug.Log("You have uncovered the glowstick.");
        //}


        ////Glow stick to flashlight
        //if (isUpgraded == true && Input.GetKeyDown(KeyCode.G) && count == 0)
        //{
        //    glow.enabled = false;
        //    beam.enabled = true;
        //    Debug.Log("You have covered your glowstick and turned on your flashlight.");
        //    count++;
        //    Debug.Log("Count now set to 1.");
        //}
        ////Flashlight to glowstick
        //else if (isUpgraded == true && Input.GetKeyDown(KeyCode.G)  && count == 1)
        //{
        //    glow.enabled = true;
        //    beam.enabled = false;
        //    Debug.Log("You have uncovered your glowstick.");
        //    count++;
        //    Debug.Log("Count now set to 2.");
        //}
        ////Glowstick to darkness
        //else if (isUpgraded == true && Input.GetKeyDown(KeyCode.G) && count == 2)
        //{
        //    glow.enabled = false;
        //    beam.enabled = false;
        //    Debug.Log("You have plunged yourself in complete darkness.");
        //    dark = true;
        //    count++;
        //    Debug.Log("Count now set to 3.");
        //}
        ////Darkness to glowstick
        //else if (isUpgraded == true && Input.GetKeyDown(KeyCode.G) && count == 3)
        //{
        //    glow.enabled = true;
        //    beam.enabled = false;
        //    Debug.Log("You have uncovered your glowstick.");
        //    count = 0; //This represents ONLY glowstick is glowing <0 = glowstick, 1 = flashlight, 2 = all off>
        //    Debug.Log("Count is reset to 0.");
        //    dark = false;
        //}
        ////Making it so that the player can move the light.
        //if (Input.GetAxis("Mouse ScrollWheel") && isUpgraded)
        //{

        //}
    }
}