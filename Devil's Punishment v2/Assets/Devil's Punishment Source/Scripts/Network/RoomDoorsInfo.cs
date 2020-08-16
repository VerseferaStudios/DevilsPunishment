using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public struct RoomDoorsTransform
{
    public Vector3 pos;
    public Vector3 rot;
    public Vector3 scale;
    public GameState.gameStateType triggerState;
}

public class RoomDoorsInfo : MonoBehaviour
{
    public RoomDoorsTransform[] roomDoorsTransformInfo;
    public Transform[] roomDoorsTransform;
    public List<Transform> roomDoorsTransformModRoom;

    //private void Start()
    //{
    //    if (isClientOnly)
    //    {
    //        gameObject.SetActive(false);
    //    }
    //}

#if UNITY_EDITOR

        //dont execute, intertactable etc details removed
    public void ConvertTransformToClassInfo()
    {

        if (roomDoorsTransformModRoom.Count != 0)
        {
            roomDoorsTransform = roomDoorsTransformModRoom.ToArray();
        }
        roomDoorsTransformInfo = new RoomDoorsTransform[roomDoorsTransform.Length];

        for (int i = 0; i < roomDoorsTransform.Length; i++)
        {
            InteractableDoor interactableDoor = null;
            if (roomDoorsTransform[i].childCount >= 3)
            {
                roomDoorsTransform[i].GetChild(2).TryGetComponent(out interactableDoor);
            }

            roomDoorsTransformInfo[i] = new RoomDoorsTransform
            {
                pos = roomDoorsTransform[i].position,
                rot = roomDoorsTransform[i].eulerAngles,
                scale = roomDoorsTransform[i].localScale,
                triggerState = interactableDoor != null ? interactableDoor.triggerState : 0

            };
        }
        EditorUtility.SetDirty(this);
    }

#endif

}

#if UNITY_EDITOR

[CustomEditor(typeof(RoomDoorsInfo))]
public class EditorRoomDoorsInfo : Editor
{
    //private RoomDoorsInfo roomDoorsInfoScript;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        RoomDoorsInfo roomDoorsInfoScript = target as RoomDoorsInfo;

        if (GUILayout.Button("Generate doors info"))
        {
            roomDoorsInfoScript.ConvertTransformToClassInfo();
        }

    }
}

#endif
