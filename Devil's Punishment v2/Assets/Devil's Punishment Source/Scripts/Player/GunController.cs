using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;
using System;

public class GunController : MonoBehaviour
{

    public LayerMask hittableMask;
    public Transform targetPoint;

    public Gun equippedGun;
    public Network_Player networkPlayer;

    public ParticleSystem muzzleFlashParticles;
    public ParticleSystem ejectionParticles;
    public GameObject hitParticles;
    public Light muzzleFlashLight;
    PlayerController_Revamped playerController;

    public Transform muzzle;
    public Transform ejector;

	public Mesh shotgunBulletCaseMesh;
	public Material shotgunBulletCaseMat;

    [Range(1f, 20f)]
    public float fireRate = 10f;
    public float FOVKick = 15f;

    public float recoilAmount = 4.0f;

    public float swayAmount = 1.0f;
    public float swaySpeed = 6.0f;

    [HideInInspector]
    public bool inputEnabled;

	public CameraShake cameraShake;
    Gun[] guns;


    bool trigger = false;
    bool fire = false;
    bool reloading = false;
    bool triggerReload = false;
    [SerializeField]
    bool running = false;
    bool moving = false;
    bool crouching = false;
    bool raised = false;
    float aiming = 0;
	bool isAiming = false;
	public float recoilRecNotAiming;
	public float recoilRecAiming;
	public float recoilFactorAiming;
	public float recoilFactorNotAiming;
	Vector3 standardPosition = new Vector3(0, -1.55f, 0);
	Quaternion standardRotation;
	Vector3 aimingPosition = new Vector3(0.0035f, -1.493f, 0.2f);
	Vector3 shottieOffset;
	Vector3 handGunOffset;

	Vector3 swayOffset = Vector3.zero;

    float timeBetweenShots = .04f;
    float shootTimer;
	public float shakeDuration;
	public float shakeMagnitude ;
	float defaultFOV;

    float bulletSpreadCoefficient;

    private Vector2 recoil;

    private SoundManager soundManager;

    public static GunController instance;

    private Inventory inventory;
	[SerializeField]
	private InvAmmoDisplay invAmmoDisplay;
    private Animator gunAnimator;

	[SerializeField]
    private Animator playerAnimator;

    private int clipStock;
    private int clipSize;
    private int clip;
    private string ammoName;

    void Awake() {
        instance = this;
    }

    void Start() {
        playerController = PlayerController_Revamped.instance;
        networkPlayer = playerController.gameObject.GetComponent<Network_Player>();
        soundManager = SoundManager.instance;
        inventory = Inventory.instance;
        defaultFOV = Camera.main.fieldOfView;
        InitTimeBetweenShots();
        guns = GetComponentsInChildren<Gun>();

        foreach (Gun gun in guns) {
            gun.gameObject.SetActive(false);
        }
		InitGun();
	}

    void SetFireRate(float f) {
        fireRate = f;
        InitTimeBetweenShots();
    }

    void InitTimeBetweenShots() {
        timeBetweenShots = 1.00f/fireRate;
    }
	private bool used1 = false;
    void Update()
	{
		UpdateClipStock();
		GatherInput();
        if(!busyFiringAlready && equippedGun != null)
		{
			/* 
			 * Uncommenting this line out helps with gun positioning. (allowing gizmo use)
			 * Commenting is better for use in the actual game, and preventing the gun from getting a floating point error that causes it to drift away to infinity
			 */
			//standardPosition = gunAnimator.transform.localPosition;

			EvaluateInput();
			Animation();
		}
	}

	public int GetClip() {
        return clip;
    }

    public int GetClipSize() {
        return clipSize;
    }

    public int GetClipStock() {
        return clipStock;
    }


    public GameObject[] HANDGUN_PARTS;

    public GameObject[] SHOTGUN_PARTS;

    public GameObject[] ASSAULT_RIFLE_PARTS;

	public void Hide3rdPersonGuns(){
		foreach (GameObject part in HANDGUN_PARTS.Concat(SHOTGUN_PARTS).Concat(ASSAULT_RIFLE_PARTS))
		{
			part.SetActive(false);
		}
	}

