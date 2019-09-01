//Author: David Bird
//Date: 
    //Last Edited:
        //By: David Bird
            //Purpose: Finish the script/clean up the scripts readability.
//Written For: Devil's Punishment v2
//Purpose: This script is part 1 of 2 part script (KeyBindingScript)to contol the players ability to remap the keybinds.
//Notes: Further information can be found at http://www.nolocationyet.com (Dead link)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{ 
    public static GameManager gm;

    public KeyCode forward { get; set; } //defined 1
    public KeyCode backward { get; set; } //defined 1
    public KeyCode left { get; set; } //defined 1
    public KeyCode right { get; set; } //defined 1
    public KeyCode interact { get; set; } //defined 1
    public KeyCode crouch { get; set; } //defined 1
    public KeyCode sprint { get; set; } //defined 1
    public KeyCode shoot { get; set; } //defined 1
    public KeyCode reload { get; set; } //defined 1
    public KeyCode flashlightToggle { get; set; } //defined 1
    public KeyCode flashlightNarrow { get; set; } //defined 1
    public KeyCode flashlightWiden { get; set; } //defined 1
    public KeyCode flashlightUp { get; set; } //defined 1
    public KeyCode flashlightDown { get; set; } //defined 1
    public KeyCode flashlightCenter { get; set; } //defined 1
    public KeyCode pause { get; set; } //defined 1
    public KeyCode inventory { get; set; } //defined 1
    public KeyCode aim { get; set; } //defined 1


    public void Awake()
    {
        if (gm == null)
        {
            DontDestroyOnLoad(gameObject);
            gm = this;
        }
        else if (gm != this)
        {
            Destroy(gameObject);
        }
    }
}
