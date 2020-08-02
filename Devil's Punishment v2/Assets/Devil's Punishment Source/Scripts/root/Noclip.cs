using Aura2API;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Noclip : MonoBehaviour
{

    public bool noclip_active = false;

    public MouseLook look;
    public PlayerController_Revamped playerController;
    // Start is called before the first frame update

    float yaw = 0;
    float pitch = 0;
    Vector3 targetRotation;

    public float speed = 5;

    public PostProcessLayer postProcessing;
    public AuraCamera auraCam;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            noclip_active = !noclip_active;

            if(noclip_active)
            {
                look.enabled = false;
                playerController.enabled = false;
                postProcessing.enabled = false;
                auraCam.enabled = false;
                Fullbright.fullbright.enable();
            }
            else
            {

                look.enabled = true;
                playerController.enabled = true;
                postProcessing.enabled = true;
                auraCam.enabled = true;
                Fullbright.fullbright.disable();
            }
            



        }

      

        if(noclip_active)
        {
            yaw += Input.GetAxis("Mouse X");
            pitch -= Input.GetAxis("Mouse Y");
            targetRotation = new Vector3(pitch, yaw);
            transform.eulerAngles = targetRotation;

            if(Input.GetKey(KeyCode.W))
            {
                transform.position += transform.forward * Time.deltaTime * speed ;
            }
            

        }
    }
}
