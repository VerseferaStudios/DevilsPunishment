using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameMenuUI : MonoBehaviour
{
    public static InGameMenuUI instance;
    public GameObject optionsPanel;
    Player viewPorts;
    
    void Awake() 
    {
        instance = this;
    }

    public void Btn_Continue()
    {
        viewPorts.ChangeCanvas();
        //Player.instance.ToggleGameMenu();
    }

    public void Btn_Options()
    {
        optionsPanel.SetActive(true);
        //Player.instance.ToggleOptionsMenu();
    }

    public void Btn_Exit()
    {
        Player.instance.ExitGame();
    }
}
