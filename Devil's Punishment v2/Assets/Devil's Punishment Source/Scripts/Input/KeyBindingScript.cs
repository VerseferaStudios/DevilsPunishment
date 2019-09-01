//Author: David Bird
//Date: Saturday, August 6, 2019
    //Last Edited: Monday, August 26, 2019
        //By: David Bird with help from Chris aka Datak and DMGregory from gamedev stackexchange
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
    
    private Dictionary<string, InputBinding> keyBinds = new Dictionary<string, InputBinding>();
    public Text forward, backward, left, right, crouch, sprint, shoot, reload, aim, flashlightToggle, flashlightNarrow, flashlightWiden, flashlightUp, flashlightDown, flashlightHome, interact, pause, inventory;
    private GameObject curKey;
    private Color32 normal = new Color32(157, 125, 52, 255);
    private Color32 clicked = new Color32(241, 196, 92, 255);


    private void Start()
    {
        keyBinds.Add("forward", new InputBinding((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("forward", "W"))));
        keyBinds.Add("backward", new InputBinding((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("backward", "S"))));
        keyBinds.Add("left", new InputBinding((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("left", "A"))));
        keyBinds.Add("right", new InputBinding((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("right", "D"))));
        keyBinds.Add("crouch", new InputBinding((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("crouch", "LeftControl"))));
        keyBinds.Add("sprint", new InputBinding((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("sprint", "LeftShift"))));
        keyBinds.Add("shoot", new InputBinding((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("shoot", "Mouse0"))));
        keyBinds.Add("aim", new InputBinding((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("aim", "Mouse1"))));
        keyBinds.Add("reload", new InputBinding((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("reload", "R"))));
        keyBinds.Add("flashlightToggle", new InputBinding((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("flashlightToggle", "G"))));
        keyBinds.Add("flashlightNarrow", new InputBinding((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("flashlightNarrow", "RightArrow"))));
        keyBinds.Add("flashlightWiden", new InputBinding((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("flashlightWiden", "LeftArrow"))));
        keyBinds.Add("flashlightUp", new InputBinding((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("flashlightUp", "UpArrow"))));
        keyBinds.Add("flashlightDown", new InputBinding((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("flashlightDown", "DownArrow"))));
        keyBinds.Add("flashlightHome", new InputBinding((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("flashlightHome", "Q"))));
        keyBinds.Add("interact", new InputBinding((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("interact", "E"))));
        keyBinds.Add("inventory", new InputBinding((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("inventory", "Tab"))));
        keyBinds.Add("pause", new InputBinding((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("pause", "Escape"))));

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
        if (keyBinds["forward"].GetKeyDown())
        {
            Debug.Log("Forward " + "using " + Input.inputString.ToString());
        }
        if (keyBinds["backward"].GetKeyDown())
        {
            Debug.Log("Backward " + "using " + Input.inputString.ToString());
        }
        if (keyBinds["left"].GetKeyDown())
        {
            Debug.Log("Left " + "using " + Input.inputString.ToString());
        }
        if (keyBinds["right"].GetKeyDown())
        {
            Debug.Log("Right " + "using " + Input.inputString.ToString());
        }
        if (keyBinds["crouch"].GetKeyDown())
        {
            Debug.Log("Crouch " + "using " + Input.inputString.ToString());
        }
        if (keyBinds["sprint"].GetKeyDown())
        {
            Debug.Log("Sprint " + "using " + Input.inputString.ToString());
        }
        if (keyBinds["shoot"].GetKeyDown())
        {
            Debug.Log("Shoot " + "using " + Input.inputString.ToString());
        }
        if (keyBinds["aim"].GetKeyDown())
        {
            Debug.Log("Aim " + "using " + Input.inputString.ToString());
        }
        if (keyBinds["reload"].GetKeyDown())
        {
            Debug.Log("Reload " + "using " + Input.inputString.ToString());
        }
        if (keyBinds["flashlightToggle"].GetKeyDown())
        {
            Debug.Log("Flashlight Toggle " + "using " + Input.inputString.ToString());
        }
        if (keyBinds["flashlightNarrow"].GetKeyDown())
        {
            Debug.Log("Flashlight Narrow " + "using " + Input.inputString.ToString());
        }
        if (keyBinds["flashlightWiden"].GetKeyDown())
        {
            Debug.Log("Flashlight Widen " + "using " + Input.inputString.ToString());
        }
        if (keyBinds["flashlightUp"].GetKeyDown())
        {
            Debug.Log("Flashlight Up " + "using " + Input.inputString.ToString());
        }
        if (keyBinds["flashlightDown"].GetKeyDown())
        {
            Debug.Log("Flashlight Down " + "using " + Input.inputString.ToString());
        }
        if (keyBinds["flashlightHome"].GetKeyDown())
        {
            Debug.Log("Flashlight Home " + "using " + Input.inputString.ToString());
        }
        if (keyBinds["interact"].GetKeyDown())
        {
            Debug.Log("Interact " + "using " + Input.inputString.ToString());
        }
        if (keyBinds["inventory"].GetKeyDown())
        {
            Debug.Log("Inventory " + "using " + Input.inputString.ToString());
        }
        if (keyBinds["pause"].GetKeyDown())
        {
            Debug.Log("Pause " + "using " + Input.inputString.ToString());
        }
    }

    InputBinding CaptureFirstInput()
    {

        Event f = Event.current;

        if (f.isKey && f.keyCode != KeyCode.None)
            return new InputBinding(f.keyCode);

        for (int i = 0; i < 3; i++)
            if (Input.GetMouseButtonDown(i))
                return new InputBinding(i);

        if (Input.mouseScrollDelta != Vector2.zero)
            return new InputBinding(Input.mouseScrollDelta);

        return null;
    }

    private void OnGUI()
    {
        if (curKey != null)
        {
            var input = CaptureFirstInput();
            if (input != null && input.type != BindingType.None)
            {
                keyBinds[curKey.name] = input;
                curKey.transform.GetChild(0).GetComponent<Text>().text = input.ToString();
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