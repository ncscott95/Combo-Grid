namespace AbilitySystem
{
    using UnityEngine;
    using UnityEngine.InputSystem;
    using System.Linq;
    using System.Collections.Generic;

    [CreateAssetMenu(fileName = "NewAbilityGridCell", menuName = "AbilitySystem/AbilityGridCell")]
    public class AbilityGridCell : ScriptableObject
    {
        // Fields to be serialized and shown in the inspector
        public Ability Ability;
        [SerializeField] private InputActionAsset _inputActionsAsset;
        [SerializeField] private string _selectedActionMapName;
        [SerializeField] private List<string> _actionNames = new() { "", "", "", "" }; // Up, Left, Down, Right

        // Runtime properties to resolve InputActions
        public InputAction UpAction { get; private set; }
        public InputAction LeftAction { get; private set; }
        public InputAction DownAction { get; private set; }
        public InputAction RightAction { get; private set; }
        public List<string> ActionNames => _actionNames;

        public bool HasUpAction { get; private set; }
        public bool HasLeftAction { get; private set; }
        public bool HasDownAction { get; private set; }
        public bool HasRightAction { get; private set; }

        public Vector2Int GridPosition { get; set; }
        public AbilityGridUICell UIElement { get; private set; }
        private AbilityGridCell[] _neighbors = new AbilityGridCell[4]; // Up, Left, Down, Right

        public void InitializeActions(AbilityGridCell[] neighbors)
        {
            UpdateActions();
            ClearAllActions();
            _neighbors = neighbors;

            // neighbors[0] = Up, neighbors[1] = Left, neighbors[2] = Down, neighbors[3] = Right
            if (_neighbors[0] != null) UpAction.started += ctx => MoveUp();
            if (_neighbors[1] != null) LeftAction.started += ctx => MoveLeft();
            if (_neighbors[2] != null) DownAction.started += ctx => MoveDown();
            if (_neighbors[3] != null) RightAction.started += ctx => MoveRight();

            HasUpAction = _neighbors[0] != null;
            HasLeftAction = _neighbors[1] != null;
            HasDownAction = _neighbors[2] != null;
            HasRightAction = _neighbors[3] != null;
        }

        public void SetUICell(AbilityGridUICell uiCell) { UIElement = uiCell; }
        private void MoveUp() { AbilityGridManager.Instance.MoveCell(_neighbors[0]); }
        private void MoveLeft() { AbilityGridManager.Instance.MoveCell(_neighbors[1]); }
        private void MoveDown() { AbilityGridManager.Instance.MoveCell(_neighbors[2]); }
        private void MoveRight() { AbilityGridManager.Instance.MoveCell(_neighbors[3]); }

        public void EnterCell()
        {
            if (UIElement != null) UIElement.EnterCell();

            if (Ability != null) Ability.Activate();

            UpAction.Enable();
            LeftAction.Enable();
            DownAction.Enable();
            RightAction.Enable();
        }

        public void ExitCell()
        {
            if (UIElement != null) UIElement.ExitCell();

            UpAction.Disable();
            LeftAction.Disable();
            DownAction.Disable();
            RightAction.Disable();
        }

        public void IdleCell()
        {
            if (UIElement != null) UIElement.IdleCell();
        }

        public void RotateCell(bool clockwise)
        {
            if (UIElement != null) UIElement.RotateCell(clockwise);
            _actionNames = ListRotator.RotateList(_actionNames, clockwise, 1);
            InitializeActions(_neighbors);
        }

        private void UpdateActions()
        {
            UpAction = ResolveInputAction(_actionNames[0]);
            LeftAction = ResolveInputAction(_actionNames[1]);
            DownAction = ResolveInputAction(_actionNames[2]);
            RightAction = ResolveInputAction(_actionNames[3]);
        }

        private InputAction ResolveInputAction(string actionName)
        {
            if (_inputActionsAsset == null || string.IsNullOrEmpty(_selectedActionMapName) || string.IsNullOrEmpty(actionName))
            {
                Debug.LogWarning("InputActionAsset, ActionMap name, or Action name is not set properly.");
                return null;
            }

            InputActionMap map = _inputActionsAsset.actionMaps.FirstOrDefault(m => m.name == _selectedActionMapName);
            if (map == null)
            {
                Debug.LogWarning($"ActionMap '{_selectedActionMapName}' not found in the InputActionAsset.");
                return null;
            }

            var original = map.actions.FirstOrDefault(a => a.name == actionName);
            if (original == null)
            {
                Debug.LogWarning($"Action '{actionName}' not found in ActionMap '{_selectedActionMapName}'.");
                return null;
            }

            // Create a new InputAction with the same name, type, and expected control type
            var newAction = new InputAction(
                name: original.name,
                type: original.type,
                expectedControlType: original.expectedControlType
            );

            // Copy all bindings
            foreach (var binding in original.bindings)
            {
                newAction.AddBinding(binding);
            }

            return newAction;
        }

        private void ClearAllActions()
        {
            if (UpAction != null) UpAction.started -= ctx => MoveUp();
            if (LeftAction != null) LeftAction.started -= ctx => MoveLeft();
            if (DownAction != null) DownAction.started -= ctx => MoveDown();
            if (RightAction != null) RightAction.started -= ctx => MoveRight();
        }
    }
}
