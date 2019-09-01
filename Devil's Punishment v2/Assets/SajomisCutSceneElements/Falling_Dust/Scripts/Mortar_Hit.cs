using UnityEngine;
using System.Collections;

public class Mortar_Hit : MonoBehaviour {

public GameObject DustParticles;
public GameObject LightBulbAnim;
public GameObject CameraShake;
public AudioSource ExplosionAudio;
public Light BulbLight;
public GameObject LightBulb;
public GameObject LightBulbOff;

private float BulbIntensity = 3.5f;
private float RocketHitting = 0;


void Start (){

    DustParticles.SetActive(false);
    LightBulbOff.SetActive(false);

}

void Update (){

    if (Input.GetButtonDown("Fire1"))
    {

		StartCoroutine("RocketStrike");
       
    }

    BulbLight.intensity = BulbIntensity;

    if (RocketHitting == 0)
    {
        BulbIntensity = (Random.Range(3.4f, 3.65f));
    }

    if (RocketHitting == 1)
    {
        BulbIntensity = (Random.Range(2.0f, 4.0f));
    }
    
    if (RocketHitting == 2)
    {
        BulbIntensity = (Random.Range(3.0f, 4.0f));
    }
    
    
}

   
IEnumerator RocketStrike (){
    
    ExplosionAudio.Play();

	yield return new WaitForSeconds (1.5f);
    
    
    DustParticles.SetActive(false);

    DustParticles.SetActive(true); 



    RocketHitting = 1;



    DustParticles.SetActive(false);
    DustParticles.SetActive(true); 

    LightBulbAnim.GetComponent<Animation>().Play();
    CameraShake.GetComponent<Animation>().Play();

    // CameraShake.Play;

    LightBulb.SetActive(false);
    LightBulbOff.SetActive(true);
	yield return new WaitForSeconds (0.4f);
    LightBulb.SetActive(true);
    LightBulbOff.SetActive(false);
	yield return new WaitForSeconds (0.4f);
    LightBulb.SetActive(false);
    LightBulbOff.SetActive(true);
	yield return new WaitForSeconds (0.2f);

    RocketHitting = 2;

    LightBulb.SetActive(true);
    LightBulbOff.SetActive(false);
	yield return new WaitForSeconds (0.2f);
    LightBulb.SetActive(false);
    LightBulbOff.SetActive(true);
	yield return new WaitForSeconds (0.1f);
    LightBulb.SetActive(true);
    LightBulbOff.SetActive(false);

	yield return new WaitForSeconds (1);
    RocketHitting = 0;

}

void FlickerBulb (){



}




}