using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Network_Chat : MonoBehaviour
{
    // Start is called before the first frame update
    List<TextMeshProUGUI> chatMessages = new List<TextMeshProUGUI>();
    public GameObject message;
    public bool chat_enabled = true;
    public float currentHeight = 0;
    public float messageDistance;
    public int messages = 0;

    public void ActivateChat()
    {
        if (chat_enabled)
        {

   
            chatMessages.Add(message.GetComponent<TextMeshProUGUI>());
            messageDistance = message.GetComponent<RectTransform>().sizeDelta.y;
            currentHeight = transform.position.y; 

            messages = 2;
            addMessage("Hello");
            addMessage("World!");
        }
    }



    public void addMessage(string text, string username)
    {
        GameObject newMsg = Instantiate(message, transform);
        newMsg.GetComponent<TextMeshProUGUI>().SetText(string.Format("[{0}] {1} : {2} ", System.DateTime.Now.ToShortTimeString(), username, text));
        newMsg.transform.position = newMsg.transform.position + new Vector3(0, messageDistance * messages, 0);
        currentHeight = newMsg.transform.position.y;
        chatMessages.Add(message.GetComponent<TextMeshProUGUI>());

        messages++;




    }
    public void addMessage(string text)
    {
        GameObject newMsg = Instantiate(message, transform);
        newMsg.GetComponent<TextMeshProUGUI>().SetText(string.Format("[{0}] {1}", System.DateTime.Now.ToShortTimeString(), text));
        newMsg.transform.position = newMsg.transform.position + new Vector3(0, messageDistance * messages, 0);
        currentHeight = newMsg.transform.position.y;
        chatMessages.Add(message.GetComponent<TextMeshProUGUI>());

        messages++;
    }
}
