using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager_Drug : NetworkBehaviour
{
    // Check to see if we're about to be destroyed.
    private static bool m_ShuttingDown = false;
    private static object m_Lock = new object();
    private static NetworkManager_Drug m_Instance;


    public List<Network_Player> playerlist = new List<Network_Player>();

    #region Singleton
    /// <summary>
    /// Access singleton instance through this propriety.
    /// </summary>
    public static NetworkManager_Drug instance
    {
        get
        {
            if (m_ShuttingDown)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(NetworkManager_Drug) +
                    "' already destroyed. Returning null.");
                return null;
            }

            lock (m_Lock)
            {
                if (m_Instance == null)
                {
                    // Search for existing instance.
                    m_Instance = (NetworkManager_Drug)FindObjectOfType(typeof(NetworkManager_Drug));

                    // Create new instance if one doesn't already exist.
                    if (m_Instance == null)
                    {
                        // Need to create a new GameObject to attach the singleton to.
                        var singletonObject = new GameObject();
                        m_Instance = singletonObject.AddComponent<NetworkManager_Drug>();
                        singletonObject.name = typeof(NetworkManager_Drug).ToString() + " (Singleton)";

                        // Make instance persistent.
                        DontDestroyOnLoad(singletonObject);
                    }
                }

                return m_Instance;
            }
        }
    }




    private void OnApplicationQuit()
    {
        m_ShuttingDown = true;
    }


    private void OnDestroy()
    {
        m_ShuttingDown = true;
    }

    #endregion



    
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
    public void sendEvent(string text)
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
    public void sendMessage(string text, string username)
    {
        foreach (Network_Player player in playerlist)

        {
            player.SendChatMessage(username, text);
        }
    }


    public void registerPlayer(Network_Player player)
    {
        // Welcome newbie !
        playerlist.Add(player);

        //Let's inform the party that you've landed

        sendEvent("Player " + player.getUsername() + " has joined the lobby!");
    }

}
