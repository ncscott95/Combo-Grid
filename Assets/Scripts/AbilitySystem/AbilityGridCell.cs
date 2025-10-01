using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewAbilityGridCell", menuName = "AbilitySystem/AbilityGridCell")]
public class AbilityGridCell : ScriptableObject
{
    // Fields to be serialized and shown in the inspector
    // public Ability Ability;
    public Skill Skill;
    [SerializeField] private List<AbilityGridTransition> _transitions = new() { null, null, null, null }; // Up, Left, Down, Right

    // Runtime properties to resolve InputActions
    public InputAction UpAction { get; private set; }
    public InputAction LeftAction { get; private set; }
    public InputAction DownAction { get; private set; }
    public InputAction RightAction { get; private set; }
    public List<AbilityGridTransition> Transitions => _transitions;

    public bool HasUpAction { get; private set; }
    public bool HasLeftAction { get; private set; }
    public bool HasDownAction { get; private set; }
    public bool HasRightAction { get; private set; }

    public Vector2Int GridPosition { get; set; }
    public AbilityGridUICell UIElement { get; private set; }
    private AbilityGridCell[] _neighbors = new AbilityGridCell[4]; // Up, Left, Down, Right

    public void InitializeActions(AbilityGridCell[] neighbors)
    {
        GetActionsFromStrings();
        UnsubscribeAllActions();
        _neighbors = neighbors;

        // neighbors[0] = Up, neighbors[1] = Left, neighbors[2] = Down, neighbors[3] = Right
        if (_neighbors[0] != null) UpAction.started += ctx => TryMoveCell(0);
        if (_neighbors[1] != null) LeftAction.started += ctx => TryMoveCell(1);
        if (_neighbors[2] != null) DownAction.started += ctx => TryMoveCell(2);
        if (_neighbors[3] != null) RightAction.started += ctx => TryMoveCell(3);

        HasUpAction = _neighbors[0] != null;
        HasLeftAction = _neighbors[1] != null;
        HasDownAction = _neighbors[2] != null;
        HasRightAction = _neighbors[3] != null;
    }

    public void SetUICell(AbilityGridUICell uiCell) { UIElement = uiCell; }

    private void TryMoveCell(int direction)
    {
        if (PlayerControllerBase.Instance.CanUseStamina(_neighbors[direction].Skill.StaminaCost))
        {
            AbilityGridManager.Instance.MoveCell(_neighbors[direction]);
        }
    }

    public void EnterCell()
    {
        if (UIElement != null) UIElement.EnterCell();

        if (Skill != null) Skill.Activate();

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
        _transitions = ListRotator.RotateList(_transitions, clockwise, 1);
        InitializeActions(_neighbors);
    }

    private void GetActionsFromStrings()
    {
        UpAction = ResolveInputAction(_transitions[0]);
        LeftAction = ResolveInputAction(_transitions[1]);
        DownAction = ResolveInputAction(_transitions[2]);
        RightAction = ResolveInputAction(_transitions[3]);
    }

    private InputAction ResolveInputAction(AbilityGridTransition transition)
    {
        InputActionAsset inputActionsAsset = transition.InputActionsAsset;
        string selectedActionMapName = transition.SelectedActionMapName;
        string actionName = transition.ActionName;

        if (inputActionsAsset == null || string.IsNullOrEmpty(selectedActionMapName) || string.IsNullOrEmpty(actionName))
        {
            Debug.LogWarning("InputActionAsset, ActionMap name, or Action name is not set properly.");
            return null;
        }

        InputActionMap map = inputActionsAsset.actionMaps.FirstOrDefault(m => m.name == selectedActionMapName);
        if (map == null)
        {
            Debug.LogWarning($"ActionMap '{selectedActionMapName}' not found in the InputActionAsset.");
            return null;
        }

        var original = map.actions.FirstOrDefault(a => a.name == actionName);
        if (original == null)
        {
            Debug.LogWarning($"Action '{actionName}' not found in ActionMap '{selectedActionMapName}'.");
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

    private void UnsubscribeAllActions()
    {
        if (UpAction != null) UpAction.started -= ctx => TryMoveCell(0);
        if (LeftAction != null) LeftAction.started -= ctx => TryMoveCell(1);
        if (DownAction != null) DownAction.started -= ctx => TryMoveCell(2);
        if (RightAction != null) RightAction.started -= ctx => TryMoveCell(3);
    }
}
