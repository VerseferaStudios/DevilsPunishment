//Author: Dave Bird
//Date: Tuesday, July 23, 2019
    //Last Edited: Saturday, July 27, 2019
        //By: Dave Bird w/ help from Unity Tutorial from Brackeys.
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
    AudioController sound;
    public GameObject playerCont;
    public GameObject optionsPanel;
    public GameObject menuPanel;
    public GameObject contactPanel;
    public GameObject controlPanel;
    public Button close;
    public Button contact;
    public Button controls;
    Resolution[] resolutions;
    public Dropdown resolutionOptions;
    public GameObject statusCanvas;

    // Start is called before the first frame update
    void Start()
    {
        statusCanvas.SetActive(false);
        controlPanel.SetActive(false);
        optionsPanel.SetActive(false);
        resolutions = Screen.resolutions;

        resolutionOptions.ClearOptions();

        List<string> options = new List<string>();

        int currentResOption = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResOption = i;
            }
        }
        resolutionOptions.AddOptions(options);
        resolutionOptions.value = currentResOption;
        resolutionOptions.RefreshShownValue();
    }


    public void Close()
    {
        //play audio click.
        optionsPanel.SetActive(false);
        menuPanel.SetActive(true);
        statusCanvas.SetActive(true);
    }

    public void ControlClose()
    {
        //play audio click.
        controlPanel.SetActive(false);
        statusCanvas.SetActive(true);
    }

    public void Controls()
    {
        //play audio click.
        controlPanel.SetActive(true);
    }

    public void Contact()
    {
        //play audio click.
        contactPanel.SetActive(true);
    }

    public void QualitySetter(int qualitySet)
    {
        QualitySettings.SetQualityLevel(qualitySet);
    }

    public void FullScreen(bool FullScreen)
    {
        Screen.fullScreen = FullScreen;
    }

    public void ResolutionPick(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            controlPanel.SetActive(false);
            statusCanvas.SetActive(true);
        }
    }
}
