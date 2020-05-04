using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_Rifle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void PlayRReloadEvent()
    {
        FMOD.Studio.EventInstance RReload = FMODUnity.RuntimeManager.CreateInstance("event:/Player/Weapons/Rifle/Rifle_Foley");
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(RReload, transform, GetComponent<Rigidbody>());
        RReload.start();
        RReload.release();

        print("FMOD-AUDIO-PLAYING");
    }

    public void PlayRFireEvent()
    {
        FMOD.Studio.EventInstance RFire = FMODUnity.RuntimeManager.CreateInstance("event:/Player/Weapons/Rifle/Rifle_Blast");
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(RFire, transform, GetComponent<Rigidbody>());
        RFire.start();
        RFire.release();

        print("FMOD-AUDIO-PLAYING");
    }

    void PlayRAimEvent()
    {
        FMOD.Studio.EventInstance RAim = FMODUnity.RuntimeManager.CreateInstance("event:/Player/Weapons/Shotgun/Shotgun_Handling");
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(RAim, transform, GetComponent<Rigidbody>());
        RAim.start();
        RAim.release();

        print("FMOD-AUDIO-PLAYING");
    }

}
