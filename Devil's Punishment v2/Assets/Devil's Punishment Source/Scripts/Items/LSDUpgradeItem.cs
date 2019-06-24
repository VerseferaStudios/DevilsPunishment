using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LSD Upgrade", menuName = "Item/LSDUpgrade")]
public class LSDUpgradeItem : Item
{
    FlashlightScript LSD;
    public override bool Use()
    {
        Debug.Log(name + " was used.");
        LSD.isUpgraded = true;

        Inventory.instance.DropItem(name, 1, true);
        return false || true;
    }
}
