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
    private Skill _skill;
    private DamageHitbox _hitbox;
    private int _lastFrame = -1;

    void Update()
    {
        if (CurrentPhase == SkillPhase.Inactive || _skill == null || _animator == null) return;

        AnimationClip clip = _skill.Animation;
        if (clip == null) return;

        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName(clip.name)) return;

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
            if (CurrentPhase == SkillPhase.Anticipation && currentFrame == _skill.StartActiveFrame)
            {
                _skill.StartActivePhase();
                CurrentPhase = SkillPhase.Active;
            }
            // End active phase
            if (CurrentPhase == SkillPhase.Active && currentFrame == _skill.EndActiveFrame)
            {
                _skill.EndActivePhase();
                CurrentPhase = SkillPhase.Recovery;
            }
        }

        // End skill when animation finishes
        if (normalizedTime >= 1f)
        {
            _skill = null;
            _hitbox = null;
            CurrentPhase = SkillPhase.Inactive;
        }
    }

    public void StartSkill(Skill skill, DamageHitbox hitbox = null)
    {
        _skill = skill;
        _hitbox = hitbox;
        if (_skill == null || _animator == null) return;

        _skill.StartSkill(_animator, _hitbox);
        _lastFrame = -1;
        CurrentPhase = SkillPhase.Anticipation;
    }
}
