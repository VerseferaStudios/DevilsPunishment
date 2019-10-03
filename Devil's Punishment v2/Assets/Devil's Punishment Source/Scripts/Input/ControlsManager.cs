using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsManager : MonoBehaviour
{
    // Start is called before the first frame update

    // For calling player from anywhere without statically binding stuff

    #region Singleton
    public static ControlsManager instance;

    public PlayerControls Player1;

    public PlayerControls Player2;



    bool player1taken = false;

    public PlayerControls claimPlayer()
    {
        return Player1;
     //   if(player1taken)
    //    {
    //        return Player2;
    //    }

    //    player1taken = true;
     //   return Player1;
    }


    public PlayerControls.InputDevice inputPlayer1 = PlayerControls.InputDevice.Keyboard;
    public PlayerControls.InputDevice inputPlayer2 = PlayerControls.InputDevice.XBox360;

    void Start()
    {
        Player1 = new PlayerControls(inputPlayer1, true);
        Player2 = new PlayerControls(inputPlayer2, true);

        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            
            DontDestroyOnLoad(gameObject);
        }
    }





    #endregion Singleton


    // Update is called once per frame
    void Update()
    {
        
    }
}

public class PlayerControls
{
    public enum InputDevice
    {
        Keyboard,
        XBox360    
    }

    public InputDevice input;

    public KeyCode Forward = KeyCode.W;
    public KeyCode Left = KeyCode.A;
    public KeyCode Right = KeyCode.D;
    public KeyCode Backwards = KeyCode.S;
    public KeyCode Jump = KeyCode.Space;

    //public KeyCode Jump = KeyCode.Joystick1
    public KeyCode Run = KeyCode.LeftShift;

    public KeyCode FirePrimary = KeyCode.Mouse0;
    public KeyCode Aim = KeyCode.Mouse1;

    public KeyCode Interact = KeyCode.E;
    public KeyCode Reload = KeyCode.R;
    public KeyCode CameraToggle = KeyCode.V;

    public KeyCode dropLeft = KeyCode.Q;
    public KeyCode dropRight = KeyCode.G;
    public string aimStance = "null lul"; // wat, only used for controller oof
  //  public string shootAxis = "XBOXSHOOTAXIS";
    public string shootAxis = "XBOXRIGHTBUMPER";
    public string aimAxis = "XBOXLEFTBUMPER";

    public PlayerControls(InputDevice d, bool autoSet)
    {
        input = d;

        if(autoSet)
        {
            if(d == InputDevice.Keyboard)
            {
                Forward = KeyCode.W;
                Left = KeyCode.A;
                Right = KeyCode.D;
                Backwards = KeyCode.S;
                Jump = KeyCode.Space;

                Run = KeyCode.LeftShift;
                FirePrimary = KeyCode.Mouse0;
                Aim = KeyCode.Mouse1;
                Interact = KeyCode.E;
                Reload = KeyCode.R;
                CameraToggle = KeyCode.V;

                dropLeft = KeyCode.Q;
                dropRight = KeyCode.G;
            }
            else if(d == InputDevice.XBox360)
            {
                aimStance = "XBOXDPAD";
                shootAxis = "XBOXRIGHTBUMPER";
                aimAxis = "XBOXLEFTBUMPER";
                Jump = KeyCode.Joystick1Button0; // A
                Interact = KeyCode.Joystick1Button3; // Y
                Run = KeyCode.Joystick1Button1; // B
                FirePrimary = KeyCode.Joystick1Button10; // right trigger
                Aim = KeyCode.Joystick1Button9; // Left trigger
                CameraToggle = KeyCode.Joystick1Button9; // Right control axis IN
                Reload = KeyCode.Joystick1Button2; // X
            }
        }
    }


}
