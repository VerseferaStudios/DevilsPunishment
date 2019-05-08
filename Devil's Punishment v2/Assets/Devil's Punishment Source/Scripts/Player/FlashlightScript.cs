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
    [HideInInspector]
    [Tooltip("Upgrade boolean used to determine if player picked up flashlight")]
    public bool isUpgraded;//Has the player picked up the flashlight?
    [SerializeField]
    private float range = 0.1f;
    [HideInInspector]
    public Light glowLight;//GlowLight GameObject
    [HideInInspector]
    public Light flashLight;//FlashLight GameObject
    private float currentIntensity;//Light Intensity
    private GameObject flashlightobj;
    private GameObject glowlightobj;

    void Start()
    {
        glowLight = this.gameObject.transform.Find("Camera/Lights/glowstick/GlowLight").GetComponent<Light>();
        flashLight = this.gameObject.transform.Find("Camera/Lights/flashlight/Flashlight").GetComponent<Light>();
        flashlightobj = flashLight.transform.parent.gameObject;
        glowlightobj = glowLight.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
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
        flashlightobj.SetActive(!flashlightobj.activeInHierarchy);//Set flashlight object to active
        flashLight.intensity = flashLightInsensity;//set intensity of flashlight light.
        flashLight.range = flashLightRange;//Set range
        flashLight.spotAngle = flashLightAngle;//Set angle of flashlight
        flashLight.color = flashColor;//Set Color
        flashLight.enabled = !flashLight.enabled;//set flashlight active
    }

    public void turnGlowLight()
    {
        glowlightobj.SetActive(!glowlightobj.activeInHierarchy);//Set glowlight model to active
        glowLight.gameObject.SetActive(!glowLight.gameObject.activeSelf);//Set glowlight gameobject active
        glowLight.intensity = glowLightIntensity;//Set intensity of glowlight light.
        glowLight.range = glowLightRadius;//Set radius
        glowLight.color = glowColor;//Set Color
        glowLight.enabled = !glowLight.enabled;//set glowlight active
    }

    public void turnFlicker()
    {
        if (isUpgraded)
        {
            flashLight.intensity = Random.Range(currentIntensity - range, currentIntensity + range);
        }
        else
        {
            glowLight.intensity = Random.Range(currentIntensity - range, currentIntensity + range);
        }
    }
}
