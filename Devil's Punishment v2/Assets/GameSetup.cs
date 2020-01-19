using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GameSetup : MonoBehaviour
{

    public GameObject levelGen01;
    public Light levelNetworkLED;
    public PostProcessVolume postFXVolume;
    public ColorGrading postFXColorGrading;

    public static GameSetup setup;

    public void Awake()
    {
        setup = this;
    }

    public bool vSync = false;
    private void Start()
    {
        postFXVolume.profile.TryGetSettings(out postFXColorGrading);

        if (!vSync)
        {
            Application.targetFrameRate = 300;
        }
    }
    public void clientActive(Network_Player ply)
    {
        // Cyan light for client
        levelNetworkLED.color = Color.cyan;
        Network_Transmitter.transmitter.startClient(ply);
        

    }

    #region Singleton
    public void generateLevel(Network_Player ply)
    {
        // Green light for host
      //  levelGen01.SetActive(true);
        levelNetworkLED.color = Color.green;
       Network_Transmitter.transmitter.startClient(ply);
       

    }

    #endregion


}
