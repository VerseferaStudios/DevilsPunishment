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
    AudioController sound;
    public GameObject playerCont;
    public GameObject optionsPanel;
    public GameObject menuPanel;
    public GameObject contactPanel;
    public GameObject controlPanel;
    public Button close;
    public Button credits;
    public Button contact;
    public Button controls;

    // Start is called before the first frame update
    void Start()
    {
        controlPanel.SetActive(false);
        optionsPanel.SetActive(false);
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
        controlPanel.SetActive(false);
    }

    //public void Credits()
    //{
    //    SceneManager.LoadScene(3);
    //}

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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            controlPanel.SetActive(false);
        }
    }
}
