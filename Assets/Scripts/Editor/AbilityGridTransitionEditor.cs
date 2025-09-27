using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using AbilitySystem;

[CustomEditor(typeof(AbilityGridTransition))]
public class AbilityGridTransitionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AbilityGridTransition transition = (AbilityGridTransition)target;
        serializedObject.Update();

        // Icon field
        SerializedProperty iconProp = serializedObject.FindProperty("Icon");
        EditorGUILayout.PropertyField(iconProp);

        // Color field
        SerializedProperty colorProp = serializedObject.FindProperty("Color");
        EditorGUILayout.PropertyField(colorProp);

        // InputActionAsset field
        SerializedProperty assetProp = serializedObject.FindProperty("_inputActionsAsset");
        EditorGUILayout.PropertyField(assetProp);
        InputActionAsset asset = assetProp.objectReferenceValue as InputActionAsset;

        SerializedProperty selectedMapProp = serializedObject.FindProperty("_selectedActionMapName");
        SerializedProperty actionNameProp = serializedObject.FindProperty("_actionName");

        if (asset != null)
        {
            string[] mapNames = asset.actionMaps.Select(map => map.name).ToArray();
            if (mapNames.Length == 0)
            {
                EditorGUILayout.HelpBox("No action maps found in asset.", MessageType.Warning);
            }
            else
            {
                int selectedMapIndex = Mathf.Max(0, System.Array.IndexOf(mapNames, selectedMapProp.stringValue));
                selectedMapIndex = EditorGUILayout.Popup("Action Map", selectedMapIndex, mapNames);
                string selectedMapName = mapNames[selectedMapIndex];
                if (selectedMapProp.stringValue != selectedMapName)
                {
                    selectedMapProp.stringValue = selectedMapName;
                }
                InputActionMap selectedMap = asset.actionMaps[selectedMapIndex];
                string[] availableActions = selectedMap.actions.Select(a => a.name).ToArray();

                // Draw dropdown for the action name
                int currentIndex = Mathf.Max(0, System.Array.IndexOf(availableActions, actionNameProp.stringValue));
                int newIndex = EditorGUILayout.Popup("Action Name", currentIndex, availableActions);
                if (newIndex != currentIndex)
                {
                    actionNameProp.stringValue = availableActions[newIndex];
                }
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Assign an InputActions asset to select actions.", MessageType.Info);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
