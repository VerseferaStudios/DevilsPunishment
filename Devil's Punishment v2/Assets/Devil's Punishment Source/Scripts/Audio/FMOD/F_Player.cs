using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_Player : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlayWalkEvent(string EventPath)
    {
        FMOD.Studio.EventInstance Walk = FMODUnity.RuntimeManager.CreateInstance(EventPath);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(Walk, transform, GetComponent<Rigidbody>());
        Walk.start();
        Walk.release();
    }
}
