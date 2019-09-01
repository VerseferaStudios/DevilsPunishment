using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource dialogSource;
    public Toggle sfxOff;
    public Toggle musicOff;
    public Toggle dialogOff;
    public Slider sfxSlide;
    public Slider musicSlide;
    public Slider dialogSlide;


    public void SetVolume(Toggle muteButton,Slider slideBar,AudioSource audioSource)
    {
        //If toggle is checked
        if (muteButton.isOn == true)
        {
            slideBar.value = 0.0f;
        }
        audioSource.volume = slideBar.value;
        audioSource.mute = muteButton.isOn;
        slideBar.interactable = !muteButton.isOn;
    }

    public void ClickedOn()
    {
        sfxSource.Play();
    }

    public void OnGUI()
    {

        SetVolume(musicOff, musicSlide, musicSource);
        SetVolume(sfxOff, sfxSlide, sfxSource);
        SetVolume(dialogOff, dialogSlide, dialogSource);
    }
}