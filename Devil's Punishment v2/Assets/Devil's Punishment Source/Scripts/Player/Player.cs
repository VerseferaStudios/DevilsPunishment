using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using static PlayerController_Revamped;
public class Player : MonoBehaviour
{
    PlayerController_Revamped controller;
    GunController gunController;
    CuffController cuffController;
    Inventory inventory;
    InGameMenuUI gameMenu;

    private bool inventoryOpen = true;
    private bool gameMenuOpen = true;

    public static Player instance;

    public ColorGrading postFXColorGrading;
    public Text brightnessPercent;

    void Awake() {
        instance = this;
    }

    void Start() {
        controller = GetComponent<PlayerController_Revamped>();
        cuffController = CuffController.instance;
        gunController = GunController.instance;
        inventory = Inventory.instance;
        gameMenu = InGameMenuUI.instance;
        ToggleInventory();
        ToggleGameMenu();
		if (inventory.equippedGun != null) { gunController.InitGun(); }
        postFXColorGrading = GameSetup.setup.postFXColorGrading;
	}

    void Update() {
        if(Input.GetButtonDown("Open Inventory")) {
            if (gameMenuOpen == false)
                ToggleInventory();
        }
        if(Input.GetButtonDown("Cancel"))
        {
            if (inventoryOpen == false)
                ToggleGameMenu();
        }
        //cuffController.ToggleCuffs();
    }

    public void ToggleGameMenu()
    {
       
        gameMenuOpen = !gameMenuOpen;
        gameMenu.gameObject.SetActive(gameMenuOpen);

        // ingame menu UI
        if(gameMenuOpen)
        {
            gameMenu.backgroundPanel.color = gameMenu.backGroundPanelColor;
        }
        else
        {
            gameMenu.backgroundPanel.color = Color.clear;
        }

        Cursor.visible = gameMenuOpen;
        Cursor.lockState = (gameMenuOpen ? CursorLockMode.None : CursorLockMode.Locked);

        controller.inputEnabled = !gameMenuOpen;
        gunController.inputEnabled = !gameMenuOpen;
    }

    void ToggleInventory() {
        inventoryOpen = !inventoryOpen;
        if(inventoryOpen) {
            inventory.gameObject.SetActive(true);
        } else { inventory.gameObject.SetActive(false);}

        Cursor.visible = inventoryOpen;
        Cursor.lockState = (inventoryOpen? CursorLockMode.None : CursorLockMode.Locked);

        controller.inputEnabled = !inventoryOpen;
        gunController.inputEnabled = !inventoryOpen;
    }

    public void ToggleOptionsMenu()
    {

    }

    public void ExitGame()
    {

    }

    public void BrightnessSlider(Slider slider)
    {
        postFXColorGrading.postExposure.value = slider.value;
        brightnessPercent.text = Mathf.Round(slider.value * 10000)/100 + "%";
    }

    public bool MenuOpen { get { return gameMenuOpen; } }
}
