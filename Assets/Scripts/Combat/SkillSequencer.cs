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
    private GameObject _currentHitbox;
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
                Destroy(_currentHitbox);
                _currentHitbox = null;
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
                    string eventName = animEvent.EventName;
                    if (string.IsNullOrEmpty(eventName)) continue;

                    MethodInfo method = null;
                    object target = null;

                    if (eventName.StartsWith("Behaviors/"))
                    {
                        string methodName = eventName.Substring("Behaviors/".Length);
                        method = typeof(SkillBehaviors).GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
                        target = null; // Static method
                    }
                    else if (eventName.StartsWith("Skill/"))
                    {
                        string methodName = eventName.Substring("Skill/".Length);
                        method = _currentSkill.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        target = _currentSkill; // Instance method
                    }

                    method?.Invoke(target, null);
                }
            }
        }
    }

    public bool TryStartSkill(Skill skill)
    {
        if (CurrentPhase == SkillPhase.Inactive)
        {
            // From idle or movement
            StartSkill(skill);
            return true;
        }
        else if (CurrentPhase == SkillPhase.Recovery)
        {
            // Combo
            InterruptCurrentSkill();
            StartSkill(skill);
            return true;
        }

        return false;
    }

    private void InterruptCurrentSkill()
    {
        if (_currentSkill == null) return;

        _currentSkill.InterruptSkill();
        Destroy(_currentHitbox);
        _currentHitbox = null;
    }

    private void StartSkill(Skill skill)
    {
        _currentSkill = skill;
        _currentHitbox = Instantiate(skill.HitboxPrefab, transform);

        if (_currentSkill == null || _animator == null) return;

        // Flip hitbox based on player facing direction
        PlayerController2D playerController = GetComponentInParent<PlayerController2D>();
        if (playerController != null)
        {
            Debug.Log($"Flipping hitbox for facing direction {playerController.FacingDirection}");
            Vector3 hitboxScale = _currentHitbox.transform.localScale;
            _currentHitbox.transform.localScale = new Vector3(Mathf.Abs(hitboxScale.x) * playerController.FacingDirection, hitboxScale.y, hitboxScale.z);
        }

        _currentSkill.StartSkill(_currentHitbox.GetComponent<DamageHitbox>());
        _animator.Play(_currentSkill.Animation.name, 0, 0f);
        _lastFrame = -1;
        CurrentPhase = SkillPhase.Anticipation;
    }   
}
