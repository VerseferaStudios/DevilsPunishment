using System;
using System.Collections;
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
		gunController = gameObject.transform.parent.GetComponentInChildren<GunController>();
		Debug.Assert(gunController != null, "gunController shouldn't be null!");
		Sort();
	}

    private void Start()
	{
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
	
	private int size;
	public bool hasSpace() { /*Debug.Log("Inventory size is: " + size);*/ return size < inventory.Count; }

    public void AddItem(Item item, int stack=1) {
        if(item is GunItem) {
			if (equippedGun != null)
			{
				//Debug.Log("Gun in hand," + equippedGun.name + " " + (equippedGun as Item).description + ")" + ", so dropping it first...(");
				DropGun();
			} else
			{
				//Debug.Log("Player doesn't appear to be holding a gun, so... ");
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
        // AddItem gets called like a billion times recursively, otherwise this would be a pretty good spot for this...
        // Something to do with how it "drags overflow to new spots by calling AddItem again"...
        // Also sorts the whole inventory...
        // Confirmed: This will break the algorithm that the inventory uses...For whatever reason, this messes stuff up...
        // gunController.UpdateClipStock();

    }

    public void DropItemAll(string name) {
        int[] indexes = GetIndexesOfItem(name);

        foreach(int i in indexes) {
            DropItemAll(i);
        }
    }

	public GameObject DropGameObject(string ResourceID, int count = 1)
	{
		//Debug.Log("Creating game object from item at "+gameObject.name+"'s position.");
		GameObject drop = Instantiate(ResourceManager.instance.getResource(ResourceID), gameObject.transform.position, gameObject.transform.rotation);
		//Debug.Assert(drop != null, "drop shouldn't be null. It didn't load correctly as a resource.");
		drop.GetComponent<InteractableLoot>().stock = count;
		drop.SetActive(true);
		return drop;
	}

	public void DropGun()
	{
		//Debug.Log("DroppingHeldGun");

		string ResourceID = string.Empty;
		Dictionary<string, GameObject>.KeyCollection resources = ResourceManager.instance.getResourceNamesList();
		foreach (string resource in resources)
		{
			GameObject resItem = ResourceManager.instance.getResource(resource);
			InteractableLoot lootComp = resItem.GetComponent<InteractableLoot>();
			if (equippedGun == lootComp.item as GunItem)
			{
				ResourceID = resource;
				//Debug.Log("Dropping gun gameObject into scene, but first, lets make sure we keep our unspent ammo.");
				if(gunController.equippedGun != null && gunController.equippedGun.gunItem != null && gunController.equippedGun.gunItem != null){
                    // Keep unused ammo...
					AddItem(gunController.equippedGun.gunItem.ammunitionType, gunController.GetClip());
                }
				equippedGun = null;
				gunController.InitGun();
				DropGameObject(resource);
				return;	
			}
		}
		Debug.Assert(ResourceID != string.Empty, "Error in inventory.cs: DropGun() logic not completed for object, " + ResourceID);
	}

	public void DropGameObject(Item item, int count = 1)
	{
		//Debug.Log("Dropping a Partial Stack of " + count + " " + item.name);
		string ResourceID = string.Empty;
		Dictionary<string, GameObject>.KeyCollection resources = ResourceManager.instance.getResourceNamesList();
		foreach (string resource in resources)
		{
			GameObject resItem = ResourceManager.instance.getResource(resource);
			InteractableLoot lootComp = resItem.GetComponent<InteractableLoot>();
			if (lootComp == null)
			{
				continue;
			}
			if (item == lootComp.item)
			{
				ResourceID = resource;
				DropGameObject(resource, count);
				return;
			}
		}
		Debug.Assert(ResourceID != string.Empty, "Error in inventory.cs: DropGameObject() logic not completed for object, " + ResourceID);

	}

	public void DropItemAll(int index, bool consume = false)
	{
		//Debug.Log("Root of DropItem func reached");
        if(index>=inventory.Count)
		{
			//Debug.Log("-> Determined that you want to drop a gun...");
			DropGun();
		}
		else if(index > -1)
		{
			if (inventory[index].item != null)
			{
				//Debug.Log("-> Determined that you want to drop an item from your inventory at position" + index.ToString() + "...");
				string name = inventory[index].item.name;
				//Debug.Log("-> Dropping item: " + name);
				if (!consume && inventory[index].stack > 0)
				{
					DropGameObject(inventory[index].item,inventory[index].stack);
				}
				inventory.RemoveAt(index);
				inventory.Add(new InventorySlot());
				size--;
			}
        }
    }

    public void DropItem(string name, int amount = 1, bool consume = false)
	{
        int dropped = 0;
        while (dropped < amount){
            int index = GetIndexOfItem(name);
            if (inventory[index].item != null && inventory[index].stack >= 1){
                int thisAmt = Math.Min(amount-dropped,inventory[index].stack);
                dropped+= thisAmt;
                DropItem(index, thisAmt, consume);
                Sort();
                CullNulls();
            }
        }
    }

    public void DropItem(int index, int amount = 1, bool consume = false) {
		if (index >= inventory.Count)
		{
			DropItemAll(index,consume);
			return;
		}
		else if (index > -1)
		{
			if (inventory[index].item != null && inventory[index].stack > 1 && amount <= inventory[index].stack)
			{
				inventory[index].stack -= amount;
				if (!consume)
				{
					DropGameObject(inventory[index].item, amount);
				}
			}
			else
			{
				DropItemAll(index, consume);
				return;
			}
		}
	}

    public void UseItem(int index) {

        Item item = GetItemFromIndex(index);
        if(item != null) {
            if(item.Use()) {
                DropItem(index,/*amount*/1,/*consume*/true);
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
		size = 0;
        for(int i = 0; i < inventory.Count; i++) {
            if(inventory[i] == null || inventory[i].item == null || inventory[i].stack <= 0) {
                DropItemAll(i);
			}
			else
			{
				size++;
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
