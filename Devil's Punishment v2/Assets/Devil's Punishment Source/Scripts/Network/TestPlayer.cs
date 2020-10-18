using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TestPlayer : NetworkBehaviour
{
    public GameObject prefab;
    private void Start()
    {
        //transform.position = 10 * Vector3.one;
        //GameObject gb = new GameObject("Lol");
        if (isServer)
        {
            GameObject gb = Instantiate(prefab, Vector3.one * 10, Quaternion.identity);
            //NetworkServer.Spawn(gb);
        }
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        //Debug.Log("client started and hasAuthority = " + hasAuthority + " isLocalPlayer = " + isLocalPlayer);
    }

}
