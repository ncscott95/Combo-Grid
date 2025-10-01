using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(Attribute))]
public class AttributeObjectEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();
        root.Add(new Label("Attribute Properties") { style = { unityFontStyleAndWeight = FontStyle.Bold } });
        root.Add(new IMGUIContainer(() => GUILayout.Space(8)));

        var horizontalContainer = new VisualElement();
        horizontalContainer.style.flexDirection = FlexDirection.Row;
        horizontalContainer.style.marginBottom = 8;

        // Left section: vertical fields
        var leftSection = new VisualElement();
        leftSection.style.flexDirection = FlexDirection.Column;
        leftSection.style.flexGrow = 1;
        leftSection.style.marginRight = 8;

        var nameProp = serializedObject.FindProperty("AttributeName");
        var nameField = new PropertyField(nameProp, "Attribute Name");
        leftSection.Add(nameField);

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
                // Tinting not natively supported in UIElements Image, so just show the texture
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

        root.Add(new Label("Attribute Values") { style = { unityFontStyleAndWeight = FontStyle.Bold } });
        root.Add(new IMGUIContainer(() => GUILayout.Space(8)));

        var maxValueProp = serializedObject.FindProperty("MaxValue");
        root.Add(new PropertyField(maxValueProp, "Max Value"));

        var regenDelayProp = serializedObject.FindProperty("RegenDelay");
        root.Add(new PropertyField(regenDelayProp, "Regen Delay (s)"));

        var regenRateProp = serializedObject.FindProperty("RegenRate");
        root.Add(new PropertyField(regenRateProp, "Regen Rate (per second)"));

        return root;
    }
}
