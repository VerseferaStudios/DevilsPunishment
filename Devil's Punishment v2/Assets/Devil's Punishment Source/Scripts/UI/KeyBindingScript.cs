

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class KeyBindingScript : MonoBehaviour
{
    private Dictionary<string, KeyCode> keyBinds = new Dictionary<string, KeyCode>();
    private Dictionary<string, KeyCode> defaultBinds = new Dictionary<string, KeyCode>();
    public Text forward, backward, left, right, crouch, sprint, shoot, reload, aim, flashlightToggle, flashlightNarrow, flashlightWiden, flashlightUp, flashlightDown, flashlightHome, interact, pause, inventory;
    public Text forward2, backward2, left2, right2, crouch2, sprint2, shoot2, reload2, aim2, flashlightToggle2, flashlightNarrow2, flashlightWiden2, flashlightUp2, flashlightDown2, flashlightHome2, interact2, pause2, inventory2;
    private GameObject curKey;
    private Color32 normal = new Color32(157, 125, 52, 255);
    private Color32 clicked = new Color32(241, 196, 92, 255);


    private void Start()
    {
        keyBinds.Add("forward", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("forward", "W")));/*)KeyCode.W);*/
        keyBinds.Add("backward", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("backward", "S")));/* KeyCode.S);*/
        keyBinds.Add("left", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("left", "A")));/* KeyCode.A);*/
        keyBinds.Add("right", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("right", "D")));/* KeyCode.D);*/
        keyBinds.Add("crouch", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("crouch", "LeftControl")));/* KeyCode.LeftControl);*/
        keyBinds.Add("sprint", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("sprint", "LeftShift")));/* KeyCode.LeftShift);*/
        keyBinds.Add("shoot", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("shoot", "Mouse0")));/* KeyCode.Mouse0);*/
        keyBinds.Add("aim", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("aim", "Mouse1")));/* KeyCode.Mouse1);*/
        keyBinds.Add("reload", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("reload", "R")));/* KeyCode.R);*/
        keyBinds.Add("flashlightToggle", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("flashlightToggle", "G")));/* KeyCode.G);*/
        keyBinds.Add("flashlightNarrow", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("flashlightNarrow", "RightArrow"))); /*KeyCode.RightArrow);*/
        keyBinds.Add("flashlightWiden", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("flashlightWiden", "LeftArrow")));/* KeyCode.LeftArrow);*/
        keyBinds.Add("flashlightUp", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("flashlightUp", "UpArrow")));/* KeyCode.UpArrow);*/
        keyBinds.Add("flashlightDown", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("flashlightDown", "DownArrow")));/* KeyCode.DownArrow);*/
        keyBinds.Add("flashlightHome", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("flashlightHome", "Q")));/* KeyCode.Q);*/
        keyBinds.Add("interact", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("interact", "E")));/* KeyCode.E);*/
        keyBinds.Add("inventory", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("inventory", "Tab")));/* KeyCode.Tab)*/;
        keyBinds.Add("pause", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("pause", "Escape")));/* KeyCode.Escape);*/

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
        if(curKey != null)
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
        foreach(var key in keyBinds)
        {
            PlayerPrefs.SetString(key.Key, key.Value.ToString());
        }
        PlayerPrefs.Save();
        Debug.Log("Settings have been saved.");
    }

    public void ResetDefault()
    {
        defaultBinds.Add("forward2", KeyCode.W);
        defaultBinds.Add("backward2", KeyCode.S);
        defaultBinds.Add("left2", KeyCode.A);
        defaultBinds.Add("right2", KeyCode.D);
        defaultBinds.Add("crouch2", KeyCode.LeftControl);
        defaultBinds.Add("sprint2", KeyCode.LeftShift);
        defaultBinds.Add("shoot2", KeyCode.Mouse0);
        defaultBinds.Add("aim2", KeyCode.Mouse1);
        defaultBinds.Add("reload2", KeyCode.R);
        defaultBinds.Add("flashlightToggle2", KeyCode.G);
        defaultBinds.Add("flashlightNarrow2", KeyCode.RightArrow);
        defaultBinds.Add("flashlightWiden2", KeyCode.LeftArrow);
        defaultBinds.Add("flashlightUp2", KeyCode.UpArrow);
        defaultBinds.Add("flashlightDown2", KeyCode.DownArrow);
        defaultBinds.Add("flashlightHome2", KeyCode.Q);
        defaultBinds.Add("interact2", KeyCode.E);
        defaultBinds.Add("inventory2", KeyCode.Tab);
        defaultBinds.Add("pause2", KeyCode.Escape);

        forward2.text = keyBinds["forward2"].ToString();
        backward2.text = keyBinds["backward2"].ToString();
        left2.text = keyBinds["left2"].ToString();
        right2.text = keyBinds["right2"].ToString();
        crouch2.text = keyBinds["crouch2"].ToString();
        sprint2.text = keyBinds["sprint2"].ToString();
        shoot2.text = keyBinds["shoot2"].ToString();
        aim2.text = keyBinds["aim2"].ToString();
        reload2.text = keyBinds["reload2"].ToString();
        flashlightToggle2.text = keyBinds["flashlightToggle2"].ToString();
        flashlightNarrow2.text = keyBinds["flashlightNarrow2"].ToString();
        flashlightWiden2.text = keyBinds["flashlightWiden2"].ToString();
        flashlightUp2.text = keyBinds["flashlightUp2"].ToString();
        flashlightDown2.text = keyBinds["flashlightDown2"].ToString();
        flashlightHome2.text = keyBinds["flashlightHome2"].ToString();
        pause2.text = keyBinds["pause2"].ToString();
        interact2.text = keyBinds["interact2"].ToString();
        inventory2.text = keyBinds["inventory2"].ToString();

        //Save();
    }













































    //Transform menuImage;
    //Event keyEvent;
    //Text buttonText;
    //KeyCode newKey;

    //private bool waitingForKey;

    //public void Start()
    //{
    //    menuImage = this.transform;
    //    //menuImage = this.transform.Find("Image.ButtonListContent").transform;
    //    //menuImage = transform.FindChild("Image.ButtonListContent");
    //    waitingForKey = false;

    //    for(int i = 0; i < 17; i++)
    //    {
    //        if(menuImage.GetChild(i).name == "Button.Forward")
    //        {
    //            menuImage.GetChild(i).GetComponentInChildren<Text>().text = GameManager.gm.forward.ToString();
    //        }
    //        else if (menuImage.GetChild(i).name == "Button.Back")
    //        {
    //            menuImage.GetChild(i).GetComponentInChildren<Text>().text = GameManager.gm.backward.ToString();
    //        }
    //        else if (menuImage.GetChild(i).name == "Button.Left")
    //        {
    //            menuImage.GetChild(i).GetComponentInChildren<Text>().text = GameManager.gm.left.ToString();
    //        }
    //        else if (menuImage.GetChild(i).name == "Button.Right")
    //        {
    //            menuImage.GetChild(i).GetComponentInChildren<Text>().text = GameManager.gm.right.ToString();
    //        }
    //        else if (menuImage.GetChild(i).name == "Button.Crouch")
    //        {
    //            menuImage.GetChild(i).GetComponentInChildren<Text>().text = GameManager.gm.crouch.ToString();
    //        }
    //        else if (menuImage.GetChild(i).name == "Button.Sprint")
    //        {
    //            menuImage.GetChild(i).GetComponentInChildren<Text>().text = GameManager.gm.sprint.ToString();
    //        }
    //        else if (menuImage.GetChild(i).name == "Button.Shoot")
    //        {
    //            menuImage.GetChild(i).GetComponentInChildren<Text>().text = GameManager.gm.shoot.ToString();
    //        }
    //        else if (menuImage.GetChild(i).name == "Button.Reload")
    //        {
    //            menuImage.GetChild(i).GetComponentInChildren<Text>().text = GameManager.gm.reload.ToString();
    //        }
    //        else if (menuImage.GetChild(i).name == "Button.FlashlightToggle")
    //        {
    //            menuImage.GetChild(i).GetComponentInChildren<Text>().text = GameManager.gm.flashlightToggle.ToString();
    //        }
    //        else if (menuImage.GetChild(i).name == "Button.FlashlightNarrow")
    //        {
    //            menuImage.GetChild(i).GetComponentInChildren<Text>().text = GameManager.gm.flashlightNarrow.ToString();
    //        }
    //        else if (menuImage.GetChild(i).name == "Button.FlashlightWiden")
    //        {
    //            menuImage.GetChild(i).GetComponentInChildren<Text>().text = GameManager.gm.flashlightWiden.ToString();
    //        }
    //        else if (menuImage.GetChild(i).name == "Button.FlashlightUp")
    //        {
    //            menuImage.GetChild(i).GetComponentInChildren<Text>().text = GameManager.gm.flashlightUp.ToString();
    //        }
    //        else if (menuImage.GetChild(i).name == "Button.FlashlightDown")
    //        {
    //            menuImage.GetChild(i).GetComponentInChildren<Text>().text = GameManager.gm.flashlightDown.ToString();
    //        }
    //        else if (menuImage.GetChild(i).name == "Button.FlashlightHome")
    //        {
    //            menuImage.GetChild(i).GetComponentInChildren<Text>().text = GameManager.gm.flashlightCenter.ToString();
    //        }
    //        else if (menuImage.GetChild(i).name == "Button.Pause")
    //        {
    //            menuImage.GetChild(i).GetComponentInChildren<Text>().text = GameManager.gm.pause.ToString();
    //        }
    //        else if (menuImage.GetChild(i).name == "Button.Inventory")
    //        {
    //            menuImage.GetChild(i).GetComponentInChildren<Text>().text = GameManager.gm.inventory.ToString();
    //        }
    //        else if (menuImage.GetChild(i).name == "Button.Interact")
    //        {
    //            menuImage.GetChild(i).GetComponentInChildren<Text>().text = GameManager.gm.interact.ToString();
    //        }
    //        else if (menuImage.GetChild(i).name == "Button.Aim")
    //        {
    //            menuImage.GetChild(i).GetComponentInChildren<Text>().text = GameManager.gm.aim.ToString();
    //        }
    //    }
    //}

    //void OnGUI()
    //{
    //    keyEvent = Event.current;

    //    if (keyEvent.isKey && waitingForKey == false)
    //    {
    //        newKey = keyEvent.keyCode;
    //        waitingForKey = false;
    //    }
    //}

    //public void StartKeyLocking(string keyName)
    //{
    //    if (!waitingForKey)
    //    {
    //        StartCoroutine(AssignKey(keyName));
    //    }
    //}

    //public void SendInfo(Text text)
    //{
    //    buttonText = text;
    //}

    //IEnumerator WaitForKey()
    //{
    //    while (!keyEvent.isKey)
    //    {
    //        yield return null;
    //    }
    //}

    //public IEnumerator AssignKey(string keyName)
    //{
    //    waitingForKey = true;
    //    yield return WaitForKey();
    //    //if (Input.inputString != "None")
    //    //{
    //    //    switch (keyName)
    //    //    {
    //    //        case "forward":
    //    //            GameManager.gm.forward = newKey;
    //    //            buttonText.text = GameManager.gm.forward.ToString();
    //    //            PlayerPrefs.SetString("Button.Forward", GameManager.gm.forward.ToString());
    //    //            Debug.Log(Input.inputString + " key was pressed");
    //    //            break;
    //    //        case "backward":
    //    //            GameManager.gm.backward = newKey;
    //    //            buttonText.text = GameManager.gm.backward.ToString();
    //    //            PlayerPrefs.SetString("Button.Back", GameManager.gm.backward.ToString());
    //    //            break;
    //    //        case "left":
    //    //            GameManager.gm.left = newKey;
    //    //            buttonText.text = GameManager.gm.left.ToString();
    //    //            PlayerPrefs.SetString("Button.Left", GameManager.gm.left.ToString());
    //    //            break;
    //    //        case "right":
    //    //            GameManager.gm.right = newKey;
    //    //            buttonText.text = GameManager.gm.right.ToString();
    //    //            PlayerPrefs.SetString("Button.Right", GameManager.gm.right.ToString());
    //    //            break;
    //    //        case "crouch":
    //    //            GameManager.gm.crouch = newKey;
    //    //            buttonText.text = GameManager.gm.crouch.ToString();
    //    //            PlayerPrefs.SetString("Button.Crouch", GameManager.gm.crouch.ToString());
    //    //            break;
    //    //        case "sprint":
    //    //            GameManager.gm.sprint = newKey;
    //    //            buttonText.text = GameManager.gm.sprint.ToString();
    //    //            PlayerPrefs.SetString("Button.Sprint", GameManager.gm.sprint.ToString());
    //    //            break;
    //    //        case "shoot":
    //    //            GameManager.gm.shoot = newKey;
    //    //            buttonText.text = GameManager.gm.sprint.ToString();
    //    //            PlayerPrefs.SetString("Button.Shoot", GameManager.gm.shoot.ToString());
    //    //            break;
    //    //        case "reload":
    //    //            GameManager.gm.reload = newKey;
    //    //            buttonText.text = GameManager.gm.reload.ToString();
    //    //            PlayerPrefs.SetString("Button.Reload", GameManager.gm.reload.ToString());
    //    //            break;
    //    //        case "flashlightToggle":
    //    //            GameManager.gm.flashlightToggle = newKey;
    //    //            buttonText.text = GameManager.gm.flashlightToggle.ToString();
    //    //            PlayerPrefs.SetString("Button.FlashlightToggle", GameManager.gm.flashlightToggle.ToString());
    //    //            break;
    //    //        case "flashlightNarrow":
    //    //            GameManager.gm.flashlightNarrow = newKey;
    //    //            buttonText.text = GameManager.gm.flashlightNarrow.ToString();
    //    //            PlayerPrefs.SetString("Button.FlashlightNarrow", GameManager.gm.flashlightNarrow.ToString());
    //    //            break;
    //    //        case "flashlightWiden":
    //    //            GameManager.gm.flashlightWiden = newKey;
    //    //            buttonText.text = GameManager.gm.flashlightWiden.ToString();
    //    //            PlayerPrefs.SetString("Button.FlashlightWiden", GameManager.gm.flashlightWiden.ToString());
    //    //            break;
    //    //        case "flashlightUp":
    //    //            GameManager.gm.flashlightUp = newKey;
    //    //            buttonText.text = GameManager.gm.flashlightUp.ToString();
    //    //            PlayerPrefs.SetString("Button.FlashlightUp", GameManager.gm.flashlightUp.ToString());
    //    //            break;
    //    //        case "flashlightDown":
    //    //            GameManager.gm.flashlightDown = newKey;
    //    //            buttonText.text = GameManager.gm.flashlightDown.ToString();
    //    //            PlayerPrefs.SetString("Button.FlashlightDown", GameManager.gm.flashlightDown.ToString());
    //    //            break;
    //    //        case "flashlightCenter":
    //    //            GameManager.gm.flashlightCenter = newKey;
    //    //            buttonText.text = GameManager.gm.flashlightCenter.ToString();
    //    //            PlayerPrefs.SetString("Button.FlashlightHome", GameManager.gm.flashlightCenter.ToString());
    //    //            break;
    //    //        case "interact":
    //    //            GameManager.gm.interact = newKey;
    //    //            buttonText.text = GameManager.gm.interact.ToString();
    //    //            PlayerPrefs.SetString("Button.Interact", GameManager.gm.interact.ToString());
    //    //            break;
    //    //        case "pause":
    //    //            GameManager.gm.pause = newKey;
    //    //            buttonText.text = GameManager.gm.pause.ToString();
    //    //            PlayerPrefs.SetString("Button.Pause", GameManager.gm.pause.ToString());
    //    //            break;
    //    //        case "inventory":
    //    //            GameManager.gm.inventory = newKey;
    //    //            buttonText.text = GameManager.gm.inventory.ToString();
    //    //            PlayerPrefs.SetString("Button.Inventory", GameManager.gm.inventory.ToString());
    //    //            break;
    //    //        default:
    //    //            break;
    //    //    }
    //    //    yield return null;
    //    //}
    //    switch (keyName)
    //    {
    //        case "forward":
    //            GameManager.gm.forward = newKey;
    //            buttonText.text = GameManager.gm.forward.ToString();
    //            PlayerPrefs.SetString("Button.Forward", GameManager.gm.forward.ToString());
    //            Debug.Log(Input.inputString + " key was pressed");
    //            break;
    //        case "backward":
    //            GameManager.gm.backward = newKey;
    //            buttonText.text = GameManager.gm.backward.ToString();
    //            PlayerPrefs.SetString("Button.Back", GameManager.gm.backward.ToString());
    //            break;
    //        case "left":
    //            GameManager.gm.left = newKey;
    //            buttonText.text = GameManager.gm.left.ToString();
    //            PlayerPrefs.SetString("Button.Left", GameManager.gm.left.ToString());
    //            break;
    //        case "right":
    //            GameManager.gm.right = newKey;
    //            buttonText.text = GameManager.gm.right.ToString();
    //            PlayerPrefs.SetString("Button.Right", GameManager.gm.right.ToString());
    //            break;
    //        case "crouch":
    //            GameManager.gm.crouch = newKey;
    //            buttonText.text = GameManager.gm.crouch.ToString();
    //            PlayerPrefs.SetString("Button.Crouch", GameManager.gm.crouch.ToString());
    //            break;
    //        case "sprint":
    //            GameManager.gm.sprint = newKey;
    //            buttonText.text = GameManager.gm.sprint.ToString();
    //            PlayerPrefs.SetString("Button.Sprint", GameManager.gm.sprint.ToString());
    //            break;
    //        case "shoot":
    //            GameManager.gm.shoot = newKey;
    //            buttonText.text = GameManager.gm.sprint.ToString();
    //            PlayerPrefs.SetString("Button.Shoot", GameManager.gm.shoot.ToString());
    //            break;
    //        case "reload":
    //            GameManager.gm.reload = newKey;
    //            buttonText.text = GameManager.gm.reload.ToString();
    //            PlayerPrefs.SetString("Button.Reload", GameManager.gm.reload.ToString());
    //            break;
    //        case "flashlightToggle":
    //            GameManager.gm.flashlightToggle = newKey;
    //            buttonText.text = GameManager.gm.flashlightToggle.ToString();
    //            PlayerPrefs.SetString("Button.FlashlightToggle", GameManager.gm.flashlightToggle.ToString());
    //            break;
    //        case "flashlightNarrow":
    //            GameManager.gm.flashlightNarrow = newKey;
    //            buttonText.text = GameManager.gm.flashlightNarrow.ToString();
    //            PlayerPrefs.SetString("Button.FlashlightNarrow", GameManager.gm.flashlightNarrow.ToString());
    //            break;
    //        case "flashlightWiden":
    //            GameManager.gm.flashlightWiden = newKey;
    //            buttonText.text = GameManager.gm.flashlightWiden.ToString();
    //            PlayerPrefs.SetString("Button.FlashlightWiden", GameManager.gm.flashlightWiden.ToString());
    //            break;
    //        case "flashlightUp":
    //            GameManager.gm.flashlightUp = newKey;
    //            buttonText.text = GameManager.gm.flashlightUp.ToString();
    //            PlayerPrefs.SetString("Button.FlashlightUp", GameManager.gm.flashlightUp.ToString());
    //            break;
    //        case "flashlightDown":
    //            GameManager.gm.flashlightDown = newKey;
    //            buttonText.text = GameManager.gm.flashlightDown.ToString();
    //            PlayerPrefs.SetString("Button.FlashlightDown", GameManager.gm.flashlightDown.ToString());
    //            break;
    //        case "flashlightCenter":
    //            GameManager.gm.flashlightCenter = newKey;
    //            buttonText.text = GameManager.gm.flashlightCenter.ToString();
    //            PlayerPrefs.SetString("Button.FlashlightHome", GameManager.gm.flashlightCenter.ToString());
    //            break;
    //        case "interact":
    //            GameManager.gm.interact = newKey;
    //            buttonText.text = GameManager.gm.interact.ToString();
    //            PlayerPrefs.SetString("Button.Interact", GameManager.gm.interact.ToString());
    //            break;
    //        case "pause":
    //            GameManager.gm.pause = newKey;
    //            buttonText.text = GameManager.gm.pause.ToString();
    //            PlayerPrefs.SetString("Button.Pause", GameManager.gm.pause.ToString());
    //            break;
    //        case "inventory":
    //            GameManager.gm.inventory = newKey;
    //            buttonText.text = GameManager.gm.inventory.ToString();
    //            PlayerPrefs.SetString("Button.Inventory", GameManager.gm.inventory.ToString());
    //            break;
    //        default:
    //            break;
    //    }
    //    yield return null;
    //}
}
