using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class KeyBindingScript : MonoBehaviour
{
    Transform menuImage;
    Event keyEvent;
    Text buttonText;
    public KeyCode newKey;

    private bool waitingForKey;

    public void Start()
    {
        menuImage = this.transform;
        //menuImage = this.transform.Find("Image.ButtonListContent").transform;
        //menuImage = transform.FindChild("Image.ButtonListContent");
        waitingForKey = false;

        for(int i = 0; i < 17; i++)
        {
            if(menuImage.GetChild(i).name == "Button.Forward")
            {
                menuImage.GetChild(i).GetComponentInChildren<Text>().text = GameManager.gm.forward.ToString();
            }
            else if (menuImage.GetChild(i).name == "Button.Back")
            {
                menuImage.GetChild(i).GetComponentInChildren<Text>().text = GameManager.gm.backward.ToString();
            }
            else if (menuImage.GetChild(i).name == "Button.Left")
            {
                menuImage.GetChild(i).GetComponentInChildren<Text>().text = GameManager.gm.left.ToString();
            }
            else if (menuImage.GetChild(i).name == "Button.Right")
            {
                menuImage.GetChild(i).GetComponentInChildren<Text>().text = GameManager.gm.right.ToString();
            }
            else if (menuImage.GetChild(i).name == "Button.Crouch")
            {
                menuImage.GetChild(i).GetComponentInChildren<Text>().text = GameManager.gm.crouch.ToString();
            }
            else if (menuImage.GetChild(i).name == "Button.Sprint")
            {
                menuImage.GetChild(i).GetComponentInChildren<Text>().text = GameManager.gm.sprint.ToString();
            }
            else if (menuImage.GetChild(i).name == "Button.Shoot")
            {
                menuImage.GetChild(i).GetComponentInChildren<Text>().text = GameManager.gm.shoot.ToString();
            }
            else if (menuImage.GetChild(i).name == "Button.Reload")
            {
                menuImage.GetChild(i).GetComponentInChildren<Text>().text = GameManager.gm.reload.ToString();
            }
            else if (menuImage.GetChild(i).name == "Button.FlashlightToggle")
            {
                menuImage.GetChild(i).GetComponentInChildren<Text>().text = GameManager.gm.flashlightToggle.ToString();
            }
            else if (menuImage.GetChild(i).name == "Button.FlashlightNarrow")
            {
                menuImage.GetChild(i).GetComponentInChildren<Text>().text = GameManager.gm.flashlightNarrow.ToString();
            }
            else if (menuImage.GetChild(i).name == "Button.FlashlightWiden")
            {
                menuImage.GetChild(i).GetComponentInChildren<Text>().text = GameManager.gm.flashlightWiden.ToString();
            }
            else if (menuImage.GetChild(i).name == "Button.FlashlightUp")
            {
                menuImage.GetChild(i).GetComponentInChildren<Text>().text = GameManager.gm.flashlightUp.ToString();
            }
            else if (menuImage.GetChild(i).name == "Button.FlashlightDown")
            {
                menuImage.GetChild(i).GetComponentInChildren<Text>().text = GameManager.gm.flashlightDown.ToString();
            }
            else if (menuImage.GetChild(i).name == "Button.FlashlightHome")
            {
                menuImage.GetChild(i).GetComponentInChildren<Text>().text = GameManager.gm.flashlightCenter.ToString();
            }
            else if (menuImage.GetChild(i).name == "Button.Pause")
            {
                menuImage.GetChild(i).GetComponentInChildren<Text>().text = GameManager.gm.pause.ToString();
            }
            else if (menuImage.GetChild(i).name == "Button.Inventory")
            {
                menuImage.GetChild(i).GetComponentInChildren<Text>().text = GameManager.gm.inventory.ToString();
            }
            else if (menuImage.GetChild(i).name == "Button.Interact")
            {
                menuImage.GetChild(i).GetComponentInChildren<Text>().text = GameManager.gm.interact.ToString();
            }
        }
    }
}
