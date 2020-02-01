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

        public  int CompareTo(InventorySlot other) {

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
        OrganizeInventory();
	}
    private void OrganizeInventory(){
        CompoundInventory();
        CullNulls();
        Sort();
    }

    private void Start()
	{
        //StartCoroutine(DebugInvi());
	}

    private IEnumerator DebugInvi()
    {
        int ctr = 0;
        while (ctr < 20)
        {
            ctr++;
            yield return new WaitForSeconds(5);
            for (int i = 0; i < inventory.Count; i++)
            {
                if(inventory[i] != null)
                {
                    if(inventory[i].item != null)
                    {
                        Debug.Log("item = " + inventory[i].item.name + " " + inventory[i].item.description + " " + inventory[i].item.maxStackSize);
                        Debug.Log("stack = " + inventory[i].stack);
                    }
                    else
                    {
                        Debug.Log("Invi [" + i + "] item =  null");
                    }
                }
                else
                {
                    Debug.Log("Invi [" + i + "] (slot) =  null");
                }
            }
        }
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
	public bool hasSpace() {return size < inventory.Count; }

    // The direct value of "inventory.Count" is NOT valid, since the inventory list ends with Three special slots (for gen parts);
    // So use this "PseudoCount()" function everywhere you would normally be using inventory.Count; Except when inventory.Count is referring to the "gun slot"; I apologize for this being so confusing...
    private int PseudoCount(){ return inventory.Count - 3; }



    private bool ItemIsSpecial_HandleSeparately(Item item, int stack=1){


        // The "special item", "gun", RESERVES THE LAST INDEX OF THE INVENTORY, but ISN'T actually "stored" there (stored in 'equippedGun' variable).
        // This item should be excluded from "inventory size".
        // The last index should be excluded from "compounding" and "sorting" algorithms.
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
			
            return true;
        }

        // The three "special items", "generator parts", RESERVE THE 3 LAST INVENTORY SPOTS BEFORE THE "GUN_INDEX", and ARE actually stored there.
        // These items should be excluded from "inventory size"
        // Their reserved indexes should also be excluded from "compounding" and "sorting" algorithms
        if (item is GeneratorPart){
            // Calculate the index that it belongs to.
            int genPartIndex =-1;
            switch(item.name){
                case "Generator Part A":
                    genPartIndex = 10;
                   
                    break;
                case "Generator Part B":
                    genPartIndex = 11;
                    
                    break;
                case "Generator Part C":
                    genPartIndex = 12;
                    
                    break;
                default:
                    break;
            }
            // Just comsume duplicate gen parts... shouldn't happen, but just do it
            // Just put the gen part in the slot it belongs in. Nothing else should be there.
            inventory[genPartIndex] = new InventorySlot(item, 1);
            return true;
        }
        return false;
    }
    public void AddItem(Item item, int stack=1) {
        Debug.Log("Add ITEM!!!!!!!!!!!!!!!!!!!!!!");
        if (ItemIsSpecial_HandleSeparately(item,stack)){
            return;
        }

        // If the "item" makes it here, it should be a "regular" item that doesn't have a reserved index position.
        // Might as well just add it to an empty spot, or combine it with something that's already there.
        // First, to make sure that the invenory REALLY hasSpace() for the item, we have to account for the fact that the only empty space might be a RESERVED "generator slot"
        int genPartCount = 0;
        // Loop over the last three inventory slots, reserved for gen parts, and count how many there are
        for (int i = inventory.Count - 4; i < inventory.Count; i++)
        {
            if (inventory[i] != null && inventory[i].item != null && inventory[i].item is GeneratorPart)
            {
                genPartCount++;
            }
        }

        // account for existence of gen parts, and see if there's still room in the inventory.
        Debug.Log("Size is: "+size);
        Debug.Log("INVI Size is: "+inventory.Count);
        Debug.Log("GenPartCount is: "+genPartCount);
        bool hasSpace = size + (3-genPartCount) < inventory.Count;
        // If it turns out you didn't have space, after all, drop the item back into the world, and stop here.
        if (!hasSpace){
            DropGameObject(item,stack);
            return;
        }
        // Otherwise you're free to add the item to the inventory.
        bool isBreakOuter = false;
        for(int k = 0; k < stack; k++) {

            for(int i = 0; i < PseudoCount(); i++) {

                if(inventory[i] != null && inventory[i].item != null) {

                    if(item.Equals(inventory[i].item))
                    {
                        if(inventory[i].stack < inventory[i].item.maxStackSize)
                        {
                            inventory[i].stack++;
                            Debug.Log("STACK++ !!!!!!!!!!!!!!!!!!!!!");
                            break;
                        }
                        else
                        {
                            /*
                            Debug.Log("q34e2wr2qw2rwq2r23r1r21!!!!!!!!!!!!!!!!!!!!!!");
                            if (isFirst)
                            {
                                inventory[i] = new InventorySlot(item, 1);
                            }
                            else
                            {
                                inventory[i] = 
                            }
                            */
                        }
                    }


                } else {
                    inventory[i] = new InventorySlot(item, stack);
                    Debug.LogError("Item "+item.name + " added to stack " + stack + " in Slot " + i);
                    isBreakOuter = true;
                    break;
                }

            }
            if (isBreakOuter)
            {
                break;
            }
        }
        OrganizeInventory();
    }

    public void DropItemAll(string name) {
        int[] indexes = GetIndexesOfItem(name);

        foreach(int i in indexes) {
            DropItemAll(i);
        }
    }

	public GameObject DropGameObject(string ResourceID, int count = 1)
	{   
        if (count < 1) return null;
		Debug.Log("Creating "+count+" "+ResourceID+"'s from item at "+gameObject.name+"'s position.");
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
                    Debug.Log("Keep unused ammo!!!!!!!!!!!!!!!");
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
        // This inventory.Count is referring to gunSlot, so don't replace with PseudoCount()
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
            /* Original
                inventory.RemoveAt(index);
                inventory.Add(new InventorySlot());
            */
                EmptySlot(index);


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
        Sort();
    }

    public void DropItem(int index, int amount = 1, bool consume = false) {
        // This inventory.Count is referring to gunSlot, so don't replace with PseudoCount()
		if (index >= inventory.Count)
		{
			DropItemAll(index,consume);
			return;
		}
		else if (index > -1)
		{
			if (inventory[index].item != null && inventory[index].stack > 1 && amount <= inventory[index].stack)
			{
                bool isAmmo = inventory[index].item is AmmoItem;
				inventory[index].stack -= amount;
                if (isAmmo){
                    transform.parent.GetComponentInChildren<GunController>().UpdateClipStock();
                }
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
        Sort();
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
        if (equippedGun == null) return 0;
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
        // I suppose this inventory.Count doesn't need to be replaced with psuedoCount either...
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
        inventory.Sort(0,PseudoCount(),null);
	}

    public int GetIndexOfItem(string name) {
        // I suppose this inventory.Count doesn't need to be replaced with PseudoCount() either...
        for(int i = 0; i < inventory.Count; i++) {
            if(inventory[i].item != null && name.Equals(inventory[i].item.name)) {
                return i;
            }
        }

        return -1;
    }
    // CompoundInventory goes step-by-step through the inventory and combines like items into their "max-stack-size".
    private void CompoundInventory() {

        //First Make Sure Special Items (guns and gen parts) aren't mixed up in the regular inventory
        for(int i=0;i<inventory.Count;i++){
            // Check if it's special
            if(inventory[i].item != null && ( inventory[i].item is GunItem || inventory[i].item is GeneratorPart ) )  {
                // Re-add it to inventory, so it goes where it's supposed to.
                InventorySlot toPlace = inventory[i];
                inventory[i] = new InventorySlot();
                Debug.Log("ReAdd SPEACIAL ITEM!!!!!!!");
                AddItem(toPlace.item,toPlace.stack);
            }
        }
        Debug.Log("Compounding InVI !");

        // Then combine like items, that aren't special items...
        for (int i = 0; i < PseudoCount(); i++) {

            if(inventory[i].item != null) {

                int[] indexes = GetIndexesOfItem(inventory[i].item.name);

                if(indexes.Length > 1) {
                    for(int y = 0; y < indexes.Length-1; y++) {
                        // Look at each two slots of the same item and put it all in the "first-most" slot.
                        while(inventory[indexes[y]].stack < inventory[indexes[y]].item.maxStackSize && inventory[indexes[y+1]].stack>0) {
                            inventory[indexes[y]].stack++;
                            inventory[indexes[y+1]].stack--;
                        }
                    }
                }

            }
        }

    }

    //
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
