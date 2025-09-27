using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using AbilitySystem;

[CustomEditor(typeof(AbilityGridCell))]
[CanEditMultipleObjects]
public class AbilityGridCellEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AbilityGridCell cell = (AbilityGridCell)target;
        serializedObject.Update();

        // Ability field
        SerializedProperty abilityProp = serializedObject.FindProperty("Ability");
        EditorGUILayout.PropertyField(abilityProp);

        // Transition fields
        SerializedProperty transitionsProp = serializedObject.FindProperty("_transitions");
        string[] directions = { "Up Transition", "Left Transition", "Down Transition", "Right Transition" };
        for (int i = 0; i < directions.Length; i++)
        {
            SerializedProperty transitionProp = transitionsProp.GetArrayElementAtIndex(i);
            EditorGUILayout.PropertyField(transitionProp, new GUIContent(directions[i]));
        }

        // InputActionAsset field
        // SerializedProperty assetProp = serializedObject.FindProperty("_inputActionsAsset");
        // EditorGUILayout.PropertyField(assetProp);
        // InputActionAsset asset = assetProp.objectReferenceValue as InputActionAsset;

        // SerializedProperty selectedMapProp = serializedObject.FindProperty("_selectedActionMapName");

        // SerializedProperty actionNamesProp = serializedObject.FindProperty("_actionNames");
        // string[] directions = { "Up Action", "Left Action", "Down Action", "Right Action" };

        // if (asset != null)
        // {
        //     string[] mapNames = asset.actionMaps.Select(map => map.name).ToArray();
        //     if (mapNames.Length == 0)
        //     {
        //         EditorGUILayout.HelpBox("No action maps found in asset.", MessageType.Warning);
        //     }
        //     else
        //     {
        //         int selectedMapIndex = Mathf.Max(0, System.Array.IndexOf(mapNames, selectedMapProp.stringValue));
        //         selectedMapIndex = EditorGUILayout.Popup("Action Map", selectedMapIndex, mapNames);
        //         string selectedMapName = mapNames[selectedMapIndex];
        //         if (selectedMapProp.stringValue != selectedMapName)
        //         {
        //             selectedMapProp.stringValue = selectedMapName;
        //         }
        //         InputActionMap selectedMap = asset.actionMaps[selectedMapIndex];
        //         string[] availableActions = selectedMap.actions.Select(a => a.name).ToArray();

        //         // Draw dropdowns for each direction using the list
        //         for (int i = 0; i < directions.Length; i++)
        //         {
        //             SerializedProperty actionNameProp = actionNamesProp.GetArrayElementAtIndex(i);
        //             int currentIndex = Mathf.Max(0, System.Array.IndexOf(availableActions, actionNameProp.stringValue));
        //             int newIndex = EditorGUILayout.Popup(directions[i], currentIndex, availableActions);
        //             if (newIndex != currentIndex)
        //             {
        //                 actionNameProp.stringValue = availableActions[newIndex];
        //             }
        //         }
        //     }
        // }
        // else
        // {
        //     EditorGUILayout.HelpBox("Assign an InputActions asset to select actions.", MessageType.Info);
        // }

        serializedObject.ApplyModifiedProperties();
    }
}
