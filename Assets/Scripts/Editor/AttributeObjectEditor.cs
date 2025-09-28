using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Attribute))]
public class AttributeObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Attribute attribute = (Attribute)target;
        serializedObject.Update();

        EditorGUILayout.LabelField("Attribute Properties", EditorStyles.boldLabel);
        EditorGUILayout.Space(8);

        // Horizontal container for AttributeName, Icon, Color and thumbnail
        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
        SerializedProperty nameProp = serializedObject.FindProperty("AttributeName");
        EditorGUILayout.PropertyField(nameProp);
        SerializedProperty iconProp = serializedObject.FindProperty("Icon");
        EditorGUILayout.PropertyField(iconProp);
        SerializedProperty colorProp = serializedObject.FindProperty("Color");
        EditorGUILayout.PropertyField(colorProp);
        EditorGUILayout.EndVertical();

        // Thumbnail preview
        Sprite sprite = iconProp.objectReferenceValue as Sprite;
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

        // Attribute Values
        EditorGUILayout.LabelField("Attribute Values", EditorStyles.boldLabel);
        EditorGUILayout.Space(8);
        SerializedProperty maxValueProp = serializedObject.FindProperty("MaxValue");
        EditorGUILayout.PropertyField(maxValueProp);

        serializedObject.ApplyModifiedProperties();
    }
}
