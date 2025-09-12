namespace AbilitySystem
{
    using UnityEngine;

    public abstract class Ability : ScriptableObject
    {
        public Sprite Icon;
        public Skill Skill;
        public float Cooldown = 5f;

        public virtual void Activate()
        {
            PlayerController2D.Instance.SkillSequencer.TryStartSkill(Skill, null);
        }
    }
}
