using UnityEngine;
using System.Collections;

public class Elevator : MonoBehaviour
{
    [Header("General")]
    public bool canUse = false;
    [SerializeField] private bool startElevator = false;
    public bool elevatorDown = true;

    [Header("Moving Elevator")]
    [SerializeField] Vector3 elevatorStartPos;
    [SerializeField] Vector3 elevatorEndPos;
    [SerializeField] float moveSpeed = 0.5f;
    [SerializeField] float startTime;
    [SerializeField] float t = 0;

    [Header("Text")]
    [SerializeField] GameObject noPowerText = null;
    [SerializeField] float delay = 1f;

    public InteractableDoor interactableElevator;

    private void Start()
    {
        elevatorEndPos = new Vector3(0, 15, 8.94f);
        elevatorStartPos = new Vector3(0, 0.06f, 8.94f);
    }

    private void Update()
    {

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
            startTime = Time.time;
            StartCoroutine(MoveElevator());
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

    private IEnumerator MoveElevator()
    {
        if (elevatorDown)
        {
            while(t < 1)
            {
                t = (Time.time - startTime) * moveSpeed / 15f;
                transform.localPosition = Vector3.Lerp(transform.localPosition, elevatorEndPos, t);
                ReachedPosition(elevatorEndPos);
                yield return new WaitForSeconds(0.01f);
            }
            t = 0;
        }
        else if (!elevatorDown)
        {
            while(t < 1)
            {
                t = (Time.time - startTime) * moveSpeed / 15f;
                transform.localPosition = Vector3.Lerp(transform.localPosition, elevatorStartPos, t);
                ReachedPosition(elevatorStartPos);
                yield return new WaitForSeconds(0.01f);
            }
            t = 0;
        }
    }

    private void ReachedPosition(Vector3 pos)
    {
        if(transform.localPosition == pos)
        {
            startElevator = false;
            elevatorDown = !elevatorDown;
            t = 1.1f;
            //call open elevator door
            interactableElevator.CallOpenElevatorFns();
        }
    }
}
