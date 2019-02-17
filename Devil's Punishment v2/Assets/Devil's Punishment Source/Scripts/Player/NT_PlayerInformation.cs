using BeardedManStudios.Forge.Networking.Generated;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking;
using System;

public class NT_PlayerInformation : PlayerInformationBehavior
{
    protected override void NetworkStart()
    {
        base.NetworkStart();

        if (!networkObject.IsOwner)
        {
            // Don't render through a camera that is not ours
            // Don't listen to audio through a listener that is not ours
            transform.Find("Camera").gameObject.SetActive(false);

            // Don't accept inputs from objects that are not ours
            GetComponent<PlayerController>().enabled = false;

            // There is no reason to try and simulate physics since the position is
            // being sent across the network anyway
            Destroy(GetComponent<Rigidbody>());
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (!networkObject.IsOwner)
        {
            // If we are not the owner then we set the position to the
            // position that is syndicated across the network for this player
            transform.position = networkObject.Position;
            transform.rotation = networkObject.Rotation;
            return;
        }
        // When our position changes the networkObject.position will detect the
        // change based on this assignment automatically, this data will then be
        // syndicated across the network on the next update pass for this networkObject
        networkObject.Position = transform.position;
        networkObject.Rotation = transform.rotation;
    }

    private void AnimateMe()
    {

    }

    public override void ShowName(RpcArgs args)
    {
        throw new NotImplementedException();
    }
}
