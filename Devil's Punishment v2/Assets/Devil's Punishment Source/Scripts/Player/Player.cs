using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    PlayerController controller;
    GunController gunController;
    CuffController cuffController;
    Inventory inventory;
    InGameMenuUI gameMenu;

    private bool inventoryOpen = true;
    private bool gameMenuOpen = true;

    public static Player instance;

    void Awake() {
        instance = this;
    }

    void Start() {
        controller = GetComponent<PlayerController>();
        cuffController = CuffController.instance;
        gunController = GunController.instance;
        inventory = Inventory.instance;
        gameMenu = InGameMenuUI.instance;
        ToggleInventory();
        ToggleGameMenu();
		if (inventory.equippedGun != null) { gunController.InitGun(); }
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

}
