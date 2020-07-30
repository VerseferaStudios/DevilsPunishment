using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditorInternal;
#endif
using UnityEngine;

public class FMODFootsteps_revamped : MonoBehaviour
{
    [Header("FMOD Setup")]
    [SerializeField] [FMODUnity.EventRef] private string FootstepsEvent;            
    [SerializeField] [FMODUnity.EventRef] private string JumpingEvent;              
    [SerializeField] private string GroundLayerParameter;                           
    [SerializeField] private string SpeedParameter;                                 
    [SerializeField] private string JumpParameter;                                  
    [Header("Game Settings")]
    [SerializeField] private float StepDistance = 2.0f;                         // How far player must movefor footsteps to trigger, static
    [SerializeField] private float RayDistance = 1.2f;                          // Distance of raycast, static (for initial setup)
    [SerializeField] private float StartRunningTime = 0.3f;                     // timer for switch to running footsteps
    [HideInInspector] private int DefaultGround;                                 // Default if raycast does not return anything


    //Footstep control
    private float StepRandom;                                                   // variation in footstep timing
    private Vector3 PrevPos;                                                    // previous position or player
    private float DistanceTravelled;                                            
    //Groundcheck
    private RaycastHit hit;                                                     
    private int F_GroundLayer;                                                  
    //Check if player on ground or in air
    private bool PlayerTouchingGround;                                          
    private bool PreviosulyTouchingGround;                                      
    //Speed
    private float TimeTakenSinceStep;
    private int F_PlayerPos;


    void Start()
    {
        StepRandom = Random.Range(0f, 0.2f);        // between 0 and 0.5
        PrevPos = transform.position;               // starting position
    }


    void Update()
    {

        Debug.DrawRay(transform.position, Vector3.down * RayDistance, Color.blue);      

        //How and when to trigger jump event
        GroundedCheck();                                                   
        if (PlayerTouchingGround && Input.GetKey(KeyCode.Space))           
        {
            GroundLayerCheck();                                               
            PlayJump(true);                                                 
        }
        if (!PreviosulyTouchingGround && PlayerTouchingGround)             
        {
            GroundLayerCheck();                                               
            PlayJump(false);                                                
        }
        PreviosulyTouchingGround = PlayerTouchingGround; 


        //How and when to trigger footstep events
        TimeTakenSinceStep += Time.deltaTime;                                
        DistanceTravelled += (transform.position - PrevPos).magnitude;
        if (DistanceTravelled >= StepDistance + StepRandom)
        {
            GroundLayerCheck();
            SpeedCheck();
            PlayFootstep();
            StepRandom = Random.Range(0f, 0.2f);
            DistanceTravelled = 0f;
        }
        PrevPos = transform.position;

    }


    void GroundLayerCheck() //Ground Layer check. What material is player walking or standing on
    {
        RaycastHit[] hit;

        hit = Physics.RaycastAll(transform.position, Vector3.down, RayDistance);

        foreach (RaycastHit rayhit in hit)
        {
            if (rayhit.transform.gameObject.layer == LayerMask.NameToLayer("vents"))
            {
                F_GroundLayer = 0;
            }
            else if (rayhit.transform.gameObject.layer == LayerMask.NameToLayer("stone"))
            {
                F_GroundLayer = 1;
            }
            else if (rayhit.transform.gameObject.layer == LayerMask.NameToLayer("ladder"))
            {
                F_GroundLayer = 2;
            }
            else if (rayhit.transform.gameObject.layer == LayerMask.NameToLayer("metal"))
            {
                F_GroundLayer = 3;
            }
            else F_GroundLayer = 0;
        }

    }


    void SpeedCheck() // Check when running switch should be set
    {
        if (TimeTakenSinceStep < StartRunningTime)                    
            F_PlayerPos = 1;                                       
        else
            F_PlayerPos = 0;
        TimeTakenSinceStep = 0f;
    }


    void GroundedCheck() // Check if player is in air or on ground - Raycast distance is cruical
    {
        Physics.Raycast(transform.position, Vector3.down, out hit, RayDistance);
        if (hit.collider)
            PlayerTouchingGround = true;
                                                                                  
        else                                                                      
            PlayerTouchingGround = false;
    }


    void PlayFootstep() // Footsteps event trigger in FMOD
    {
        if (PlayerTouchingGround)
        {
            FMOD.Studio.EventInstance Footstep = FMODUnity.RuntimeManager.CreateInstance(FootstepsEvent);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(Footstep, transform, GetComponent<Rigidbody>());
            Footstep.setParameterByName("groundlayer", F_GroundLayer);
            Footstep.setParameterByName(SpeedParameter, F_PlayerPos);
            Footstep.start();
            Footstep.release();
        }
    }


    void PlayJump(bool F_JumpLandCalc) // Jump event trigger in FMOD.
    {
        FMOD.Studio.EventInstance Jl = FMODUnity.RuntimeManager.CreateInstance(JumpingEvent);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(Jl, transform, GetComponent<Rigidbody>());
        Jl.setParameterByName("groundlayer", F_GroundLayer);
        Jl.setParameterByName(JumpParameter, F_JumpLandCalc ? 0f : 1f);
        Jl.start();
        Jl.release();
    }
}