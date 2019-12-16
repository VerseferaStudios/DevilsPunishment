using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public  class Network_Disable_Component : NetworkBehaviour
{

    [SerializeField]
    public List<Behaviour> objects_to_disable = new List<Behaviour>();
    public List<GameObject> UIThings = new List<GameObject>();
    public GameObject Third_Person_Model;
    public GameObject chat;
    public GameObject InvView;

    // Disables components that should not be visible to other players (like UI and stats)
    void Start()
    {

        
        if (!isLocalPlayer)
        {
            Debug.Log("Player is not local!");
            

            foreach (GameObject behaviour in UIThings)
            {
                behaviour.SetActive(false);

            }

            foreach (Behaviour behaviour in objects_to_disable)
            {
                behaviour.enabled = false;
            }

            chat.GetComponent<CanvasGroup>().alpha = 1; // Shows chat
            InvView.SetActive(false);
            Third_Person_Model.SetActive(true);

        }
        else
        {
            InvView.SetActive(true);
          //  Third_Person_Model.SetActive(false);
        }

    }


}