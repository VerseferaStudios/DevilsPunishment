using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Range(0f, 3f)]
    public float movementSpeed;
    public float movementSmoothingSpeed = 4.0f;

    [Range(.1f, 1.0f)]
    public float lookSensitivity = 1.0f;
    public float lookSensitivityHorizontal = 3.0f;
    public float lookSensitivityVertical = 4.0f;
    public bool invertY;

    [HideInInspector]
    public bool inputEnabled = true;

    private bool isClimbing;
    private float originalHeight;

    public Animator characterAnimator;
    private CharacterController controller;
    private Camera headCamera;

    private Vector2 movementInputRaw;
    private float speed;
    private float animationSpeed;
    private bool isCrouching;
    private bool isSprinting;

    private float horizontalAngle;
    private float verticalAngle;

    void Start() {

        headCamera = GetComponentInChildren<Camera>();
        controller = GetComponent<CharacterController>();

    }

    void Update() {
        GatherInput();
        if(!isClimbing) {
            Locomotion();
        }
        VerticalLocomotion();
        Turning();
        Animation();
    }

    public void ToggleCrouch() {isCrouching = !isCrouching;}
    public void ToggleSprinting() {isSprinting = !isSprinting;}


    void GatherInput() {
        if(inputEnabled) {
            movementInputRaw = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            if(Input.GetButtonDown("Crouch")) {
                ToggleCrouch();
            }

            isSprinting = Input.GetButton("Sprint");


            horizontalAngle += Input.GetAxisRaw("Mouse X") * Time.deltaTime * lookSensitivity * lookSensitivityHorizontal;
            verticalAngle += (invertY? 1.0f : -1.0f) * Input.GetAxisRaw("Mouse Y") * Time.deltaTime * lookSensitivity * lookSensitivityVertical;
        } else {
            movementInputRaw = Vector2.zero;
        }
    }

    void Locomotion() {

        Vector2 movementDirection = movementInputRaw.normalized;
        float generalSpeedMultiplier = 1.0f *
            (isCrouching? .5f : 1.0f) *
            (isSprinting? 2f : 1.0f);

        float targetSpeed = movementSpeed * generalSpeedMultiplier;
        float targetAnimationSpeed = movementDirection.sqrMagnitude * (isSprinting? 2.0f : 1.0f) * Mathf.Sign(movementInputRaw.y);

        speed = Mathf.Lerp(speed, targetSpeed, Time.deltaTime * movementSmoothingSpeed);
        animationSpeed = Mathf.Lerp(animationSpeed, targetAnimationSpeed, Time.deltaTime * movementSmoothingSpeed * 2f);

        Vector3 velocity =
        ((movementDirection.y * transform.forward) + (movementDirection.x * transform.right))
         * speed * Time.deltaTime;

        controller.Move(velocity);

    }

    void VerticalLocomotion() {
        if(!isClimbing) {
            controller.Move(Vector3.down * Time.deltaTime * 9.81f);
        } else {
            controller.Move(Vector3.up * Time.deltaTime * movementInputRaw.y);
        }
    }

    void Turning() {

        Vector3 targetEulerAngles = new Vector3(0f, horizontalAngle, 0f);
        Quaternion targetRotation = Quaternion.Euler(targetEulerAngles);
        transform.rotation = targetRotation;

        verticalAngle = Mathf.Clamp(verticalAngle, -90f, 80f);
        headCamera.transform.localEulerAngles = new Vector3(verticalAngle, 0f, 0f);

    }

    void Animation() {

        characterAnimator.SetFloat("Speed", animationSpeed);
        characterAnimator.SetBool("IsCrouching", isCrouching);
        characterAnimator.SetBool("IsClimbing", isClimbing);
    }

    // climb [This is assuming that we have a specific regions that are climbable tagged and indicated by colliders acting as triggers]
    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.tag == "Climbable")
        {
            isClimbing = true;
            Debug.Log("hit a ladder!");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Climbable")
        {
            Debug.Log("Triggered");
            transform.position += transform.forward * 0.5f;
            isClimbing = false;
        }
    }
}
