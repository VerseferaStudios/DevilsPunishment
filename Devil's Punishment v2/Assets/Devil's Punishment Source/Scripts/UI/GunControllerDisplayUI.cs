using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GunControllerDisplayUI : MonoBehaviour
{
    public RectTransform crosshairsParent;
    public Color crosshairColor;

    public TextMeshProUGUI clipText;
    public TextMeshProUGUI clipStockText;
	public GameObject AmmoDisplay;

    Image[] crosshairs;
    GunController gunController;
    PlayerController playerController;
	Inventory inventory;

    void Start() {
        gunController = GunController.instance;
        playerController = PlayerController.instance;
        crosshairs = crosshairsParent.gameObject.GetComponentsInChildren<Image>();
    }

    void Update() {
        Crosshairs();
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

    void Crosshairs() {
        float spread = gunController.GetBulletSpreadCoefficient();

        float size = 32f * spread;

/*
        crosshairsParent.localRotation =
        Quaternion.Lerp(crosshairsParent.localRotation, Quaternion.Euler(0, 0, playerController.IsCrouching()? 45f : 0f),
        Time.deltaTime * 10.0f);
*/

        crosshairColor.a = Mathf.Clamp01(spread - 1.0f);

        foreach(Image crosshair in crosshairs) {
            crosshair.color = crosshairColor;
        }

        crosshairsParent.sizeDelta = new Vector2(size, size);

    }

}
