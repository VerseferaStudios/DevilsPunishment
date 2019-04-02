using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class SWLobbyManager : MonoBehaviour {

    public GameObject mainMenu;
    public GameObject LobbyList;

    public GameObject lobby;

    public GameObject lobbyAreaContent;
    public GameObject uiLobbyItem;

    public GameObject lobbyPlayerPrefab;
    public GameObject lobbyPlayerAreaContent;

    public GameObject chatInput;

    protected Callback<LobbyCreated_t> Callback_lobbyCreated;
    protected Callback<LobbyMatchList_t> Callback_lobbyList;
    protected Callback<LobbyEnter_t> Callback_lobbyEnter;
    protected Callback<LobbyDataUpdate_t> Callback_lobbyInfo;
    protected Callback<LobbyChatMsg_t> Callback_ChatMsgRec;

    private int numberOfPlayersInLobby = 0;

    ulong current_lobbyID;
    List<CSteamID> lobbyIDS;

    void Start () {

        lobbyIDS = new List<CSteamID>();
        Callback_lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        Callback_lobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbiesList);
        Callback_lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        Callback_lobbyInfo = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyInfo);
        Callback_ChatMsgRec = Callback<LobbyChatMsg_t>.Create(OnChatMsgRec);

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
        if(numberOfPlayersInLobby < SteamMatchmaking.GetNumLobbyMembers((CSteamID)current_lobbyID))
        {
            int numPlayers = SteamMatchmaking.GetNumLobbyMembers((CSteamID)current_lobbyID);
            MakePlayerLobbyItems(numPlayers);
        }
        // Command - List lobby members
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log(SteamMatchmaking.GetLobbyOwner((CSteamID)current_lobbyID));
        }
        if (Input.GetKeyDown(KeyCode.Return) && current_lobbyID != 0) {
            string chattext = chatInput.GetComponent<Text>().text;
            SteamMatchmaking.SendLobbyChatMsg((CSteamID)current_lobbyID, Encoding.ASCII.GetBytes(chattext), chattext.Length + 1);
        }
    }

    void OnChatMsgRec(LobbyChatMsg_t result)
    {
        lobby.GetComponentInChildren<Text>().text = lobby.GetComponentInChildren<Text>().text + "\n" + "\t" + result.m_ulSteamIDUser + ":" + result.m_iChatID;
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

            ////////////////////////
            //////GET THE AVATAR////
            ////////////////////////
            Texture2D m_MediumAvatarTexture = GetSteamImageAsTexture2D(SteamFriends.GetLargeFriendAvatar(myownID));
            //Rect rec = new Rect(0, 0, m_MediumAvatarTexture.width, m_MediumAvatarTexture.height);
            lobby.GetComponentInChildren<RawImage>().texture = m_MediumAvatarTexture;

            int numPlayers = SteamMatchmaking.GetNumLobbyMembers((CSteamID)current_lobbyID);
            lobby.GetComponentInChildren<Text>().text = "\n"+ "\t Number of players currently in lobby : " + numPlayers;
            MakePlayerLobbyItems(numPlayers);           
        }
        else
        {
            Debug.Log("Failed to join lobby.");
        }
    }

    public void MakePlayerLobbyItems(int numPlayers)
    {
        for (int i = numberOfPlayersInLobby; i < numPlayers; i++)
        {
            Texture2D m_smallavatar = GetSteamImageAsTexture2D(SteamFriends.GetLargeFriendAvatar(SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)current_lobbyID, i)));
            GameObject newPlayer = Instantiate(lobbyPlayerPrefab) as GameObject;
            newPlayer.transform.SetParent(lobbyPlayerAreaContent.transform, false);
            newPlayer.GetComponentInChildren<Text>().text = SteamFriends.GetFriendPersonaName(SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)current_lobbyID, i));
            newPlayer.GetComponentInChildren<RawImage>().texture = m_smallavatar;
            lobby.GetComponentInChildren<Text>().text = lobby.GetComponentInChildren<Text>().text + "\n" + "\t Player(" + i + ") == " + SteamFriends.GetFriendPersonaName(SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)current_lobbyID, i));
        }
        numberOfPlayersInLobby = numPlayers;
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
