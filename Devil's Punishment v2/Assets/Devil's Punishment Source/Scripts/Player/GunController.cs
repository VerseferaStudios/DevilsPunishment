using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{

    public LayerMask hittableMask;

    public Animator animator;
    public ParticleSystem muzzleFlashParticles;
    public ParticleSystem ejectionParticles;
    public GameObject hitParticles;
    public Light muzzleFlashLight;
    PlayerController playerController;

    [Range(1f, 20f)]
    public float fireRate = 10f;
    public float FOVKick = 15f;

    public float recoilAmount = 4.0f;

    public float swayAmount = 2.0f;
    public float swaySpeed = 3.0f;


    bool trigger = false;
    bool fire = false;
    bool reloading = false;
    bool triggerReload = false;
    bool running = false;
    bool moving = false;
    bool crouching = false;
    float aiming = 0;

    Vector3 standardPosition = new Vector3(0, -1.55f, 0);
    Vector3 aimingPosition = new Vector3(0.0035f,-1.493f,0.2f);

    Vector3 swayOffset = Vector3.zero;

    float timeBetweenShots = .04f;
    float shootTimer;

    float defaultFOV;

    float bulletSpreadCoefficient;

    float sprintShootTimer;

    private Vector2 recoil;
    private new AudioSource audioSource;

    public static GunController instance;

    void Awake() {
        instance = this;
    }

    void Start() {
        defaultFOV = Camera.main.fieldOfView;
        playerController = PlayerController.instance;
        timeBetweenShots = 1.00f/fireRate;
        audioSource = GetComponent<AudioSource>();
    }

    void Update() {
        GatherInput();
        Shooting();
        Sway();
        Animation();
        CameraUpdate();
    }

    public float GetAimingCoefficient() {return aiming;}

    public float GetBulletSpreadCoefficient() {
        return bulletSpreadCoefficient;
    }

    void GatherInput() {
        running = playerController.IsSprinting();
        moving = playerController.IsMoving();
        crouching = playerController.IsCrouching();
        trigger = Input.GetButton("Fire1");
        triggerReload = Input.GetButtonDown("Reload");
        aiming = Mathf.Lerp(aiming, Input.GetButton("Fire2")? 1.0f : 0.0f, Time.deltaTime * 13.0f);
    }

    void Shooting() {

        float aimingCoefficient = 1.0f/(1.0f+aiming*2.0f);

        playerController.AddToViewVector(recoil.x*aimingCoefficient, 0f);
        playerController.AddToVerticalAngleSubractive(-.2f * aimingCoefficient);
        playerController.AddToVerticalAngleSubractive(-recoil.y * .3f * aimingCoefficient);

        recoil = Vector2.Lerp(recoil, Vector2.zero, Time.deltaTime * 14.0f);
        bulletSpreadCoefficient = Mathf.Lerp(bulletSpreadCoefficient, 2.0f * (moving? 2.0f : 1.0f) * (1.0f - aiming), Time.deltaTime * 3.0f);

        if(!running) {
            sprintShootTimer -= Time.deltaTime;
        } else {
            sprintShootTimer = .3f;
        }

        if(!reloading && triggerReload && sprintShootTimer <= 0f) {

            StartCoroutine(Reload());
            animator.SetTrigger("Reload");
        }

        if(shootTimer <= 0f && trigger && sprintShootTimer <= 0f) {
            Fire();
        }

        shootTimer -= Time.deltaTime;



        muzzleFlashLight.intensity = 2f*Mathf.Clamp01(shootTimer * fireRate);

    }

    void Sway() {

        float swayLimit = 10.0f;

        if(aiming<.5f) {
            swayOffset = new Vector3(
                Mathf.Clamp(swayOffset.x - Input.GetAxisRaw("Mouse X"), -swayLimit, swayLimit) + recoil.x*2.0f,
                Mathf.Clamp(swayOffset.y - Input.GetAxisRaw("Mouse Y"), -swayLimit, swayLimit),
                0f);
        } else {
            swayOffset += Vector3.forward * recoil.y * .5f;
        }

        swayOffset = Vector3.Lerp(swayOffset, Vector3.zero, Time.deltaTime * swaySpeed);
        transform.localPosition = swayOffset * .001f * swayAmount;

        transform.localRotation = Quaternion.Lerp(transform.localRotation,
        Quaternion.Euler(0f, 0f, crouching? -20f : 0f),
        Time.deltaTime * 10.0f);

    }

    void Animation() {
        animator.SetFloat("Aiming", aiming);
        animator.SetBool("Running", running);
        animator.transform.localPosition = Vector3.Lerp(standardPosition, aimingPosition, aiming);
    }

    void CameraUpdate() {
        Camera.main.fieldOfView = Mathf.Lerp(defaultFOV, defaultFOV-FOVKick, aiming);
    }

    void Fire() {
        animator.SetTrigger("Fire");
        shootTimer = timeBetweenShots;
        muzzleFlashParticles.Play();
        ejectionParticles.Play();
        recoil += new Vector2(Random.Range(-.2f, .2f), -1.0f) * recoilAmount * .05f;

        Vector3 offset = bulletSpreadCoefficient * .05f * new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);

        Ray ray = new Ray(Camera.main.transform.position + offset, Camera.main.transform.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray.origin, ray.direction, out hit, 20f, hittableMask.value)) {
            Instantiate(hitParticles, hit.point, Quaternion.LookRotation(hit.normal));
        }

        bulletSpreadCoefficient += 1.0f - aiming;

        audioSource.Play();
    }

    IEnumerator Reload() {

        reloading = true;

        yield return new WaitForSeconds(.1f);

        while (animator.GetCurrentAnimatorStateInfo(0).nameHash == -1507367648)
        {
            yield return null;
        }

        reloading = false;

    }

}
