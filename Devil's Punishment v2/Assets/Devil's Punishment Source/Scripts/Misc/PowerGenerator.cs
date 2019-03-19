using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class PowerGenerator : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] GameObject txtPowerOn = null;
    [SerializeField] GameObject txtMissingPart = null;
    [SerializeField] float delay = 1f;
    [Header("Event")]
    public UnityEvent powerOnEvent = null;
    bool returnedPartA = false;
    bool returnedPartB = false;
    bool returnedPartC = false;
    bool powerOn = false;

    public void OnUse()
    {
        ActivateParts();
        if (CheckIfAllPartsAreReturned() && !powerOn)
        {
            TriggerEvent();
        }
        else if (!powerOn)
        {
            StartCoroutine(DisplayText(txtMissingPart));
        }
    }

    private void ActivateParts()
    {
        if (!returnedPartA) { ActivatePartA(); }
        if (!returnedPartB) { ActivatePartB(); }
        if (!returnedPartC) { ActivatePartC(); }

    }

    private void ActivatePartA()
    {
        if (Inventory.instance.ContainsItem("Generator Part A"))
        {
            int index = Inventory.instance.GetIndexOfItem("Generator Part A");
            Inventory.instance.DropItem(index);
            returnedPartA = true;
            Debug.Log("Part A returned");
        }
    }

    private void ActivatePartB()
    {
        if (Inventory.instance.ContainsItem("Generator Part B"))
        {
            int index = Inventory.instance.GetIndexOfItem("Generator Part B");
            Inventory.instance.DropItem(index);
            returnedPartB = true;
            Debug.Log("Part B returned");
        }
    }

    private void ActivatePartC()
    {
        if (Inventory.instance.ContainsItem("Generator Part C"))
        {
            int index = Inventory.instance.GetIndexOfItem("Generator Part C");
            Inventory.instance.DropItem(index);
            returnedPartC = true;
            Debug.Log("Part C returned");
        }
    }

    private bool CheckIfAllPartsAreReturned()
    {
        if (returnedPartA && returnedPartB && returnedPartC) { return true; }
        else { return false; }
    }

    private void TriggerEvent()
    {
        StartCoroutine(DisplayText(txtPowerOn));
        powerOnEvent.Invoke();
    }

    IEnumerator DisplayText(GameObject text)
    {
        text.SetActive(true);
        yield return new WaitForSecondsRealtime(delay);
        text.SetActive(false);
    }
}
