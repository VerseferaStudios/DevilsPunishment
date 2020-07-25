using System.Linq;
using UnityEngine;
using Mirror;

public class TPSNetAnimationSyncBehaviour : NetworkBehaviour
{

    public GameObject[] HANDGUN_PARTS;

    public GameObject[] SHOTGUN_PARTS;

    public GameObject[] ASSAULT_RIFLE_PARTS;

    public void SendCmd_HideTpsGuns()
    {
        Cmd_Hide3rdPersonGuns();
    }

    public void SendCmd_ShowTpsRifle()
    {
        Cmd_ShowTPSRifle();
    }

    public void SendCmd_ShowTpsShotgun()
    {
        Cmd_ShowTPSShotgun();
    }

    public void SendCmd_ShowTpsPistol()
    {
        Cmd_ShowTPSPistol();
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

    public void SendCmd_LayerWeightAnim(int layerIdx, int weight)
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
