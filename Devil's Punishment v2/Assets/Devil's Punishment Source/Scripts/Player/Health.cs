//Author: Dave Bird
//Date: Sunday, June 23, 2019
    //Last Edited: Monday, July 7, 2019
        //By: Dave Bird
            //Purpose: Finish health will be back to work on death sequences
//Written For: Devil's Punishment v2
//Purpose: This script details all things that has to do with the health and infection rate.
//Notes: Further information can be found at http://www.nolocationyet.com (Dead link)

using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    /* Need to do!
     * Set health  //Done
     * Set cur health //Done
     * Set infection rate //Done
     * Set infection rate in regards to cur health //Done
     * Change Medkit script to adjust the health of both infected and noninfected players //Done
     * Change Medkit Use script to adjust the time delay for being infected to +2 seconds //Done
     * Change pill script to add pillsConsumed++ //Done
     * Change StatusUI script to reflect the infection rate. 
     * Create/Adjust creature attack script to run the chance to infect. //Done
     */
    [Header("Setting Health")]
    [SerializeField]
    public float curHealth;
    [SerializeField]
    private float maxHealth;
    public GameObject player; 
    public int pillsConsumed; //How many pills have you consumed to stall the infection
    public int chanceToInfect; //Set random range from 0-150, if the number is between 0-20 infection sets in.
    public bool infected; //Are you infected?
    public bool canInfect; //Can you get infected?

    [Header("Setting Infection Rates")]
    public float infectRate = 900.0f;
    public int infectionRate;
    public float infectMore; //Used to add +9, +10, +11, +12 then add +1% to infectionRate
    public StatusDisplayUI infectStatus;

    [Header("Has a pill been consumed?")]
    public bool pill1Used; 
    public bool pill2Used;
    public bool pill3Used;
    [Header("When player dies")]
    public GameObject nightTerror;


    private void Start()
    {
        maxHealth = 100;
        curHealth = maxHealth;
        pillsConsumed = 0; //Need to reference this variable from the pills used script set it +1 each one max of 3.
        infected = false;
        canInfect = true;
        infectMore = 0;
    }

    public void IncreaseHealth(float healthIncreaseValue, float timeInSeconds)
    {
        if (infected)
        {
            timeInSeconds += (1 + pillsConsumed) * 2;
            healthIncreaseValue -= pillsConsumed * 5;
        }
        if (timeInSeconds <= 0)
        {
            curHealth = Mathf.Min(curHealth + healthIncreaseValue, maxHealth);
        }
        else
        {
            StartCoroutine(_ChangeHPCoroutine(healthIncreaseValue, timeInSeconds));
        }
    }

    private IEnumerator _ChangeHPCoroutine(float HPValueChange, float time)
    {

        float finalHP = curHealth + HPValueChange;

        float deltaHP = (finalHP - curHealth) / (time);
        float amountHealed = 0;
        while (amountHealed <= HPValueChange)
        {
            curHealth = Mathf.Min(curHealth + deltaHP * Time.deltaTime, maxHealth);
            amountHealed += deltaHP * Time.deltaTime;
            yield return null;
        }
    }

        public float GetHPPercentage()
    {
        return curHealth / maxHealth;
    }

    public void InfectChance()
    {
        chanceToInfect = Random.Range(0, 150);
        Debug.Log("The attack dealth damage and you rolled a " + chanceToInfect + " if it was below 15 you are infected.");
        if (chanceToInfect <= 10)
        {
            Debug.Log("You were infected.");
            infected = true;
            canInfect = false;
            Debug.Log("You've been infected. You now have 900 seconds before the infection takes you.");
        }
        else
        {
            return;
        }
    }

    public void Death()
    {
        if (curHealth <= 0)
        {
            StartCoroutine("NormalDeath");
        }
        else if (infectionRate >= 100.0f)
        {
            StartCoroutine("InfectionDeath");
        }
        else
        {
            return;
        }
    }

    IEnumerator NormalDeath()
    {
        yield return null;
    }

    IEnumerator InfectionDeath()
    {
        yield return null;
        //instantiate night terror
    }

    public int InfectionRate()
    {
        return infectionRate;
    }


    private void Update()
    {
        if (curHealth <= 0)//Add infection meter call and connect it to the infection game object
        {
            curHealth = 0;
            StartCoroutine("NormalDeath");
        }


        if (infected)
        {
            maxHealth = 75;
            infectRate -= Time.deltaTime;
            infectMore += Time.deltaTime;
            infectStatus.UpdateInfectionDisplay();
            if (pillsConsumed == 0)
            {
                if (infectMore >= 9)
                {
                    infectMore = 0;
                    infectionRate++;
                }
            }
            else if (pillsConsumed == 1)
            {
                if (pill1Used == false)
                {
                    pill1Used = true;
                    infectRate += 100;
                }
                if (infectMore >= 10)
                {
                    infectMore = 0;
                    infectionRate++;
                }
            }
            else if (pillsConsumed == 2)
            {
                if (pill2Used == false)
                {
                    pill2Used = true;
                    infectRate += 100;
                }
                if (infectMore >= 11)
                {
                    infectMore = 0;
                    infectionRate++;
                }
            }
            else if (pillsConsumed == 3)
            {
                if (pill3Used == false)
                {
                    pill3Used = true;
                    infectRate += 100;
                }
                if (infectMore >= 12)
                {
                    infectMore = 0;
                    infectionRate++;
                }
            }
            if (infectionRate == 100)
            {
                Death();
            }
        }
    }
}