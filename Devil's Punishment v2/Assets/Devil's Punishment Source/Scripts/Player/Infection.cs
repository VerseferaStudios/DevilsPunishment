using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Infection : MonoBehaviour
{
    [Header("Settings")]
    public bool infected = false;
    public float infection_Amount = 0f;
    public float infection_DamageAmount = 2f;
    public float infection_MaxAmount = 100f;
    public float infection_Multiplier = 1f;

    public static Infection instance;

    private Health health;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        health = Player.instance.GetComponent<Health>();
        InvokeRepeating("UpdateInfection", 0f, 1f);
    }

    public float GetInfectionAmount()
    {
        return infection_Amount;
    }

    public void InfectToggle()
    {
        infected = !infected;
    }

    void UpdateInfection()
    {
        if (infected == true)
        {
            if (infection_Amount < infection_MaxAmount)
            {
                infection_Amount += Random.Range(0f, 0.05f) * infection_Multiplier;
            }
            else
            {
                health.InflictDamage(infection_DamageAmount);
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
