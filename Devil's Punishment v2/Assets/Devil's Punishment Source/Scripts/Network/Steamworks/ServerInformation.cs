using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class ServerInformation : MonoBehaviour
{
    public CSteamID lobby;
    public CSteamID[] players;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
