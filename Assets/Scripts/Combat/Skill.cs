using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Combat System/Skill")]
public class Skill : ScriptableObject
{
    [SerializeField] private AnimationClip _animation;
    [SerializeField] private int _startActiveFrame;
    [SerializeField] private int _endActiveFrame;
    public AnimationClip Animation => _animation;
    public int StartActiveFrame => _startActiveFrame;
    public int EndActiveFrame => _endActiveFrame;
    private DamageHitbox _hitbox;

    public void StartSkill(Animator animator, DamageHitbox hitbox)
    {
        _hitbox = hitbox;
        animator.Play(_animation.name);
    }

    public void StartActivePhase()
    {
        Debug.Log("Start Active Phase");
        if (_hitbox != null)
        {
            _hitbox.SetHitboxActive(true);
        }
    }

    public void EndActivePhase()
    {
        Debug.Log("End Active Phase");
        if (_hitbox != null)
        {
            _hitbox.SetHitboxActive(false);
        }
    }
}
