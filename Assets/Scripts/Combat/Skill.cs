using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Combat System/Skill")]
public class Skill : ScriptableObject
{
    [System.Serializable]
    public class AnimationEvent
    {
        public int Frame;
        public string EventName;
    }

    // Ability properties
    [SerializeField] private Sprite _icon;
    [SerializeField] private float _damage = 1f;
    // [SerializeField] private float _cooldown = 5f;
    [SerializeField] private float _staminaCost = 1f;
    [SerializeField] private GameObject _hitboxPrefab;

    public Sprite Icon => _icon;
    public float Damage => _damage;
    // public float Cooldown => _cooldown;
    public float StaminaCost => _staminaCost;
    public GameObject HitboxPrefab => _hitboxPrefab;

    // Animation and event handling
    [SerializeField] private AnimationClip _animation;
    [SerializeField] private int _startActiveFrame;
    [SerializeField] private int _endActiveFrame;
    [SerializeField] private List<AnimationEvent> _animationEvents = new();

    public AnimationClip Animation => _animation;
    public int StartActiveFrame => _startActiveFrame;
    public int EndActiveFrame => _endActiveFrame;
    public List<AnimationEvent> AnimationEvents => _animationEvents;

    private DamageHitbox _sequencedHitbox;

    public void Activate()
    {
        // TODO: handle hitboxes
        PlayerController2D.Instance.SkillSequencer.TryStartSkill(this);
    }

    public virtual void StartSkill(DamageHitbox hitbox)
    {
        _sequencedHitbox = hitbox;
        _sequencedHitbox.InitializeHitbox(1, LayerMask.GetMask("Enemy"));
    }

    public void StartActivePhase()
    {
        if (_sequencedHitbox != null)
        {
            _sequencedHitbox.SetHitboxActive(true);
        }
    }

    public void EndActivePhase()
    {
        if (_sequencedHitbox != null)
        {
            _sequencedHitbox.SetHitboxActive(false);
        }
    }

    public virtual void InterruptSkill()
    {
        if (_sequencedHitbox != null)
        {
            _sequencedHitbox.SetHitboxActive(false);
        }
        ClearData();
    }

    public virtual void EndSkill()
    {
        ClearData();
    }

    public virtual void ConsumeStamina()
    {
        PlayerControllerBase.Instance.ConsumeStamina(_staminaCost);
    }

    public virtual void StopAllowMovement()
    {
        PlayerControllerBase.Instance.ToggleMovement(false);
    }

    public virtual void StartAllowMovement()
    {
        PlayerControllerBase.Instance.ToggleMovement(true);
    }

    private void ClearData()
    {
        if (_sequencedHitbox != null)
        {
            _sequencedHitbox = null;
        }
    }
}
