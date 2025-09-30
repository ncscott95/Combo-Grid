using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Combat System/Skill")]
public class Skill : ScriptableObject
{
    private static readonly BindingFlags MethodBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    [System.Serializable]
    public class AnimationEvent
    {
        public int Frame;
        public string EventName;
        [System.NonSerialized] public MethodInfo Method;
    }

    // Ability properties
    [SerializeField] private Sprite _icon;
    [SerializeField] private float _cooldown = 5f;
    [SerializeField] private float _staminaCost = 1f;
    public Sprite Icon => _icon;
    public float Cooldown => _cooldown;
    public float StaminaCost => _staminaCost;

    // Animation and event handling
    [SerializeField] private AnimationClip _animation;
    [SerializeField] private int _startActiveFrame;
    [SerializeField] private int _endActiveFrame;
    [SerializeField] private List<AnimationEvent> _animationEvents = new();
    public AnimationClip Animation => _animation;
    public int StartActiveFrame => _startActiveFrame;
    public int EndActiveFrame => _endActiveFrame;
    public List<AnimationEvent> AnimationEvents => _animationEvents;

    private DamageHitbox _hitbox;

    public void Activate()
    {
        // Sort events by frame
        _animationEvents.Sort((a, b) => a.Frame.CompareTo(b.Frame));

        // Cache MethodInfo for each event
        foreach (var animEvent in _animationEvents)
        {
            if (animEvent.Method == null && !string.IsNullOrEmpty(animEvent.EventName))
                animEvent.Method = GetType().GetMethod(animEvent.EventName, MethodBindingFlags);
        }

        // TODO: handle hitboxes

        PlayerController2D.Instance.SkillSequencer.TryStartSkill(this, null);
    }

    public virtual void StartSkill(DamageHitbox hitbox)
    {
        _hitbox = hitbox;
    }

    public void StartActivePhase()
    {
        if (_hitbox != null)
        {
            _hitbox.SetHitboxActive(true);
        }
    }

    public void EndActivePhase()
    {
        if (_hitbox != null)
        {
            _hitbox.SetHitboxActive(false);
        }
    }

    public virtual void InterruptSkill()
    {
        if (_hitbox != null)
        {
            _hitbox.SetHitboxActive(false);
        }
        ClearData();
    }

    public virtual void EndSkill()
    {
        ClearData();
    }

    private void ClearData()
    {
        if (_hitbox != null)
        {
            _hitbox = null;
        }
    }

    private void TestPrint()
    {
        Debug.Log("TestPrint from Skill: " + name);
    }
}
