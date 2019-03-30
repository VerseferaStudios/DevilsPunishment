using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusDisplayUI : MonoBehaviour
{

    // Status: Health
    public Image bloodOnScreen;
    public Slider healthSlider;
    public TextMeshProUGUI percentageText;
    private Health health;

    // Status: Infection
    public GameObject infectionDisplay;
    private Infection infection;
    private Slider infectionSlider;
    private TextMeshProUGUI infectionPerecentage;

    Color deepRed;

    private void Start() {
        health = Player.instance.GetComponent<Health>();
        deepRed = new Color(.5f,0,0);

        // Infection setup
        infection = Player.instance.GetComponent<Infection>();
        infectionSlider = infectionDisplay.GetComponentInChildren<Slider>();
        infectionPerecentage = infectionDisplay.GetComponentInChildren<TextMeshProUGUI>();
        InvokeRepeating("UpdateInfectionDisplay", 0f, 1f);
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

    private void UpdateInfectionDisplay()
    {
        float infectionAmount = infection.GetInfectionAmount();
        Debug.Log(infectionAmount.ToString());
        if (infectionDisplay.activeSelf == false && infectionAmount > 0f) 
        {
            infectionDisplay.SetActive(true);
        }
        else if (infectionAmount <= 0f)
        {
            infectionDisplay.SetActive(false);
        }
            

        infectionSlider.value = infectionAmount / 100;
        infectionPerecentage.text = infectionAmount.ToString("0.##") + "%";
    }


}
