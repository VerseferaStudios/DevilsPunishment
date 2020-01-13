using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameState : NetworkBehaviour
{
    public static GameState gameState;

    public List<gameStateType> game_state;



    public enum gameStateType
    {
        none,
        FleeGrenRoom,
        CutCuffs,
        EndRoom
    }

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

    public void addState(gameStateType gST)
    {
        if (!game_state.Contains(gST) && gST != gameStateType.none)
        {
            game_state.Add(gST);
            switch (gST)
            {
                case gameStateType.FleeGrenRoom:
                    Network_Transmitter.transmitter.sendNetworkMessage("Fled gen room!", "SYSTEM");
                    break;
                case gameStateType.CutCuffs:
                    Network_Transmitter.transmitter.sendNetworkMessage("Cut cuffs", "SYSTEM");
                    break;
                case gameStateType.EndRoom:
                    Network_Transmitter.transmitter.sendNetworkMessage("Fled end room", "SYSTEM");
                    break;
                default:
                    break;
            }
        }

        if(game_state.Count == 3)
        {
            Network_Transmitter.transmitter.sendNetworkMessage("Won game!","SYSTEM");
        }

        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
