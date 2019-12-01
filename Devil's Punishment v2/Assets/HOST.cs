using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HOST : NetworkBehaviour
{
    public static HOST h;
    void Awake()
    { h = this; }
  
    [Command]
    public void CmdSendMessages()
    {

        print("ONSRVER");
    }
}
