using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightScript : MonoBehaviour
{
    [Header("Debug Settings")]
    public KeyCode flashlightButton;//Key to activate flashlight
    public float glowLightIntensity;//Insenity of glowLight light.
    public float glowLightRadius;//Size of glowLight
    public Color glowColor;//GlowLight Color
    public float flashLightInsensity;//Insenity of flashLight light.
    public float flashLightRange;//Range of Flashlight
    public float flashLightAngle;//Angle of FlashLight(The circle of Light)
    public Color flashColor;//FlashLight Color
    [Header("Flicker Variables")]
    [SerializeField]
    private bool isFlickerEnabled = false;
    [SerializeField]
    private bool isUpgraded = false;
    [SerializeField]
    private float range = 0.1f;
    private Light glowLight;//GlowLight GameObject
    private Light flashLight;//FlashLight GameObject
    private float currentIntensity;//Light Intensity

    void Start()
    {
        glowLight = this.gameObject.transform.Find("Lights/GlowLight").GetComponent<Light>();
        flashLight = this.gameObject.transform.Find("Lights/Flashlight").GetComponent<Light>();
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
                flashLight.gameObject.SetActive(!flashLight.gameObject.activeSelf);
                flashLight.intensity = flashLightInsensity;
                flashLight.range = flashLightRange;
                flashLight.spotAngle = flashLightAngle;
                flashLight.color = flashColor;
                flashLight.enabled = !flashLight.enabled;
            }
            else
            {
                glowLight.gameObject.SetActive(!glowLight.gameObject.activeSelf);
                glowLight.intensity = glowLightIntensity;
                glowLight.range = glowLightRadius;
                glowLight.color = glowColor;
                glowLight.enabled = !glowLight.enabled;
            }
                
        }

        if (isFlickerEnabled)
        {
            if(isUpgraded)
            {
                flashLight.intensity = Random.Range(currentIntensity - range, currentIntensity + range);
            }
            else
            {
                glowLight.intensity = Random.Range(currentIntensity - range, currentIntensity + range);
            }
        }
    }
}
