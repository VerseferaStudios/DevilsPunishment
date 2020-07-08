using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TestPlayer : NetworkBehaviour
{
    private void Start()
    {
        transform.position = 10 * Vector3.one;
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("client started and hasAuthority = " + hasAuthority + " isLocalPlayer = " + isLocalPlayer);
    }
}
