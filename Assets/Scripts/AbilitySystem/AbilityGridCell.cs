namespace AbilitySystem
{
    using UnityEngine;
    using UnityEngine.InputSystem;
    using System.Linq;

    [CreateAssetMenu(fileName = "NewAbilityGridCell", menuName = "AbilitySystem/AbilityGridCell")]
    public class AbilityGridCell : ScriptableObject
    {
        // Fields to be serialized and shown in the inspector
        public Ability Ability;
        [SerializeField] private InputActionAsset _inputActionsAsset;
        [SerializeField] private string _selectedActionMapName;
        [SerializeField] private string _leftActionName;
        [SerializeField] private string _rightActionName;
        [SerializeField] private string _upActionName;
        [SerializeField] private string _downActionName;

        // Runtime properties to resolve InputActions
        public InputAction LeftAction { get; private set; }
        public InputAction RightAction { get; private set; }
        public InputAction UpAction { get; private set; }
        public InputAction DownAction { get; private set; }

        public bool HasLeftAction { get; private set; }
        public bool HasRightAction { get; private set; }
        public bool HasUpAction { get; private set; }
        public bool HasDownAction { get; private set; }

        public Vector2Int GridPosition { get; set; }
        public AbilityGridUICell UIElement { get; private set; }
        private AbilityGridCell[] _neighbors = new AbilityGridCell[4]; // Left, Right, Up, Down

        public void InitializeActions(AbilityGridCell[] neighbors)
        {
            UpdateActions();
            ClearAllActions();
            _neighbors = neighbors;

            // neighbors[0] = Left, neighbors[1] = Right, neighbors[2] = Up, neighbors[3] = Down
            if (_neighbors[0] != null) LeftAction.started += ctx => MoveLeft();
            if (_neighbors[1] != null) RightAction.started += ctx => MoveRight();
            if (_neighbors[2] != null) UpAction.started += ctx => MoveUp();
            if (_neighbors[3] != null) DownAction.started += ctx => MoveDown();

            HasLeftAction = _neighbors[0] != null;
            HasRightAction = _neighbors[1] != null;
            HasUpAction = _neighbors[2] != null;
            HasDownAction = _neighbors[3] != null;
        }

        public void SetUICell(AbilityGridUICell uiCell) { UIElement = uiCell; }
        private void MoveLeft() { AbilityGridManager.Instance.MoveCell(_neighbors[0]); }
        private void MoveRight() { AbilityGridManager.Instance.MoveCell(_neighbors[1]); }
        private void MoveUp() { AbilityGridManager.Instance.MoveCell(_neighbors[2]); }
        private void MoveDown() { AbilityGridManager.Instance.MoveCell(_neighbors[3]); }

        public void EnterCell()
        {
            if (UIElement != null) UIElement.EnterCell();

            if (Ability != null) Ability.Activate();

            LeftAction.Enable();
            RightAction.Enable();
            UpAction.Enable();
            DownAction.Enable();
        }

        public void ExitCell()
        {
            if (UIElement != null) UIElement.ExitCell();

            LeftAction.Disable();
            RightAction.Disable();
            UpAction.Disable();
            DownAction.Disable();
        }

        public void IdleCell()
        {
            if (UIElement != null) UIElement.IdleCell();
        }

        public void RotateCell(bool clockwise)
        {
            if (UIElement != null) UIElement.RotateCell(clockwise);

            if (clockwise)
            {
                // Shift actions clockwise
                string temp = _upActionName;
                _upActionName = _leftActionName;
                _leftActionName = _downActionName;
                _downActionName = _rightActionName;
                _rightActionName = temp;
            }
            else
            {
                // Shift actions counter-clockwise
                string temp = _upActionName;
                _upActionName = _rightActionName;
                _rightActionName = _downActionName;
                _downActionName = _leftActionName;
                _leftActionName = temp;
            }
            InitializeActions(_neighbors);
        }

        private void UpdateActions()
        {
            LeftAction = ResolveInputAction(_leftActionName);
            RightAction = ResolveInputAction(_rightActionName);
            UpAction = ResolveInputAction(_upActionName);
            DownAction = ResolveInputAction(_downActionName);
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
            if (LeftAction != null) LeftAction.started -= ctx => MoveLeft();
            if (RightAction != null) RightAction.started -= ctx => MoveRight();
            if (UpAction != null) UpAction.started -= ctx => MoveUp();
            if (DownAction != null) DownAction.started -= ctx => MoveDown();
        }
    }
}
