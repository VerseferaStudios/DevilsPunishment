using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
public class GunController : MonoBehaviour
{

    public LayerMask hittableMask;
    public Transform targetPoint;

    public Gun equippedGun;


    public ParticleSystem muzzleFlashParticles;
    public ParticleSystem ejectionParticles;
    public GameObject hitParticles;
    public Light muzzleFlashLight;
    PlayerController playerController;

    public Transform muzzle;
    public Transform ejector;

    [Range(1f, 20f)]
    public float fireRate = 10f;
    public float FOVKick = 15f;

    public float recoilAmount = 4.0f;

    public float swayAmount = 2.0f;
    public float swaySpeed = 3.0f;

    [HideInInspector]
    public bool inputEnabled;


    Gun[] guns;


    bool trigger = false;
    bool fire = false;
    bool reloading = false;
    bool triggerReload = false;
    bool running = false;
    bool moving = false;
    bool crouching = false;
    bool raised = false;
    float aiming = 0;

	Vector3 standardPosition = new Vector3(0, -1.55f, 0);
	Quaternion standardRotation;
	Vector3 aimingPosition = new Vector3(0.0035f, -1.493f, 0.2f);
	Vector3 shottieOffset;
	Vector3 handGunOffset;

	Vector3 swayOffset = Vector3.zero;

    float timeBetweenShots = .04f;
    float shootTimer;

    float defaultFOV;

    float bulletSpreadCoefficient;

    private Vector2 recoil;

    private SoundManager soundManager;

    public static GunController instance;

    private Inventory inventory;

    private Animator gunAnimator;

    private int clipStock;
    private int clipSize;
    private int clip;
    private string ammoName;

    void Awake() {
        instance = this;
    }

    void Start() {
        defaultFOV = Camera.main.fieldOfView;
        playerController = PlayerController.instance;
        InitTimeBetweenShots();
        soundManager = SoundManager.instance;
        guns = GetComponentsInChildren<Gun>();
        foreach(Gun gun in guns) {
            gun.gameObject.SetActive(false);
        }
        inventory = Inventory.instance;

	}

    void SetFireRate(float f) {
        fireRate = f;
        InitTimeBetweenShots();
    }

    void InitTimeBetweenShots() {
        timeBetweenShots = 1.00f/fireRate;
    }

