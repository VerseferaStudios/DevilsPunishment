//Author: Dave Bird
//Date: Sunday, June 23, 2019
    //Last Edited: Monday, July 1, 2019
        //By: Dave Bird
            //Purpose: Write the script
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
     * Set infection rate //50% done
     * Set infection rate in regards to cur health //50% done
     * Change Medkit script to adjust the health of both infected and noninfected players
     * Change Medkit Use script to adjust the time delay for being infected to +2 seconds
     * Change pill script to add pillsConsumed++
     * Change StatusUI script to reflect the infection rate.
     * Create/Adjust creature attack script to run the chance to infect.
     */
    [Header("Setting Health")]
    [SerializeField]
    private float curHealth;
    [SerializeField]
    private float maxHealth;
    public GameObject player;
    public int pillsConsumed;
    public int chanceToInfect; //Set random range from 0-150, if the number is between 0-20 infection sets in.
    public bool infected;
    public bool canInfect;

    [Header("Setting Infection Rates")]
    public float infectRate = 900.0f;
    public int infectionRate;
    public float infectMore; //Used to add +9, +10, +11, +12 then add +1% to infectionRate

    [Header("Has a pill been consumed?")]
    public bool changeRate1;
    public bool changeRate2;
    public bool changeRate3;
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
        if (chanceToInfect >= 0 && chanceToInfect <= 50)
        {
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
            //player throes
        }
        else if (infectionRate >= 100.0f)
        {
            //player throes
            //instantiate night terror
        }
        else
        {
            return;
        }
    }

    private void Update()
    {
        //...................................
        //For Testing to infect press i
        //...................................
        if (Input.GetKeyDown(KeyCode.I) && !infected)
        {
            infected = true;
            canInfect = false;
        }
        else if (Input.GetKeyDown(KeyCode.I) && infected)
        {
            infected = false;
            canInfect = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            pillsConsumed++;
        }
        //Remove above after testing...

        //Add infection meter call and connect it to the infection game object 
        //Add a while loop that while its running at every 9 , 10, 11, or 12 seconds it add 1% to the infection meter

        if (infected)
        {
            maxHealth = 75;
            infectRate -= Time.deltaTime;
            //Debug.Log("You have " + infectRate + " left.");
            infectMore += Time.deltaTime;
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
                if (changeRate1 == false)
                {
                    changeRate1 = true;
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
                if (changeRate2 == false)
                {
                    changeRate2 = true;
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
                if (changeRate3 == false)
                {
                    changeRate3 = true;
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