using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

[CustomEditor(typeof(AbilityGridTransition))]
public class AbilityGridTransitionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Transition Properties", EditorStyles.boldLabel);
        EditorGUILayout.Space(8);

        // Horizontal container for Icon/Color and thumbnail
        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
        SerializedProperty iconProp = serializedObject.FindProperty("Icon");
        EditorGUILayout.PropertyField(iconProp);
        SerializedProperty colorProp = serializedObject.FindProperty("Color");
        EditorGUILayout.PropertyField(colorProp);
        EditorGUILayout.EndVertical();

        // Thumbnail preview
        SerializedProperty iconPreviewProp = serializedObject.FindProperty("Icon");
        Sprite sprite = iconPreviewProp.objectReferenceValue as Sprite;
        EditorGUILayout.BeginVertical(GUILayout.Width(84), GUILayout.Height(84));
        Rect previewRect = GUILayoutUtility.GetRect(72, 72, GUILayout.Width(72), GUILayout.Height(72));
        Color prevColor = GUI.color;
        EditorGUI.DrawRect(previewRect, new Color(0.15f, 0.15f, 0.15f, 1f));
        Handles.color = Color.gray;
        Handles.DrawAAPolyLine(2f, new Vector3[] {
            new Vector3(previewRect.xMin, previewRect.yMin), new Vector3(previewRect.xMax, previewRect.yMin),
            new Vector3(previewRect.xMax, previewRect.yMax), new Vector3(previewRect.xMin, previewRect.yMax), new Vector3(previewRect.xMin, previewRect.yMin)
        });
        if (sprite != null && sprite.texture != null)
        {
            Texture2D tex = sprite.texture;
            Rect spriteRect = sprite.rect;
            Rect uv = new Rect(
                spriteRect.x / tex.width,
                spriteRect.y / tex.height,
                spriteRect.width / tex.width,
                spriteRect.height / tex.height
            );
            // Tint with assigned color
            colorProp = serializedObject.FindProperty("Color");
            Color tintColor = colorProp.colorValue;
            GUI.color = tintColor;
            GUI.DrawTextureWithTexCoords(previewRect, tex, uv, true);
            GUI.color = prevColor;
        }
        else
        {
            EditorGUI.LabelField(previewRect, "No Icon", new GUIStyle() { alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.gray } });
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        // InputActionAsset and related fields below

        EditorGUILayout.LabelField("Input Action", EditorStyles.boldLabel);
        EditorGUILayout.Space(8);

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
