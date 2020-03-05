using System.Collections;
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
        FMOD.Studio.EventInstance HReload = FMODUnity.RuntimeManager.CreateInstance("event:/Player/Weapons/Handgun/Foley");
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(HReload, transform, GetComponent<Rigidbody>());
        HReload.start();
        HReload.release();

        print("FMOD-AUDIO-PLAYING");
    }
}
