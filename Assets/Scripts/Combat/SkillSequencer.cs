using System.Reflection;
using UnityEngine;

public class SkillSequencer : MonoBehaviour
{
    public enum SkillPhase
    {
        Inactive,
        Anticipation,
        Active,
        Recovery
    }

    [Header("Skill Execution")]
    [SerializeField] private Animator _animator;

    public SkillPhase CurrentPhase { get; private set; } = SkillPhase.Inactive;
    public bool CanStartSkill => CurrentPhase == SkillPhase.Inactive || CurrentPhase == SkillPhase.Recovery;
    private Skill _currentSkill;
    private DamageHitbox _hitbox;
    private int _lastFrame = -1;

    void Update()
    {
        if (CurrentPhase == SkillPhase.Inactive || _currentSkill == null || _animator == null) return;

        AnimationClip clip = _currentSkill.Animation;
        if (clip == null) return;

        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName(clip.name))
        {
            // End skill when animation is no longer playing the assigned clip
            if (CurrentPhase == SkillPhase.Recovery)
            {
                _currentSkill.EndSkill();
                _currentSkill = null;
                _hitbox = null;
                CurrentPhase = SkillPhase.Inactive;
            }
            return;
        }

        float normalizedTime = stateInfo.normalizedTime;
        float animLength = clip.length;
        float frameRate = clip.frameRate;
        int totalFrames = Mathf.RoundToInt(animLength * frameRate);
        int currentFrame = Mathf.FloorToInt(Mathf.Clamp01(normalizedTime % 1f) * totalFrames);

        // Only process if frame changed
        if (currentFrame != _lastFrame)
        {
            _lastFrame = currentFrame;
            // Start active phase
            if (CurrentPhase == SkillPhase.Anticipation && currentFrame == _currentSkill.StartActiveFrame)
            {
                _currentSkill.StartActivePhase();
                CurrentPhase = SkillPhase.Active;
            }
            // End active phase
            if (CurrentPhase == SkillPhase.Active && currentFrame == _currentSkill.EndActiveFrame)
            {
                _currentSkill.EndActivePhase();
                CurrentPhase = SkillPhase.Recovery;
            }

            // Handle animation events
            foreach (var animEvent in _currentSkill.AnimationEvents)
            {
                if (animEvent.Frame == currentFrame)
                {
                    animEvent.Method?.Invoke(_currentSkill, null); // Pass parameters if needed
                }
            }
        }
    }

    public bool TryStartSkill(Skill skill, DamageHitbox hitbox = null)
    {
        if (CurrentPhase == SkillPhase.Inactive)
        {
            // From idle or movement
            StartSkill(skill, hitbox);
            return true;
        }
        else if (CurrentPhase == SkillPhase.Recovery)
        {
            // Combo
            InterruptCurrentSkill();
            StartSkill(skill, hitbox);
            return true;
        }

        return false;
    }

    private void InterruptCurrentSkill()
    {
        if (_currentSkill == null) return;

        _currentSkill.InterruptSkill();
    }

    private void StartSkill(Skill skill, DamageHitbox hitbox = null)
    {
        _currentSkill = skill;
        _hitbox = hitbox;
        if (_currentSkill == null || _animator == null) return;

        _currentSkill.StartSkill(_hitbox);
        _animator.Play(_currentSkill.Animation.name, 0, 0f);
        _lastFrame = -1;
        CurrentPhase = SkillPhase.Anticipation;
    }
}
