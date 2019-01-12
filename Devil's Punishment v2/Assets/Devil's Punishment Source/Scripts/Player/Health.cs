using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHP = 100;
    public float HP;
    
    public float GetHP()
    {
        return HP;
    }

    public float GetHPPercentage() {
        return HP/maxHP;
    }

    public void InflictDamage(float damage)
    {
        HP -= damage;
        CapHP();
    }

    public void InflictDamage(float totalDamage, float timeInSeconds)
    {
        if(timeInSeconds<=0) {InflictDamage(totalDamage);}
        StartCoroutine(_ChangeHPCoroutine(-totalDamage, timeInSeconds));
    }

    public void IncreaseHealth(float healthIncreaseValue)
    {
        HP += healthIncreaseValue;
        CapHP();
    }

    public void IncreaseHealth(float healthIncreaseValue, float timeInSeconds)
    {
        if(timeInSeconds<=0) {IncreaseHealth(healthIncreaseValue);}
        StartCoroutine(_ChangeHPCoroutine(healthIncreaseValue, timeInSeconds));
    }


    private IEnumerator _ChangeHPCoroutine(float HPValueChange, float time)
    {

        float finalHP = HP + HPValueChange;

        float delta = (finalHP-HP)/(time*100f);

        while (HP < finalHP)
        {
            HP = HP + delta;
            yield return new WaitForSeconds(0.01f);
        }

        CapHP();
    }

    private void CapHP()
    {
        HP = Mathf.Floor(HP);
        HP = Mathf.Min(HP, maxHP);
        HP = Mathf.Max(HP, 0);
    }
}
