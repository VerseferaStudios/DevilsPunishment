using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusDisplayUI : MonoBehaviour
{

    public Image bloodOnScreen;
    public Slider healthSlider;
    public TextMeshProUGUI percentageText;
    private Health health;

    Color deepRed;

    private void Start() {
        health = Player.instance.GetComponent<Health>();
        deepRed = new Color(.5f,0,0);
    }

    private void Update() {
        UpdateHealthDisplay();
    }

    private void UpdateHealthDisplay() {

        float HPPercentage = health.GetHPPercentage();
        healthSlider.value = HPPercentage;

        percentageText.text = Mathf.FloorToInt(HPPercentage*100f) + "%";

        if(HPPercentage > .2f) {
            deepRed.a = 0;
        } else {
            deepRed.a = Mathf.Sin(10f*Time.time)*.02f + (.2f-HPPercentage)*2f;
        }

        bloodOnScreen.color = deepRed;

    }


}
