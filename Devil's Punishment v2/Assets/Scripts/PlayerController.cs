using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Motion Variables")]
    [SerializeField]
    private float walkSpeed = 1f;

    [SerializeField]
    private float sprintSpeed = 1.5f;

    [Header("Crouch Variables")]
    [SerializeField]
    private float crouchSpeed = 0.5f;

    [SerializeField]
    private float crouchHeight = 0.5f;

    [Header("Climbing Variables")]
    [SerializeField]
    private float climbSpeed = 0.5f;

    [Header("Mouse variables")]
    [SerializeField]
    private float sensitivity = 60;

    //Private variables
    private new Rigidbody rigidbody;
    private new CapsuleCollider collider;
    private Animator anim;

    private bool isCrouching;
    private bool isSprinting;
    private bool isClimbing;

    private float originalHeight;

    private float speedForAnim;
    private int speedHash = Animator.StringToHash("Speed");
    private int crouchHash = Animator.StringToHash("Is Crouching");
    private int climbHash = Animator.StringToHash("Is Climbing");
    private int emoteHash = Animator.StringToHash("Is Emoting");

    // instantiates private variables
    public void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        anim = GetComponent<Animator>();
        isCrouching = false;
        isSprinting = false;
        isClimbing = false;
        originalHeight = collider.height;
    }

    public void Update()
    {
        // For rotating the object on horizontal axis [Gimball principle]
        transform.Rotate(new Vector3(0f, Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime, 0f));

        float xAxisValue = Input.GetAxis("Horizontal"); // [AD] Motion control input
        float yAxisValue = Input.GetAxis("Vertical"); // [WS] Motion control input

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetAxis("Vertical") > 0)
        {
            isSprinting = true;
            isCrouching = false;
        }

        // crouch logic
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;
            if (isCrouching) { collider.height = crouchHeight; }
            else { collider.height = originalHeight; }
            collider.center = new Vector3(0, collider.height/2, 0);
        }

        Vector3 newPosition = transform.position;

        // Walk and sprint
        if (yAxisValue != 0)
        {
            if (isClimbing)
            {
                newPosition += transform.up * yAxisValue * climbSpeed * Time.deltaTime;
            }
            else if (isSprinting)
            {
                newPosition += transform.forward * yAxisValue * sprintSpeed * Time.deltaTime;
            }
            else if (isCrouching)
            {
                newPosition += transform.forward * yAxisValue * crouchSpeed * Time.deltaTime;
            }
            else
            {
                newPosition += transform.forward * yAxisValue * walkSpeed * Time.deltaTime;
            }
        }

        if (xAxisValue != 0 && !isClimbing)
        {
            if (isCrouching)
            {
                newPosition += transform.right * xAxisValue * crouchSpeed * Time.deltaTime;
            }
            else
            {
                newPosition += transform.right * xAxisValue * walkSpeed * Time.deltaTime;
            }
        }

        transform.position = newPosition;

        // Setting anims
        anim.SetBool(crouchHash, isCrouching);
        anim.SetBool(climbHash, isClimbing);
        if (Input.GetAxis("Vertical") < 0) { speedForAnim = -6f; }
        else if (isSprinting) { speedForAnim = 3f; }
        else if(Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0) { speedForAnim = 0f; }
        else { speedForAnim = 1f; }
        anim.SetFloat(speedHash, speedForAnim);

        isSprinting = false; // So, it doesnt act like a trigger
    }

    // climb [This is assuming that we have a specific regions that are climbable tagged and indicated by colliders acting as triggers]
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Climbable")
        {
            isClimbing = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Climbable")
        {
            isClimbing = false;
        }
    }
}
