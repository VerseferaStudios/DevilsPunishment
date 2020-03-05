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
        FMOD.Studio.EventInstance RReload = FMODUnity.RuntimeManager.CreateInstance("event:/Player/Weapons/Rifle/Foley");
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(RReload, transform, GetComponent<Rigidbody>());
        RReload.start();
        RReload.release();

        print("FMOD-AUDIO-PLAYING");
    }
}
