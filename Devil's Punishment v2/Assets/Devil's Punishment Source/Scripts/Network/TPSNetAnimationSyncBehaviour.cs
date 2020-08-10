using System.Linq;
using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class TPSNetAnimationSyncBehaviour : NetworkBehaviour
{

    public GameObject[] HANDGUN_PARTS;

    public GameObject[] SHOTGUN_PARTS;

    public GameObject[] ASSAULT_RIFLE_PARTS;
    
    //private Dictionary<int, int> gunLayerWeights;
    //private Dictionary<int, bool> gunVisible;
    [SerializeField] private GunItem.WeaponClassification activeGun; // only in server? sync var??

    private void Start()
    {
        //Not only for local player
        //This code is necessary for when a player joins a game, and he needs to see the right gun in all other players
        switch (activeGun)
        {
            case GunItem.WeaponClassification.NONE:
                HidetpsGunsInThisClient();
                break;
            case GunItem.WeaponClassification.ASSAULTRIFLE:
                ShowtpsRifleInThisClient();
                break;
            case GunItem.WeaponClassification.SHOTGUN:
                ShowtpsShotgunInThisClient();
                break;
            case GunItem.WeaponClassification.HANDGUN:
                ShowtpsPistolInThisClient();
                break;
            default:
                break;
        }
        base.OnStartAuthority();
    }

    /// <summary>
    /// Send command to hide all guns
    /// Also does layer weight stuff (all layer weights 0)
    /// </summary>
    public void SendCmd_HideTpsGuns()
    {
        activeGun = GunItem.WeaponClassification.NONE;
        Cmd_Hide3rdPersonGuns();

        SendCmd_LayerWeightAnim(tpsAnimator.GetLayerIndex("Handgun - Arms"), 0);
        SendCmd_LayerWeightAnim(tpsAnimator.GetLayerIndex("Shotgun - Arms"), 0);
        SendCmd_LayerWeightAnim(tpsAnimator.GetLayerIndex("Rifle - Arms"), 0);
    }

    /// <summary>
    /// Send command to show rifle
    /// Also does layer weight stuff (rifle layer weight 1)
    /// </summary>
    public void SendCmd_ShowTpsRifle()
    {
        activeGun = GunItem.WeaponClassification.ASSAULTRIFLE;
        Cmd_ShowTPSRifle();
        SendCmd_LayerWeightAnim(tpsAnimator.GetLayerIndex("Rifle - Arms"), 1);
    }

    /// <summary>
    /// Send command to show shotgun
    /// Also does layer weight stuff (shotgun layer weight 1)
    /// </summary>
    public void SendCmd_ShowTpsShotgun()
    {
        activeGun = GunItem.WeaponClassification.SHOTGUN;
        Cmd_ShowTPSShotgun();
        SendCmd_LayerWeightAnim(tpsAnimator.GetLayerIndex("Shotgun - Arms"), 1);
    }

    /// <summary>
    /// Send command to show pistol
    /// Also does layer weight stuff (pistol layer weight 1)
    /// </summary>
    public void SendCmd_ShowTpsPistol()
    {
        activeGun = GunItem.WeaponClassification.HANDGUN;
        Cmd_ShowTPSPistol();
        SendCmd_LayerWeightAnim(tpsAnimator.GetLayerIndex("Handgun - Arms"), 1);
    }

    [Command]
    private void Cmd_Hide3rdPersonGuns()
    {
        Rpc_HidetpsGunsInClients();
    }

    [Command]
    private void Cmd_ShowTPSRifle()
    {
        Rpc_ShowtpsRifleInClients();
    }

    [Command]
    private void Cmd_ShowTPSShotgun()
    {
        Rpc_ShowtpsShotgunInClients();
    }

    [Command]
    private void Cmd_ShowTPSPistol()
    {
        Rpc_ShowtpsPistolInClients();
    }

    [ClientRpc]
    private void Rpc_HidetpsGunsInClients()
    {
        HidetpsGunsInThisClient();
    }
    private void HidetpsGunsInThisClient()
    {
        Debug.Log("hiding all guns for player " + netId);
        foreach (GameObject part in HANDGUN_PARTS.Concat(SHOTGUN_PARTS).Concat(ASSAULT_RIFLE_PARTS))
        {
            part.SetActive(false);
        }
    }

    [ClientRpc]
    public void Rpc_ShowtpsRifleInClients()
    {
        ShowtpsRifleInThisClient();
    }
    public void ShowtpsRifleInThisClient()
    {
        foreach (GameObject part in ASSAULT_RIFLE_PARTS)
        {
            part.SetActive(true);
        }
    }

    [ClientRpc]
    public void Rpc_ShowtpsShotgunInClients()
    {
        ShowtpsShotgunInThisClient();
    }
    public void ShowtpsShotgunInThisClient()
    {
        foreach (GameObject part in SHOTGUN_PARTS)
        {
            part.SetActive(true);
        }
    }

    [ClientRpc]
    public void Rpc_ShowtpsPistolInClients()
    {
        ShowtpsPistolInThisClient();
    }
    public void ShowtpsPistolInThisClient()
    {
        foreach (GameObject part in HANDGUN_PARTS)
        {
            part.SetActive(true);
        }
    }



    public Animator tpsAnimator;

    private void SendCmd_LayerWeightAnim(int layerIdx, int weight)
    {
        Cmd_SetLayerWeight(layerIdx, weight);
    }

    [Command]
    private void Cmd_SetLayerWeight(int layerIdx, int weight)
    {
        Rpc_SetLayerWeight(layerIdx, weight);
    }

    [ClientRpc]
    private void Rpc_SetLayerWeight(int layerIdx, int weight)
    {
        tpsAnimator.SetLayerWeight(layerIdx, weight);
    }

}
