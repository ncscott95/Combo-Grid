using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Skill))]
public class SkillObjectEditor : Editor
{
    private int _previewFrame = 0;
    private int _maxFrame = 0;

    private SerializedProperty _animationProp;
    private SerializedProperty _startActiveFrameProp;
    private SerializedProperty _endActiveFrameProp;

    private const float LabelWidth = 120f;
    private const float ValueWidth = 45f;
    private const float SliderHeight = 20f;

    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();
        serializedObject.Update();
        _animationProp = serializedObject.FindProperty("_animation");
        _startActiveFrameProp = serializedObject.FindProperty("_startActiveFrame");
        _endActiveFrameProp = serializedObject.FindProperty("_endActiveFrame");

        root.Add(CreateAnimationClipField());
        root.Add(new IMGUIContainer(() => GUILayout.Space(8)));
        root.Add(new IMGUIContainer(() => DrawCustomAnimationPreview()));

        return root;
    }

    private ObjectField CreateAnimationClipField()
    {
        var animationField = new ObjectField("Animation")
        {
            objectType = typeof(AnimationClip),
            value = _animationProp.objectReferenceValue as AnimationClip
        };
        animationField.RegisterValueChangedCallback(evt =>
        {
            serializedObject.Update();
            _animationProp.objectReferenceValue = evt.newValue as AnimationClip;
            serializedObject.ApplyModifiedProperties();
        });
        return animationField;
    }

    private void DrawCustomAnimationPreview()
    {
        var clip = _animationProp.objectReferenceValue as AnimationClip;
        if (clip != null)
        {
            // Try to get the Sprite keyframes (for 2D sprite animations)
            var bindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);
            foreach (var binding in bindings)
            {
                if (binding.propertyName.Contains("m_Sprite"))
                {
                    var keyframes = AnimationUtility.GetObjectReferenceCurve(clip, binding);
                    if (keyframes != null && keyframes.Length > 0)
                    {
                        _maxFrame = keyframes.Length - 1;
                        _previewFrame = Mathf.Clamp(_previewFrame, 0, _maxFrame);
                        var sprite = keyframes[_previewFrame].value as Sprite;
                        if (sprite != null)
                        {
                            float previewHeight = 180f;
                            Rect rect = GUILayoutUtility.GetRect(10, previewHeight, GUILayout.ExpandWidth(true));
                            if (Event.current.type == EventType.Repaint)
                            {
                                // Draw background
                                Color prevColor = GUI.color;
                                Color bgColor = new Color(0.15f, 0.15f, 0.15f, 1f);
                                EditorGUI.DrawRect(rect, bgColor);

                                // Draw border
                                Handles.color = Color.gray;
                                Handles.DrawAAPolyLine(2f, new Vector3[] {
                                    new Vector3(rect.xMin, rect.yMin), new Vector3(rect.xMax, rect.yMin),
                                    new Vector3(rect.xMax, rect.yMax), new Vector3(rect.xMin, rect.yMax), new Vector3(rect.xMin, rect.yMin)
                                });

                                // Draw sprite
                                var tex = sprite.texture;
                                var spriteRect = sprite.rect;
                                Rect uv = new Rect(
                                    spriteRect.x / tex.width,
                                    spriteRect.y / tex.height,
                                    spriteRect.width / tex.width,
                                    spriteRect.height / tex.height
                                );

                                // Maintain aspect ratio
                                float aspect = spriteRect.width / spriteRect.height;
                                float targetWidth = rect.height * aspect;
                                if (targetWidth > rect.width)
                                {
                                    rect.height = rect.width / aspect;
                                }
                                else
                                {
                                    rect.width = targetWidth;
                                }
                                GUI.DrawTextureWithTexCoords(rect, tex, uv, true);
                                GUI.color = prevColor;
                            }

                            GUILayout.Space(8);

                            // Frame slider
                            GUILayout.BeginHorizontal();
                            GUILayout.Label($"Frame Preview: ", GUILayout.Width(LabelWidth));
                            float sliderWidth = EditorGUIUtility.currentViewWidth - LabelWidth - ValueWidth - 50f; // fudge factor for padding/scrollbar
                            Rect sliderRect = GUILayoutUtility.GetRect(sliderWidth, SliderHeight);
                            _previewFrame = (int)GUI.HorizontalSlider(sliderRect, _previewFrame, 0, _maxFrame);
                            GUILayout.Label($"{_previewFrame + 1} / {keyframes.Length}", GUILayout.Width(ValueWidth));
                            GUILayout.EndHorizontal();
                            // Draw ticks for each frame, vertically centered on the slider
                            if (Event.current.type == EventType.Repaint)
                            {
                                Handles.color = Color.gray;
                                int tickCount = _maxFrame + 1;
                                float tickHeight = 6f;
                                float yOffset = 1f; // Nudge up for better centering
                                float yCenter = sliderRect.center.y - yOffset;
                                float yMin = yCenter - tickHeight / 2f;
                                float yMax = yCenter + tickHeight / 2f;
                                for (int i = 0; i < tickCount; i++)
                                {
                                    float t = (float)i / _maxFrame;
                                    float x = Mathf.Lerp(sliderRect.xMin, sliderRect.xMax, t);
                                    Handles.DrawLine(new Vector3(x, yMin), new Vector3(x, yMax));
                                }
                            }

                            // Active phase MinMaxSlider
                            int start = _startActiveFrameProp.intValue;
                            int end = _endActiveFrameProp.intValue;
                            float min = start;
                            float max = end;

                            // Snap to int and clamp before drawing label and slider
                            // Let the slider use the original float values for smooth dragging
                            GUILayout.BeginHorizontal();
                            GUILayout.Label($"Active Phase: ", GUILayout.Width(LabelWidth));
                            float minMaxSliderWidth = EditorGUIUtility.currentViewWidth - LabelWidth - ValueWidth - 50f;
                            Rect minMaxSliderRect = GUILayoutUtility.GetRect(minMaxSliderWidth, SliderHeight);
                            EditorGUI.MinMaxSlider(minMaxSliderRect, ref min, ref max, 0, _maxFrame);

                            // Draw ticks for each frame, vertically centered on the slider
                            if (Event.current.type == EventType.Repaint)
                            {
                                Handles.color = Color.gray;
                                int tickCount = _maxFrame + 1;
                                float tickHeight = 6f;
                                float yOffset = 1f; // Nudge up for better centering
                                float yCenter = minMaxSliderRect.center.y - yOffset;
                                float yMin = yCenter - tickHeight / 2f;
                                float yMax = yCenter + tickHeight / 2f;
                                for (int i = 0; i < tickCount; i++)
                                {
                                    float t = (float)i / _maxFrame;
                                    float x = Mathf.Lerp(minMaxSliderRect.xMin, minMaxSliderRect.xMax, t);
                                    Handles.DrawLine(new Vector3(x, yMin), new Vector3(x, yMax));
                                }
                            }

                            // After dragging, clamp and round for display and saving
                            int newStart = Mathf.Clamp(Mathf.RoundToInt(min), 0, Mathf.Max(0, Mathf.RoundToInt(max) - 1));
                            int newEnd = Mathf.Clamp(Mathf.RoundToInt(max), Mathf.Min(newStart + 1, _maxFrame), _maxFrame);
                            GUILayout.Label($"{newStart + 1} - {newEnd + 1}", GUILayout.Width(ValueWidth));
                            GUILayout.EndHorizontal();

                            // Save changes if needed
                            if (newStart != _startActiveFrameProp.intValue || newEnd != _endActiveFrameProp.intValue)
                            {
                                _startActiveFrameProp.intValue = newStart;
                                _endActiveFrameProp.intValue = newEnd;
                                serializedObject.ApplyModifiedProperties();
                            }
                        }
                        else
                        {
                            GUILayout.Label("No sprite found for this frame.");
                        }
                        return;
                    }
                }
            }
            GUILayout.Label("No sprite preview available for this AnimationClip.");
        }
        else
        {
            GUILayout.Label("No AnimationClip assigned.");
        }
    }
}
