//Author: Dave Bird
//Date: Sunday, June 23, 2019
    //Last Edited: Sunday, June 23, 2019
        //By: Dave Bird
            //Purpose: Write the script
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

    public void Upgraded()
    {
        isUpgraded = true;
        flashLight.SetActive(true);
    }

    //Swtich levels of light - DONE
    void Update()
    {
        //Switching the different levels of light.

        /////////////////////////////////////////////////////////////////////////
        //FOR TESTING ONLY REMOVE AFTER TESTING
        if (isUpgraded == false && Input.GetKeyDown(KeyCode.Z))
        {
            Upgraded();
            Debug.Log("You have set the upgrade to being true temporarily.");
        }
        ////////////////////////////////////////////////////////////////////////
        
        //Glow stick to flashlight
        if (isUpgraded == true && Input.GetKeyDown(KeyCode.G) && count == 0)
        {
            glow.enabled = false;
            beam.enabled = true;
            Debug.Log("You have covered your glowstick and turned on your flashlight.");
            count++;
            Debug.Log("Count now set to 1.");
        }
        //Flashlight to glowstick
        else if (isUpgraded == true && Input.GetKeyDown(KeyCode.G)  && count == 1)
        {
            glow.enabled = true;
            beam.enabled = false;
            Debug.Log("You have uncovered your glowstick.");
            count++;
            Debug.Log("Count now set to 2.");
        }
        //Glowstick to darkness
        else if (isUpgraded == true && Input.GetKeyDown(KeyCode.G) && count == 2)
        {
            glow.enabled = false;
            beam.enabled = false;
            Debug.Log("You have plunged yourself in complete darkness.");
            dark = true;
            count++;
            Debug.Log("Count now set to 3.");
        }
        //Darkness to glowstick
        else if (isUpgraded == true && Input.GetKeyDown(KeyCode.G) && count == 3)
        {
            glow.enabled = true;
            beam.enabled = false;
            Debug.Log("You have uncovered your glowstick.");
            count = 0; //This represents ONLY glowstick is glowing <0 = glowstick, 1 = flashlight, 2 = all off>
            Debug.Log("Count is reset to 0.");
            dark = false;
        }
    }

}



























//    [Header("Debug Settings")]
//    [Tooltip("Button used to turn on lighting")]
//    public KeyCode flashlightButton;//Key to activate flashlight
//    [Header("GlowLight Settings")]
//    [Tooltip("GlowLight light intensity")]
//    public float glowLightIntensity;//Insenity of glowLight light.
//    [Tooltip("Radius of the GlowLight")]
//    public float glowLightRadius;//Size of glowLight
//    [Tooltip("GlowLight color")]
//    public Color glowColor;//GlowLight Color
//    [Header("FlashLight Settings")]
//    [Tooltip("Flashlight light intensity")]
//    public float flashLightInsensity;//Insenity of flashLight light.
//    [Tooltip("Length of Flashlight beam")]
//    public float flashLightRange;//Range of Flashlight
//    [Tooltip("Radius of Light Beam")]
//    public float flashLightAngle;//Angle of FlashLight(The circle of Light)
//    [Tooltip("Flashlight color")]
//    public Color flashColor;//FlashLight Color
//    [Header("Flicker Variables")]
//    [SerializeField]
//    [Tooltip("Option to flicker the lights")]
//    private bool isFlickerEnabled = false;//Flickering
//    [SerializeField]
//    [Tooltip("Upgrade boolean used to determine if player picked up flashlight")]
//    private bool isUpgraded = false;//Has the player picked up the flashlight?
//    [SerializeField]
//    private float flicker_range = 0.1f;
//    private float currentIntensity;//Light Intensity
//	private Light glowLight;//GlowLight GameObject
//	private Light flashLight;//FlashLight GameObject

//	void Start()
//	{
//		Debug.Log("current gameObject for FlashLightScript.cs is: " + gameObject.name);
//		Transform lights = gameObject.transform.Find("Camera/GunCamera/Lights");
//		Debug.Assert(lights != null, "FlashlightScript.cs could not find Player/Lights Transform");

//		glowLight = lights.Find("GlowLight").Find("Point Light").GetComponent<Light>();
//		Debug.Assert(glowLight != null, "FlashlightScript.cs could not find light component in Player/Lights/Glowlight gameObject");

//		flashLight = lights.Find("Flashlight").Find("Spotlight").GetComponent<Light>();
//		Debug.Assert(flashLight != null, "FlashlightScript.cs could not find light component in Player/Lights/Flashlight gameObject");
//	}

//	// Update is called once per frame
//	void Update()
//	{
//		//isUpgraded will be coming from the player scripts
//		//isUpgraded == player.isUpgraded();
//		if (Input.GetKeyDown(flashlightButton))
//        {
//            if(isUpgraded)
//            {
//                turnFlashLight();
//            }
//            else
//            {
//                turnGlowLight();
//            }
//        }

//        if (isFlickerEnabled)
//        {
//            turnFlicker();
//        }
//    }

//    public void turnFlashLight()
//    {
//		Debug.Log("Attempting to toggle flashLight.");
//        flashLight.intensity = flashLightInsensity;
//        flashLight.range = flashLightRange;
//        flashLight.spotAngle = flashLightAngle;
//        flashLight.color = flashColor;
//        flashLight.enabled = !flashLight.enabled;
//    }

//    public void turnGlowLight()
//    {
//		Debug.Log("Attempting to toggle glowLight");
//		glowLight.enabled = !glowLight.enabled;
//        glowLight.intensity = glowLightIntensity;
//        glowLight.range = glowLightRadius;
//        glowLight.color = glowColor;
//    }

//    public void turnFlicker()
//    {
//        if (isUpgraded)
//		{
//			flashLight.intensity = Random.Range(currentIntensity - flicker_range, currentIntensity + flicker_range);
//        }
//        else
//        {
//            glowLight.intensity = Random.Range(currentIntensity - flicker_range, currentIntensity + flicker_range);
//        }
//    }
//}
