using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    PlayerController controller;
    GunController gunController;
    Inventory inventory;

    private bool inventoryOpen = true;

    public static Player instance;

    void Awake() {
        instance = this;
    }

    void Start() {
        controller = GetComponent<PlayerController>();
        gunController = GunController.instance;
        inventory = Inventory.instance;
        ToggleInventory();
    }

    void Update() {
        if(Input.GetButtonDown("Open Inventory")) {
            ToggleInventory();
        }
    }

    void ToggleInventory() {
        inventoryOpen = !inventoryOpen;
        if(inventoryOpen) {
            inventory.gameObject.SetActive(true);
        } else { inventory.gameObject.SetActive(false);}

        Cursor.visible = inventoryOpen;
        Cursor.lockState = (inventoryOpen? CursorLockMode.None : CursorLockMode.Locked);

        SetControllerEnabled();
    }

    void SetControllerEnabled() {
        controller.inputEnabled = !inventoryOpen;
        gunController.inputEnabled = !inventoryOpen;
    }

}
