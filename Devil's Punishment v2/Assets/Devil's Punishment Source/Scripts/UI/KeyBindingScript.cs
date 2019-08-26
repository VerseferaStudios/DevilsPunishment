//Author: David Bird
//Date: Saturday, August 6, 2019
    //Last Edited: Monday, August 26, 2019
        //By: David Bird
            //Purpose: Finish the script/clean up the scripts readability.
//Written For: Devil's Punishment v2
//Purpose: This script is part 2 of 2 part script (Game Manager Script)to contol the players ability to remap the keybinds.
//Notes: Further information can be found at http://www.nolocationyet.com (Dead link)


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class KeyBindingScript : MonoBehaviour
{
    private Dictionary<string, KeyCode> keyBinds = new Dictionary<string, KeyCode>();
    public Text forward, backward, left, right, crouch, sprint, shoot, reload, aim, flashlightToggle, flashlightNarrow, flashlightWiden, flashlightUp, flashlightDown, flashlightHome, interact, pause, inventory;
    private GameObject curKey;
    private Color32 normal = new Color32(157, 125, 52, 255);
    private Color32 clicked = new Color32(241, 196, 92, 255);


    private void Start()
    {
        keyBinds.Add("forward", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("forward", "W")));
        keyBinds.Add("backward", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("backward", "S")));
        keyBinds.Add("left", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("left", "A")));
        keyBinds.Add("right", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("right", "D")));
        keyBinds.Add("crouch", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("crouch", "LeftControl")));
        keyBinds.Add("sprint", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("sprint", "LeftShift")));
        keyBinds.Add("shoot", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("shoot", "Mouse0")));
        keyBinds.Add("aim", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("aim", "Mouse1")));
        keyBinds.Add("reload", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("reload", "R")));
        keyBinds.Add("flashlightToggle", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("flashlightToggle", "G")));
        keyBinds.Add("flashlightNarrow", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("flashlightNarrow", "RightArrow")));
        keyBinds.Add("flashlightWiden", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("flashlightWiden", "LeftArrow")));
        keyBinds.Add("flashlightUp", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("flashlightUp", "UpArrow")));
        keyBinds.Add("flashlightDown", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("flashlightDown", "DownArrow")));
        keyBinds.Add("flashlightHome", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("flashlightHome", "Q")));
        keyBinds.Add("interact", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("interact", "E")));
        keyBinds.Add("inventory", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("inventory", "Tab")));
        keyBinds.Add("pause", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("pause", "Escape")));

        forward.text = keyBinds["forward"].ToString();
        backward.text = keyBinds["backward"].ToString();
        left.text = keyBinds["left"].ToString();
        right.text = keyBinds["right"].ToString();
        crouch.text = keyBinds["crouch"].ToString();
        sprint.text = keyBinds["sprint"].ToString();
        shoot.text = keyBinds["shoot"].ToString();
        aim.text = keyBinds["aim"].ToString();
        reload.text = keyBinds["reload"].ToString();
        flashlightToggle.text = keyBinds["flashlightToggle"].ToString();
        flashlightNarrow.text = keyBinds["flashlightNarrow"].ToString();
        flashlightWiden.text = keyBinds["flashlightWiden"].ToString();
        flashlightUp.text = keyBinds["flashlightUp"].ToString();
        flashlightDown.text = keyBinds["flashlightDown"].ToString();
        flashlightHome.text = keyBinds["flashlightHome"].ToString();
        pause.text = keyBinds["pause"].ToString();
        interact.text = keyBinds["interact"].ToString();
        inventory.text = keyBinds["inventory"].ToString();
    }


    public void Update()
    {
        if (Input.GetKeyDown(keyBinds["forward"]))
        {
            Debug.Log("Forward " + "using " + Input.inputString.ToString());
        }
        if (Input.GetKeyDown(keyBinds["backward"]))
        {
            Debug.Log("Backward " + "using " + Input.inputString.ToString());
        }
        if (Input.GetKeyDown(keyBinds["left"]))
        {
            Debug.Log("Left " + "using " + Input.inputString.ToString());
        }
        if (Input.GetKeyDown(keyBinds["right"]))
        {
            Debug.Log("Right " + "using " + Input.inputString.ToString());
        }
        if (Input.GetKeyDown(keyBinds["crouch"]))
        {
            Debug.Log("Crouch " + "using " + Input.inputString.ToString());
        }
        if (Input.GetKeyDown(keyBinds["sprint"]))
        {
            Debug.Log("Sprint " + "using " + Input.inputString.ToString());
        }
        if (Input.GetKeyDown(keyBinds["shoot"]))
        {
            Debug.Log("Shoot " + "using " + Input.inputString.ToString());
        }
        if (Input.GetKeyDown(keyBinds["aim"]))
        {
            Debug.Log("Aim " + "using " + Input.inputString.ToString());
        }
        if (Input.GetKeyDown(keyBinds["reload"]))
        {
            Debug.Log("Reload " + "using " + Input.inputString.ToString());
        }
        if (Input.GetKeyDown(keyBinds["flashlightToggle"]))
        {
            Debug.Log("Flashlight Toggle " + "using " + Input.inputString.ToString());
        }
        if (Input.GetKeyDown(keyBinds["flashlightNarrow"]))
        {
            Debug.Log("Flashlight Narrow " + "using " + Input.inputString.ToString());
        }
        if (Input.GetKeyDown(keyBinds["flashlightWiden"]))
        {
            Debug.Log("Flashlight Widen " + "using " + Input.inputString.ToString());
        }
        if (Input.GetKeyDown(keyBinds["flashlightUp"]))
        {
            Debug.Log("Flashlight Up " + "using " + Input.inputString.ToString());
        }
        if (Input.GetKeyDown(keyBinds["flashlightDown"]))
        {
            Debug.Log("Flashlight Down " + "using " + Input.inputString.ToString());
        }
        if (Input.GetKeyDown(keyBinds["flashlightHome"]))
        {
            Debug.Log("Flashlight Home " + "using " + Input.inputString.ToString());
        }
        if (Input.GetKeyDown(keyBinds["interact"]))
        {
            Debug.Log("Interact " + "using " + Input.inputString.ToString());
        }
        if (Input.GetKeyDown(keyBinds["inventory"]))
        {
            Debug.Log("Inventory " + "using " + Input.inputString.ToString());
        }
        if (Input.GetKeyDown(keyBinds["pause"]))
        {
            Debug.Log("Pause " + "using " + Input.inputString.ToString());
        }
    }

    private void OnGUI()
    {
        if (curKey != null)
        {
            Event e = Event.current;
            if (e.isKey)
            {
                keyBinds[curKey.name] = e.keyCode;
                curKey.transform.GetChild(0).GetComponent<Text>().text = e.keyCode.ToString();
                Debug.Log("Key has been changed.");
                curKey.GetComponent<Image>().color = normal;
                curKey = null;
            }
        }
    }

    public void KeyChange(GameObject click)
    {
        if (curKey != null)
        {
            curKey.GetComponent<Image>().color = normal;
        }
        curKey = click;
        curKey.GetComponent<Image>().color = clicked;
    }

    public void Save()
    {
        foreach (var key in keyBinds)
        {
            PlayerPrefs.SetString(key.Key, key.Value.ToString());
        }
        PlayerPrefs.Save();
        Debug.Log("Settings have been saved.");
    }

    public void ResetDefault()
    {
        keyBinds.Clear();
        Start();
    }
}