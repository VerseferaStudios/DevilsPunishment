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
    [SerializeField] private GunItem.WeaponClassification activeGun; // only in server?

    public override void OnStartAuthority()
    {
        //switch (activeGun)
        //{
        //    case GunItem.WeaponClassification.NONE:
        //        SendCmd_HideTpsGuns();
        //        //do layer all weights 0 too
        //        break;
        //    case GunItem.WeaponClassification.ASSAULTRIFLE:
        //        SendCmd_ShowTpsRifle();
        //        //layer stuff too
        //        break;

        //}
        base.OnStartAuthority();
    }

    public void SendCmd_HideTpsGuns()
    {
        activeGun = GunItem.WeaponClassification.NONE;
        Cmd_Hide3rdPersonGuns();

        SendCmd_LayerWeightAnim(tpsAnimator.GetLayerIndex("Handgun - Arms"), 0);
        SendCmd_LayerWeightAnim(tpsAnimator.GetLayerIndex("Shotgun - Arms"), 0);
        SendCmd_LayerWeightAnim(tpsAnimator.GetLayerIndex("Rifle - Arms"), 0);
    }

    public void SendCmd_ShowTpsRifle()
    {
        activeGun = GunItem.WeaponClassification.ASSAULTRIFLE;
        Cmd_ShowTPSRifle();
        SendCmd_LayerWeightAnim(tpsAnimator.GetLayerIndex("Rifle - Arms"), 1);
    }

    public void SendCmd_ShowTpsShotgun()
    {
        activeGun = GunItem.WeaponClassification.SHOTGUN;
        Cmd_ShowTPSShotgun();
        SendCmd_LayerWeightAnim(tpsAnimator.GetLayerIndex("Shotgun - Arms"), 1);
    }

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
        foreach (GameObject part in HANDGUN_PARTS.Concat(SHOTGUN_PARTS).Concat(ASSAULT_RIFLE_PARTS))
        {
            part.SetActive(false);
        }
    }

    [ClientRpc]
    public void Rpc_ShowtpsRifleInClients()
    {
        foreach (GameObject part in ASSAULT_RIFLE_PARTS)
        {
            part.SetActive(true);
        }
    }

    [ClientRpc]
    public void Rpc_ShowtpsShotgunInClients()
    {
        foreach (GameObject part in SHOTGUN_PARTS)
        {
            part.SetActive(true);
        }
    }

    [ClientRpc]
    public void Rpc_ShowtpsPistolInClients()
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
