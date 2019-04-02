using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamWorksNetworkManager : MonoBehaviour
{
    public GameObject PlayerPrefab;
    /*
    // create a callback field. Having a field will make sure that the callback
    // handle won't be eaten by garbage collector.
    private Callback<P2PSessionRequest_t> _p2PSessionRequestCallback;

    void Start()
    {
        // setup the callback method
        _p2PSessionRequestCallback = Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequest);
    }

    void OnP2PSessionRequest(P2PSessionRequest_t request)
    {
       
        CSteamID clientId = request.m_steamIDRemote;
        if (ExpectingClient(clientId))
        {
            SteamNetworking.AcceptP2PSessionWithUser(clientId);
        }
        else
        {
            Debug.LogWarning("Unexpected session request from " + clientId);
        }
    }

    void Update() 
    {
        uint size;
 
        // repeat while there's a P2P message available
        // will write its size to size variable
        while (SteamNetworking.IsP2PPacketAvailable(out size))
        {
            // allocate buffer and needed variables
            var buffer = new byte[size];
            uint bytesRead;
            CSteamID remoteId;
 
            // read the message into the buffer
            if (SteamNetworking.ReadP2PPacket(buffer, size, out bytesRead, out remoteId))
            {
                // convert to string
                char[] chars = new char[bytesRead / sizeof(char)];
                Buffer.BlockCopy(buffer, 0, chars, 0, length);
 
                string message = new string(chars, 0, chars.Length);
                Debug.Log("Received a message: " + message);
            }
        }
    }*/
}
