﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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

    public Network_Player(GameObject playerO)
    {
        username = SystemInfo.deviceName; // Using the computer name for now as username
        player = playerO;
        playerTransform = playerO.transform;

        fetchPlayer(); // Go on to fetch single assets that might (or might not) exist on the player
    }

    public string getUsername()
    {
        return username;
    }

    [Command]
    public void Cmdbroadcast_Shots(Vector3 start, Vector3 end)
    {
        GameObject shotRandy = Instantiate(ShotRenderer);
        shotRandy.GetComponent<LineRenderer>().SetPosition(0, start);
        shotRandy.GetComponent<LineRenderer>().SetPosition(1, end);


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

    public void SendChatMessage(string text)
    {
        chat.addMessage(text);
       
    }

    public void receiveShot(Vector3 start, Vector3 end)
    {
        ShotRenderer.GetComponent<RegisterShot>().registerShot(start, end);
    }

    public void SendChatMessage(string username, string text)
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

        // tell Network Manager we're awake
        NetworkManager_Drug.instance.registerPlayer(this);

        // Display current players in chat
        chat.ActivateChat();

        



    }
}