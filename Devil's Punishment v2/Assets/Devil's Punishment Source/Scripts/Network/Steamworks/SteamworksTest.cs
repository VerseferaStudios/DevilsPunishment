using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
public class SteamworksTest : MonoBehaviour
{
    CSteamID receiver = (CSteamID)480; // your steam id for test
    private Steamworks.Callback<P2PSessionRequest_t> _p2PSessionRequestCallback;
    
    void Start()
    {
        // setup the callback method
        _p2PSessionRequestCallback = Steamworks.Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequest);
    }

    void OnP2PSessionRequest(P2PSessionRequest_t request)
    {
        CSteamID clientId = request.m_steamIDRemote;
        SteamNetworking.AcceptP2PSessionWithUser(receiver);
    }
    uint size;
    string comand;
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * Time.deltaTime);
            Send("A");
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * Time.deltaTime);
            Send("D");
        }

        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            Send("");
        }

        ///read message
        if (comand == "A") {
            transform.Translate(Vector3.left * Time.deltaTime);
        }

        if (comand == "D") {
            transform.Translate(Vector3.right * Time.deltaTime);
        }
        // repeat while there’s a P2P message available
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
                //Buffer.BlockCopy(buffer, 0, chars, 0, chars.Length);
                System.Buffer.BlockCopy(buffer, 0, chars, 0, buffer.Length);
                string message = new string(chars, 0, chars.Length);
                //Debug.Log("Received a message: " + message);
                CheckMessage(message);
            }
        }
    }
    void Send(string message)
    {
        // allocate new bytes array and copy string characters as bytes
        byte[] bytes = new byte[message.Length * sizeof(char)];
        System.Buffer.BlockCopy(message.ToCharArray(), 0, bytes, 0, bytes.Length);

        SteamNetworking.SendP2PPacket((CSteamID)480, bytes, (uint)bytes.Length, EP2PSend.k_EP2PSendReliable);
    }
    void CheckMessage(string mess)
    {
        if (mess == "A" || mess == "D")
            comand = mess;
        else
            comand = "";
    }
}
