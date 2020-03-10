using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController_Revamped : MonoBehaviour
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

    //Hylian 01/10/2020
    private Inventory inventory;

    [HideInInspector]
    public bool inputEnabled = true;

    private bool isClimbing;
    private float originalHeight;

    public GameObject playerModel;
    private Animator characterAnimator;
    public Animator CharacterAnimator { get => characterAnimator; set => characterAnimator = value; }

    private CharacterController controller;
    private Camera headCamera;

    

    private Vector2 movementInputRaw;
    private float speed;
    private float forwardAnimationSpeed;
    private float rightAnimationSpeed;
    private float climbSpeed;
    private bool isCrouching;
    [SerializeField]
    private bool isSprinting;
    private bool isMoving;

    private float horizontalAngle;
    private float verticalAngle;

    private float verticalAngleSubtractive;


    public static PlayerController_Revamped instance;

    public bool isInteractLaser = false;
    public Vector3 laserSpot, laserMonitor;
    public LaserCutter laserCutterScript;

    public bool shadowOnly = false;
    void Awake() {


        instance = this;
        characterAnimator = playerModel.GetComponent<Animator>();
        headCamera = GetComponentInChildren<Camera>();
        controller = GetComponent<CharacterController>();
        inventory = GetComponentInChildren<Inventory>();

    }
        
    void Start() {
        chcon = GetComponent<CharacterController>();
        Controls = ControlsManager.instance.claimPlayer();
        inputDev = Controls.input;
        if (Data.instance != null) {
            Data.instance.playerController = this;
        } else {
            Debug.Log("Unable to set 'Data.instance.playerController' Data instance not found.");
        }

        transform.position = new Vector3(-30, 10, -30);
        
    }

    Vector3 startVel;
    int stoppingPoint = 0;

    Vector2 input;

    //During cut scenes or movements like laserScript
    public bool killUpdate;
    

    void Update() {

        if(killUpdate)
        {
            return;
        }

        if (!inventory.gameObject.activeInHierarchy)
        {
            GatherInputNew();
            if (Input.GetKeyUp(KeyCode.E))
            {
                isInteractLaser = false; // do in other script if needed
            }
            if (isInteractLaser)
            {
                Debug.Log("moving player");// + Vector2.Distance(laserSpot, transform.position));
                                           //movementInputRaw = new Vector2(1, 1);
              //  input = new Vector2(0, 1);//new Vector2(-2, 0) - new Vector2(transform.position.x, transform.position.z) - new Vector2(0, 0)/*(0, 0) is the position of InteractableLaser Script*/;

                Camera.main.transform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, Quaternion.LookRotation(laserSpot - transform.position, transform.up).eulerAngles.y, Camera.main.transform.eulerAngles.z);

                //inputDirection = laserSpot - transform.position;
                //Debug.Log(horizontalAngle);

                //  Movement();
                





                if (Vector2.Distance(laserSpot, transform.position) < 1.18f) //Mathf.Approximately(transform.position.x/10, -2/10) && Mathf.Approximately(transform.position.z/10, 0/10))
                {
                    Camera.main.transform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, Quaternion.LookRotation(laserMonitor - transform.position, transform.up).eulerAngles.y, Camera.main.transform.eulerAngles.z);
                    //inputDirection = laserMonitor - transform.position;

                    Movement();

                    //Debug.LogError("already done");
                    laserCutterScript.cuffed = Player.instance;//NetworkManager_Drug.instance.findPlayer(GetComponent<Network_Player>().getUsername()).gameObject.GetComponent<Player>();
                    laserCutterScript.BeginSequences();
                    isInteractLaser = false;
                    transform.position = laserSpot;


                }
            }
            else
            {
                if (isClimbing)
                {
                    VerticalLocomotion();
                }
                else
                {
                    Movement();
                }
            }

            Animation();
            CameraUpdate();
         //T   GatherInput();
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            ToggleCrouch();
        }
    }

    public float RunSpeed = 2f;
    public float WalkSpeed = 1f;
    public float velocityY;
    public float gravity = 1f;
    public Transform LookDirectionObject;

    //Moonjump was 10f
    public float jumpHeight = 3f;
    CharacterController chcon;

    void jump()
    {
        if (chcon.isGrounded)
        {
            
            if(velocityY < -20)
            {
                velocityY = 0;
                    return;
            }
            else
            {
                float jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
                velocityY = jumpVelocity;
            }
            
            //Debug.Log("JUMP");
        }
        if (velocityY < -20)
        {
            velocityY = 0;
            return;
        }


    }

    public bool jumping;

    Vector2 GatherInputNew()
    {
        #region Basic Movement


        #region Input Direction Angle
        if (inputDev == PlayerControls.InputDevice.Keyboard) // Keyboard
        {
            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
        else // Controller
        {
            input = new Vector2(Input.GetAxis("JAxisX"), Input.GetAxis("JAxisY"));
        }

        movementInputRaw = input;

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

        #endregion


        //Debug.Log(string.Format("{0} X : {1} Y ; {2}", input.x, input.y, angle));
        return input;
    }

    void Movement()
    {
        Vector2 inputDirection = input.normalized;
        isSprinting = Input.GetKey(KeyCode.LeftShift);// Controls.Run);
        float speed = ((isSprinting) ? RunSpeed : WalkSpeed) * inputDirection.magnitude;
        float targetRotation;

        jumping = false;
        if (Input.GetKeyDown(Controls.Jump))
        {
            jump();
            jumping = true;
        }

        //Jump ?
        
        
        if(velocityY > -5)
        {
            velocityY += Time.deltaTime * gravity;
        }

        Vector3 velocity = transform.forward * speed + Vector3.up * velocityY;



        if (inputAngle < 270f && inputAngle > 90f)
        {
            targetRotation = Mathf.Atan2(-inputDirection.x, -inputDirection.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSMoothVelocity, turnSmoothing);
            velocity = -transform.forward * speed + Vector3.up * velocityY;

            switch (isSprinting)
            {
                case true:
                  //  _animator.SetInteger("RunIntensity", -2);
                    break;
                case false:
                  //  _animator.SetInteger("RunIntensity", -1);
                    break;
            }
        }
        //The Backward half
        else if (inputAngle > 270f || inputAngle < 90f)
        {
            targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSMoothVelocity, turnSmoothing);
            velocity = transform.forward * speed + Vector3.up * velocityY;
            switch (isSprinting)
            {
                case true:
                  //  _animator.SetInteger("RunIntensity", 2);
                    break;
                case false:
                 //   _animator.SetInteger("RunIntensity", 1);
                    break;
            }
        }
        //Exactly Left
        else if (inputAngle == 90f)
        {

            Vector3 lookPos = new Vector3(0, 0, 0);
            lookPos = transform.position - LookDirectionObject.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = rotation;
            switch (isSprinting)
            {
                case true:
                 //   _animator.SetInteger("RunIntensity", 4);
                    break;
                case false:
                  //  _animator.SetInteger("RunIntensity", 3);
                    break;
            }

       //     Debug.Log(lookPos);
            velocity = -transform.right * speed + Vector3.up * velocityY;
        }
        else if (inputAngle == 270f)
        {

            //Exactly Right

            Vector3 lookPos = new Vector3(0, 0, 0);
            lookPos = transform.position - LookDirectionObject.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 10);
            switch (isSprinting)
            {
                case true:
            //        _animator.SetInteger("RunIntensity", 6);
                    break;
                case false:
               //     _animator.SetInteger("RunIntensity", 5);
                    break;
            }

       //     Debug.Log(lookPos);
            velocity = transform.right.normalized * speed + Vector3.up * velocityY;

        }
        else if (inputAngle == 0f)
        {
            Vector3 lookPos = new Vector3(0, 0, 0);
            lookPos = transform.position - LookDirectionObject.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 10);

        }


        else if (inputDirection == Vector2.zero)
        {
       //     _animator.SetInteger("RunIntensity", 0);
        }


        #endregion


       chcon.Move(velocity * Time.deltaTime);



    }


    public Animator _animator;

    public float turnSmoothing = 1f;
    public float turnSMoothVelocity = 0f;

    public void VerticalLocomotion()
    {
        if (!isClimbing)
        {
            Debug.Log("not is Climbing");
            chcon.Move(Vector3.down * Time.deltaTime * 9.81f);
        }
        else
        {
            Debug.Log("yes is Climbing");
            chcon.Move(Vector3.up * Time.deltaTime * movementInputRaw.y);
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


        // Vents are currently on Default..
        // TODO: Adjust crouch check for every layer ?

        Ray ray = new Ray(transform.position, Vector3.up);

        RaycastHit Hit;

        if (Physics.Raycast(ray, out Hit))
        {
            // We hit something above
            float distanceUp = Vector3.Distance(transform.position, Hit.point);
            Debug.Log(distanceUp + " dist");
        }

        
        if (isCrouching && !Physics.Raycast(transform.position, Vector3.up, .75f))
        {
            isCrouching = false;
            CrouchControllerColliderHeight();
                
        }   
        else 
        {
            isCrouching = true;
            CrouchControllerColliderHeight();
            
        }
        
    }

    public bool slowdown = false;
    public void ToggleSprinting()
    {
        isSprinting = !isSprinting;
        if (!isSprinting)
        {
            startVel = GetComponent<CharacterController>().velocity;
            slowdown = true;
        }
        else
        {
            slowdown = false;
        }
    }



    void GatherInput() {
        if(inputEnabled) {

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

          //  Debug.Log(inputAngle);

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

            switch(sprintTH) {
                case ToggleHold.TOGGLE:
                    if(Input.GetButtonDown("C")){
                        ToggleCrouch();
                        //Debug.Log("Crouching !");
                        NPCManager.instance.alertClosestNPCs(25f, transform);
                    }
                    break;
                default:
                case ToggleHold.HOLD:
                    isCrouching = Input.GetButton("C");
                break;
            }



            switch(sprintTH) {
                case ToggleHold.TOGGLE:
                    if (inputDev == PlayerControls.InputDevice.Keyboard)
                    {
                        if (Input.GetButtonDown("Sprint"))
                        {
                        Debug.Log("TOGGLE");
                            ToggleSprinting();
                        }
                    }
                    else if(inputDev == PlayerControls.InputDevice.XBox360)
                    {
                        if (Input.GetKeyDown(Controls.Run))
                        {
                        Debug.Log("TOGGLE");
                            ToggleSprinting();
                        }
                    }
                break;
                default:
                case ToggleHold.HOLD:
                    if (inputDev == PlayerControls.InputDevice.Keyboard)
                    {
                        Debug.Log("NOT TOGGLE BUT HOLD");
                        isSprinting = Input.GetButton("Sprint");
                    }
                    else if (inputDev == PlayerControls.InputDevice.XBox360)
                    {
                        Debug.Log("NOT TOGGLE BUT HOLD");
                        isSprinting = Input.GetKey(Controls.Run);
                    }
                    
                break;
            }

            if(movementInputRaw.y <= 0) { isSprinting = false; }

            float aimMultiplier = Mathf.Lerp(1.0f, lookSensitivityAimingMultiplier, GunController.instance.GetAimingCoefficient());

            if (inputDev == PlayerControls.InputDevice.Keyboard) // Keyboard
            {
                horizontalAngle += Input.GetAxisRaw("Mouse X") * Time.deltaTime * lookSensitivity * lookSensitivityHorizontal * aimMultiplier;
                verticalAngle += (invertY ? 1.0f : -1.0f) * Input.GetAxisRaw("Mouse Y") * Time.deltaTime * lookSensitivity * lookSensitivityVertical * aimMultiplier;
            }
            else if (inputDev == PlayerControls.InputDevice.XBox360)
            {
                horizontalAngle += Input.GetAxisRaw("XBOX X L") * Time.deltaTime * lookSensitivity * lookSensitivityHorizontal * aimMultiplier;
                verticalAngle += (invertY ? 1.0f : -1.0f) * Input.GetAxisRaw("XBOX Y R") * Time.deltaTime * lookSensitivity * lookSensitivityVertical * aimMultiplier;
            }
            //shrug

        } else {
            movementInputRaw = Vector2.zero;
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
        if(shadowOnly){
            foreach (SkinnedMeshRenderer part in playerModel.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                part.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }
        } else {
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

        if (Inventory.instance.equippedGun != null){
            switch (Inventory.instance.equippedGun.weaponClassification)
            {
                case GunItem.WeaponClassification.HANDGUN:
                    characterAnimator.SetLayerWeight(0,0);
                    characterAnimator.SetLayerWeight(1,1);
                    characterAnimator.SetLayerWeight(2,0);
                    characterAnimator.SetLayerWeight(3,0);
                    break;
                case GunItem.WeaponClassification.SHOTGUN:
                    characterAnimator.SetLayerWeight(0,0);
                    characterAnimator.SetLayerWeight(1,0);
                    characterAnimator.SetLayerWeight(2,1);
                    characterAnimator.SetLayerWeight(3,0);
                    break;
                case GunItem.WeaponClassification.ASSAULTRIFLE:
                    characterAnimator.SetLayerWeight(0,0);
                    characterAnimator.SetLayerWeight(1,0);
                    characterAnimator.SetLayerWeight(2,0);
                    characterAnimator.SetLayerWeight(3,1);
                    break;

                default: // Pass
                    break;
            }
        } else {
            characterAnimator.SetLayerWeight(0,1);
            characterAnimator.SetLayerWeight(1,0);
            characterAnimator.SetLayerWeight(2,0);
            characterAnimator.SetLayerWeight(3,0);
        }
    }

    void CameraUpdate() {

        Vector3 targetCameraPosition = new Vector3(0, isCrouching? 1f : 1.5f, .25f);
        headCamera.transform.localPosition = Vector3.Lerp(headCamera.transform.localPosition, targetCameraPosition, Time.deltaTime * 7.0f);

    }
    Vector3 controllerCenter;

    void CrouchControllerColliderHeight() {
        if(isCrouching) {
           // controller.center = Vector3.up * .625f;
            controller.height = .6f;
        } else {
          //  controller.center = controllerCenter;
            controller.height = 1.8f;
        }
    }

    public void ChangePlayerSpeed(Slider slider)
    {
        movementSpeed = slider.value * 25;
    }

}
