using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GunControllerDisplayUI : MonoBehaviour
{
    public TextMeshProUGUI clipText;
    public TextMeshProUGUI clipStockText;
	public GameObject AmmoDisplay;
    GunController gunController;
    PlayerController_Revamped playerController;
	Inventory inventory;

    void Start() {
        gunController = GunController.instance;
        playerController = PlayerController_Revamped.instance;
    }

    void Update() {
        Ammo();
    }

    void Ammo() {

		AmmoDisplay.SetActive(Inventory.instance !=null && Inventory.instance.equippedGun != null);
        int clip = gunController.GetClip();

        if(clip == 0) {
            clipText.color = Color.red;
        } else if(clip == gunController.GetClipSize()) {
            clipText.color = Color.green;
        } else {
            clipText.color = Color.white;
        }
        clipText.text = clip + "";

        clipStockText.text = gunController.GetClipStock() + "";

    }
}
