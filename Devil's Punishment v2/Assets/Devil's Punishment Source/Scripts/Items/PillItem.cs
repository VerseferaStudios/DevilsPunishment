using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pill", menuName = "Item/Pill")]
public class PillItem : Item
{

    public float duration; // Time in seconds
    public float strength; // Time in seconds

    private float protectionDuration;
    private float protectionStrength;

    private Infection infect;

    public override bool Use() {
        infect = Player.instance.GetComponent<Infection>();
        protectionDuration = duration;
        protectionStrength = strength;
        Debug.Log("Pill taken! Player is " + protectionStrength + "% more resistant to infection for " + protectionDuration + " seconds (" + (protectionDuration/60) + " minutes).");
        infect.UsePill(protectionDuration, protectionStrength);
        
        return true;
    }

}
