using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using System.Collections;
using System.Collections.Generic;

public class SWLobbyManager : MonoBehaviour {

    public GameObject mainMenu;
    public GameObject LobbyList;

    public GameObject lobby;

    public GameObject lobbyAreaContent;
    public GameObject uiLobbyItem;

    protected Callback<LobbyCreated_t> Callback_lobbyCreated;
    protected Callback<LobbyMatchList_t> Callback_lobbyList;
    protected Callback<LobbyEnter_t> Callback_lobbyEnter;
    protected Callback<LobbyDataUpdate_t> Callback_lobbyInfo;

    ulong current_lobbyID;
    List<CSteamID> lobbyIDS;

    void Start () {

        lobbyIDS = new List<CSteamID>();
        Callback_lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        Callback_lobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbiesList);
        Callback_lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        Callback_lobbyInfo = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyInfo);

        if (SteamAPI.Init())
            Debug.Log("Steam API init -- SUCCESS!");
        else
            Debug.Log("Steam API init -- failure ...");
    }

    public void ButtonCreateLobby()
    {
        Debug.Log("Trying to create lobby ...");
        SteamAPICall_t try_toHost = SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 8);
    }

    public void ButtonShowLobbyList()
    {
        Debug.Log("Trying to get list of available lobbies ...");

        SteamAPICall_t try_getList = SteamMatchmaking.RequestLobbyList();
        ToggleMainMenu(false);
        ToggleLobbyList(true);

    }

    public void ButtonJoinLobby(int i)
    {
        ToggleLobbyList(false);
        ToggleLobby(true);
        SteamAPICall_t try_joinLobby = SteamMatchmaking.JoinLobby(SteamMatchmaking.GetLobbyByIndex(i));
    }



    private void ToggleMainMenu(bool B)
    {
        mainMenu.SetActive(B);
    }

    private void ToggleLobby(bool B)
    {
        lobby.SetActive(B);
    }

    private void ToggleLobbyList(bool B)
    {
        LobbyList.SetActive(B);
    }

    void Update()
    {
        SteamAPI.RunCallbacks();


        // Command - Join lobby at index 0 (testing purposes)
        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("Trying to join FIRST listed lobby ...");

        }

        // Command - List lobby members
        if (Input.GetKeyDown(KeyCode.Q))
        {
            int numPlayers = SteamMatchmaking.GetNumLobbyMembers((CSteamID)current_lobbyID);

            Debug.Log("\t Number of players currently in lobby : " + numPlayers);
            for (int i = 0; i < numPlayers; i++)
            {
                Debug.Log("\t Player(" + i + ") == " + SteamFriends.GetFriendPersonaName(SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)current_lobbyID, i)));
            }
        }
    }

    void OnLobbyCreated(LobbyCreated_t result)
    {
        if (result.m_eResult == EResult.k_EResultOK)
            Debug.Log("Lobby created -- SUCCESS!");
        else
            Debug.Log("Lobby created -- failure ...");

        string personalName = SteamFriends.GetPersonaName();
        SteamMatchmaking.SetLobbyData((CSteamID)result.m_ulSteamIDLobby, "name", personalName+"'s game");
    }

    void OnGetLobbiesList(LobbyMatchList_t result)
    {

        Debug.Log("Found " + result.m_nLobbiesMatching + " lobbies!");
        for(int i=0; i< result.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            lobbyIDS.Add(lobbyID);
            SteamMatchmaking.RequestLobbyData(lobbyID);
        }
    }

    void OnGetLobbyInfo(LobbyDataUpdate_t result)
    {
        for(int i=0; i<lobbyIDS.Count; i++)
        {
            if (lobbyIDS[i].m_SteamID == result.m_ulSteamIDLobby)
            {
                GameObject newLobby = Instantiate(uiLobbyItem) as GameObject;
                newLobby.transform.SetParent(lobbyAreaContent.transform, false);
                //newLobby.transform.localPosition = new Vector3(newLobby.transform.localPosition.x, -21 - (i * 40) ,newLobby.transform.localPosition.z);

                newLobby.GetComponentInChildren<Text>().text = "Lobby " + i + " | Lobby Name: " + SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDS[i].m_SteamID, "name") + " | Players: "+ SteamMatchmaking.GetNumLobbyMembers((CSteamID)lobbyIDS[i]) + "/4";
                newLobby.GetComponentInChildren<Button>().onClick.AddListener(() => ButtonJoinLobby(i));
                Debug.Log("Lobby " + i+" :: " +SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDS[i].m_SteamID, "name"));
                return;
            }
        }
    }

    void OnLobbyEntered(LobbyEnter_t result)
    {
        current_lobbyID = result.m_ulSteamIDLobby;

        if (result.m_EChatRoomEnterResponse == 1)
        {
            Debug.Log("Lobby joined!");
            CSteamID myownID = SteamUser.GetSteamID();

            Texture2D m_MediumAvatarTexture = GetSteamImageAsTexture2D(SteamFriends.GetLargeFriendAvatar(myownID));
            //Rect rec = new Rect(0, 0, m_MediumAvatarTexture.width, m_MediumAvatarTexture.height);
            lobby.GetComponentInChildren<RawImage>().texture = m_MediumAvatarTexture;
        }
        else
        {
            Debug.Log("Failed to join lobby.");
        }
    }

    public static Texture2D GetSteamImageAsTexture2D(int iImage)
    {
        Texture2D ret = null;
        uint ImageWidth;
        uint ImageHeight;
        bool bIsValid = SteamUtils.GetImageSize(iImage, out ImageWidth, out ImageHeight);

        if (bIsValid)
        {
            byte[] Image = new byte[ImageWidth * ImageHeight * 4];

            bIsValid = SteamUtils.GetImageRGBA(iImage, Image, (int)(ImageWidth * ImageHeight * 4));
            if (bIsValid)
            {
                ret = new Texture2D((int)ImageWidth, (int)ImageHeight, TextureFormat.RGBA32, false, true);
                ret.LoadRawTextureData(Image);
                ret.Apply();
            }
        }

        return ret;
    }

}
