using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class PlayerInformation : MonoBehaviour
{
    public CSteamID steamid;
    public CSteamID lobby;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
