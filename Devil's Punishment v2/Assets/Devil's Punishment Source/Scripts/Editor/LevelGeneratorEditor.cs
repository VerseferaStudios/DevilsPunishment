using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor {

	private ReorderableList list;

	private void OnEnable() {
		list = new ReorderableList (serializedObject,
			serializedObject.FindProperty ("tileData"),
			true, true, true, true);

		list.drawElementCallback =
			(Rect rect, int index, bool isActive, bool isFocused) => {
			var element = list.serializedProperty.GetArrayElementAtIndex (index);
			rect.y += 2;
            rect.height += 16;
            EditorGUI.PropertyField (
                new Rect (rect.x + 20, rect.y, 15, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative ("above"), GUIContent.none);
            EditorGUI.PropertyField (
                new Rect (rect.x + 40, rect.y, 15, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative ("below"), GUIContent.none);
            EditorGUI.PropertyField (
                new Rect (rect.x + 60, rect.y, 15, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative ("left"), GUIContent.none);
            EditorGUI.PropertyField (
                new Rect (rect.x + 80, rect.y, 15, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative ("right"), GUIContent.none);
            EditorGUI.PropertyField (
				new Rect (rect.x + 100, rect.y, rect.width - 250, EditorGUIUtility.singleLineHeight),
				element.FindPropertyRelative ("prefab"), GUIContent.none);
            EditorGUI.PropertyField (
				new Rect (rect.width-40, rect.y, 60, EditorGUIUtility.singleLineHeight),
				element.FindPropertyRelative ("yRotation"), GUIContent.none);
            EditorGUI.PropertyField(
               new Rect(rect.width - 80, rect.y, 40, EditorGUIUtility.singleLineHeight),
               element.FindPropertyRelative("xRotation"), GUIContent.none);
            EditorGUI.PropertyField(
               new Rect(rect.width - 120, rect.y, 40, EditorGUIUtility.singleLineHeight),
               element.FindPropertyRelative("ID"), GUIContent.none);            
            };
	}

	public override void OnInspectorGUI() {

		serializedObject.Update ();

		LevelGenerator generator = (LevelGenerator)target;

		GUILayout.Label ("Generates a level based on given map data.");
		generator.offset = EditorGUILayout.Vector3Field ("Map Offset", generator.offset);
		generator.offsetMultiplier = EditorGUILayout.FloatField ("Tile Size", generator.offsetMultiplier);

		generator.map = EditorGUILayout.ObjectField ("Map Texture", generator.map, typeof(Texture2D), true) as Texture2D;

        GUILayout.Label("N/S/E/W/Prefab/ID/YRotation");
		list.DoLayoutList ();


		if (GUILayout.Button ("Generate Level")) {
			generator.GenerateLevel ();
		}

		if (GUILayout.Button ("Destroy Level")) {
			generator.DestroyLevel (false);
		}

		serializedObject.ApplyModifiedProperties ();

	}

}
