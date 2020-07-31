//Author: Dave Bird
//Date: Tuesday, July 23, 2019
    //Last Edited: Saturday, July 27, 2019
        //By: Dave Bird w/ help from Unity Tutorial from Brackeys.
            //Purpose: Write the script.
//Written For: Devil's Punishment v2
//Purpose: This script handles the controls for the game options.
//Notes: Further information can be found at http://www.nolocationyet.com (Dead link)


using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class OptionsMenuController : MonoBehaviour
{
    AudioController sound;
    //public GameObject playerCont;
    public GameObject optionsPanel;
    public GameObject menuPanel;
    public GameObject contactPanel;
    //public GameObject controlPanel;
    public Button close;
    public Button contact;
    public Button controls;
    //Resolution[] resolutions;
    List<Resolution> resolutions;
    public TMP_Dropdown resolutionOptions;


    private void OnEnable()
    {
        //controlPanel.SetActive(false);
        //optionsPanel.SetActive(false);
        resolutions = Screen.resolutions.ToList();
        //Resolution newRes = new Resolution();
        //newRes.width = 800;
        //newRes.height = 640;
        //newRes.refreshRate = 30;

        //resolutions.Add(newRes);

        //newRes = new Resolution();
        //newRes.width = 1000;
        //newRes.height = 1000;
        //newRes.refreshRate = 30;

        //resolutions.Add(newRes);

        resolutionOptions.ClearOptions();

        List<string> options = new List<string>();

        int currentResOption = 0;
        for (int i = 0; i < resolutions.Count; i++)
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
        Debug.Log("res options no = " + resolutions.Count);
    }


    public void Close()
    {
        //play audio click.
        optionsPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void ControlClose()
    {
        //play audio click.
        //controlPanel.SetActive(false);
    }

    public void Controls()
    {
        //play audio click.
        //controlPanel.SetActive(true);
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

}
