using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCinematic : MonoBehaviour
{
    public Animation zoom;
    public GameObject canvasOut;
    public GameObject door1;
    public GameObject door2;
    public GameObject door3;
    public GameObject door4;
    public GameObject terror;
    public AudioSource sounds;
    public AudioSource sounds2;
    public AudioClip longway;
    public GameObject sounds3;
    public GameObject dust1;
    public GameObject dust2;
    public GameObject dust3;
    //public GameObject terror2;
    public GameObject f_light;
    public AudioClip[] list;
    float quieter = .25f;
    float louder = 1f;


    // Start is called before the first frame update
    void Start()
    {
        canvasOut.SetActive(false);
        door1.SetActive(true);
        door2.SetActive(false);
        door3.SetActive(false);
        door4.SetActive(false);
        terror.SetActive(false);
        //terror2.SetActive(false);
        dust1.SetActive(false);
        dust2.SetActive(false);
        f_light.SetActive(false);
        dust3.SetActive(false);
        sounds3.SetActive(false);
        
        StartCoroutine("HookMe");
    }

    IEnumerator HookMe()
    {
        //Start Up
        sounds2.volume = quieter;
        sounds2.PlayOneShot(longway);
        //yield return new WaitForSeconds(3);
        sounds2.volume = louder;
        sounds3.SetActive(true);
        sounds2.PlayOneShot(list[1]); //Flashlight_Taps_Double
        yield return new WaitForSeconds(.5f/*list[1].length*/);
        f_light.SetActive(true);
        yield return new WaitForSeconds(.25f);
        f_light.SetActive(false);
        sounds.PlayOneShot(list[1]); //Flashlight_Taps_Double
        yield return new WaitForSeconds(.5f);
        f_light.SetActive(true);
        yield return new WaitForSeconds(.5f);
        sounds.PlayOneShot(list[2]); //Night_Terror_02
        
        //Door 2
        sounds.PlayOneShot(list[3]); //Metal_Door_Heavy_Impact_01_Short
        yield return new WaitForSeconds(.75f);
        sounds2.PlayOneShot(list[4]); //Groaning_Metal_Short_01
        door1.SetActive(false);
        door2.SetActive(true);
        dust1.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        sounds.PlayOneShot(list[5]); //Rock_Debris_Short-001
        dust1.SetActive(false);
        sounds.PlayOneShot(list[6]); //Night_Terror_03
        yield return new WaitForSeconds(.75f);
        sounds2.PlayOneShot(list[7]); //Metal_Door_Heavy_Impact_02_Short
        sounds.PlayOneShot(list[8]); //Groaning_Metal_Short_03
        door2.SetActive(false);

        //Door 3
        door3.SetActive(true);
        dust1.SetActive(true);
        yield return new WaitForSeconds(.5f);
        dust2.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        dust1.SetActive(false);
        dust2.SetActive(false);
        sounds.PlayOneShot(list[2]);
        yield return new WaitForSeconds(1.5f);
        sounds.PlayOneShot(list[9]); //Metal_Door_Impact_01_Short
        yield return new WaitForSeconds(.5f);
        sounds2.PlayOneShot(list[10]); //Metal_Door_Impact_02_Short

        //Door4
        sounds.PlayOneShot(list[3]); // >>Heavy Impact<<
        sounds2.PlayOneShot(list[4]); // >>Groaning Metal<<
        dust3.SetActive(true);
        dust1.SetActive(true);
        door3.SetActive(false);
        door4.SetActive(true);
        yield return new WaitForSeconds(2f);
        //dust3.SetActive(false);
        //dust1.SetActive(false);

        //Night Terror
        yield return new WaitForSeconds(.25f);
        sounds2.PlayOneShot(list[11]); //Night_Terror_Happy_Scream
        f_light.SetActive(false);
        sounds.PlayOneShot(list[2]); //>>Night Terror<<
        yield return new WaitForSeconds(1);
        terror.SetActive(true);
        sounds2.PlayOneShot(list[1]); //>>Light Tapping<<
        f_light.SetActive(true);
        yield return new WaitForSeconds(13);
        f_light.SetActive(false);
        yield return new WaitForSeconds(1);
        sounds2.PlayOneShot(list[1]); //>>Light Tapping<<
        sounds.PlayOneShot(list[12]); //Nails_Creature_Scratching_Metal-001
        yield return new WaitForSeconds(1);
        f_light.SetActive(true);
        //terro.SetActive(false);
        //terror2.SetActive(true);
        zoom.Play("CameraZoom");
        yield return new WaitForSeconds(.5f);
        canvasOut.SetActive(true);
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("MainMenu");
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
