using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Lobby;
using BeardedManStudios.Forge.Networking.Unity.Lobby;
using UnityEngine.SceneManagement;

public class DebugVariables : MonoBehaviour
{
    public string Username;
    public LobbyManager lobby;
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // called first
    void OnEnable()
    {
        Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Lobby")
        {
            lobby = GameObject.Find("LobbySystem").GetComponent<LobbyManager>();
            lobby.ChangeName(lobby.Myself, Username);
        }
        Debug.Log("OnSceneLoaded: " + scene.name);
        Debug.Log(mode);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
