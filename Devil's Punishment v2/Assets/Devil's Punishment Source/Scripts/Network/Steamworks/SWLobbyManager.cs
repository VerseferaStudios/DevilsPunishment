using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.SceneManagement;

public class SWLobbyManager : MonoBehaviour {

    public string _PLAYSCENE = "Prototype Scene Steamworks";

    private string secret = "DEVILISINTHEDETAILS";

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
    private Callback<P2PSessionRequest_t> Callback_SessionRequest;


    private int numberOfPlayersInLobby = 0;

    private GameObject ServerInformationObject;

    ulong current_lobbyID;
    List<CSteamID> lobbyIDS;

    void Start () {
        ServerInformationObject = GameObject.Find("SteamServerInformation");
        ServerInformationObject.GetComponent<ServerInformation>().players = new PlayerInformation[8];
        for (int i = 0; i < 8; i++)
            ServerInformationObject.GetComponent<ServerInformation>().players[i].steamid = (CSteamID)0;

        lobbyIDS = new List<CSteamID>();
        Callback_lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        Callback_lobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbiesList);
        Callback_lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        Callback_lobbyInfo = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyInfo);
        Callback_ChatMsgRec = Callback<LobbyChatMsg_t>.Create(OnChatMsgRec);
        Callback_SessionRequest = Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequest);

        if (SteamAPI.Init())
            Debug.Log("Steam API init -- SUCCESS!");
        else
            Debug.Log("Steam API init -- failure ...");

    }

    //P2P communication//////
    void OnP2PSessionRequest(P2PSessionRequest_t request)
    {
        CSteamID clientID = request.m_steamIDRemote;
        for (int i = 0; i < ServerInformationObject.GetComponent<ServerInformation>().players.Length; i++)
        {
            if (current_lobbyID != 0)
                SteamNetworking.AcceptP2PSessionWithUser(SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)current_lobbyID, i));
        }
    }

    private void HandleMessage(string playMessage)
    {
        string playmessage = Md5Sum(secret + current_lobbyID + secret);
        Debug.Log("Message Received " + playMessage);
        Debug.Log("MD5 Secret " + playmessage);
        
        SteamMatchmaking.LeaveLobby((CSteamID)current_lobbyID);

        string SecretPlayMessage = Md5Sum(secret + current_lobbyID + secret);

        if (SecretPlayMessage == playmessage) //This will be changed to a more secure md5 hash
        {
            SceneManager.LoadScene(_PLAYSCENE);
        }
        
    }

    public void ButtonCreateLobby()
    {
        Debug.Log("Trying to create lobby ...");
        SteamAPICall_t try_toHost = SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 8);
        ToggleLobby(true);
        int i = 0;
        while(i < lobby.transform.childCount) //(Transform child in lobby.transform.GetChildCount)
        {
            if (lobby.transform.GetChild(i).name == "Start")
            {
                lobby.transform.GetChild(i).gameObject.SetActive(true);
                return;
            }
            i++;
        }
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

    public void ButtonStartGame()
    {
        Debug.Log("HOW MANY MEMBERS? - "+SteamMatchmaking.GetNumLobbyMembers((CSteamID)current_lobbyID));
        for (int i = 0; i < SteamMatchmaking.GetNumLobbyMembers((CSteamID)current_lobbyID); i++)
        {
            CSteamID receiver = SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)current_lobbyID, i);
            Debug.Log("SENDING TO - " + i + " : " + SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)current_lobbyID, i));
            string start = Md5Sum(secret + current_lobbyID + secret);

            Debug.Log("SENDING " + start);

            // allocate new bytes array and copy string characters as bytes
            byte[] bytes = new byte[start.Length * sizeof(char)];
            System.Buffer.BlockCopy(start.ToCharArray(), 0, bytes, 0, bytes.Length);

            SteamNetworking.SendP2PPacket(receiver, bytes, (uint)bytes.Length, EP2PSend.k_EP2PSendReliable);
        }
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
            UpdateServerInformationList();
        }
        // Command - List lobby members
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log(SteamMatchmaking.GetLobbyOwner((CSteamID)current_lobbyID));
        }
        if (Input.GetKeyDown(KeyCode.Return) && current_lobbyID != 0) {

            byte[] MsgBody = System.Text.Encoding.UTF8.GetBytes(chatInput.GetComponent<InputField>().text + char.MinValue);
            bool ret = SteamMatchmaking.SendLobbyChatMsg((CSteamID)current_lobbyID, MsgBody, MsgBody.Length+1);
            chatInput.GetComponent<InputField>().text = " ";
        }

        uint size;
        while (SteamNetworking.IsP2PPacketAvailable(out size))
        {
            Debug.Log("GOT A MESSAGE READY!!!");
            //allocate the buffer
            var buffer = new byte[size];
            uint bytesRead;
            CSteamID remoteid;
            //read message
            if (SteamNetworking.ReadP2PPacket(buffer, size, out bytesRead, out remoteid))
            {
                Debug.Log("GOT A MESSAGE!!!");
                char[] chars = new char[bytesRead / sizeof(char)];
                System.Buffer.BlockCopy(buffer, 0, chars, 0, buffer.Length);
                string message = new string(chars, 0, chars.Length);
                HandleMessage(message);
            }
        }
    }

    void UpdateServerInformationList()
    {
        int lobbynum = SteamMatchmaking.GetNumLobbyMembers((CSteamID)current_lobbyID);
        int i = 0;
        while (i < lobbynum)
        {
            ServerInformationObject.GetComponent<ServerInformation>().players[i].steamid = SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)current_lobbyID, i);
            i++;
        }
        ServerInformationObject.GetComponent<ServerInformation>().lobby = (CSteamID)current_lobbyID;
    }

    void OnChatMsgRec(LobbyChatMsg_t result)
    {

        CSteamID SteamIDUser;
        byte[] Data = new byte[4096];
        EChatEntryType ChatEntryType;
        SteamMatchmaking.GetLobbyChatEntry((CSteamID)result.m_ulSteamIDLobby, (int)result.m_iChatID, out SteamIDUser, Data, Data.Length, out ChatEntryType);
        byte[] dataResized = new byte[Data.Length];
        int ret = SteamMatchmaking.GetLobbyChatEntry((CSteamID)result.m_ulSteamIDLobby, (int)result.m_iChatID, out SteamIDUser, dataResized, dataResized.Length, out ChatEntryType);
        Debug.Log("GetLobbyChatEntry(" + (CSteamID)result.m_ulSteamIDLobby + ", " + (int)result.m_iChatID + ", out SteamIDUser, Data, Data.Length, out ChatEntryType) : ret " + ret + " --iduser " + SteamIDUser + " -- data" + System.Text.Encoding.UTF8.GetString(Data) + " --" + ChatEntryType);
        string message = Encoding.UTF8.GetString(Data) + "\0";
        if ((int)ChatEntryType == 1)
        {
            lobby.GetComponentInChildren<Text>().text = lobby.GetComponentInChildren<Text>().text + "\n" + SteamFriends.GetFriendPersonaName(SteamIDUser) + ":" + message;
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
            SteamMatchmaking.LeaveLobby((CSteamID)lobbyID);
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
            UpdateServerInformationList();         
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
            lobby.GetComponentInChildren<Text>().text = lobby.GetComponentInChildren<Text>().text + "\n" + "\t Player(" + i + "): " + SteamFriends.GetFriendPersonaName(SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)current_lobbyID, i));
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
    public string Md5Sum(string strToEncrypt)
    {
        UTF8Encoding ue = new UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);

        // encrypt bytes
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);

        // Convert the encrypted bytes back to a string (base 16)
        string hashString = "";

        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }

        return hashString.PadLeft(32, '0');
    }

}
