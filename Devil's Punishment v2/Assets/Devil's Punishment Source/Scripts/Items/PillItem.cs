using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pill", menuName = "Item/Pill")]
public class PillItem : Item
{

    [Range(30f, 180f)]
    public float protectionDuration;

    [Range(0f, 100f)]
    public float protectionStrength;

    public override bool Use() {
        Debug.Log("Pill taken! Player is " + protectionStrength + "% more resistant to infection for " + protectionDuration + " seconds (" + (protectionDuration/60) + " minutes).");
        return true;
    }

}
