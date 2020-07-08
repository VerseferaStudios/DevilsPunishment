using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Infection : MonoBehaviour
{
    [Header("Settings")]
    public bool infected = false;
    [Range(0f, 100f)]
    public float infection_Amount = 0f;
    [Range(0f, 100f)]
    public float infection_DamageAmount = 2f;
    [Range(0f, 100f)]
    public float infection_MaxAmount = 100f;
    [Range(0f, 100f)]
    public float infection_Multiplier = 1f;

    // Pill Stuff
    [Range(0f, 100f)]
    public float pill_Dampener = 0f;
    [Range(0f, 100f)]
    public float pill_Duration = 0f;

    public static Infection instance;

    private Health health;

    
    private void OnEnable()
    {
        PlayerController_Revamped.CallbackAssignStaticInstances += AssignInstance;
    }
    private void OnDestroy()
    {
        PlayerController_Revamped.CallbackAssignStaticInstances -= AssignInstance;
    }
    private void AssignInstance()
    {
        instance = this;
        StartInfectionLogic();
    }

    void StartInfectionLogic()
    {
        health = Player.instance.GetComponent<Health>();
        InvokeRepeating("UpdateInfection", 0f, 1f);
    }

    public float GetInfectionAmount()
    {
        return infection_Amount;
    }

    public void UsePill(float duration, float strength)
    {
        pill_Dampener = strength;
        pill_Duration = duration;
        InvokeRepeating("UpdatePillDuration", 0f, 1f);
    }

    public void InfectToggle()
    {
        infected = !infected;
    }

    void UpdatePillDuration()
    {
        if (pill_Duration > 0)
        {
            pill_Duration--;
        }
        else
        {
            pill_Dampener = 0;
            CancelInvoke("UpdatePillDuration");
        }
    }

    void UpdateInfection()
    {
        if (infected == true)
        {
            if (infection_Amount < infection_MaxAmount)
            {
                if (pill_Dampener > 0f)
                    infection_Amount += (Random.Range(0f, 0.05f) * infection_Multiplier) / pill_Dampener;
                else
                    infection_Amount += (Random.Range(0f, 0.05f) * infection_Multiplier);
            }
            else
            {
                //health.InflictDamage(infection_DamageAmount);
            }
        }
        else
        {
            if (infection_Amount > 0)
                infection_Amount -= Random.Range(0f, 0.05f) * infection_Multiplier;
            else if (infection_Amount < 0)
                infection_Amount = 0;
        }
    }

}
