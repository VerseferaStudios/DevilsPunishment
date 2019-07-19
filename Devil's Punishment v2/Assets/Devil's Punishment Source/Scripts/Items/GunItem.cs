using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "Item/Gun")]
public class GunItem : ScriptableObject, Item
{

    //Different weapon types, used for selecting different animations, and different code, such as "reloading style"
    public enum WeaponClassification {
    //IMPORTANT: Numerical values given in the enum should NEVER be altered.
    // If you want to insert a new item in the middle of the list, give it a new numerical value that hasn't existed in the list before.
        NONE = 0,
        HANDGUN = 1,
        SHOTGUN = 2,
        ASSAULTRIFLE = 3,
    }

    public WeaponClassification weaponClassification = WeaponClassification.NONE;

    public Item ammunitionType;

    public int clipSize;

    [Range(1f, 100f)]
    public float fireRate = 10f;

	[Range(25, 45)]
	public float recoilAmount = 32f;

	[Range(0, 100)]
	public int damage = 100;



}
