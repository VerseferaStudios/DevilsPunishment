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
    public Button setEx;
    public Button repEx;

    public GameObject reportPanel;
    public GameObject settingsCanvas;

    public int[] scenes;
    public Cursor pointer;
       
    
    // Start is called before the first frame update
    void Awake()
    {
        singlePlayer.GetComponent<Button>().enabled = true;
        multiPlayer.GetComponent<Button>().enabled = true;
        credits.GetComponent<Button>().enabled = true;
        reportBug.GetComponent<Button>().enabled = true;
        settings.GetComponent<Button>().enabled = true;
        quit.GetComponent<Button>().enabled = true;
        //reportPanel.SetActive(false);
        settingsCanvas.SetActive(false);
    }

    public void SingleSart()
    {
        SceneManager.LoadScene(scenes[1]);
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
