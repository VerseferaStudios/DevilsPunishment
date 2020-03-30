using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameState : NetworkBehaviour
{
    public static GameState gameState;

    public List<gameStateType> game_state;

    public string computerName;

    //References to Start Rooms StartRoom script
    public Transform spawnPosition;

    public void setSpawnPos(Transform pos)
    {
        spawnPosition = pos;
        Network_Transmitter.transmitter.player.gameObject.GetComponent<CharacterController>().enabled = false;
        //Teleport player to start Room
        Network_Transmitter.transmitter.player.gameObject.transform.position = pos.position;
        Network_Transmitter.transmitter.player.gameObject.GetComponent<CharacterController>().enabled = true;
    }


    public enum gameStateType
    {
        none,
        FleeGenRoom,
        CutCuffs,
        EndRoom,
        GenPartA,
        GenPartB,
        GenPartC
    }

    [SyncVar]
    public string state;
    // Start is called before the first frame update
    public void StartGame()
    {
        computerName = SystemInfo.deviceName;
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

    //TODO: Improve performance and readability..
    public void addState(gameStateType gST)
    {

        if (!game_state.Contains(gST) && gST != gameStateType.none)
        {
            game_state.Add(gST);
            string username = SystemInfo.deviceName;


            switch (gST)
            {
                case gameStateType.FleeGenRoom:
                    Network_Transmitter.transmitter.sendNetworkMessage("Fled gen room!", username);
                    break;
                case gameStateType.CutCuffs:
                    Network_Transmitter.transmitter.sendNetworkMessage("Cut cuffs", username);
                    break;
                case gameStateType.EndRoom:
                    Network_Transmitter.transmitter.sendNetworkMessage("Reached end room, activated generator?", username);
                    break;
                case gameStateType.GenPartA:
                    Network_Transmitter.transmitter.sendNetworkMessage("Gen Part A inserted!", username);
                    break;
                case gameStateType.GenPartB:
                    Network_Transmitter.transmitter.sendNetworkMessage("Gen Part B inserted!", username);
                    break;
                case gameStateType.GenPartC:
                    Network_Transmitter.transmitter.sendNetworkMessage("Gen Part C inserted!", username);
                    break;
                default:
                    break;
            }

            if (gotGenParts())
            {
                Network_Transmitter.transmitter.sendNetworkMessage("Got all gen parts!", username);
            }

        }

        if (game_state.Count == 3)
            if (gotGenParts() && gST == gameStateType.FleeGenRoom)
            {
                Network_Transmitter.transmitter.sendNetworkMessage("Won game!", "SYSTEM");
                Network_Transmitter.transmitter.sendNetworkMessage("Activated generator and reached end room!", "SYSTEM");
            }





    }




    public bool gotGenParts()
    {
        if (game_state.Contains(gameStateType.GenPartA) &&
        game_state.Contains(gameStateType.GenPartB) &&
        game_state.Contains(gameStateType.GenPartC))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
