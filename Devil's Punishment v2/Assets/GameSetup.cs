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
    public void clientActive()
    {
        // Cyan light for client
        //levelNetworkLED.color = Color.cyan;
    }

    #region Singleton
    public void generateLevel()
    {
        // Green light for host
        levelGen01.SetActive(true);
        //levelNetworkLED.color = Color.green;
    }

    #endregion



    // Start is called before the first frame update

}
