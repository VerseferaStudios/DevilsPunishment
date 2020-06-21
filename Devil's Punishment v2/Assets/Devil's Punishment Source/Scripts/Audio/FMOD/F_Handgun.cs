using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_Handgun : MonoBehaviour
{
      private float lastFireTime;

    // Start is called before the first frame update
    void Start()
    {
          lastFireTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Aim"))
        {
            FMOD.Studio.EventInstance HAim = FMODUnity.RuntimeManager.CreateInstance("event:/Player/Weapons/Gun Handling/Gun_Aim");
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(HAim, transform, GetComponent<Rigidbody>());
            HAim.start();
            HAim.release();
        }
    }

    void PlayHgReloadEvent()
    {
        FMOD.Studio.EventInstance HReload = FMODUnity.RuntimeManager.CreateInstance("event:/Player/Weapons/Handgun/Handgun_Foley");
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(HReload, transform, GetComponent<Rigidbody>());
        HReload.start();
        HReload.release();

        print("FMOD-AUDIO-PLAYING");
    }

    void PlayHgAimEvent()
    {
        FMOD.Studio.EventInstance HAim = FMODUnity.RuntimeManager.CreateInstance("event:/Pickups/WeaponPU/Handgun_PU");
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(HAim, transform, GetComponent<Rigidbody>());
        HAim.start();
        HAim.release();

        print("FMOD-AUDIO-PLAYING");
    }

    public void PlayHgFireEvent()
    {
        if (Time.time > lastFireTime + 0.15f)
        {
            FMOD.Studio.EventInstance HFire = FMODUnity.RuntimeManager.CreateInstance("event:/Player/Weapons/Handgun/Handgun_Blast");
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(HFire, transform, GetComponent<Rigidbody>());
            HFire.start();
            HFire.release();

            print("FMOD-AUDIO-PLAYING");
        }
    }

}
