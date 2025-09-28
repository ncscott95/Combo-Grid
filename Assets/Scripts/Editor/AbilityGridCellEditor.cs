using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AbilityGridCell))]
[CanEditMultipleObjects]
public class AbilityGridCellEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AbilityGridCell cell = (AbilityGridCell)target;
        serializedObject.Update();

        // Ability field
        SerializedProperty abilityProp = serializedObject.FindProperty("Skill");
        EditorGUILayout.PropertyField(abilityProp);

        // Transition fields
        SerializedProperty transitionsProp = serializedObject.FindProperty("_transitions");
        string[] directions = { "Up Transition", "Left Transition", "Down Transition", "Right Transition" };
        for (int i = 0; i < directions.Length; i++)
        {
            SerializedProperty transitionProp = transitionsProp.GetArrayElementAtIndex(i);
            EditorGUILayout.PropertyField(transitionProp, new GUIContent(directions[i]));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