    void Update()
	{
		GatherInput();
        if(!busyFiringAlready && equippedGun != null)
		{
			/* 
			 * Uncommenting this line out helps with gun positioning. (allowing gizmo use)
			 * Commenting is better for use in the actual game, and preventing the gun from getting a floating point error that causes it to drift away to infinity
			 */
			//standardPosition = gunAnimator.transform.localPosition;

			Shooting();
            Sway();
            Animation();
        }
        CameraUpdate();
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
					clip = 0;
					clipSize = equippedGun.gunItem.clipSize;
					clipStock = inventory.GetEquippedGunAmmo();
					ammoName = equippedGun.gunItem.ammunitionType.name;
					gunAnimator = equippedGun.GetComponent<Animator>();
					standardPosition = gunAnimator.transform.localPosition;
					standardRotation = gunAnimator.transform.localRotation;

					break;
				}
			}
            
			
        }

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
            trigger = Input.GetButton("Fire1");

			if (trigger)
			{
				// Stop reloading if you're trying to shoot!
				reloading = false;
				triggerReload = false;
			} else
			{
				triggerReload = Input.GetButtonDown("Reload") && clip < clipSize && clipStock > 0;
			}
            if(running || gunAnimator == null) {aiming = 0f; } else {
				if (triggerReload)
				{
					aiming = 0f;
				} else
				{
					aiming = Mathf.Lerp(aiming, Input.GetButton("Fire2") ? 1.0f : 0.0f, Time.deltaTime * 13.0f);
				}
			}
        }
    }

    void Shooting() {

        clipStock = inventory.GetEquippedGunAmmo();

		//Debug.Log(clip + "/" + clipStock + "===" + clipSize);
		//Debug.Log(reloading);

		// Make sure muzzle and ejector are attached to root bone on guns, otherwise the positions will move unpredictably
		muzzle.position = equippedGun.muzzle.position;
		ejector.position = equippedGun.ejector.position;


		float aimingCoefficient = 1.0f/(1.0f+aiming*2.0f);

        playerController.AddToViewVector(recoil.x*aimingCoefficient, 0f);
        playerController.AddToVerticalAngleSubractive(-.2f * aimingCoefficient);
        playerController.AddToVerticalAngleSubractive(-recoil.y * .3f * aimingCoefficient);

        recoil = Vector2.Lerp(recoil, Vector2.zero, Time.deltaTime * 14.0f);
        bulletSpreadCoefficient = Mathf.Lerp(bulletSpreadCoefficient, 2.0f * (moving? 2.0f : 1.0f) * (1.0f - aiming), Time.deltaTime * 3.0f);


		if (inputEnabled)
		{
			if (shootTimer <= 0f && trigger && clip > 0)
			{
				Fire();
			}
			else if (triggerReload/*||clip<=0*/)
			{
				//Debug.Log("ReloadTriggered!");
				gunAnimator.SetTrigger("Reload");
				StartCoroutine(Reload());
			}
		}

        shootTimer -= Time.deltaTime;

        muzzleFlashLight.intensity = 2f*Mathf.Clamp01(shootTimer * fireRate);

    }

    void Sway() {

		float swayLimit = 10.0f;

		if (aiming < .5f && inputEnabled)
		{
			swayOffset = new Vector3(
				Mathf.Clamp(swayOffset.x - Input.GetAxisRaw("Mouse X"), -swayLimit, swayLimit) + recoil.x * 2.0f,
				Mathf.Clamp(swayOffset.y - Input.GetAxisRaw("Mouse Y"), -swayLimit, swayLimit),
				0f);
		}
		else
		{
			swayOffset += Vector3.forward * recoil.y * .5f;
		}

		swayOffset = Vector3.Lerp(swayOffset, Vector3.zero, Time.deltaTime * swaySpeed);
		transform.localPosition = swayOffset * .001f * swayAmount;
		/*
		transform.localRotation = Quaternion.Lerp(transform.localRotation,
		Quaternion.Euler(0f, 0f, crouching ? -20f : 0f),
		Time.deltaTime * 10.0f);
		*/


	}

	void Animation() {
        gunAnimator.SetFloat("Aiming", aiming);
        gunAnimator.SetBool("Running", running);
        gunAnimator.SetBool("Raised", raised);
		


		//	Commenting these lines out  (and the "standardPosition=... in the update()") allow you to use the Unity Gizmo to find the position. Just make sure to uncomment it when you're done.
			gunAnimator.transform.localPosition = Vector3.Lerp(standardPosition, equippedGun.gameObject.GetComponent<OffsetTransform>().position, aiming);
			gunAnimator.transform.localRotation = Quaternion.Lerp(standardRotation, Quaternion.Euler(equippedGun.gameObject.GetComponent<OffsetTransform>().rotation), aiming);




		if (trigger)
		{
			//Get color from gun's ITEM description?
			//Color color = equippedGun.gunItem.color;
			//Only set color for shotgun
			if (weaponIsShotgun())
			{

				Renderer rend = GameObject.Find("Ejector/CartridgeEjectEffect").GetComponent<Renderer>();
				rend.material.shader = Shader.Find("_Color");
				rend.material.SetColor("_Color", new Color(0.863f, 0.078f, 0.235f));

				//Find the Specular shader and change its Color to red
				rend.material.shader = Shader.Find("Specular");
				rend.material.SetColor("_SpecColor", new Color(0.863f, 0.078f, 0.235f));
			}

		}
	}

	void CameraUpdate() {
        Camera.main.fieldOfView = Mathf.Lerp(defaultFOV, defaultFOV-FOVKick, aiming);
    }

	//Don't allow multiple activations of the fire method just because you're waiting for an animation to finish...
	bool busyFiringAlready = false;
	async Task Fire()
	{
		if (busyFiringAlready) return;
		busyFiringAlready = true;
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

		shootTimer = timeBetweenShots;
		bool isShotgun = weaponIsShotgun();

		for (int i = 0; i < (isShotgun?10:1); i++)
		{
			//Debug.Log("NUM Projectiles: " + (weaponIsShotgun() ? 10 : 1));
			Random.seed = Random.Range(-9999, 9999);
			bulletSpreadCoefficient += 1.0f - aiming * 0.15f;
			Vector3 offset = bulletSpreadCoefficient * .0075f * new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);

			Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward + offset);
			RaycastHit hit;

			if (Physics.Raycast(ray.origin, ray.direction, out hit, 20f, hittableMask.value))
			{
				targetPoint.position = hit.point;
				GameObject bp = Instantiate(hitParticles, hit.point, Quaternion.LookRotation(hit.normal));
				bp.transform.SetParent(hit.collider.transform);

				//This is just for testing a thing in the elimination system, can be removed later / SkitzFist
				IfEnemyHit(hit);

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
        


        //recoil += new Vector2(Random.Range(-.2f, .2f), -1.0f) * recoilAmount * .05f;
        clip--;
		busyFiringAlready = false;
    }
	private bool weaponIsShotgun()
	{
		return equippedGun.gunItem.ammunitionType == (ResourceManager.instance.getResource("Pickup_Shotgun").GetComponent<InteractableLoot>().item as GunItem).ammunitionType;
	}

	// Don't reload if already doing it once (don't stack reloads); Can't figure out how to pull this off...
	IEnumerator Reload()
	{

		reloading = true;
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
				inventory.DropItem(ammoName,/*ammount*/1,/*consume*/true);
				clip++;
				clipStock--;

			}
			if (!trigger && clipStock > 0 && clip < clipSize && aiming <= 0)
			{
				//Wait for animation to finish
				yield return new WaitForSeconds(0.33f*(gunAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime + gunAnimator.GetCurrentAnimatorStateInfo(0).length));
				StartCoroutine(Reload());
			}
			else
			{
				reloading = false;
				gunAnimator.SetBool("Reload", false);
				//Wait for animation to finish
				yield return new WaitForSeconds(0.5f * (gunAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime + gunAnimator.GetCurrentAnimatorStateInfo(0).length));
			}
		}
		else if (clipStock >= clipSize - clip)
		{
			//Debug.Log("reloading with stock left: " + ammoName);
			clip = clipSize;
			inventory.DropItem(ammoName,/*ammount*/ clip,/*consume*/true);
			reloading = false;
			gunAnimator.SetBool("Reload", false);
			//Wait for animation to finish
			yield return new WaitForSeconds(0.5f * (gunAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime + gunAnimator.GetCurrentAnimatorStateInfo(0).length));
		}
		else
		{
			//Debug.Log("almost empty!");
			clip = clipStock;
			inventory.DropItem(ammoName,/*ammount*/ clip,/*consume*/true);
			reloading = false;
			gunAnimator.SetBool("Reload", false);
			//Wait for animation to finish
			yield return new WaitForSeconds(0.5f * (gunAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime + gunAnimator.GetCurrentAnimatorStateInfo(0).length));
		}
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
}
