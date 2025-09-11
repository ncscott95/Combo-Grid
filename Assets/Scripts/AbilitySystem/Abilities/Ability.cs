namespace AbilitySystem
{
    using UnityEngine;

    public abstract class Ability : ScriptableObject
    {
        public Sprite Icon;
        public Skill Skill;
        public float Cooldown = 5f;

        public void Initialize(int index) { PlayerControllerBase.Instance.AbilityManager.RegisterAbility(this, index); }
        public void Deinitialize(int index) { PlayerControllerBase.Instance.AbilityManager.UnRegisterAbility(this, index); }

        public virtual void Activate()
        {
            AbilityManager.Instance.PlayerSkillSequencer.TryStartSkill(Skill, null);
        }
    }
}
