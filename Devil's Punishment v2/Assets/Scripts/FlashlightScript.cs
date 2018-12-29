using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightScript : MonoBehaviour
{
    [Header("Flicker Variables")]
    [SerializeField]
    private bool isFlickerEnabled = false;

    [SerializeField]
    private float range = 0.1f;

    private Light lightComponent;
    private float currentIntensity;

    void Start()
    {
        lightComponent = GetComponent<Light>();
        currentIntensity = lightComponent.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            lightComponent.enabled = !lightComponent.enabled;
        }

        if (isFlickerEnabled)
        {
            lightComponent.intensity = Random.Range(currentIntensity - range, currentIntensity + range);
        }
    }
}
