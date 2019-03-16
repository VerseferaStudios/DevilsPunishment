using UnityEngine;
using System.Collections;

public class Elevator : MonoBehaviour
{
    [Header("General")]
    public bool canUse = false;
    private bool startElevator = false;
    private bool elevatorDown = true;

    [Header("Moving Elevator")]
    [SerializeField] Transform elevatorStartPos = null;
    [SerializeField] Transform elevatorEndPos = null;
    [SerializeField] float moveSpeed = 20f;

    [Header("Text")]
    [SerializeField] GameObject noPowerText = null;
    [SerializeField] float delay = 1f;

    private void Update()
    {
        if (startElevator)
        {
            MoveElevator();
        }
    }

    public void OnPowerOn()
    {
        canUse = true;
    }

    public void ActivateElevator()
    {
        if (canUse)
        {
            startElevator = true;
        }
        else
        {
            StartCoroutine(DisplayNoPowerText());
        }
    }

    IEnumerator DisplayNoPowerText()
    {
        noPowerText.SetActive(true);
        yield return new WaitForSecondsRealtime(delay);
        noPowerText.SetActive(false);
    }

    private void MoveElevator()
    {
        if (elevatorDown)
        {
            transform.position = Vector3.MoveTowards(transform.position, elevatorEndPos.position, moveSpeed * Time.deltaTime);
            ReachedPosition(elevatorEndPos);
        }
        else if (!elevatorDown)
        {
            transform.position = Vector3.MoveTowards(transform.position, elevatorStartPos.position, moveSpeed * Time.deltaTime);
            ReachedPosition(elevatorStartPos);
        }
    }

    private void ReachedPosition(Transform pos)
    {
        if(transform.position == pos.position)
        {
            startElevator = false;
            elevatorDown = !elevatorDown;
        }
    }
}
