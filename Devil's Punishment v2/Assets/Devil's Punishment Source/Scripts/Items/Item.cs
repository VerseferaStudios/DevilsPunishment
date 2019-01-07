using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item/Item")]
public class Item : ScriptableObject, IComparable<Item>
{

    //Only for use with base items
    public enum ItemClassification {
        NONE,
        BULLET,
        KEY,
    }

    //Does this item get consumed when used? true - yes false - no
    public new string name;
    [TextArea(3,5)]
    public string description;
    public int maxStackSize;

    public ItemClassification itemClassification = ItemClassification.NONE;

    public Color color = Color.white;

    public Sprite itemIcon;

    public virtual bool Use() {
        Debug.Log(name + " was used... no effect.");
        return false;
    }

    public bool Equals(Item other) {
        return name == other.name;
    }

    public int CompareToAlphabetical(Item other) {
        if(other != null) {
            return name.CompareTo(other.name);
        }

        return 1;

    }

    public int CompareTo(Item other) {
        if(other != null) {
            if(this.GetType().Equals(other.GetType())) {
                return itemClassification.CompareTo(other.itemClassification);
            }
            return this.GetType().ToString().CompareTo(other.GetType().ToString());
        }

        return 1;

    }

}
