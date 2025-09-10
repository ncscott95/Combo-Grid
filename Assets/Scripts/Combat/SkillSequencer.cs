using UnityEngine;

public class SkillSequencer : MonoBehaviour
{
    [Header("Skill Execution")]
    [SerializeField] private Animator _animator;
    private Skill _skill;
    private DamageHitbox _hitbox;

    private bool isSkillActive = false;
    private int lastFrame = -1;
    private bool activePhaseStarted = false;
    private bool activePhaseEnded = false;

    // Update is called once per frame
    void Update()
    {
        if (!isSkillActive || _skill == null || _animator == null) return;
        var clip = _skill.Animation;
        if (clip == null) return;

        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName(clip.name)) return;

        float normalizedTime = stateInfo.normalizedTime;
        float animLength = clip.length;
        float frameRate = clip.frameRate;
        int totalFrames = Mathf.RoundToInt(animLength * frameRate);
        int currentFrame = Mathf.FloorToInt(Mathf.Clamp01(normalizedTime % 1f) * totalFrames);

        // Only process if frame changed
        if (currentFrame != lastFrame)
        {
            lastFrame = currentFrame;
            // Start active phase
            if (!activePhaseStarted && currentFrame == _skill.StartActiveFrame)
            {
                _skill.StartActivePhase();
                activePhaseStarted = true;
            }
            // End active phase
            if (!activePhaseEnded && currentFrame == _skill.EndActiveFrame)
            {
                _skill.EndActivePhase();
                activePhaseEnded = true;
            }
        }

        // End skill when animation finishes
        if (normalizedTime >= 1f)
        {
            isSkillActive = false;
        }
    }

    public void StartSkill(Skill skill, DamageHitbox hitbox = null)
    {
        _skill = skill;
        _hitbox = hitbox;
        if (_skill == null || _animator == null) return;

        _skill.StartSkill(_animator, _hitbox);
        isSkillActive = true;
        lastFrame = -1;
        activePhaseStarted = false;
        activePhaseEnded = false;
    }
}
