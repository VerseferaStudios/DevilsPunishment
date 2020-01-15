using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Range(0f, 30f)]
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

    //Drug 10/02/2019
    PlayerControls Controls;
    PlayerControls.InputDevice inputDev;
    public float inputAngle;

    [HideInInspector]
    public bool inputEnabled = true;

    private bool isClimbing;
    private float originalHeight;

    public GameObject playerModel;
    private Animator characterAnimator;
    private CharacterController controller;
    private Camera headCamera;

    private Vector2 movementInputRaw;
    private float speed;
    private float forwardAnimationSpeed;
    private float rightAnimationSpeed;
    private float climbSpeed;
    private bool isCrouching;
    private bool isSprinting;
    private bool isMoving;

    private float horizontalAngle;
    private float verticalAngle;

    private float verticalAngleSubtractive;


    public static PlayerController instance; // wouldn't work with networking!!!

    public bool isInteractLaser = false;
    public Vector3 laserSpot, laserMonitor;
    public LaserCutter laserCutterScript;

    public bool shadowOnly = false;
    void Awake()
    {


        instance = this;
        characterAnimator = playerModel.GetComponent<Animator>();
        headCamera = GetComponentInChildren<Camera>();
        controller = GetComponent<CharacterController>();
    }

    void Start()
    {
        Controls = ControlsManager.instance.claimPlayer();
        inputDev = Controls.input;
        if (Data.instance != null){
           // Data.instance.playerController = this;
        } else {
            Debug.Log("Unable to set 'Data.instance.playerController' Data instance not found.");
        }
    }

    Vector3 startVel;
    int stoppingPoint = 0;
    void Update() 
   {

	if (isMoving == true)
        {
            print("Walking");
        }
        if (isMoving == false)
        {
            print("Stopped");
        }
/*
        if(slowdown)
        {
            GetComponent<CharacterController>().Move(-GetComponent<CharacterController>().velocity);
        }
        */
        GatherInput();
        if (Input.GetKeyUp(KeyCode.E))
        {
            isInteractLaser = false; // do in other script if needed
        }
        if (isInteractLaser)
        {
            Debug.Log("moving player");// + Vector2.Distance(laserSpot, transform.position));
            //movementInputRaw = new Vector2(1, 1);
            movementInputRaw = new Vector2(0, 1);//new Vector2(-2, 0) - new Vector2(transform.position.x, transform.position.z) - new Vector2(0, 0)/*(0, 0) is the position of InteractableLaser Script*/;
            horizontalAngle = Quaternion.LookRotation(laserSpot - transform.position, transform.up).eulerAngles.y;
            //Debug.Log(horizontalAngle);
            Turning();
            Locomotion();
            if (Vector2.Distance(laserSpot, transform.position) < 1.18f) //Mathf.Approximately(transform.position.x/10, -2/10) && Mathf.Approximately(transform.position.z/10, 0/10))
            {
                horizontalAngle = Quaternion.LookRotation(laserMonitor - transform.position, transform.up).eulerAngles.y;
                Turning();
                //Debug.LogError("already done");
                laserCutterScript.cuffed = Player.instance;//NetworkManager_Drug.instance.findPlayer(GetComponent<Network_Player>().getUsername()).gameObject.GetComponent<Player>();
                laserCutterScript.BeginSequences();
                isInteractLaser = false;
            }
        }
        else
        {
            if (!isClimbing)
            {
                Locomotion();
            }
            Turning();
        }

        VerticalLocomotion();
        Animation();
        CameraUpdate();

    }
    public void AddToViewVector(float x, float y)
    {
        horizontalAngle += x;
        verticalAngle += y;
    }

    public void AddToVerticalAngleSubractive(float v)
    {
        verticalAngleSubtractive += v;
    }

    public void AddToViewVector(Vector2 v)
    {
        AddToViewVector(v.x, v.y);
    }

    public bool IsSprinting() { return isSprinting; }
    public bool IsCrouching() { return isCrouching; }
    public bool IsMoving() { return isMoving; }

    public void ToggleCrouch()
    {
        isCrouching = !isCrouching;
        CrouchControllerColliderHeight();
    }
    public void ToggleSprinting() { isSprinting = !isSprinting; }



    void GatherInput()
    {
        if (inputEnabled)
        {

            #region Input Direction Angle
            Vector2 input;
            if (inputDev == PlayerControls.InputDevice.Keyboard) // Keyboard
            {
                input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            }
            else // Controller
            {
                input = new Vector2(Input.GetAxis("JAxisX"), Input.GetAxis("JAxisY"));
            }

            //forward
            if (input.x > 0 && input.y > 0)
            {

                inputAngle = 270 + Mathf.Atan(input.y / input.x) * 58f;
            }
            else if (input.x < 0 && input.y > 0)
            {

                inputAngle = Mathf.Atan(-input.x / input.y) * 58f;
            }

            else if (input.x == 0 && input.y > 0)
            {
                //De  Debug.Log("Forward");
                inputAngle = 360f;
            }

            //Right
            else if (input.x > 0 && input.y == 0)
            {
                //    Debug.Log("Right");
                inputAngle = 270f;
            }
            //Left
            else if (input.x < 0 && input.y == 0)
            {
                //   Debug.Log("Left");
                inputAngle = 90f;
            }

            //Backward

            else if (input.x == 0 && input.y < 0)
            {
                //  Debug.Log("Backward");
                inputAngle = 180;
            }

            else if (input.x > 0 && input.y < 0)
            {

                inputAngle = 180 + (-1f) * Mathf.Atan(input.x / input.y) * 58f;
            }

            else if (input.x < 0 && input.y < 0)
            {

                inputAngle = 90 + Mathf.Atan(-input.y / -input.x) * 58f;
            }


            //Not Moving
            if (float.IsNaN(inputAngle))
            {
                inputAngle = 0f;
            }

            //Debug.Log(inputAngle);

            #endregion

            Vector2 moveInputRaw = Vector2.zero;

            if (inputDev == PlayerControls.InputDevice.Keyboard) // Keyboard
            {
                input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            }
            else if (inputDev == PlayerControls.InputDevice.XBox360)
            {
                input = new Vector2(Input.GetAxis("JAxisX"), Input.GetAxis("JAxisY"));
            }

            movementInputRaw = input;

            switch (sprintTH)
            {
                case ToggleHold.TOGGLE:
                    if (Input.GetButtonDown("Crouch"))
                    {
                        ToggleCrouch();

                        NPCManager.instance.alertClosestNPCs(25f, transform);
                    }
                    break;
                default:
                case ToggleHold.HOLD:
                    isCrouching = Input.GetButton("Crouch");
                    break;
            }



            switch (sprintTH)
            {
                case ToggleHold.TOGGLE:
                    if (inputDev == PlayerControls.InputDevice.Keyboard)
                    {
                        if (Input.GetButtonDown("Sprint"))
                        {
                            ToggleSprinting();
                        }
                    }
                    else if (inputDev == PlayerControls.InputDevice.XBox360)
                    {
                        if (Input.GetKeyDown(Controls.Run))
                        {
                            ToggleSprinting();
                        }
                    }
                    break;
                default:
                case ToggleHold.HOLD:
                    if (inputDev == PlayerControls.InputDevice.Keyboard)
                    {
                        isSprinting = Input.GetButton("Sprint");
                    }
                    else if (inputDev == PlayerControls.InputDevice.XBox360)
                    {
                        isSprinting = Input.GetKey(Controls.Run);
                    }

                    break;
            }

            if (movementInputRaw.y <= 0) { isSprinting = false; }

            float aimMultiplier = Mathf.Lerp(1.0f, lookSensitivityAimingMultiplier, GunController.instance.GetAimingCoefficient());

            if (inputDev == PlayerControls.InputDevice.Keyboard) // Keyboard
            {
                // if(inventory != active)
                horizontalAngle += Input.GetAxisRaw("Mouse X") * Time.deltaTime * lookSensitivity * lookSensitivityHorizontal * aimMultiplier;
                verticalAngle += (invertY ? 1.0f : -1.0f) * Input.GetAxisRaw("Mouse Y") * Time.deltaTime * lookSensitivity * lookSensitivityVertical * aimMultiplier;
            }
            else if (inputDev == PlayerControls.InputDevice.XBox360)
            {
                horizontalAngle += Input.GetAxisRaw("XBOX X L") * Time.deltaTime * lookSensitivity * lookSensitivityHorizontal * aimMultiplier;
                verticalAngle += (invertY ? 1.0f : -1.0f) * Input.GetAxisRaw("XBOX Y R") * Time.deltaTime * lookSensitivity * lookSensitivityVertical * aimMultiplier;
            }
            //shrug

        }
        else
        {
            movementInputRaw = Vector2.zero;
        }
    }

    void Locomotion()
    {

        Vector2 movementDirection = movementInputRaw.normalized;
        //Debug.Log(movementDirection + " " + movementInputRaw);
        float generalSpeedMultiplier = 1.0f *
            (isCrouching ? .5f : 1.0f) *
            (isSprinting ? 2f : 1.0f) *
            (1.0f - .5f * GunController.instance.GetAimingCoefficient());

        float targetSpeed = movementSpeed * generalSpeedMultiplier;
        float targetForwardAnimationSpeed = movementInputRaw.y * (isSprinting ? 2.0f : 1.0f);
        float targetRightAnimationSpeed = movementInputRaw.x;

        speed = Mathf.Lerp(speed, targetSpeed, Time.deltaTime * movementSmoothingSpeed);
        forwardAnimationSpeed = Mathf.Lerp(forwardAnimationSpeed, targetForwardAnimationSpeed, Time.deltaTime * movementSmoothingSpeed * 2f);
        rightAnimationSpeed = Mathf.Lerp(rightAnimationSpeed, targetRightAnimationSpeed, Time.deltaTime * movementSmoothingSpeed * 2f);

        Vector3 velocity =
        ((movementDirection.y * transform.forward) + (movementDirection.x * transform.right))
         * speed * Time.deltaTime;

        isMoving = (velocity.sqrMagnitude > 0f);

        controller.Move(velocity);

    }

    public void VerticalLocomotion()
    {
        if (!isClimbing)
        {
            //Debug.Log("not is Climbing");
            controller.Move(Vector3.down * Time.deltaTime * 9.81f);
        }
        else
        {
            //Debug.Log("yes is Climbing");
            controller.Move(Vector3.up * Time.deltaTime * movementInputRaw.y);
            climbSpeed = movementInputRaw.y >= 0 ? 1 : -1;
        }
    }

    public void SetIsClimbing(bool isClimbing)
    {
        this.isClimbing = isClimbing;
    }

    public bool GetIsClimbing()
    {
        return isClimbing;
    }

    void Turning() 
   {

        Vector3 targetEulerAngles = new Vector3(0f, horizontalAngle, 0f);
        Quaternion targetRotation = Quaternion.Euler(targetEulerAngles);
        transform.rotation = targetRotation;

        verticalAngleSubtractive = Mathf.Clamp(verticalAngleSubtractive, 0f, 5f);

        verticalAngle = Mathf.Clamp(verticalAngle, -80f, 80f);
        headCamera.transform.localEulerAngles = new Vector3(verticalAngle - verticalAngleSubtractive, 0f, 0f);

    }

    void Animation()
    {
        if (shadowOnly)
        {
            foreach (SkinnedMeshRenderer part in playerModel.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                part.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }
        }
        else
        {
            foreach (SkinnedMeshRenderer part in playerModel.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                part.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }
        }

        characterAnimator.SetFloat("ForwardSpeed", forwardAnimationSpeed);
        characterAnimator.SetFloat("RightSpeed", rightAnimationSpeed);
        characterAnimator.SetBool("IsCrouching", isCrouching);
        characterAnimator.SetBool("IsClimbing", isClimbing);
        characterAnimator.SetFloat("ClimbSpeed", climbSpeed);
        characterAnimator.SetFloat("isCuffed_Normalized", GetComponent<CuffController>().isCuffed == true ? 1f : 0f);

        if (Inventory.instance.equippedGun != null)
        {
            switch (Inventory.instance.equippedGun.weaponClassification)
            {
                case GunItem.WeaponClassification.HANDGUN:
                    characterAnimator.SetLayerWeight(0, 0);
                    characterAnimator.SetLayerWeight(1, 1);
                    characterAnimator.SetLayerWeight(2, 0);
                    characterAnimator.SetLayerWeight(3, 0);
                    break;
                case GunItem.WeaponClassification.SHOTGUN:
                    characterAnimator.SetLayerWeight(0, 0);
                    characterAnimator.SetLayerWeight(1, 0);
                    characterAnimator.SetLayerWeight(2, 1);
                    characterAnimator.SetLayerWeight(3, 0);
                    break;
                case GunItem.WeaponClassification.ASSAULTRIFLE:
                    characterAnimator.SetLayerWeight(0, 0);
                    characterAnimator.SetLayerWeight(1, 0);
                    characterAnimator.SetLayerWeight(2, 0);
                    characterAnimator.SetLayerWeight(3, 1);
                    break;

                default: // Pass
                    break;
            }
        }
        else
        {
            characterAnimator.SetLayerWeight(0, 1);
            characterAnimator.SetLayerWeight(1, 0);
            characterAnimator.SetLayerWeight(2, 0);
            characterAnimator.SetLayerWeight(3, 0);
        }
    }

    void CameraUpdate()
    {

        Vector3 targetCameraPosition = new Vector3(0, isCrouching ? 1.0f : 1.5f, .25f);
        headCamera.transform.localPosition = Vector3.Lerp(headCamera.transform.localPosition, targetCameraPosition, Time.deltaTime * 7.0f);

    }

    void CrouchControllerColliderHeight()
    {
        if (isCrouching)
        {
            controller.center = Vector3.up * .625f;
            controller.height = 1f;
        }
        else
        {
            controller.center = Vector3.up * .875f;
            controller.height = 1.75f;
        }
    }

    public void ChangePlayerSpeed(Slider slider)
    {
        movementSpeed = slider.value * 25;
    }
   
}
