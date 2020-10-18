//Author: David Bird
//Date:
    //Last Edited:
        //By: David Bird
            //Purpose: Finish the script/clean up the scripts readability.
//Written For: Devil's Punishment v2
//Purpose: This script handles main menu buttons and functionality.
//Notes: Further information can be found at http://www.nolocationyet.com (Dead link)



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public Button singlePlayer;
    public Button multiPlayer;
    public Button credits;
    public Button reportBug;
    public Button settings;
    public Button quit;

    public GameObject reportPanel;
    public GameObject settingsCanvas;

    public int[] scenes;
    public Cursor pointer;

    public GameObject graphicPanel;
    public GameObject audioPanel;
    public GameObject controlPanel;
    
    // Start is called before the first frame update
    void Awake()
    {
        //singlePlayer.GetComponent<Button>().enabled = true;
        //multiPlayer.GetComponent<Button>().enabled = true;
        //credits.GetComponent<Button>().enabled = true;
        //reportBug.GetComponent<Button>().enabled = true;
        //settings.GetComponent<Button>().enabled = true;
        //quit.GetComponent<Button>().enabled = true;
        //reportPanel.SetActive(false);
        //settingsCanvas.SetActive(false);
    }

    public void SetRoomsNo(string n)
    {
        if (int.TryParse(n, out int res))
        {
            ScenesDataTransfer.numberOfRooms = res;
            Debug.Log("input for number of rooms = " + res);
        }
        else
        {
            Debug.LogError("couldn't parse input for number of rooms");
        }
    }

    public void SingleSart()
    {
        SceneManager.LoadSceneAsync(scenes[1]);
    }

    public void MultiStart()
    {
        SceneManager.LoadScene(scenes[2]);
    }

    public void Credits()
    {
        SceneManager.LoadScene(scenes[3]);
    }

    public void ReportBug()
    {
        reportPanel.SetActive(true);
    }

    public void Settings()
    {
        settingsCanvas.SetActive(true);
        graphicPanel.SetActive(true);
        audioPanel.SetActive(false);
        controlPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ReportExit()
    {
        reportPanel.SetActive(false);
    }

    public void SettingsExit()
    {
        settingsCanvas.SetActive(false);
        graphicPanel.SetActive(false);
        audioPanel.SetActive(false);
        controlPanel.SetActive(false);
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
