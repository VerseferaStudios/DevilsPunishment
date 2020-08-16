using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class StartRoom : MonoBehaviour
{
    public Transform spawnPos;

    public bool isNoNetMan = false;

    private void OnEnable()
    {
        //NetworkManager.singleton.OnServerConnect += 
        RoomNew.MapGenCorridorsDone += DoorStuff;
    }

    private void OnDisable()
    {
        RoomNew.MapGenCorridorsDone -= DoorStuff;
    }

    private void Start()
    {
        GameState.gameState.setSpawnPos(spawnPos);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            DoorStuff();
        }
    }

    private void DoorStuff()
    {

#if UNITY_EDITOR
        bool? x = FindObjectOfType<MapGenBase>()?.isNoNetMan;
        isNoNetMan = x == null ? false : (bool)x;
#endif

        //room doors stuff, for network
        RoomDoorsInfo roomDoorsInfo = GetComponent<RoomDoorsInfo>();

        //server
        Debug.Log("09876 is it server? NetworkManager.singleton.mode = " + NetworkManager.singleton.mode);

        if (isNoNetMan || NetworkManager.singleton.mode == NetworkManagerMode.Host ||
           NetworkManager.singleton.mode == NetworkManagerMode.ServerOnly)
        {
            Debug.Log("09876 in server");
            for (int r = 0; roomDoorsInfo.roomDoorsTransform != null && r < roomDoorsInfo.roomDoorsTransform.Length; r++)
            {
                Debug.Log("09876 inn server spawning door");

                Vector3 scale = roomDoorsInfo.roomDoorsTransform[r].localScale;
                scale.x *= roomDoorsInfo.roomDoorsTransform[r].parent.localScale.x;
                scale.y *= roomDoorsInfo.roomDoorsTransform[r].parent.localScale.y;
                scale.z *= roomDoorsInfo.roomDoorsTransform[r].parent.localScale.z;

                Server_DoorSpawner.instance.Spawn_ServerDoor(roomDoorsInfo.roomDoorsTransform[r].position,
                    roomDoorsInfo.roomDoorsTransform[r].eulerAngles,
                    scale,
                    roomDoorsInfo.roomDoorsTransformInfo[r].triggerState,
                    true);
                    //roomDoorsInfo.roomDoorsTransformInfo[r].rot,
                    //roomDoorsInfo.roomDoorsTransformInfo[r].scale,
                    //roomDoorsInfo.roomDoorsTransformInfo[r].triggerState, true);
            }
        }
    }
}
