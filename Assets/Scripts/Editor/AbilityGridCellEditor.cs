using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(AbilityGridCell))]
[CanEditMultipleObjects]
public class AbilityGridCellEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();

        var abilityProp = this.serializedObject.FindProperty("Skill");
        root.Add(new PropertyField(abilityProp, "Skill"));

        var transitionsProp = this.serializedObject.FindProperty("_transitions");
        string[] directions = { "Up Transition", "Left Transition", "Down Transition", "Right Transition" };
        for (int i = 0; i < directions.Length; i++)
        {
            SerializedProperty transitionProp = transitionsProp.GetArrayElementAtIndex(i);
            root.Add(new PropertyField(transitionProp, directions[i]));
        }

        return root;
    }
}
