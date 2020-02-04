using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ModularRoomAssembler))]
public class EditorScriptModularRoom : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ModularRoomAssembler modularRoomAssembler = target as ModularRoomAssembler;

        EditorGUILayout.BeginHorizontal();

        if(GUILayout.Button("Generate Room"))
        {
            modularRoomAssembler.StartScript();
        }

        if(GUILayout.Button("Clean Scene"))
        {
            GameObject[] gbs = GameObject.FindGameObjectsWithTag("Modular Room stuff");
            foreach (GameObject gb in gbs)
            {
                DestroyImmediate(gb);
            }
        }

        EditorGUILayout.EndHorizontal();

    }
}
