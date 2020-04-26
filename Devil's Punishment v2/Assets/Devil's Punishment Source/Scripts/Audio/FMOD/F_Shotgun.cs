using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_Shotgun : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void PlaySgReloadEvent()
    {
        FMOD.Studio.EventInstance SReload = FMODUnity.RuntimeManager.CreateInstance("event:/Player/Weapons/Shotgun/Shotgun_Foley");
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(SReload, transform, GetComponent<Rigidbody>());
        SReload.start();
        SReload.release();

        print("FMOD-AUDIO-PLAYING");
    }

    void PlaySgFireEvent()
    {
        FMOD.Studio.EventInstance SFire = FMODUnity.RuntimeManager.CreateInstance("event:/Player/Weapons/Shotgun/Shotgun_Blast");
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(SFire, transform, GetComponent<Rigidbody>());
        SFire.start();
        SFire.release();

        print("FMOD-AUDIO-PLAYING");
    }

    void PlaySgAimEvent()
    {
        FMOD.Studio.EventInstance SgAim = FMODUnity.RuntimeManager.CreateInstance("event:/Player/Weapons/Shotgun/Shotgun_Handling");
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(SgAim, transform, GetComponent<Rigidbody>());
        SgAim.start();
        SgAim.release();

        print("FMOD-AUDIO-PLAYING");
    }

}
