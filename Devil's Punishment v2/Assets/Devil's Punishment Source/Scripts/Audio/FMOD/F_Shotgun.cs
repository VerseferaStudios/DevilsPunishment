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
        FMOD.Studio.EventInstance SReload = FMODUnity.RuntimeManager.CreateInstance("event:/Player/Weapons/Shotgun/Foley");
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(SReload, transform, GetComponent<Rigidbody>());
        SReload.start();
        SReload.release();

        print("FMOD-AUDIO-PLAYING");
    }
}
