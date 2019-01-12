using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunControllerDisplayUI : MonoBehaviour
{
    public RectTransform crosshairsParent;
    public Color crosshairColor;

    Image[] crosshairs;
    GunController gunController;
    PlayerController playerController;

    void Start() {
        gunController = GunController.instance;
        playerController = PlayerController.instance;
        crosshairs = crosshairsParent.gameObject.GetComponentsInChildren<Image>();
    }

    void Update() {
        Crosshairs();
    }

    void Crosshairs() {
        float spread = gunController.GetBulletSpreadCoefficient();

        float size = 32f * spread;


        crosshairsParent.localRotation =
        Quaternion.Lerp(crosshairsParent.localRotation, Quaternion.Euler(0, 0, playerController.IsCrouching()? 45f : 0f),
        Time.deltaTime * 10.0f);

        crosshairColor.a = Mathf.Clamp01(spread - 1.0f);

        foreach(Image crosshair in crosshairs) {
            crosshair.color = crosshairColor;
        }

        crosshairsParent.sizeDelta = new Vector2(size, size);

    }

}
