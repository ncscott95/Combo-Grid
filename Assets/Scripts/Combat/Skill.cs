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

    public void StartSkill(DamageHitbox hitbox)
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

    public void InterruptSkill()
    {
        if (_hitbox != null)
        {
            _hitbox.SetHitboxActive(false);
        }
        ClearData();
    }

    public void EndSkill()
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
}
