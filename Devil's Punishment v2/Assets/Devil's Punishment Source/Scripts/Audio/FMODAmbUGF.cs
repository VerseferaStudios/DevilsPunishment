using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODAmbUGF : MonoBehaviour { 

public static FMOD.Studio.EventInstance Amb;

[FMODUnity.EventRef]
public string eventPath;

// Use this for initialization
void Start()
{
    Amb = FMODUnity.RuntimeManager.CreateInstance(eventPath);
    Amb.start();
    Amb.release();
}

// Update is called once per frame
void Update()
{

}

void OnDestroy()
{
    Amb.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

}
}
 