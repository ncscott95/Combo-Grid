using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Combat System/Skill")]
public class Skill : ScriptableObject
{
    [SerializeField] private AnimationClip _animation;
    [SerializeField] private int _startActiveFrame;
    [SerializeField] private int _endActiveFrame;
    private int _currentFrame = 0;
    private DamageHitbox _hitbox;

    public void StartSkill(Animator animator, DamageHitbox hitbox)
    {
        _hitbox = hitbox;
        animator.Play(_animation.name);
    }

    public AnimationClip GetAnimationClip() => _animation;
    public void SetAnimationClip(AnimationClip clip) => _animation = clip;
}
