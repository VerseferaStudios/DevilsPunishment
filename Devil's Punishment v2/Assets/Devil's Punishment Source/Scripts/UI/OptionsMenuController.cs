//Author: Dave Bird
//Date: Tuesday, July 23, 2019
    //Last Edited: Tuesdau, July 23, 2019
        //By: Dave Bird
            //Purpose: Write the script.
//Written For: Devil's Punishment v2
//Purpose: This script handles the controls for the game options.
//Notes: Further information can be found at http://www.nolocationyet.com (Dead link)


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionsMenuController : MonoBehaviour
{
    public GameObject playerCont;
    public AudioSource audioMusic;
    public AudioSource audioSFX;
    public GameObject optionsPanel;
    public GameObject menuPanel;
    public GameObject contactPanel;
    public GameObject controlPanel;
    public Button close;
    public Button credits;
    public Button contact;
    public Button controls;
    public Slider music;
    public Slider sfx;

    public Toggle sfxOff;
    public Toggle musicOff;

    public float musicVol = 1f;
    public float sfxVol = 1f;



    // Start is called before the first frame update
    void Start()
    {
        optionsPanel.SetActive(false);
        audioMusic = GetComponent<AudioSource>();
        audioSFX = GetComponent<AudioSource>();
    }


    public void Close()
    {
        optionsPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void Credits()
    {
        SceneManager.LoadScene(3);
    }

    public void Controls()
    {
        controlPanel.SetActive(false);
    }

    public void Contact()
    {
        contactPanel.SetActive(true);
    }

    public void SFXController()
    {
        audioSFX.volume = sfxVol;
    }

    public void MusicController()
    {
        audioMusic.volume = musicVol;
    }

    // Update is called once per frame
    void Update()
    {
        if (sfxOff)
        {
            sfxVol = 0.0f;
            sfx.enabled = false;
        }
        else
        {
            SFXController();
            sfx.enabled = true;
        }
        if (musicOff)
        {
            musicVol = 0.0f;
            music.enabled = false;
        }
        else
        {
            MusicController();
            music.enabled = true;
        }
    }
}
