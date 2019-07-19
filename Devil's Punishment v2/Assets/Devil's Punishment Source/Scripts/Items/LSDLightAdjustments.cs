//Author: Dave Bird
//Date: Saturday, July 13, 2019
    //Last Edited: Saturday, July 14, 2019
        //By: Dave Bird
            //Purpose: Finish the script.
//Written For: Devil's Punishment v2
//Purpose: This script details all things that has to do with the limitations of the cuffed mechanic.
//Notes: Further information can be found at http://www.nolocationyet.com (Dead link)


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LSDLightAdjustments : MonoBehaviour
{
    public Light beam; //"Flashlight" on player model under Player > Player Model > GlowStick/Flashlight GameObject > FlashlightCase - EmptyGameObject > Flashlight
    public GameObject flashlightCase;
    public Animation[] clips; 

    public float homeAngle = 45.0f;
    [SerializeField]
    private bool isRaised = false;
    [SerializeField]
    private bool isCenter = true;
    [SerializeField]
    private bool isLowered = false;
    [SerializeField]
    private bool isWide = false;
    [SerializeField]
    private bool isNarrow = false;
    [SerializeField]
    private bool isNormal = true;


    public void Awake()
    {
        beam = GetComponent<Light>();
        beam.GetComponent<Light>().range = 30;
        beam.GetComponent<Light>().spotAngle = 30;
    }

    void SetToNormal()
    {
        isCenter = true;
        isRaised = false;
        isLowered = false;
        isWide = false;
        isNarrow = false;
        isNormal = true;
    }

    // Update is called once per frame
    void Update()
    {

        //Raising the light.
        if (Input.GetButtonDown("Flashlight Up"))
        {
            if (isCenter)
            {
                flashlightCase.GetComponent<Animation>().Play("CenterUp");
                isRaised = true;
                isCenter = false;
                Debug.Log("Player has raised the light up to the ceiling");
            }
            else if (isLowered)
            {
                flashlightCase.GetComponent<Animation>().Play("DownCenter");
                isCenter = true;
                isLowered = false;
                Debug.Log("Player has raised the light from the floor.");
            }
        }
        //Lowering the light.
        else if (Input.GetButtonDown("Flashlight Down"))
        {
            if (isCenter)
            {
                flashlightCase.GetComponent<Animation>().Play("CenterDown");
                isLowered = true;
                isCenter = false;
                Debug.Log("Player has lowered the light down to the floor");
            }
            else if (isRaised)
            {
                flashlightCase.GetComponent<Animation>().Play("UpCenter");
                isRaised = false;
                isCenter = true;
                Debug.Log("Player has lowered the light down to eye level.");
            }
        }
        //Widening the light.
        if (Input.GetButtonDown("Flashlight Wide"))
        {
            if (isNormal)
            {
                flashlightCase.GetComponent<Animation>().Play("CenterWiden");
                isNormal = false;
                isWide = true;
                Debug.Log("Player has made the beam wider.");
            }
            else if (isNarrow)
            {
                flashlightCase.GetComponent<Animation>().Play("NarrowCenter");
                isNormal = true;
                isNarrow = false;
                Debug.Log("Player has lowered the light down to eye level.");
            }
        }
        //Narrowing the light.
        else if (Input.GetButtonDown("Flashlight Narrow"))
        {
            if (isNormal)
            {
                flashlightCase.GetComponent<Animation>().Play("CenterNarrow");
                isNormal = false;
                isNarrow = true;
                Debug.Log("Player has made the beam wider.");
            }
            else if (isWide)
            {
                flashlightCase.GetComponent<Animation>().Play("WidenCenter");
                isNormal = true;
                isWide = false;
                Debug.Log("Player has made the beam back to normal.");
            }
        }
        //Snapping the light back to its home position.
        if (Input.GetButtonDown("Flashlight Home"))
        {
            if (isRaised && isWide)
            {
                flashlightCase.GetComponent<Animation>().Play("UpCenter");
                flashlightCase.GetComponent<Animation>().Play("WidenCenter");
            }
            else if (isRaised && isNarrow)
            {
                flashlightCase.GetComponent<Animation>().Play("UpCenter");
                flashlightCase.GetComponent<Animation>().Play("NarrowCenter");
            }
            else if (isLowered && isWide)
            {
                flashlightCase.GetComponent<Animation>().Play("DownCenter");
                flashlightCase.GetComponent<Animation>().Play("WidenCenter");
            }
            else if (isLowered && isNarrow)
            {
                flashlightCase.GetComponent<Animation>().Play("DownCenter");
                flashlightCase.GetComponent<Animation>().Play("NarrowCenter");
            }
            else if (isCenter && isWide)
            {
                flashlightCase.GetComponent<Animation>().Play("WidenCenter");
            }
            else if (isCenter && isNarrow)
            {
                flashlightCase.GetComponent<Animation>().Play("NarrowCenter");
            }
            beam.spotAngle = homeAngle;
            SetToNormal();
            Debug.Log("Player has reset the location of the light source");
        }
    }
}
