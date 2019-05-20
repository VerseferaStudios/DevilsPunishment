using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class PlayerInformation : MonoBehaviour
{
    public CSteamID steamid;
    public Vector3 playersPos;
    public Quaternion playersRot;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }


}
