using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    public int index;
    public Image itemImage;
    public Dropdown dropdown;

    public GameObject quantityObject;
    public TextMeshProUGUI quantityText;



    public void SetItemContained(bool b) {
        if(b) {
            itemImage.enabled = true;
            dropdown.interactable = true;
            quantityObject.SetActive(true);
        } else {
            itemImage.enabled = false;
            dropdown.interactable = false;
            quantityObject.SetActive(false);
        }
    }

    public void SetSelected() {
        Debug.Log("Selecting " + gameObject.name);
        dropdown.Select();
        InventoryUI.instance.SetSelectedItemIndex(index);
    }

    public void ClearSelection() {
        EventSystem.current.SetSelectedGameObject(null);
        InventoryUI.instance.ClearSelection();
    }

    public void OnDropdownSelection(int selection) {
        dropdown.value = 3;
        switch(selection) {
            case 0:
                Inventory.instance.UseItem(index);
            break;
            case 1:
                Inventory.instance.DropItem(index);
            break;
            case 2:
                Inventory.instance.DropItemAll(index);
            break;
            default:
            break;
        }
    }

}
