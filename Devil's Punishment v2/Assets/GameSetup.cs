using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : MonoBehaviour
{

    public GameObject levelGen01;
    public Light levelNetworkLED;

    public static GameSetup setup;

    public void Awake()
    {
        setup = this;
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
        levelGen01.SetActive(true);
        levelNetworkLED.color = Color.green;
        Network_Transmitter.transmitter.startClient(ply);

    }

    #endregion



    // Start is called before the first frame update

}
