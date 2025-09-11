namespace AbilitySystem
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "NewDebugAbility", menuName = "AbilitySystem/Abilities/DebugAbility")]
    public class DebugAbility : Ability
    {
        public override void Activate()
        {
            base.Activate();

            Debug.Log("DebugAbility activated");
        }
    }
}
