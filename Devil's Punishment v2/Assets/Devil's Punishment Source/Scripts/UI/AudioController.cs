using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioSource sfxSource;
    private float musicVol = 1;
    private float sfxVol = 1;
    public Toggle sfxOff;
    public Toggle musicOff;
    public Slider sfxSlide;
    public Slider musicSlide;

    private float curSFXVol;
    private float curMusicVol;


    // Start is called before the first frame update
    void Start()
    {
        musicSource.GetComponent<AudioSource>();
        sfxSource.GetComponent<AudioSource>();
        sfxOff.GetComponent<Toggle>().isOn = false;
        musicOff.GetComponent<Toggle>().isOn = false;
    }

    public void SetMusicVolume(float vol)
    {
        //If toggle is checked
        if (musicOff == false)
        {
            //Must stop all music, make the slider inoperable and grayed out, slider goes all the way to the left.
            musicSource.volume = 0.0f;
            //musicSlide.GetComponent<Slider>().interactable = false;
            musicSource.mute = true;
            musicSlide.interactable = false;
        }
        //If toggle is not checked.
        else
        {
            //Must play/resume music, make slider operable, and not grayed out, and play at the volume previously left at.
            musicSource.volume = 0.5f;
            musicVol = vol;
            //musicSlide.GetComponent<Slider>().interactable = true;
            musicSource.mute = false;
            musicSlide.interactable = true;
        }
    }

    public void SetSFXVolume(float vol)
    {

        //problem when checkmarking and uncheckmarking the volume does not play you have to reslide

        if(sfxOff == false)
        {
            sfxSource.volume = 0.0f;
            sfxSlide.GetComponent<Slider>().interactable = true;
        }
        else
        {
            sfxVol = vol;
            sfxSlide.GetComponent<Slider>().interactable = true;
            sfxSource.volume = 0.5f;
        }
        //sfxVol = vol;
    }

    // Update is called once per frame
    void Update()
    {
        musicSource.volume = musicVol;
        sfxSource.volume = sfxVol;
        curMusicVol = musicVol;
    }
}
