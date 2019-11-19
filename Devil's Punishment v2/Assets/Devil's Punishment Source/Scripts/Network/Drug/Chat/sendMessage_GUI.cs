using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class sendMessage_GUI : MonoBehaviour
{
    TMP_InputField tmp;
 
   
    // Start is called before the first frame update
    void Start()
    {
        tmp = GetComponent<TMP_InputField>();
    }

    public void sendMessage(BaseEventData e)
    {
        Network_Transmitter.transmitter.sendNetworkMessage(tmp.text, SystemInfo.deviceName);
        tmp.text = "";

    }


}
