using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//<<<<<<< HEAD
using UnityEngine.SceneManagement;

//=======
using static PlayerController_Revamped;
//>>>>>>> a4682b2e5ab22ca11d637c89d7871a76100f5093
public class Player : MonoBehaviour
{
    PlayerController_Revamped controller;
    GunController gunController;
    CuffController cuffController;
    Inventory inventory;
    InGameMenuUI gameMenu;

    public GameObject gameScreen;
    public GameObject pauseScreen;

    private bool inventoryOpen = true;
    private bool gameMenuOpen = true;

    public static Player instance;

    void Awake() {
        instance = this;
        ChangeCanvas();
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
        Debug.Log("Got past here");
    }

    public void ChangeCanvas()
    {
        gameScreen.SetActive(true);
        pauseScreen.SetActive(false);
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
        gameScreen.SetActive(true);
        pauseScreen.SetActive(false);
        Debug.Log("Pressed the button to open Options");
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
