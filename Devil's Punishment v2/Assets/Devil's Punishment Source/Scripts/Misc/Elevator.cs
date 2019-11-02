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
    float moveSpeed = 0.5f;///2f;
    [SerializeField] float startTime;
    [SerializeField] float t = 0;

    [Header("Text")]
    [SerializeField] GameObject noPowerText = null;
    [SerializeField] float delay = 1f;

    public InteractableElevator interactableElevator;

    public Transform player, prevParent;

    private void Start()
    {
        elevatorEndPos = new Vector3(0, 15, 0);
        elevatorStartPos = new Vector3(0, 0, 0);
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
        Vector3 lerpVal;
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        prevParent = player.parent;
        player.parent = transform;
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
            player.parent = prevParent;
            t = 1.1f;
            //call open elevator door
            interactableElevator.CallOpenElevatorFns();
        }
    }
}
