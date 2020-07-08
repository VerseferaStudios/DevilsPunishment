using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{

    private int selectedItemIndex;


    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;

    public static InventoryUI instance;

    private InventorySlotUI[] inventorySlots;

    private RectTransform rectTransform;
    [SerializeField] private GameObject tooltipPanel;
    Inventory inventory;
    GunController gunController;

    private void OnEnable()
    {
        PlayerController_Revamped.CallbackAssignStaticInstances += AssignInstance;
    }
    private void OnDestroy()
    {
        PlayerController_Revamped.CallbackAssignStaticInstances -= AssignInstance;
    }
    private void AssignInstance()
    {
        instance = this;
    }

    void Awake() {
        rectTransform = GetComponent<RectTransform>();
        inventorySlots = GetComponentsInChildren<InventorySlotUI>();
        tooltipPanel.SetActive(false);
        itemNameText.text = "";
        itemDescriptionText.text = "";
    }

    void Start() {
        gunController = GunController.instance;
        inventory = Inventory.instance;
    }

    void Update() {
        DrawInventory();
    }

    public void SetSelectedItemIndex(int i) {
        selectedItemIndex = i;
        UpdateTooltip();
        //FMOD AUDIO EVENT - UI Hover Sound
        FMOD.Studio.EventInstance UiStart = FMODUnity.RuntimeManager.CreateInstance("event:/UI/Inventory/UI_Hover");
        UiStart.start();
        UiStart.release();
    }

    public void Show() {
        rectTransform.anchoredPosition = new Vector3(64,0,0);
    }

    public void Hide() {
        Debug.Log("Who calls us ?!");
       // rectTransform.anchoredPosition = new Vector3(-3000,0,0);
    }

    private string QuantityTextGenerator(int stack, int maxStackSize) {

        string ret = "";
        if(stack == 0) {ret="<color=red>";}
        else if(stack == maxStackSize) {ret="<color=green>";}

        ret += stack;
        return ret;
    }

    private void DrawInventory() {
        for(int i = 0; i < inventory.inventory.Count; i++) {
            if(inventory.inventory[i] != null && inventory.inventory[i].item != null) {
                /*
                Debug.Log("i = " + i + " " + inventory.inventory.Count);
                foreach (var item in inventorySlots)
                {
                    Debug.Log(item.gameObject.name);
                }
                */
                inventorySlots[i].SetItemContained(true);
                inventorySlots[i].itemImage.sprite = inventory.inventory[i].item.itemIcon;
                inventorySlots[i].itemImage.color = inventory.inventory[i].item.color;
                inventorySlots[i].quantityText.text = QuantityTextGenerator(inventory.inventory[i].stack, inventory.inventory[i].item.maxStackSize);
            } else {
                inventorySlots[i].SetItemContained(false);
            }

        }

        if(inventory.equippedGun != null) {
            inventorySlots[inventory.inventory.Count].SetItemContained(true);
            inventorySlots[inventory.inventory.Count].itemImage.sprite = inventory.equippedGun.itemIcon;
            inventorySlots[inventory.inventory.Count].quantityText.text = QuantityTextGenerator(gunController.GetClip(), -1);


        } else {
            inventorySlots[inventory.inventory.Count].SetItemContained(false);
        }
    }

    public void ClearSelection() {
        selectedItemIndex = -1;
        tooltipPanel.SetActive(false);
        itemNameText.text = "";
        itemDescriptionText.text = "";
    }

    private void UpdateTooltip() {
        Item item = inventory.GetItemFromIndex(selectedItemIndex);

        if(item != null) {
            tooltipPanel.SetActive(true);
            itemNameText.text = item.name;
            itemDescriptionText.text = item.description;
            return;
        }
        else
        {
            tooltipPanel.SetActive(false);
            itemNameText.text = "";
            itemDescriptionText.text = "";
        }

        
    }



}
