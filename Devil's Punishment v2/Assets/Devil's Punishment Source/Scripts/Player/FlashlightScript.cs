using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightScript : MonoBehaviour
{
    [Header("Debug Settings")]
    [Tooltip("Button used to turn on lighting")]
    public KeyCode flashlightButton;//Key to activate flashlight
    [Header("GlowLight Settings")]
    [Tooltip("GlowLight light intensity")]
    public float glowLightIntensity;//Insenity of glowLight light.
    [Tooltip("Radius of the GlowLight")]
    public float glowLightRadius;//Size of glowLight
    [Tooltip("GlowLight color")]
    public Color glowColor;//GlowLight Color
    [Header("FlashLight Settings")]
    [Tooltip("Flashlight light intensity")]
    public float flashLightInsensity;//Insenity of flashLight light.
    [Tooltip("Length of Flashlight beam")]
    public float flashLightRange;//Range of Flashlight
    [Tooltip("Radius of Light Beam")]
    public float flashLightAngle;//Angle of FlashLight(The circle of Light)
    [Tooltip("Flashlight color")]
    public Color flashColor;//FlashLight Color
    [Header("Flicker Variables")]
    [SerializeField]
    [Tooltip("Option to flicker the lights")]
    private bool isFlickerEnabled = false;//Flickering
    [SerializeField]
    [Tooltip("Upgrade boolean used to determine if player picked up flashlight")]
    private bool isUpgraded = false;//Has the player picked up the flashlight?
    [SerializeField]
    private float flicker_range = 0.1f;
    private float currentIntensity;//Light Intensity
	private Light glowLight;//GlowLight GameObject
	private Light flashLight;//FlashLight GameObject

	void Start()
	{
		Debug.Log("current gameObject for FlashLightScript.cs is: " + gameObject.name);
		Transform lights = gameObject.transform.Find("Lights");
		Debug.Assert(lights != null, "FlashlightScript.cs could not find Player/Lights Transform");

		glowLight = lights.Find("GlowLight").Find("Point Light").GetComponent<Light>();
		Debug.Assert(glowLight != null, "FlashlightScript.cs could not find light component in Player/Lights/Glowlight gameObject");

		flashLight = lights.Find("Flashlight").Find("Spotlight").GetComponent<Light>();
		Debug.Assert(flashLight != null, "FlashlightScript.cs could not find light component in Player/Lights/Flashlight gameObject");
	}

	// Update is called once per frame
	void Update()
	{
		//isUpgraded will be coming from the player scripts
		//isUpgraded == player.isUpgraded();
		if (Input.GetKeyDown(flashlightButton))
        {
            if(isUpgraded)
            {
                turnFlashLight();
            }
            else
            {
                turnGlowLight();
            }
        }

        if (isFlickerEnabled)
        {
            turnFlicker();
        }
    }

    public void turnFlashLight()
    {
		Debug.Log("Attempting to toggle flashLight.");
        //flashLight.gameObject.SetActive(!flashLight.gameObject.activeSelf);
        flashLight.intensity = flashLightInsensity;
        flashLight.range = flashLightRange;
        flashLight.spotAngle = flashLightAngle;
        flashLight.color = flashColor;
        flashLight.enabled = !flashLight.enabled;
    }

    public void turnGlowLight()
    {
		Debug.Log("Attempting to toggle glowLight");
		glowLight.enabled = !glowLight.enabled;
		//glowLight.gameObject.SetActive(!glowLight.enabled);
        glowLight.intensity = glowLightIntensity;
        glowLight.range = glowLightRadius;
        glowLight.color = glowColor;
    }

    public void turnFlicker()
    {
        if (isUpgraded)
		{
			flashLight.intensity = Random.Range(currentIntensity - flicker_range, currentIntensity + flicker_range);
        }
        else
        {
            glowLight.intensity = Random.Range(currentIntensity - flicker_range, currentIntensity + flicker_range);
        }
    }
}
