using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pill", menuName = "Item/Pill")]
public class PillItem : Item
{
    public override bool Use()
    {
        Player player = Player.instance;
        Health health = player.GetComponent<Health>();
        Debug.Assert(health != null, "Player health script was NULL inside of the pill's use method");
        Debug.Log("In 'pills use()' -> health.infected: " + health.infected);
        if (health.infected && health.pillsConsumed < 3)
        {
            health.pillsConsumed++;
            return true;
        }
        Debug.Log("Attempted to use item, was not able to either weren't infected or already reached maximum of pills used. (we dont want you to O.D.)");
        return false;
    }
}