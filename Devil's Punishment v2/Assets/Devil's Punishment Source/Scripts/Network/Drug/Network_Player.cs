using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Network_Player : NetworkBehaviour
{
    string username;
    GameObject player;


    PlayerController playerController;
    Transform playerTransform;
    Camera cam;
    Player playerMenu;
    Infection infection;
    Health health;
    public GameObject ShotRenderer;
    public Network_Chat chat;
    public GameObject shot;

    void Start()
    {

        username = SystemInfo.deviceName; // Using the computer name for now as username

        if (isServer)
        {
            GameSetup.setup.generateLevel(this);
        }
        else
        {
            Network_Transmitter.transmitter.startClient(this); // say hi we spawned°
        }
       
        
    }




    public string getUsername()
    {
        return username;
    }

    public GameObject bp; // bulletpoint
    [Command]
    public void Cmdbroadcast_Shots(Vector3 start, Quaternion rot)
    {
        GameObject shotRandy = Instantiate(bp);
     //   shotRandy.transform.position = start;
      //  shotRandy.transform.rotation = rot;


        NetworkServer.Spawn(shotRandy);
    }

    ///<summary>
    ///Returns the gameObject of the Player
    ///</summary>
    public GameObject GetPlayer()
    {
        return player;
    }

    ///<summary>
    ///Return the playerController which holds
    ///Input Processing and Animation Handling
    ///</summary>
    public PlayerController GetPlayerController()
    {
        return playerController;
    }

    ///<summary>
    ///Returns the camera of the player
    ///</summary>
    public Camera GetCamera()
    {
        return cam;
    }

    ///<summary>
    ///Returns the script that toggles interface visible
    ///</summary>
    public Player GetPlayerMenu()
    {
        return playerMenu;
    }

    ///<summary>
    ///Returns infection status and pill usage
    ///</summary>
    public Infection GetInfection()
    {
        return infection;
    }

    ///<summary>
    ///Returns the players wellbeeing
    ///</summary>
    public Health GetHealth()
    {
        return health;
    }


    [ClientRpc]
    public void RpcSendChatMessage( string text)
    {
        chat.addMessage(text);
       
    }

    public void receiveShot(Vector3 start, Vector3 end)
    {
        ShotRenderer.GetComponent<RegisterShot>().registerShot(start, end);
    }


    [ClientRpc]
    public void RpcSendChatUserMessage(string username, string text)
    {
        chat.addMessage(text, username);
    }


    

    void fetchPlayer()
    {
        cam = playerTransform.Find("Camera").GetComponent<Camera>();
        playerMenu = playerTransform.GetComponent<Player>();
        infection = playerTransform.GetComponent<Infection>();
        health = playerTransform.GetComponent<Health>();
        playerController = playerTransform.GetComponent<PlayerController>();




    }


    void Awake()
    {
        Debug.LogWarning("network player added");
        Network_Transmitter.transmitter.player = this;
        // tell Network Manager we're awake
        Network_Transmitter.transmitter.registerPlayer(this);

        // Display current players in chat
        chat.ActivateChat();

        



    }
}