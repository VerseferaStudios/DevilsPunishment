using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Server_DoorSpawner : NetworkBehaviour
{
    //temp

    public List<AnimationClip> animClips;
    public List<Object> otherObjects;

#if UNITY_EDITOR
    public AnimatorOverrideController animatorOController;
    public List<KeyValuePair<AnimationClip, AnimationClip>> overrides;
#endif

    public static Server_DoorSpawner instance;
    private void Awake()
    {
        instance = this;
    }

    public GameObject roomDoorPrefab;

    [Server]
    public void Spawn_ServerDoor(Vector3 pos, Vector3 rot, Vector3 scale, GameState.gameStateType triggerState)
    {
        GameObject roomDoor = Instantiate(roomDoorPrefab, pos, Quaternion.Euler(rot));
        roomDoor.transform.localScale = scale;
        Debug.Log("0987 " + pos);
        if (roomDoor.TryGetComponent(out InteractableDoor interactableDoor))
        {
            interactableDoor.triggerState = triggerState;
            interactableDoor.mainDoorPos = pos;
        }
        NetworkServer.Spawn(roomDoor);
    }

}

#if UNITY_EDITOR

[CustomEditor(typeof(Server_DoorSpawner))]
public class EditorScriptAnimation : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Server_DoorSpawner server_DoorSpawner = target as Server_DoorSpawner;

        if(GUILayout.Button("Copy Animations From Selection Into AOC"))
        {
            Object[] selectedObjects = Selection.objects;
            server_DoorSpawner.animClips = new List<AnimationClip>();
            server_DoorSpawner.otherObjects = new List<Object>();
            AnimationClip curr;

            for (int i = 0; i < selectedObjects.Length; i++)
            {
                server_DoorSpawner.overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(server_DoorSpawner.animatorOController.overridesCount);
                server_DoorSpawner.animatorOController.GetOverrides(server_DoorSpawner.overrides);
                //Debug.Log("count = " + overrides.Count);
                ////AnimationClipPair[] overrideAnimationClips = server_DoorSpawner.animatorOController.clips;
                //Debug.Log("count = " + overrides.Count);
                if (selectedObjects[i] is AnimationClip)
                {
                    curr = selectedObjects[i] as AnimationClip;
                    server_DoorSpawner.animClips.Add(curr);
                    Debug.Log("curr name = " + curr.name);

                    Debug.Log("count = " + server_DoorSpawner.animatorOController.overridesCount);
                    Debug.Log("count = " + server_DoorSpawner.overrides.Count);
                    for (int r = 0; r < server_DoorSpawner.animatorOController.overridesCount; r++)
                    {
                        //Debug.Log(server_DoorSpawner.overrides[r].Key + " && " + server_DoorSpawner.overrides[r].Value);
                        if (server_DoorSpawner.overrides[r].Key.ToString().StartsWith(curr.name))
                        {
                            Debug.Log("g");
                            server_DoorSpawner.overrides[r] = new KeyValuePair<AnimationClip, AnimationClip>(server_DoorSpawner.overrides[r].Key, curr);
                        }
                    }

                    server_DoorSpawner.animatorOController.ApplyOverrides(server_DoorSpawner.overrides);

                    Debug.Log("server_DoorSpawner.animatorOController.overridesCount) " + server_DoorSpawner.animatorOController.overridesCount);
                }
                else
                {
                    server_DoorSpawner.otherObjects.Add(selectedObjects[i]);
                    Debug.LogWarning("not selected an anim clip but its a -> " + selectedObjects[i].GetType());
                }
            }
        }

    }
}

#endif