using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "Item/Gun")]
public class GunItem : Item
{
    public Item ammunitionType;

    public int clipSize;

    [Range(1f, 20f)]
    public float fireRate = 10f;

	[Range(25, 45)]
	public float recoilAmount = 32f;

	[Range(0, 100)]
	public int damage = 100;


}
