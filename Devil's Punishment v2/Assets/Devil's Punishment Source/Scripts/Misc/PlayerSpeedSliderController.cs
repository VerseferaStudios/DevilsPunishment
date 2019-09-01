using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpeedSliderController : MonoBehaviour
{
    private bool prev = false;
    // Start is called before the first frame update
    void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            transform.GetChild(0).gameObject.SetActive(!prev);
            prev = !prev;
        }
    }
}
