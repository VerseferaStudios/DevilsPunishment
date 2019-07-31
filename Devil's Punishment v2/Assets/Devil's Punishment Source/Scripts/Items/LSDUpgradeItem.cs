using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LSD Upgrade", menuName = "Item/LSDUpgrade")]
public class LSDUpgradeItem : Item
{
	// THIS NEEDS TO GET THE FLASHLIGHT SCRIPT COMPONENT FROM THE PLAYER (EX: .parent.parent of this object?, right now it causes null-ptr issues)
    public override bool Use()
    {
        Debug.Log(name + " was used.");

        Player.instance.gameObject.GetComponent<FlashlightScript>().isUpgraded = true;
        return true;
    }
}
