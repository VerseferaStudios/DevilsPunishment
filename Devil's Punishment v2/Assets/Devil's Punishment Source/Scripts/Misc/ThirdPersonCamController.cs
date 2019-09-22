using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamController : MonoBehaviour
{
    public GameObject backCam;
    public GameObject sideCam;
    public GameObject canvas;
    // Start is called before the first frame update
    void Start()
    {
        backCam.SetActive(true);
        sideCam.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("ToggleCam")){
            if (backCam.activeSelf){
                backCam.SetActive(false);
                sideCam.SetActive(true);
            } else if (sideCam.activeSelf){
                sideCam.SetActive(false);
                canvas.SetActive(false);
            } else {
                backCam.SetActive(true);
                canvas.SetActive(true);
            }
            Debug.Log("backCam.activeSelf: "+backCam.activeSelf);
            Debug.Log("sideCam.activeSelf: "+sideCam.activeSelf);
        }
    }
}
