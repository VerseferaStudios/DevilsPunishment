using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Text;
using System.Text.RegularExpressions;

namespace BeardedManStudios.Forge.Networking.Unity.Lobby
{
	public class LobbyPlayerItem : MonoBehaviour
	{
		public Color[] TeamColors;
		public Color[] AvatarColors;
		public GameObject KickButton;
		public Image AvatarBG;
		public Text AvatarID;
        public string PlayerName;
		public Text PlayerTeamID;

        public Button[] Buttons;

		[HideInInspector]
		public Transform ThisTransform;

		[HideInInspector]
		public GameObject ThisGameObject;

		public LobbyPlayer AssociatedPlayer { get; private set; }
        [HideInInspector]
        public LobbyManager _manager;
/// <summary>
/// ///////////////////////////
/// </summary>
        public string username = "Matt";
        public string password = "Matt";
        public string LoginURL = "http://crystalclearsecurity.com/DevilPunishment/login.php";

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
                StartCoroutine(NetworkConnect());
                Debug.Log("LOADDDDEEEDDD");
                //RequestChangeName(PlayerName);
            }
            Debug.Log("OnSceneLoaded: " + scene.name);
            Debug.Log(mode);
        }

        IEnumerator NetworkConnect()
        {
            Debug.Log("Starting Form");
            WWWForm form = new WWWForm();
            form.AddField("username", username);
            form.AddField("password", password);
            Debug.Log("Requesting");
            UnityWebRequest www = UnityWebRequest.Post(LoginURL, form);
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("POST successful!");
                /*StringBuilder sb = new StringBuilder();
                foreach (System.Collections.Generic.KeyValuePair<string, string> dict in www.GetResponseHeaders())
                {
                    sb.Append(dict.Key).Append(": \t[").Append(dict.Value).Append("]\n");
                }

                Debug.Log(sb.ToString());*/

                Debug.Log(www.downloadHandler.text);
                string[] tempPlayerName = Regex.Split(www.downloadHandler.text, "\r\n|\r|\n");

                //string tempPlayerName = Regex.Replace(www.downloadHandler.text, @"[^0-9a-zA-Z:,]+", "");
                RequestChangeName(tempPlayerName);
            }

        }

/// <summary>
/// ////////////////
/// </summary>
/// <param name="manager"></param>
        public void Init(LobbyManager manager)
		{
			ThisGameObject = gameObject;
			ThisTransform = transform;
			_manager = manager;
		}

		public void Setup(LobbyPlayer associatedPlayer, bool interactableValue)
		{
			ToggleInteractables(interactableValue);
			AssociatedPlayer = associatedPlayer;
            ChangeAvatarID(associatedPlayer.AvatarID);
            ChangeName(associatedPlayer.Name);
            ChangeTeam(associatedPlayer.TeamID);
		}

		public void SetParent(Transform parent)
		{
			ThisTransform.SetParent(parent);
			ThisTransform.localPosition = Vector3.zero;
			ThisTransform.localScale = Vector3.one;
		}

		public void KickPlayer()
		{
			_manager.KickPlayer(this);
		}
		
		public void RequestChangeTeam()
		{
			int nextID = AssociatedPlayer.TeamID + 1;
			if (nextID >= TeamColors.Length)
				nextID = 0;

			_manager.ChangeTeam(this, nextID);
		}

		public void RequestChangeAvatarID()
		{
			int nextID = AssociatedPlayer.AvatarID + 1;
			if (nextID >= AvatarColors.Length)
				nextID = 0;

			_manager.ChangeAvatarID(this, nextID);
		}

		public void RequestChangeName(string username)
		{
			_manager.ChangeName(this, username);
		}

		public void ChangeAvatarID(int id)
		{
			Color avatarColor = Color.white;

			//Note: This is just an example, you are free to make your own team colors and
			// change this to however you see fit
			if (TeamColors.Length > id && id >= 0)
				avatarColor = AvatarColors[id];

			AvatarID.text = id.ToString();
			AvatarBG.color = avatarColor;
		}

		public void ChangeName(string name)
		{
			PlayerName = name;
		}

		public void ChangeTeam(int id)
		{
			PlayerTeamID.text = string.Format("Team {0}", id);
		}

		public void ToggleInteractables(bool value)
		{
            for (int i = 0; i < Buttons.Length; ++i)
                Buttons[i].interactable = value;

            AvatarBG.raycastTarget = value;
			PlayerTeamID.raycastTarget = value;
		}

		public void ToggleObject(bool value)
		{
			ThisGameObject.SetActive(value);
		}
	}
}