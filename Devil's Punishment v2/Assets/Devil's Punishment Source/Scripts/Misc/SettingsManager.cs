//Author: David Bird



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    Resolution[] resolutions;
    public Dropdown resolutionOptions;
    // Start is called before the first frame update
    void Start()
    {
        resolutions = Screen.resolutions;

        resolutionOptions.ClearOptions();

        List<string> options = new List<string>();

        int currentResOption = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResOption = i;
            }
        }
        resolutionOptions.AddOptions(options);
        resolutionOptions.value = currentResOption;
        resolutionOptions.RefreshShownValue();
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
        
    }
}
