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
        forward = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Vertical", "W"));
        backward = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Vertical", "S"));
        left = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Horizontal", "A"));
        right = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Horizontal", "D"));
        crouch = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Crouch", "LeftControl"));
        sprint = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Sprint", "LeftShift"));
        interact = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Interact", "E"));
        reload = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Reload", "R"));
        shoot = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Fire1", "Mouse0")); 
        flashlightToggle = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("toggleflashlight", "G"));
        flashlightNarrow = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Flashlight Narrow", "RightArrow"));
        flashlightWiden = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Flashlight Wide", "LeftArrow"));
        flashlightUp = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Flashlight Up", "UpArrow"));
        flashlightDown = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Flashlight Down", "DownArrow"));
        flashlightCenter = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Flashlight Home", "Q"));
        inventory = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Open Inventory", "Tab"));
        pause = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Cancel", "Escape"));
    }


    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}
