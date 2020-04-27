using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Network_Transmitter : NetworkBehaviour
{


    public List<Network_Player> playerlist;

    public Network_Player player;
    public NetworkManager_Drug networkManager;
    public MapGen3 mapGen;
    public static Network_Transmitter transmitter;
    public GameState gS;

    [SyncVar]
    private int mapSeed = 135;


    bool genOnce = false;
    
    public void startOnlineGeneration()
    {   
        if (isServer && !genOnce)
        {
            
           // mapSeed = Random.Range(0, 1000);
            player.RpcSendChatMessage(mapSeed.ToString()+  " We generated");          
            genOnce = true;
            mapGen.startMapGeneration(mapSeed);
            gS.StartGame();    

        }
        else
        {
            mapGen.startMapGeneration(mapSeed);
          //  player.RpcSendChatMessage(mapSeed.ToString()+ " We fetched from server");
            gS.StartGame();
        }

        

    }

    void Awake()
    {
        transmitter = this;

    }

    public int getSeed()
    {
        return mapSeed;
    }

    


    public void startClient(Network_Player ply)
    {
        player = ply;
        Debug.Log("STARTING CLIENT");
        if(isServer)
        {
            player.RpcSendChatMessage("Host started hosting!"); // it's us haha
        }
        else
        {
            CmdsendMessage("Player " + " has joined the lobby!","client ");
            
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

    [Command]
    private void CmdinformHost(string msg)
    {
      // player.RpcSendChatMessage(msg); // host is a lone wanderer
        RpcsendEvent(msg); // Nice that they told me ! Better tell the clients too now
    }

    
    public void sendNetworkMessage(string text, string username)
    {
        string msg = username + " : " + text;
        if (isServer)
        {
            print("Server sends!");

          
            //Since we're the server let's tell the others what we have to say!
            RpcsendEvent(msg);
            
        }
        else
        {
            print("Client sends!");
            //We're a client!
            // Go tell the host waht we have to say!

            CmdsendMessage(text, username);
            

        }
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
        player.RpcSendChatMessage(text);
        
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
        
       player.RpcSendChatUserMessage(username, text);
        
    }


 
    public void registerPlayer(Network_Player player)
    {
        // Welcome newbie !
        playerlist.Add(player);

        //Let's inform the party that you've landed

       // RpcsendEvent("Player " + player.getUsername() + " has joined the lobby!");
    }
}
