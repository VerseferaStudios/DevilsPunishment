using UnityEngine;
using System.Collections;

public class Camera_Shake : MonoBehaviour {
	
public GameObject CameraShake;

void Start (){

}

void Update (){

    if (Input.GetButtonDown("Fire1"))
    {

        ShakeTheCamera();
       
    }
   
}

   
void ShakeTheCamera (){

	CameraShake.GetComponent<Animation>().Play ();

}



}