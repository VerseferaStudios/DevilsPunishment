﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_Handgun : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlayHgReloadEvent()
    {
        FMOD.Studio.EventInstance HReload = FMODUnity.RuntimeManager.CreateInstance("event:/Player/Weapons/Handgun/Handgun_Foley");
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(HReload, transform, GetComponent<Rigidbody>());
        HReload.start();
        HReload.release();

        print("FMOD-AUDIO-PLAYING");
    }

    void PlayHgFireEvent()
    {
        FMOD.Studio.EventInstance HFire = FMODUnity.RuntimeManager.CreateInstance("event:/Player/Weapons/Handgun/Handgun_Blast");
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(HFire, transform, GetComponent<Rigidbody>());
        HFire.start();
        HFire.release();

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

}
