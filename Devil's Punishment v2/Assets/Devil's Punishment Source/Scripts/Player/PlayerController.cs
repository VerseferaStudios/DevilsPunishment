using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Range(0f, 3f)]
    public float movementSpeed;
    public float movementSmoothingSpeed = 4.0f;

    public ToggleHold sprintTH;
    public ToggleHold crouchTH;

    [Range(.1f, 1.0f)]
    public float lookSensitivity = 1.0f;
    [Range(.1f, 1.0f)]
    public float lookSensitivityAimingMultiplier = .5f;
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
    private bool isMoving;

    private float horizontalAngle;
    private float verticalAngle;

    private float verticalAngleSubtractive;

    private GunController gunController;

    public static PlayerController instance;

    void Awake() {
        instance = this;
    }

    void Start() {

        headCamera = GetComponentInChildren<Camera>();
        controller = GetComponent<CharacterController>();
        gunController = GunController.instance;

    }

    void Update() {
        GatherInput();
        if(!isClimbing) {
            Locomotion();
        }
        VerticalLocomotion();
        Turning();
        //Animation();
        CameraUpdate();
    }

    public void AddToViewVector(float x, float y) {
        horizontalAngle += x;
        verticalAngle += y;
    }

    public void AddToVerticalAngleSubractive(float v) {
        verticalAngleSubtractive += v;
    }

    public void AddToViewVector(Vector2 v) {
        AddToViewVector(v.x, v.y);
    }

    public bool IsSprinting() {return isSprinting;}
    public bool IsCrouching() {return isCrouching;}
    public bool IsMoving() {return isMoving;}

    public void ToggleCrouch() {
        isCrouching = !isCrouching;
        CrouchControllerColliderHeight();
    }
    public void ToggleSprinting() {isSprinting = !isSprinting;}


    void GatherInput() {
        if(inputEnabled) {
            movementInputRaw = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            switch(sprintTH) {
                case ToggleHold.TOGGLE:
                    if(Input.GetButtonDown("Crouch")){
                        ToggleCrouch();
                    }
                break;
                default:
                case ToggleHold.HOLD:
                    isCrouching = Input.GetButton("Crouch");
                break;
            }



            switch(sprintTH) {
                case ToggleHold.TOGGLE:
                    if(Input.GetButtonDown("Sprint")){
                        ToggleSprinting();
                    }
                break;
                default:
                case ToggleHold.HOLD:
                    isSprinting = Input.GetButton("Sprint");
                break;
            }

            if(movementInputRaw.y <= 0) { isSprinting = false; }

            float aimMultiplier = Mathf.Lerp(1.0f, lookSensitivityAimingMultiplier, gunController.GetAimingCoefficient());

            horizontalAngle += Input.GetAxisRaw("Mouse X") * Time.deltaTime * lookSensitivity * lookSensitivityHorizontal * aimMultiplier;
            verticalAngle += (invertY? 1.0f : -1.0f) * Input.GetAxisRaw("Mouse Y") * Time.deltaTime * lookSensitivity * lookSensitivityVertical * aimMultiplier;
        } else {
            movementInputRaw = Vector2.zero;
        }
    }

    void Locomotion() {

        Vector2 movementDirection = movementInputRaw.normalized;
        float generalSpeedMultiplier = 1.0f *
            (isCrouching? .5f : 1.0f) *
            (isSprinting? 2f : 1.0f) *
            (1.0f - .5f * gunController.GetAimingCoefficient());

        float targetSpeed = movementSpeed * generalSpeedMultiplier;
        float targetAnimationSpeed = movementDirection.sqrMagnitude * (isSprinting? 2.0f : 1.0f) * Mathf.Sign(movementInputRaw.y);

        speed = Mathf.Lerp(speed, targetSpeed, Time.deltaTime * movementSmoothingSpeed);
        animationSpeed = Mathf.Lerp(animationSpeed, targetAnimationSpeed, Time.deltaTime * movementSmoothingSpeed * 2f);

        Vector3 velocity =
        ((movementDirection.y * transform.forward) + (movementDirection.x * transform.right))
         * speed * Time.deltaTime;

        isMoving = (velocity.sqrMagnitude > 0f);

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

        verticalAngleSubtractive = Mathf.Clamp(verticalAngleSubtractive, 0f, 5f);

        verticalAngle = Mathf.Clamp(verticalAngle, -80f, 80f);
        headCamera.transform.localEulerAngles = new Vector3(verticalAngle-verticalAngleSubtractive, 0f, 0f);

    }

    void Animation() {

        characterAnimator.SetFloat("Speed", animationSpeed);
        characterAnimator.SetBool("IsCrouching", isCrouching);
        characterAnimator.SetBool("IsClimbing", isClimbing);
    }

    void CameraUpdate() {

        Vector3 targetCameraPosition = new Vector3(0, isCrouching? 1.0f : 1.5f, .25f);
        headCamera.transform.localPosition = Vector3.Lerp(headCamera.transform.localPosition, targetCameraPosition, Time.deltaTime * 7.0f);

    }

    void CrouchControllerColliderHeight() {
        if(isCrouching) {
            controller.center = Vector3.up * .625f;
            controller.height = 1.25f;
        } else {
            controller.center = Vector3.up * .875f;
            controller.height = 1.75f;
        }
    }
}
