//Author: David Bird
//Date:
    //Last Edited:
        //By: David Bird
            //Purpose: Finish the script/clean up the scripts readability.
//Written For: Devil's Punishment v2
//Purpose: This script handles the entire cinematic for the splashscreen cinematic leading to Main Menu scene.
//Notes: Further information can be found at http://www.nolocationyet.com (Dead link)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCinematic : MonoBehaviour
{
    public GameObject door1;
    public GameObject door2;
    public GameObject door3;
    public GameObject door4;
    public GameObject eye;
    public GameObject hand;
    public GameObject dust;
    public GameObject clawMark;
    public GameObject lightCont;
    public Light flashlight;
    public AudioSource soundMaker;
    public AudioClip[] sounds;
    public Animation[] anims;

    //sounds
    //0 - breathing
    //1 - heartbeat
    //2 - Tapping flashlight
    //3 - Night Terror Hunting
    //4 - Night Terror Scream
    //5 - Night Terror Hit
    //6 - Night Terror Hit 2
    //7 - Night Terror Heavy Hit
    //8 - Metal Door Groan
    //9 - Dust and fine debris hit floor
    //10 - Metal Scratching


    // Start is called before the first frame update
    void Awake()
    {
        door1.SetActive(true);
        door2.SetActive(false);
        door3.SetActive(false);
        door4.SetActive(false);
        lightCont.SetActive(false);
        StartCoroutine("FaceThePunishment");
    }

    IEnumerator FaceThePunishment()
    {
        yield return new WaitForSeconds(.5f); //Wait for a second amp up the sound.
        //black
        //breathing + heart beat heartbeat ends
        //yield return new WaitWhile(sounds[1].length);
//        Player hears sounds muffled undetermined what they are get faster and louder
//(This is the nightcrawler outside the door sniffing, walking... basically hunting the prisoners)
//-Scene loads to show 3d door no damage
//- Player hears nightcrawler scream
//- Player hears fleshy thud on metal door(first)
//-Flashlight turns on illuminating the door more
//-Player hears fleshy thud on metal door(2nd)
//-Flashlight moves around the door inspecting the dmg
//- Player hears fleshy thud on metal door(3rd)
//-Flashlight inspects the door more
//-Player hears fleshy thud on metal door(final)
//-Flashlight lands on the hole to show an eye move into view
//- Eye squints from the light
//-Eye moves away and hand comes through then pulls out scratching the door.
//- Hand stops at the threshold of the door(see background image)
//   - Player hears nightcrawler scream one more time then flashlight(flickers, flickers, flickers, out)
//    -Scene goes black again then slowly fades to show the current background image w / only the name of the game on it.
//- Main Menu music plays
//- Buttons pop up one at a time in .25 second intervals until all buttons are up.

         yield return new WaitForSeconds(.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
