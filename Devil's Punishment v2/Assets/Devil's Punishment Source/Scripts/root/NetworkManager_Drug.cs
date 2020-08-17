using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Mirror;
using static GameSetup;

public class NetworkManager_Drug : NetworkManager
{
    // Check to see if we're about to be destroyed.
    private static bool m_ShuttingDown = false;
    private static object m_Lock = new object();
    private static NetworkManager_Drug m_Instance;

    public GameObject startCamera;



    public TMP_InputField IPInputField;
    public Network_Transmitter transmitter;

    //Referenced by JoinGame Button
    public void joinIPGame()
    {
        base.networkAddress = IPInputField.text;
        Debug.Log(base.networkAddress);
       
        base.StartClient();


    }


    //Referenced by HostGame Button
    public void startHosting()
    {

        base.StartHost();

         // Generates level for host
        
    }


    /// <summary>
    /// Should only be there on server!
    /// </summary>
    public List<Transform> playerList = new List<Transform>();
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        //base.OnServerAddPlayer(conn);
        Transform startPos = GetStartPosition();
        GameObject player = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);

        playerList.Add(player.transform);

        NetworkServer.AddPlayerForConnection(conn, player);
    }

    ////[Server]
    ///// <summary>
    ///// Call Rpc for vent cover indices on all players, should be called on server only
    ///// </summary>
    ///// <param name="ventCoverIndices"></param>
    //public void CallRpcOnAllPlayers(int[] ventCoverIndices)
    //{
    //    for (int l = 0; l < playerList.Count; l++)
    //    {
    //        playerList[l].GetComponent<PlayerRemoteCallsBehaviour>().Rpc_VentCoverIndicies(ventCoverIndices);
    //    }
    //}




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




    public override void OnApplicationQuit()
    {
        m_ShuttingDown = true;
        base.OnApplicationQuit();
    }


    public override void OnDestroy()
    {
        base.OnDestroy();
        m_ShuttingDown = true;
    }

    #endregion



    
   

}
