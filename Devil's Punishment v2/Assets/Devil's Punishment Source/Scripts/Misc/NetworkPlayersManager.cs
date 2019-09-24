using BeardedManStudios.Forge.Networking.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NetworkPlayersManager : MonoBehaviour
{
    
    void Start()
    {
        NetworkManager.Instance.InstantiatePlayerInformation();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
