using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fullbright : MonoBehaviour
{
    public Light light;
    public static Fullbright fullbright;
    // Start is called before the first frame update

    void Awake()
    {
        fullbright = this;
    }
    void Start()
    {
        light = transform.GetComponent<Light>();

    }

    public void enable()
    {
        light.enabled = true;
    }

    public void disable()
    {
        light.enabled = false;
    }

}
