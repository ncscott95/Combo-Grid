using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;

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
        root.Add(new Label("Skill Properties") { style = { unityFontStyleAndWeight = FontStyle.Bold } });
        root.Add(new IMGUIContainer(() => GUILayout.Space(8)));

        var horizontalContainer = new VisualElement();
        horizontalContainer.style.flexDirection = FlexDirection.Row;
        horizontalContainer.style.marginBottom = 8;

        // Left section: vertical fields
        var leftSection = new VisualElement();
        leftSection.style.flexDirection = FlexDirection.Column;
        leftSection.style.flexGrow = 1;
        leftSection.style.marginRight = 8;

        var iconProp = serializedObject.FindProperty("_icon");
        var iconField = new PropertyField(iconProp, "Icon");
        leftSection.Add(iconField);

        var damageProp = serializedObject.FindProperty("_damage");
        var damageField = new PropertyField(damageProp, "Damage");
        leftSection.Add(damageField);

        // var cooldownProp = serializedObject.FindProperty("_cooldown");
        // var cooldownField = new PropertyField(cooldownProp, "Cooldown");
        // leftSection.Add(cooldownField);

        var staminaCostProp = serializedObject.FindProperty("_staminaCost");
        var staminaCostField = new PropertyField(staminaCostProp, "Stamina Cost");
        leftSection.Add(staminaCostField);

        var hitboxPrefabProp = serializedObject.FindProperty("_hitboxPrefab");
        var hitboxPrefabField = new PropertyField(hitboxPrefabProp, "Hitbox Prefab");
        leftSection.Add(hitboxPrefabField);

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

        // Animation sequencer
        _animationProp = serializedObject.FindProperty("_animation");
        _startActiveFrameProp = serializedObject.FindProperty("_startActiveFrame");
        _endActiveFrameProp = serializedObject.FindProperty("_endActiveFrame");
        var animationEventsProp = serializedObject.FindProperty("_animationEvents");

        root.Add(new Label("Animation Sequencer") { style = { unityFontStyleAndWeight = FontStyle.Bold } });
        root.Add(new IMGUIContainer(() => GUILayout.Space(8)));
        root.Add(new PropertyField(_animationProp, "Animation"));
        root.Add(new IMGUIContainer(() => GUILayout.Space(8)));
        root.Add(new IMGUIContainer(() => DrawCustomAnimationPreview()));

        // Animation Events List (ListView)
        root.Add(new IMGUIContainer(() => GUILayout.Space(8)));
        root.Add(new Label("Custom Events") { style = { unityFontStyleAndWeight = FontStyle.Bold } });
        root.Add(new IMGUIContainer(() => GUILayout.Space(8)));
        var skillTarget = (Skill)target;
        var methodNames = new List<string>();

        // Get instance methods from the Skill class and its subclasses
        var skillMethods = skillTarget.GetType().GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        foreach (var m in skillMethods)
        {
            if (!m.IsSpecialName && (m.DeclaringType == typeof(Skill) || m.DeclaringType.IsSubclassOf(typeof(Skill))) && !methodNames.Contains(m.Name))
            {
                // Add with a "Skill/" prefix for grouping
                methodNames.Add($"Skill/{m.Name}");
            }
        }

        // Get static methods from SkillBehaviors class
        var skillBehaviorsType = typeof(SkillBehaviors);
        if (skillBehaviorsType != null)
        {
            var behaviorMethods = skillBehaviorsType.GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            foreach (var m in behaviorMethods)
            {
                if (!m.IsSpecialName && m.DeclaringType == skillBehaviorsType && !methodNames.Contains(m.Name))
                {
                    // Add with a "Behaviors/" prefix for grouping
                    methodNames.Add($"Behaviors/{m.Name}");
                }
            }
        }

        var animationEventsList = new ListView
        {
            itemsSource = skillTarget.AnimationEvents,
            selectionType = SelectionType.None,
            style = { marginBottom = 8 },
            fixedItemHeight = 24
        };
        animationEventsList.makeItem = () =>
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Center;
            row.style.marginBottom = 4;
            var frameField = new IntegerField();
            frameField.style.width = 50;
            row.Add(frameField);
            var eventDropdown = new PopupField<string>(methodNames, methodNames.Count > 0 ? methodNames[0] : "");
            eventDropdown.style.marginLeft = 8;
            eventDropdown.style.flexGrow = 1;
            row.Add(eventDropdown);
            var removeButton = new Button();
            removeButton.text = "-";
            removeButton.style.marginLeft = 8;
            removeButton.style.width = 30;
            row.Add(removeButton);
            return row;
        };
        animationEventsList.bindItem = (element, i) =>
        {
            var row = (VisualElement)element;
            var frameField = row.ElementAt(0) as IntegerField;
            var eventDropdown = row.ElementAt(1) as PopupField<string>;
            var removeButton = row.ElementAt(2) as Button;
            frameField.SetValueWithoutNotify(skillTarget.AnimationEvents[i].Frame);
            frameField.RegisterValueChangedCallback(evt =>
            {
                skillTarget.AnimationEvents[i].Frame = evt.newValue;
                EditorUtility.SetDirty(skillTarget);
            });
            eventDropdown.value = skillTarget.AnimationEvents[i].EventName;
            eventDropdown.choices = methodNames;
            eventDropdown.RegisterValueChangedCallback(evt =>
            {
                skillTarget.AnimationEvents[i].EventName = evt.newValue;
                EditorUtility.SetDirty(skillTarget);
            });
            removeButton.clicked += () =>
            {
                skillTarget.AnimationEvents.RemoveAt(i);
                EditorUtility.SetDirty(skillTarget);
                animationEventsList.Rebuild(); // Rebuild is necessary after removing an item.
            };
        };
        root.Add(animationEventsList);

        var addButton = new Button(() => {
            skillTarget.AnimationEvents.Add(new Skill.AnimationEvent { Frame = 0, EventName = methodNames.Count > 0 ? methodNames[0] : string.Empty });
            EditorUtility.SetDirty(skillTarget);
            animationEventsList.Rebuild();
        });
        addButton.text = "Add Event";
        root.Add(addButton);
        return root;
    }

    private void DrawCustomAnimationPreview()
    {
        AnimationClip clip = _animationProp.objectReferenceValue as AnimationClip;
        if (clip != null)
        {
            // Try to get the Sprite keyframes (for 2D sprite animations)
            EditorCurveBinding[] bindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);
            foreach (var binding in bindings)
            {
                if (binding.propertyName.Contains("m_Sprite"))
                {
                    ObjectReferenceKeyframe[] keyframes = AnimationUtility.GetObjectReferenceCurve(clip, binding);
                    if (keyframes != null && keyframes.Length > 0)
                    {
                        _maxFrame = keyframes.Length - 1;
                        _previewFrame = Mathf.Clamp(_previewFrame, 0, _maxFrame);
                        Sprite sprite = keyframes[_previewFrame].value as Sprite;
                        if (sprite != null)
                        {
                            float previewHeight = 180f;
                            Rect rect = GUILayoutUtility.GetRect(10, previewHeight, GUILayout.ExpandWidth(true));
                            if (Event.current.type == EventType.Repaint)
                            {
                                // Draw background
                                Color prevColor = GUI.color;
                                Color bgColor = new(0.15f, 0.15f, 0.15f, 1f);
                                EditorGUI.DrawRect(rect, bgColor);

                                // Draw border
                                Handles.color = Color.gray;
                                Handles.DrawAAPolyLine(2f, new Vector3[] {
                                    new(rect.xMin, rect.yMin), new(rect.xMax, rect.yMin),
                                    new(rect.xMax, rect.yMax), new(rect.xMin, rect.yMax), new(rect.xMin, rect.yMin)
                                });

                                // Draw sprite
                                Texture2D tex = sprite.texture;
                                Rect spriteRect = sprite.rect;
                                Rect uv = new(
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
                            GUILayout.Label($"{_previewFrame} / {_maxFrame}", GUILayout.Width(ValueWidth));
                            GUILayout.EndHorizontal();
                            // Draw ticks for each frame, aligned with slider snap points
                            DrawSliderTicks(sliderRect, _maxFrame + 1);

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

                            // Draw ticks for each frame, aligned with slider snap points
                            DrawSliderTicks(minMaxSliderRect, _maxFrame + 1);

                            // After dragging, clamp and round for display and saving
                            int newStart = Mathf.Clamp(Mathf.RoundToInt(min), 0, Mathf.Max(0, Mathf.RoundToInt(max) - 1));
                            int newEnd = Mathf.Clamp(Mathf.RoundToInt(max), Mathf.Min(newStart + 1, _maxFrame), _maxFrame);
                            GUILayout.Label($"{newStart} - {newEnd}", GUILayout.Width(ValueWidth));
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

    private void DrawSliderTicks(Rect rect, int tickCount)
    {
        if (Event.current.type != EventType.Repaint) return;
        Handles.color = Color.gray;
        float tickHeight = 6f;
        float yOffset = 1f; // Nudge up for better centering
        float yCenter = rect.center.y - yOffset;
        float yMin = yCenter - tickHeight / 2f;
        float yMax = yCenter + tickHeight / 2f;
        float handleWidth = 10f; // Unity IMGUI slider handle width
        float xStart = rect.xMin + handleWidth / 2f;
        float xEnd = rect.xMax - handleWidth / 2f;
        for (int i = 0; i < tickCount; i++)
        {
            float t = (tickCount == 1) ? 0.5f : (float)i / (tickCount - 1);
            float x = Mathf.Lerp(xStart, xEnd, t);
            Handles.DrawLine(new Vector3(x, yMin), new Vector3(x, yMax));
        }
    }
}