	public void DeactivateWeaponArmsBlendingLayers(){
		playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("Handgun - Arms"),0);
		playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("Shotgun - Arms"),0);
		playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("Rifle - Arms"),0);
	}

    public void InitGun() {

        raised = false;
		reloading = false;

		equippedGun = null;
		if (guns == null)
		{
			return;
		}
        foreach(Gun gun in guns) {
            gun.gameObject.SetActive(false);
        }

        if(inventory.equippedGun != null) {

            string gunName = inventory.equippedGun.name;

			foreach (Gun gun in guns)
			{
				if (gun.gunItem.name == gunName)
				{
					equippedGun = gun;
					equippedGun.gameObject.SetActive(true);
					raised = true;
					recoilAmount = equippedGun.gunItem.recoilAmount;
					SetFireRate(equippedGun.gunItem.fireRate);
					ammoName = equippedGun.gunItem.ammunitionType.name;
					gunAnimator = equippedGun.GetComponent<Animator>();
					standardPosition = gunAnimator.transform.localPosition;
					standardRotation = gunAnimator.transform.localRotation;
					clip = 0;
					clipSize = equippedGun.gunItem.clipSize;
					clipStock = inventory.GetEquippedGunAmmo();

					Hide3rdPersonGuns();
					DeactivateWeaponArmsBlendingLayers();
					if (Inventory.instance.equippedGun != null){
						switch (Inventory.instance.equippedGun.weaponClassification)
						{
							case GunItem.WeaponClassification.HANDGUN:
								foreach (GameObject part in HANDGUN_PARTS)
								{
									part.SetActive(true);
								}
								playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("Handgun"),1);
								break;
							case GunItem.WeaponClassification.SHOTGUN:
								foreach (GameObject part in SHOTGUN_PARTS)
								{
									part.SetActive(true);
								}
								playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("Shotgun"),1);
								break;
							case GunItem.WeaponClassification.ASSAULTRIFLE:
								foreach (GameObject part in ASSAULT_RIFLE_PARTS)
								{
									part.SetActive(true);
								}
								playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("Rifle"),1);
								break;

							default: // Pass
								break;
						}
					}

					break;
				}
			}
        } else {
			// Reset some empty values (might not all be necessary)
			equippedGun = null;
			raised = false;
			recoilAmount = 4.0f;
			SetFireRate(10.0f);
			ammoName = null;
			//gunAnimator = null;
			clip = 0;
			clipSize = 0;
			clipStock = 0;
			Hide3rdPersonGuns();
			DeactivateWeaponArmsBlendingLayers();
		}
		invAmmoDisplay.SetGunIcons();

    }

    public float GetAimingCoefficient() {return aiming;}

    public float GetBulletSpreadCoefficient() {
        return bulletSpreadCoefficient;
    }

    void GatherInput() {
        if(inputEnabled) {
            running = playerController.IsSprinting();
            moving = playerController.IsMoving();
            crouching = playerController.IsCrouching();
            trigger = !running && Input.GetButton("Fire1") && clip > 0;


			if (trigger)
			{
				// Stop reloading if you're trying to shoot!
				reloading = false;
				triggerReload = false;
			} else
			{
				triggerReload = 
					clip < clipSize && clipStock > 0
					&& !reloading && !gunAnimator.GetCurrentAnimatorStateInfo(0).IsName("Reload")
					&& (Input.GetButtonDown("Reload") /*|| clip<=0*/);
			}
            if(running || gunAnimator == null) {aiming = 0f; } else {
				if (triggerReload)
				{
					aiming = 0f;
				} else
				{
					aiming = (Input.GetButton("Fire2") ? 1.0f : 0.0f);
					if(Input.GetButton("Fire2"))
					{
						isAiming = true;
						recoil = Vector2.zero;
					}
					else
					{
						isAiming = false;
					}
				}
			}
		}
    }

    void EvaluateInput() {
		//Debug.Log(clip + "/" + clipStock + "===" + clipSize);
		//Debug.Log(reloading);

        bulletSpreadCoefficient = Mathf.Lerp(bulletSpreadCoefficient, 2.0f * (moving? 2.0f : 1.0f) * (1.0f - aiming), Time.deltaTime * 3.0f);
		if (inputEnabled)
		{
			if (trigger)
			{
				/*await (should be put here, but it just propagates up the code. This may be problematic) */
				Fire();
			}
			else if (triggerReload)
			{
				StartCoroutine(Reload());
			}
		}

		// Update muzzle flash light intensity every frame, and in this order:
		{	
			if (muzzleFlashLight.intensity > 0)
			{
        		muzzleFlashLight.intensity = 2f*Mathf.Clamp01(shootTimer * fireRate);
			}
        	shootTimer -= Time.deltaTime;
		}
    }

	//Don't allow multiple activations of the fire method just because you're waiting for an animation to finish...
	bool busyFiringAlready = false;
	async Task Fire()
	{
		if (shootTimer > 0f) return;
		if (busyFiringAlready) return;
		busyFiringAlready = true;

		shootTimer = timeBetweenShots;
		clipStock = inventory.GetEquippedGunAmmo();
		// Make sure muzzle and ejector are childed to root bone on guns, otherwise the positions will move unpredictably
		muzzle.position = equippedGun.muzzle.position;
		ejector.position = equippedGun.ejector.position;

		float aimingCoefficient = 1.0f / (1.0f + aiming * 2.0f);

		playerController.AddToViewVector(recoil.x * aimingCoefficient, 0f);
		playerController.AddToVerticalAngleSubractive(-.2f * aimingCoefficient);
		playerController.AddToVerticalAngleSubractive(-recoil.y * .3f * aimingCoefficient);

		StartCoroutine(cameraShake.Shake(shakeDuration, shakeMagnitude));
		//void wait4ReloadAsync()
		//{
		do
			{
				gunAnimator.SetBool("Reload", false);
				gunAnimator.SetTrigger("Fire");
				await Task.Delay(1);
			} while (gunAnimator.GetCurrentAnimatorStateInfo(0).IsName("Reload") || gunAnimator.GetCurrentAnimatorStateInfo(0).IsName("Basic"));
			
		//}
		//await Task.Run(() =>wait4ReloadAsync());
        
		muzzleFlashLight.intensity = 0.5F;

		bool isShotgun = weaponIsShotgun();

		for (int i = 0; i < (isShotgun?10:1); i++)
		{
			//Debug.Log("NUM Projectiles: " + (weaponIsShotgun() ? 10 : 1));
			UnityEngine.Random.seed = UnityEngine.Random.Range(-9999, 9999);
			bulletSpreadCoefficient += 1.0f - aiming * 0.15f;
			Vector3 offset = bulletSpreadCoefficient * .0075f * new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
            //Say hey we're shooting to server
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward + offset*2);
			RaycastHit hit;
                

            

            if (Physics.Raycast(ray.origin, ray.direction, out hit, 100f, hittableMask.value, QueryTriggerInteraction.Ignore))
			{
               // NetworkManager_Drug.instance.BroadcastShot(ray.origin, hit.point);

                targetPoint.position = hit.point;
				GameObject bp = Instantiate(hitParticles, hit.point, Quaternion.LookRotation(hit.normal));
				bp.transform.SetParent(hit.collider.transform);
                Debug.Log("SHOOTIN");
                networkPlayer.Cmdbroadcast_Shots(hit.point, Quaternion.LookRotation(hit.normal));

				//This is just for testing a thing in the elimination system, can be removed later / SkitzFist
				//IfEnemyHit(hit);

			}
			else
			{
				targetPoint.position = transform.position + transform.forward * 50f;
			}
		}

        muzzleFlashParticles.Play();
		//Debug.Log("Playing Ejection Particles...");
        ejectionParticles.Play();
        if(soundManager != null)
        {
            soundManager.PlaySound("AssaultRifle", "Shot");
        }
        
		// magic number to adjust also here

		if(isAiming)
		{
			recoil += new Vector2(UnityEngine.Random.Range(-.05f, .05f), 0.0f) * recoilAmount * recoilFactorAiming;
		}
		else
		{
			recoil += new Vector2(UnityEngine.Random.Range(-.2f, .2f), 0.0f) * recoilAmount * recoilFactorNotAiming;
		}

        clip--;
		busyFiringAlready = false;
    }

	// Don't reload if already doing it once (don't stack reloads); Can't figure out how to pull this off...
	IEnumerator Reload()
	{
		reloading = true;
		//Debug.Log("ReloadTriggered!");
		gunAnimator.SetTrigger("Reload");
		float breakPoint = .5f;
		// Wait for other states to finish
		while (!gunAnimator.GetCurrentAnimatorStateInfo(0).IsName("Reload") || gunAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f)
		{
			yield return null;
		}
		// Wait for reload to animate a "little bit"
		while (gunAnimator.GetCurrentAnimatorStateInfo(0).IsName("Reload") && gunAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < breakPoint) {
			yield return null;
		}
		///////////////////////////////////////////
		//Process the reload
		clipStock = inventory.GetEquippedGunAmmo();
		//Debug.Log("ClipStock is: " + clipStock);
		if (weaponIsShotgun())
		{
			if (!trigger && clipStock > 0 && clip < clipSize)
			{
				clip++;
				inventory.DropItem(ammoName,/*ammount*/ 1,/*consume*/true);
				clipStock = inventory.GetEquippedGunAmmo();

			}
			if (!trigger && clipStock > 0 && clip < clipSize && aiming <= 0)
			{
				//Wait for animation to finish
				yield return new WaitForSeconds(0.33f*(gunAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime + gunAnimator.GetCurrentAnimatorStateInfo(0).length));
				StartCoroutine(Reload());
			}
			else
			{
				gunAnimator.SetBool("Reload", false);
				//Wait for animation to finish
				yield return new WaitForSeconds(0.5f * (gunAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime + gunAnimator.GetCurrentAnimatorStateInfo(0).length));
				reloading = false;
			}
		}
		else if (clipStock >= clipSize - clip)
		{
			int amt = clipSize - clip;
			//Debug.Log("reloading with stock left: " + ammoName);
			clip += amt;
			inventory.DropItem(ammoName,/*ammount*/ amt,/*consume*/true);
			clipStock = inventory.GetEquippedGunAmmo();
			gunAnimator.SetBool("Reload", false);
			//Wait for animation to finish
			yield return new WaitForSeconds(0.5f * (gunAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime + gunAnimator.GetCurrentAnimatorStateInfo(0).length));
			reloading = false;
		}
		else
		{
			//Debug.Log("almost empty!");
			clip += clipStock;
			inventory.DropItem(ammoName,/*ammount*/ clipStock,/*consume*/true);
			clipStock = inventory.GetEquippedGunAmmo();
			gunAnimator.SetBool("Reload", false);
			//Wait for animation to finish
			yield return new WaitForSeconds(0.5f * (gunAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime + gunAnimator.GetCurrentAnimatorStateInfo(0).length));
			reloading = false;
		}
	}

	void Animation() {
        Sway();

        if (gunAnimator != null)
        {


            gunAnimator.SetFloat("Aiming", aiming);
            gunAnimator.SetBool("Running", running);
            gunAnimator.SetBool("Raised", raised);



            //	Commenting these lines out  (and the "standardPosition=... in the update()") allow you to use the Unity Gizmo to find the position. Just make sure to uncomment it when you're done.
            gunAnimator.transform.localPosition = Vector3.Lerp(standardPosition, equippedGun.gameObject.GetComponent<OffsetTransform>().position, aiming);
            gunAnimator.transform.localRotation = Quaternion.Lerp(standardRotation, Quaternion.Euler(equippedGun.gameObject.GetComponent<OffsetTransform>().rotation), aiming);

        }


		if (trigger)
		{
			//Get color from gun's ITEM description?
			//Color color = equippedGun.gunItem.color;
			//Only set color for shotgun
			if (weaponIsShotgun() || weaponIsAssaultRifle())
			{

                ejector.gameObject.SetActive(true);

                ParticleSystemRenderer particleSystemRenderer = ejectionParticles.gameObject.GetComponent<ParticleSystemRenderer>();
				particleSystemRenderer.mesh = shotgunBulletCaseMesh;
				particleSystemRenderer.material = shotgunBulletCaseMat;


				Renderer rend = GameObject.Find("Ejector/CartridgeEjectEffect").GetComponent<Renderer>();
				rend.material.shader = Shader.Find("_Color");
				rend.material.SetColor("_Color", Color.white); // new Color(0.863f, 0.078f, 0.235f));

				//Find the Specular shader and change its Color to red
				rend.material.shader = Shader.Find("Specular");
				rend.material.SetColor("_SpecColor", Color.black); // new Color(0.863f, 0.078f, 0.235f));

                if (weaponIsAssaultRifle())
                {
                    ejectionParticles.startSize = 1;
                    ejectionParticles.startSpeed = 0.5f;
                }
                else if (weaponIsShotgun())
                {
                    ejectionParticles.startSize = 1.3f;
                    ejectionParticles.startSpeed = 0.6f;
                }

			}

            if (weaponIsHandGun())
            {
                ejector.gameObject.SetActive(false);
            }

		}
		// Camera update:
		
        Camera.main.fieldOfView = Mathf.Lerp(defaultFOV, defaultFOV-FOVKick, aiming);
	}

    void Sway() {

		float swayLimit = 5.0f;

		if (inputEnabled)
		{
			swayOffset = new Vector3(
				Mathf.Clamp(swayOffset.x - Input.GetAxisRaw("Mouse X"), -swayLimit, swayLimit) + recoil.x,
				Mathf.Clamp(swayOffset.y - Input.GetAxisRaw("Mouse Y"), -swayLimit, swayLimit),
				0f);
		}

		
		if (isAiming)
		{
			recoil = Vector2.Lerp(recoil, Vector2.zero, Time.deltaTime * recoilRecAiming);
			swayOffset = Vector3.Lerp(swayOffset, Vector3.zero, Time.deltaTime * recoilRecAiming);
		}
		else
		{
			recoil = (Vector2.Lerp(recoil, Vector2.zero, Time.deltaTime * recoilRecNotAiming));
			swayOffset = Vector3.Lerp(swayOffset, Vector3.zero, Time.deltaTime * recoilRecNotAiming);
		}

		//swayOffset = Vector3.Lerp(swayOffset, Vector3.zero, Time.deltaTime * swaySpeed);
		transform.localPosition = swayOffset * .001f * swayAmount;



		/*
		transform.localRotation = Quaternion.Lerp(transform.localRotation,
		Quaternion.Euler(0f, 0f, crouching ? -20f : 0f),
		Time.deltaTime * 10.0f);
		*/


	}

	private bool weaponIsShotgun()
	{
		return equippedGun.gunItem.weaponClassification == GunItem.WeaponClassification.SHOTGUN;
	}

	private bool weaponIsAssaultRifle()
	{
		return equippedGun.gunItem.weaponClassification == GunItem.WeaponClassification.ASSAULTRIFLE;
	}

    private bool weaponIsHandGun()
    {
        return equippedGun.gunItem.weaponClassification == GunItem.WeaponClassification.HANDGUN;
    }

    public void UpdateClipStock(){
        clipStock = inventory.GetEquippedGunAmmo();
	}

	//This is just for testing a thing for the elimination system, this can be removed later /SkitzFist
	private void IfEnemyHit(RaycastHit hit)
    {
        TestEnemy enemyHit = hit.transform.GetComponent<TestEnemy>();
        if(enemyHit != null)
        {
            enemyHit.TakeDamage(equippedGun.gunItem.damage);
        }
    }

	public void ReloadAnimationBehvioursOnStateEnterCallback()
	{
		playerAnimator.SetTrigger("Reload");
		switch (Inventory.instance.equippedGun.weaponClassification)
		{
			case GunItem.WeaponClassification.HANDGUN:
				foreach (GameObject part in HANDGUN_PARTS)
				{
					playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("Handgun - Arms"),1);
				}
				break;
			case GunItem.WeaponClassification.SHOTGUN:
				foreach (GameObject part in SHOTGUN_PARTS)
				{
					playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("Shotgun - Arms"),1);
				}
				break;
			case GunItem.WeaponClassification.ASSAULTRIFLE:
				foreach (GameObject part in ASSAULT_RIFLE_PARTS)
				{
					playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("Rifle - Arms"),1);
				}
				break;

			default: // Pass
				break;
		}
    }

	public void ReloadAnimationBehvioursOnStateExitCallback()
	{
		reloading = gunAnimator.GetBool("Reload");
		if(!reloading){
		playerAnimator.SetBool("Reload", reloading);
		}
    }

	public void ShootAnimationBehvioursOnStateEnterCallback()
	{
		bool shooting = gunAnimator.GetCurrentAnimatorStateInfo(0).IsName("Shoot");
		if(shooting){
			playerAnimator.SetTrigger("Fire");
			switch (Inventory.instance.equippedGun.weaponClassification)
			{
				case GunItem.WeaponClassification.HANDGUN:
					foreach (GameObject part in HANDGUN_PARTS)
					{
						playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("Handgun - Arms"),1);
					}
					break;
				case GunItem.WeaponClassification.SHOTGUN:
					foreach (GameObject part in SHOTGUN_PARTS)
					{
						playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("Shotgun - Arms"),1);
					}
					break;
				case GunItem.WeaponClassification.ASSAULTRIFLE:
					foreach (GameObject part in ASSAULT_RIFLE_PARTS)
					{
						playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("Rifle - Arms"),1);
					}
					break;

				default: // Pass
					break;
			}
		}
    }

	public void ShootAnimationBehvioursOnStateExitCallback()
	{
		bool shooting = gunAnimator.GetCurrentAnimatorStateInfo(0).IsName("Shoot");
		if(!shooting){
			playerAnimator.SetBool("Fire",shooting);
		}
    }
}
