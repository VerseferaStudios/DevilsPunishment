using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

public class Server_DoorSpawner : NetworkBehaviour
{
    //temp

    public List<AnimationClip> animClips;
    public List<Object> otherObjects;

    public AnimatorOverrideController animatorOController;


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


            for (int i = 0; i < selectedObjects.Length; i++)
            {
                List<KeyValuePair<AnimationClip, AnimationClip>> overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                Debug.Log("count = " + overrides.Count);
                server_DoorSpawner.animatorOController.GetOverrides(overrides);
                Debug.Log("count = " + overrides.Count);
                if (selectedObjects[i] is AnimationClip)
                {
                    server_DoorSpawner.animClips.Add(selectedObjects[i] as AnimationClip);

                    for (int i = 0; i < server_DoorSpawner.animatorOController.overridesCount; i++)
                    {

                    }

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