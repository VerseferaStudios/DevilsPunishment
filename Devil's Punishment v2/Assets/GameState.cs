using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameState : NetworkBehaviour
{
    public static GameState gameState;

    [SyncVar]
    public string state;
    // Start is called before the first frame update
    public void StartGame()
    {
        gameState = this;
        
        if(isServer)
        {
            state = "GAME_START";
            Network_Transmitter.transmitter.player.RpcSendChatMessage("Set state to " + state);
        }
        else
        {
            Network_Transmitter.transmitter.player.RpcSendChatMessage(state);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
