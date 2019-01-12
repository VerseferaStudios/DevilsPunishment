using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Medkit", menuName = "Item/Medkit")]
public class MedkitItem : Item
{

    public float healAmount;
    public float timeToApply;


    public override bool Use() {
        Debug.Log("Medkit used! Player healed by " + healAmount + " health points.");

        Player player = Player.instance;
        Health health = player.GetComponent<Health>();
        health.IncreaseHealth(healAmount, timeToApply);

        return true; //Medkits are consumed upon usage
    }

}
