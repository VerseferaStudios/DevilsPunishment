using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Network_Transmitter : NetworkBehaviour
{


    public List<Network_Player> playerlist;

    public Network_Player player;
    public NetworkManager_Drug networkManager;
    public static Network_Transmitter transmitter;

    void Awake()
    {
        transmitter = this;
    }

    public void startClient()
    {
        if(isServer)
        {

        }
        else
        {
            RpcsendEvent("Player " + " has joined the lobby!");
            
        }
    }

    // Start is called before the first frame update
    void Start()
    {
      //  networkManager = GetComponent<NetworkManager_Drug>();

        
        // Generate seed for the map
    }


    public string[] getPlayerlist()
    {
        string[] usernames = new string[playerlist.Count];
        int i = 0;
        foreach (Network_Player player in playerlist)
        {
            usernames[i] = player.getUsername();
        }

        return usernames;
    }

    public int getPlayerCount()
    {
        return playerlist.Count;
    }

    public List<Network_Player> getPlayers()
    {
        return playerlist;
    }

    public Network_Player findPlayer(string username)
    {
        foreach (Network_Player player in playerlist)
        {
            if (username == player.getUsername())
            {



                return player;
            }
        }
        throw new MissingComponentException("Whoops.. player " + username + " not found"); // nasty! take care
    }



    ///<summary>
    ///Sends a text to all player without a given username
    ///</summary>
    [ClientRpc]
    public void RpcsendEvent(string text)
    {
        foreach (Network_Player player in playerlist)

        {
            player.SendChatMessage(text);
        }
    }

    public void BroadcastShot(Vector3 start, Vector3 end)
    {
        foreach (Network_Player ply in playerlist)
        {
            ply.receiveShot(start, end);
        }

        Debug.Log("Shot!");
    }

    ///<summary>
    ///Sends a text message to all players with your username
    ///</summary>
    [Command]
    public void CmdsendMessage(string text, string username)
    {
        foreach (Network_Player player in playerlist)

        {
            player.RpcSendChatMessage(username, text);
        }
    }


 
    public void registerPlayer(Network_Player player)
    {
        // Welcome newbie !
        playerlist.Add(player);

        //Let's inform the party that you've landed

       // RpcsendEvent("Player " + player.getUsername() + " has joined the lobby!");
    }
}
