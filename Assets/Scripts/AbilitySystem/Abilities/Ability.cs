namespace AbilitySystem
{
    using UnityEngine;

    public abstract class Ability : ScriptableObject
    {
        public readonly float Cooldown = 5f;
        public Sprite Icon;

        public void Initialize(int index) { PlayerControllerBase.Instance.AbilityManager.RegisterAbility(this, index); }
        public void Deinitialize(int index) { PlayerControllerBase.Instance.AbilityManager.UnRegisterAbility(this, index); }

        public abstract void Activate();
    }
}
