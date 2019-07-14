//Author: Dave Bird
//Date: Saturday, July 13, 2019
//Last Edited: Saturday, July 13, 2019
//By: Dave Bird
//Purpose: This script handles the four abilities of the flashlight.
//Written For: Devil's Punishment v2
//Purpose: This script details all things that has to do with the limitations of the cuffed mechanic.
//Notes: Further information can be found at http://www.nolocationyet.com (Dead link)


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSDLightAdjustments : MonoBehaviour
{
    public Light beam; //"Flashlight" on player model under Player > Player Model > GlowStick/Flashlight GameObject > FlashlightCase - EmptyGameObject > Flashlight
    [Header("Rotation of the Light")]
    public float rotSpeed = 1.0f;
    public float homeRotx = 4.03f;
    public float homeRoty = -33.412f;
    public float homeRotz = 1.97f;
    public float maxRotx = 0.0f;
    public float maxRoty = 0.0f;
    public float maxRotz = 0.0f;
    public float minRotx = -10.0f;
    public float minRoty = -10.0f;
    public float minRotz = -10.0f;

    public float curRotx;
    public float curRoty;
    public float curRotz;

    [Header("Spot Angle of Light")]
    public float turnSpeed = 1.0f;
    public float homeAngle = 45f;
    public float maxAngle = 65f;
    public float minAngle = 25f;

    public void Awake()
    {
        beam = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        //Raising the light.
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {

            Debug.Log("Player has raised the light up.");
            if (curRotx >= 10.0f)
            {
                //If it exceeds the maximum maxvalue then put it at x.xxf
            }
            if (curRoty >= 10.0f)
            {
                //If it exceeds the maximum maxvalue then put it at x.xxf
            }
            if (curRotz >= 10.0f)
            {
                //If it exceeds the maximum maxvalue then put it at x.xxf
            }

        }
        //Lowering the light.
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Debug.Log("Player has lowered the light down.");
            if (curRotx <= -10.0f)
            {
                //If it exceeds the maximum maxvalue then put it at x.xxf
            }
            if (curRoty <= -10.0f)
            {
                //If it exceeds the maximum maxvalue then put it at x.xxf
            }
            if (curRotz <= -10.0f)
            {
                //If it exceeds the maximum maxvalue then put it at x.xxf
            }
        }

        //Widening the beam.
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            beam.spotAngle = 62; //Must slowly expand the angle to maximum of 62
            Debug.Log("Player has expanded the lights view.");
            if (maxAngle >= 62)
            {
                //If the angles maximum value exceeds maxvalue then put it at x.xxf
            }
        }
        //Narrowing the beam.
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            beam.spotAngle = 22; //Must slowly expand the angle to maximum of 22
            Debug.Log("Player has expanded the lights view.");
            if (maxAngle <= 22)
            {
                //If the angles maximum value exceeds maxvalue then put it at x.xxf
            }
        }
        //Snapping the light back to its home position.
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //Snap the Rotation back to its normal rotation on all 3 x,y,z and back to its normal angle of 42
            beam.transform.Rotate(homeRotx, homeRoty, homeRotz);
            beam.spotAngle = homeAngle;
            Debug.Log("Player has reset the location of the light source");
        }
    }
}
