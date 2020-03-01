using Aura2API;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{

    public float mouseSensivity = 10;
    public Transform target;
    public float targtDist;
    public Transform AimSightPos;
    public Transform AimTransform;
    public GameObject ShootFrom;
    public GameObject myInventory;

    public bool AimDownSight = false;
    public bool ThirdPerson = true;

    public GameObject FPSPosition;

    public Vector2 pitchMinMax = new Vector2(-40, 85);

    float yaw;
    public float pitch;

    // Use this for initialization
    void Start()
    {
        //transform.GetComponent<AuraCamera>().enabled = false;
        //transform.GetComponent<AuraCamera>().enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public int pitchUp = 200;
    public int pitchDown = 400;
    public int pitchForward = 85;
    public int pitchBack = 200;

    PlayerControls.InputDevice inputDev = PlayerControls.InputDevice.Keyboard; // standard keyboard

    public void setInputDevice(PlayerControls.InputDevice d)
    {
        inputDev = d;
    }

    // public float YBoy;

    // Update is called once per frame

    public Vector3 Problemkind;
    void LateUpdate()
    {
        if (!myInventory.activeInHierarchy)
        {
            Vector3 targetRotation = Vector3.zero;
            if (inputDev == PlayerControls.InputDevice.Keyboard)
            {
                yaw += Input.GetAxis("Mouse X");
                pitch -= Input.GetAxis("Mouse Y");
                pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
                targetRotation = new Vector3(pitch, yaw);
            }
            else
            {
                yaw += Input.GetAxis("XBOX X L");
                pitch -= Input.GetAxis("XBOX Y R");
                pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

                targetRotation = new Vector3(pitch * 4, yaw * 4); // speed boost
            }



            transform.eulerAngles = targetRotation;



            if (ThirdPerson == true)
            {
                transform.position = target.position - transform.forward * targtDist;
            }
            else if (ThirdPerson == false)
            {
                if (AimDownSight == true)
                {
                    Vector3 smoothedChange = Vector3.Lerp(transform.position, AimSightPos.transform.position, 0.5f);
                    GetComponent<Camera>().nearClipPlane = 0.01f;
                    transform.position = smoothedChange;
                }
                else if (AimDownSight == false)
                {
                    Vector3 smoothedChange = Vector3.Lerp(transform.position, FPSPosition.transform.position, 0.5f);
                    GetComponent<Camera>().nearClipPlane = 0.3f;

                    if (pitch < 0)
                    {
                        transform.position = FPSPosition.transform.position - transform.forward * (-pitch / pitchBack) + transform.up * (-pitch / pitchUp);
                    }
                    else
                    {
                        transform.position = FPSPosition.transform.position + transform.forward * (pitch / pitchForward) + transform.up * (pitch / pitchDown);
                    }

                    // transform.position -= transform.forward;

                    //  transform.Rotate(0, 0, pitch);

                    //  transform.RotateAround(FPSPosition.transform.parent.parent.position, new Vector3(0,1,0), -pitch);
                    //  transform.Rotate(new Vector3(pitch,0,0));

                    //  transform.Rotate(FPSPosition.transform.parent.parent.position, pitch);
                    // transform.Rotate(FPSPosition.transform.parent.Find("xx").transform.position, Input.GetAxis("Mouse Y"));
                    // transform.rotation = FPSPosition.transform.rotation;

                }
            }

        }
    }
}

