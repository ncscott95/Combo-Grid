using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(AbilityGridTransition))]
public class AbilityGridTransitionEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();
        root.Add(new Label("Transition Properties") { style = { unityFontStyleAndWeight = FontStyle.Bold } });
        root.Add(new IMGUIContainer(() => GUILayout.Space(8)));

        var horizontalContainer = new VisualElement();
        horizontalContainer.style.flexDirection = FlexDirection.Row;
        horizontalContainer.style.marginBottom = 8;

        // Left section: vertical fields
        var leftSection = new VisualElement();
        leftSection.style.flexDirection = FlexDirection.Column;
        leftSection.style.flexGrow = 1;
        leftSection.style.marginRight = 8;

        var iconProp = serializedObject.FindProperty("Icon");
        var iconField = new PropertyField(iconProp, "Icon");
        leftSection.Add(iconField);

        var colorProp = serializedObject.FindProperty("Color");
        var colorField = new PropertyField(colorProp, "Color");
        leftSection.Add(colorField);

        // Right section: thumbnail preview
        var rightSection = new VisualElement();
        rightSection.style.width = 84;
        rightSection.style.height = 84;
        rightSection.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f, 1f);
        rightSection.style.borderTopWidth = 2;
        rightSection.style.borderBottomWidth = 2;
        rightSection.style.borderLeftWidth = 2;
        rightSection.style.borderRightWidth = 2;
        rightSection.style.borderTopColor = Color.gray;
        rightSection.style.borderBottomColor = Color.gray;
        rightSection.style.borderLeftColor = Color.gray;
        rightSection.style.borderRightColor = Color.gray;
        rightSection.style.marginTop = 4;
        rightSection.style.justifyContent = Justify.Center;
        rightSection.style.alignItems = Align.Center;

        var iconImage = new Image();
        iconImage.style.width = 72;
        iconImage.style.height = 72;
        iconImage.scaleMode = ScaleMode.ScaleToFit;
        rightSection.Add(iconImage);

        void UpdateIconPreview()
        {
            Sprite sprite = iconProp.objectReferenceValue as Sprite;
            if (sprite != null && sprite.texture != null)
            {
                iconImage.image = sprite.texture;
            }
            else
            {
                iconImage.image = null;
            }
        }
        UpdateIconPreview();
        iconField.RegisterValueChangeCallback(evt =>
        {
            UpdateIconPreview();
        });

        horizontalContainer.Add(leftSection);
        horizontalContainer.Add(rightSection);
        root.Add(horizontalContainer);

        root.Add(new Label("Input Action") { style = { unityFontStyleAndWeight = FontStyle.Bold } });
        root.Add(new IMGUIContainer(() => GUILayout.Space(8)));

        // InputActionAsset and related fields below
        var assetProp = serializedObject.FindProperty("_inputActionsAsset");
        root.Add(new PropertyField(assetProp, "Input Actions Asset"));

        var selectedMapProp = serializedObject.FindProperty("_selectedActionMapName");
        var actionNameProp = serializedObject.FindProperty("_actionName");

        // Use IMGUIContainer for dynamic dropdowns
        root.Add(new IMGUIContainer(() =>
        {
            InputActionAsset asset = assetProp.objectReferenceValue as InputActionAsset;
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
        }));

        return root;
    }
}
