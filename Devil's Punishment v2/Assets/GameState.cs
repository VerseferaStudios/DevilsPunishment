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
        doneMechanics = new List<MatchMechanics>();
        
        if(isServer)
        {
            state = "GAME_START";
            Network_Transmitter.transmitter.player.RpcSendChatMessage(state);
        }
    }

    public void sendMessage(string msg)
    {

        Network_Transmitter.transmitter.sendNetworkMessage(msg, "State");
        
    }

    public enum MatchMechanics
    {
        EspaceGenRoom,
        BreakCuffs,
        EndGame
    }

    public List<MatchMechanics> doneMechanics;


    public void setMechanics(MatchMechanics mc)
    {

        if (!doneMechanics.Contains(mc))
        {
            switch (mc)
            {
                case MatchMechanics.EspaceGenRoom:
                    print("Escpaped Gen room!");
                    sendMessage("Escaped Gen room !");
                    break;
                case MatchMechanics.BreakCuffs:
                    sendMessage("Broke cuffs !");
                    break;
                case MatchMechanics.EndGame:
                    sendMessage("Reached end room !");
                    break;
                default:
                    break;
            }
            doneMechanics.Add(mc);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
