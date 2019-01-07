using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    public float maxHp = 100;
    public float lowHp = 20;

    public float hp;
    private float xScale;

    public GameObject redScreen;
    public GameObject hpBar;

    void Awake()
    {
        hp = maxHp;
        xScale = hpBar.transform.localScale.x;
    }

    void Update()
    {

        //Just a fail safe
        if (hp > maxHp)
            hp = maxHp;
            /*
        //Set the hp bars scale
        hpBar.transform.localScale = new Vector3((hp / 100 * xScale), hpBar.transform.localScale.y, hpBar.transform.localScale.z);

        //Making screen red
        if (hp <= lowHp && redScreen.activeSelf == false)
            redScreen.SetActive(true);

        //Clearing screen up
        else if (hp > lowHp && redScreen.activeSelf == true)
            redScreen.SetActive(false);

        //Don't have the spectate code but it should look somethinng like this
        //if (hp <= 0)
        //Anim.Setbool("Dead", true);
        //alive = false;
        //spectateCamOne.setActive(true);
        */
    }

    public float GetHP()
    {
        return hp;
    }

    public void InflictDamage(float damage)
    {
        hp -= damage;
    }

    public void InflictDamage(float TotalDamage, float timeInSeconds)
    {
        float damagePerDeciSecond = TotalDamage / (timeInSeconds * 10f);
        StartCoroutine(_ChangeHPCoroutine(-damagePerDeciSecond, timeInSeconds));
    }

    public void IncreaseHealth(float healthIncreaseValue)
    {
        hp += healthIncreaseValue;
    }

    public void IncreaseHealth(float healthIncreaseValue, float timeInSeconds)
    {
        if(timeInSeconds<=0) {IncreaseHealth(healthIncreaseValue);}
        float healthIncreasePerDeciSecond = healthIncreaseValue / (timeInSeconds * 10f);
        StartCoroutine(_ChangeHPCoroutine(healthIncreasePerDeciSecond, timeInSeconds));
    }


    private IEnumerator _ChangeHPCoroutine(float hpValueChange, float time)
    {
        float timePassed = 0f;
        while (timePassed < time)
        {
            timePassed += 0.1f;
            hp = hp + hpValueChange;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
