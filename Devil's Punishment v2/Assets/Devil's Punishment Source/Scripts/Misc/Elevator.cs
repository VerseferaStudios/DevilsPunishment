using UnityEngine;
using System.Collections;

public class Elevator : MonoBehaviour
{
    [Header("General")]
    public bool canUse = false;
    [SerializeField] private bool startElevator = false;
    private bool elevatorDown = true;

    [Header("Moving Elevator")]
    [SerializeField] Vector3 elevatorStartPos;
    [SerializeField] Vector3 elevatorEndPos;
    [SerializeField] float moveSpeed = 5f;

    [Header("Text")]
    [SerializeField] GameObject noPowerText = null;
    [SerializeField] float delay = 1f;

    private void Start()
    {
        elevatorEndPos = new Vector3(0, 15, 8.94f);
        elevatorStartPos = new Vector3(0, 0, 8.94f);
    }

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
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, elevatorEndPos, moveSpeed * Time.deltaTime);
            ReachedPosition(elevatorEndPos);
        }
        else if (!elevatorDown)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, elevatorStartPos, moveSpeed * Time.deltaTime);
            ReachedPosition(elevatorStartPos);
        }
    }

    private void ReachedPosition(Vector3 pos)
    {
        if(transform.localPosition == pos)
        {
            startElevator = false;
            elevatorDown = !elevatorDown;
        }
    }
}
