using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Mirror;


//#if UNITY_EDITOR
//using UnityEditor;
//#endif

//[System.Serializable]
//public class RoomDoorsTransform
//{
//    public Vector3 pos;
//    public Vector3 rot;
//    public Vector3 scale;
//}

public class RoomDoorsInfo : MonoBehaviour
{
    //public RoomDoorsTransform[] roomDoorsTransformInfo;
    public Transform[] roomDoorsTransform;

    //private void Start()
    //{
    //    if (isClientOnly)
    //    {
    //        gameObject.SetActive(false);
    //    }
    //}

    //#if UNITY_EDITOR

    //    public void ConvertTransformToClassInfo()
    //    {
    //        roomDoorsTransformInfo = new RoomDoorsTransform[roomDoorsTransform.Length];

    //        for (int i = 0; i < roomDoorsTransform.Length; i++)
    //        {
    //            roomDoorsTransformInfo[i] = new RoomDoorsTransform
    //            {
    //                pos = roomDoorsTransform[i].position,
    //                rot = roomDoorsTransform[i].eulerAngles,
    //                scale = roomDoorsTransform[i].localScale
    //            };
    //        }
    //        EditorUtility.SetDirty(this);
    //    }

    //#endif

}

//#if UNITY_EDITOR

//[CustomEditor(typeof(RoomDoorsInfo))]
//public class EditorRoomDoorsInfo : Editor
//{
//    //private RoomDoorsInfo roomDoorsInfoScript;

//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();
//        RoomDoorsInfo roomDoorsInfoScript = target as RoomDoorsInfo;

//        if (GUILayout.Button("Generate doors info"))
//        {
//            roomDoorsInfoScript.ConvertTransformToClassInfo();
//        }

//    }
//}

//#endif
