using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HideNetManHUDBehaviour : MonoBehaviour
{
    public NetworkManagerHUD netManHUD;
    private void Start()
    {
        if(netManHUD == null)
        {
            netManHUD = GetComponent<NetworkManagerHUD>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            netManHUD.showGUI = !netManHUD.showGUI;
        }
    }
}
