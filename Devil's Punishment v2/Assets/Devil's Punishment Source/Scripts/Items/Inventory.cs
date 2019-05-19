using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Inventory : MonoBehaviour
{

    public enum SortType {
        TYPE,
        ALPHABETICAL
    }

    [System.Serializable]
    public class InventorySlot : IComparable<InventorySlot> {
        public Item item;
        public int stack;

        public InventorySlot(Item _item, int _stack) {
            item = _item;
            stack = _stack;
        }
        public InventorySlot() {
            item = null;
            stack = 0;
        }

        public int CompareTo(InventorySlot other) {

            if(item == null && other.item == null) {return 0;}

            if(item != null && other.item == null) {
                return -1;
            } else if (item == null && other.item != null) {
                return 1;
            } else {
                switch(Inventory.sortType) {
                    case SortType.ALPHABETICAL:
                        return item.CompareToAlphabetical(other.item);
                    default:
                    case SortType.TYPE:
                        return item.CompareTo(other.item);
                }
            }

        }

    }

    public List<InventorySlot> inventory;
    public GunItem equippedGun;
    public static Inventory instance;
    public static SortType sortType;

    private GunController gunController;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        Sort();
        gunController = GunController.instance;
    }

    public Item GetItemFromIndex(int index) {
        if(index == inventory.Count) {
            return equippedGun;
        } else if(index > -1 && inventory[index] != null) {
            return inventory[index].item;
        } else {
            return null;
        }
    }

    public void AddItem(Item item, int stack=1) {

        if(item is GunItem) {
			if (equippedGun != null)
			{
				Debug.Log("Gun in hand," + equippedGun.name +" " + (equippedGun as Item).description + ")" + ", so dropping it first...(");
				DropItem(10); // Drop the "GUN" in hand
			} else
			{
				Debug.Log("Player doesn't appear to be holding a gun, so... ");

			}


			Debug.Log("Equipping picked gun " +item.name);
			equippedGun = item as GunItem;
            gunController.InitGun();

            return;
        }

        for(int k = 0; k < stack; k++) {

            for(int i = 0; i < inventory.Count; i++) {

                if(inventory[i] != null && inventory[i].item != null) {

                    if(item.Equals(inventory[i].item) && inventory[i].stack < inventory[i].item.maxStackSize) {
                        inventory[i].stack ++;
                        break;
                    }


                } else {
                    inventory[i] = new InventorySlot(item, 1);
                    break;
                }

            }

            CompoundInventory();
        }

    }

    public void DropItemAll(string name) {
        int[] indexes = GetIndexesOfItem(name);

        foreach(int i in indexes) {
            DropItemAll(i);
        }
    }

	public GameObject DropGameObject(string ResourceID, int count = 1/*Count Not implemented*/)
	{
		Debug.Log("Creating game object from item at "+gameObject.name+"'s position.");
		GameObject drop = Instantiate(ResourceManager.instance.getResource(ResourceID), gameObject.transform.position, gameObject.transform.rotation);
		Debug.Assert(drop != null, "drop shouldn't be null. It didn't load correctly as a resource.");
		drop.SetActive(true);
		return drop;
	}

	public void DropGun()
	{
		Debug.Log("DroppingGun");
		if (equippedGun.clipSize >= 32) // This bool is probably not good enough long-term
		{
			DropGameObject("Pickup_Assault_Rifle");
		}
		else
		{
			DropGameObject("Pickup_Handgun");
		}

		equippedGun = null;
		gunController.InitGun();
	}

	public void DropGameObject(Item item)
	{
		GameObject itemPrefab;
		String ResourceID = string.Empty	;
		// ToDo: The Rest of them
		if (item.name == "Rifle Ammo") // This method of picking is probably not good enough long-term
		{
			Debug.Log("Rifle Ammo Dropped, but resource doesn't exist yet.");
			ResourceID = "Pickup_Rifle_Ammo";

			//Also need to implement junk for flashlight & glowstick
		}
		else if (item.name == "Assault_Rifle")
		{
			ResourceID = "Pickup_Assault_Rifle";
		}
		else if (item.name == "Handgun")
		{
			ResourceID = "Pickup_Handgun";
		}
		else if (item.name == "Basic Medkit")
		{
			ResourceID = "Pikcup_Medkit_Basic";
		}
		else if (item.name == "Pills")
		{
			ResourceID = "Pickup_Pills";
		}
		else if (item.name == "Shotgun Ammo")
		{
			ResourceID = "Pickup_Shotgun_Ammo";
		}
		else
		{
			Debug.Assert(false, "Error in inventory.cs: DropGameObject() logic not completed for object, " + item.name);
			return;
		}
		DropGameObject(ResourceID);
	}

	public void DropItemAll(int index)
	{
		Debug.Log("Root of DropItem func reached");
        if(index>=inventory.Count)
		{
			Debug.Log("-> Determined that you want to drop a gun...");
			DropGun();
		}
		else if(index > -1)
		{
			Debug.Log("-> Determined that you want to drop an item from your inventory...");
			if (inventory[index].item != null)
			{
				string name = inventory[index].item.name;
				Debug.Log("-> Dropping item: " + name);
				DropGameObject(inventory[index].item);
				inventory.RemoveAt(index);
                inventory.Add(new InventorySlot());
            }
        }

    }

    public void DropItem(string name, int amount = 1) {
		Debug.Log("Attempting to drop " + name);
        int x = amount;

        while(x > 0) {

            int index = GetIndexOfItem(name);

            if(index > -1) {
				Debug.Log("Item was found to be at index:" + index);
                DropItem(index, 1);
                x--;
            } else {
				Debug.Log("Item index was not found");
                break;
            }

        }

        Sort();

    }

    public void DropItem(int index, int amount = 1) {

        for(int i = 0; i < amount; i++) {

            if(index>=inventory.Count) {
				DropItemAll(index);
                return;
            } else if(index > -1) {
                if(inventory[index].item != null && inventory[index].stack > 1) {
                    inventory[index].stack--;
					Debug.Log("Appears to be dropping part of a stack... Must remember to create the game object for it.");
                } else {
                    DropItemAll(index);
                    return;
                }
            }

        }

    }

    public void UseItem(int index) {

        Item item = GetItemFromIndex(index);
        if(item != null) {
            if(item.Use()) {
                DropItem(index);
            }
        }

    }

    public bool ContainsItem(string name) {
        int index = GetIndexOfItem(name);

        return (index != -1);

    }

    public int GetEquippedGunAmmo() {

        return GetQuantityOfItem(equippedGun.ammunitionType.name);
    }

    public int GetQuantityOfItem(string name) {
        int[] indexes = GetIndexesOfItem(name);

        if(indexes.Length == 0) {
            return 0;
        }

        int quantity = 0;

        for(int i = 0; i < indexes.Length; i++) {
            quantity += inventory[indexes[i]].stack;
        }

        return quantity;
    }

    public int[] GetIndexesOfItem(string name) {
        List<int> indexes = new List<int>();
        for(int i = 0; i < inventory.Count; i++) {
            if(inventory[i].item != null && name.Equals(inventory[i].item.name)) {
                indexes.Add(i);
            }
        }

        return indexes.ToArray();
    }

    public void ChangeSortType(int sortTypeIndex) {
        sortType = (SortType) sortTypeIndex;
        Sort();
    }

    public void Sort() {
        CompoundInventory();
        inventory.Sort();
    }

    public int GetIndexOfItem(string name) {
        for(int i = 0; i < inventory.Count; i++) {
            if(inventory[i].item != null && name.Equals(inventory[i].item.name)) {
                return i;
            }
        }

        return -1;
    }

    private void CompoundInventory() {
        for(int i = 0; i < inventory.Count; i++) {

            if(inventory[i].item != null) {
                int[] indexes = GetIndexesOfItem(inventory[i].item.name);

                if(indexes.Length > 1) {
                    for(int y = 0; y < indexes.Length-1; y++) {
                        while(inventory[indexes[y]].stack < inventory[indexes[y]].item.maxStackSize && inventory[indexes[y+1]].stack>0) {
                            inventory[indexes[y]].stack++;
                            inventory[indexes[y+1]].stack--;
                        }
                        while(inventory[indexes[y]].stack > inventory[indexes[y]].item.maxStackSize) {
                            inventory[indexes[y]].stack--;
                            inventory[indexes[y+1]].stack++;
                        }

                    }
                }

            }
        }

        //If overflow, drag to another slot
        for(int i = 0; i < inventory.Count; i++) {
            if(inventory[i].item != null && inventory[i].stack > 0) {
                if(inventory[i].stack > inventory[i].item.maxStackSize) {
                    int dif = inventory[i].stack - inventory[i].item.maxStackSize;
                    inventory[i].stack = inventory[i].item.maxStackSize;
                    AddItem(inventory[i].item, dif);
                }
            }
        }

        //Cull nulls
        CullNulls();
    }

    private void CullNulls() {
        for(int i = 0; i < inventory.Count; i++) {
            if(!(inventory[i] != null && inventory[i].item != null && inventory[i].stack > 0)) {
                DropItemAll(i);
            }
        }
    }

    private void EmptySlot(int index) {
        inventory[index].item = null;
        inventory[index].stack = 0;
    }

    public void PrintInventory() {

        for(int i = 0; i < inventory.Count; i++) {
            if(inventory[i].item != null) {
                Debug.Log(inventory[i].item.name + " x " + inventory[i].stack + " : " + inventory[i].item.description);
            } else {
                Debug.Log("EMPTY");
            }
        }
    }

}
