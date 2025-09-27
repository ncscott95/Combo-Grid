namespace AbilitySystem
{
    using UnityEngine;
    using UnityEngine.InputSystem;
    using System.Linq;

    [CreateAssetMenu(fileName = "NewAbilityGridTransition", menuName = "AbilitySystem/AbilityGridTransition")]
    public class AbilityGridTransition : ScriptableObject
    {
        public Sprite Icon;
        public Color Color;

        [SerializeField] private InputActionAsset _inputActionsAsset;
        [SerializeField] private string _selectedActionMapName;
        [SerializeField] private string _actionName;

        public InputActionAsset InputActionsAsset => _inputActionsAsset;
        public string SelectedActionMapName => _selectedActionMapName;
        public string ActionName => _actionName;
    }
}
