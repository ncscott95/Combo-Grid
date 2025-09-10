namespace AbilitySystem
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "NewAbilityGridCell", menuName = "AbilitySystem/AbilityGridCell")]
    public class AbilityGridCell : ScriptableObject
    {
        public Ability Ability;
        public UnityEngine.InputSystem.InputAction LeftAction;
        public UnityEngine.InputSystem.InputAction RightAction;
        public UnityEngine.InputSystem.InputAction UpAction;
        public UnityEngine.InputSystem.InputAction DownAction;
        public float Cooldown = 0.5f;

        public Vector2Int GridPosition { get; set; }
        public AbilityGridUICell UIElement { get; private set; }

        public void EnterCell()
        {
            UIElement.EnterCell();

            if (Ability != null) Ability.Activate();

            LeftAction.Enable();
            RightAction.Enable();
            UpAction.Enable();
            DownAction.Enable();
        }

        public void ExitCell()
        {
            UIElement.ExitCell();

            LeftAction.Disable();
            RightAction.Disable();
            UpAction.Disable();
            DownAction.Disable();
        }

        public void IdleCell()
        {
            UIElement.IdleCell();
        }

        public void RotateCell(bool clockwise)
        {
            UIElement.RotateCell(clockwise);

            if (clockwise)
            {
                // Shift actions clockwise
                UnityEngine.InputSystem.InputAction temp = UpAction;
                UpAction = RightAction;
                RightAction = DownAction;
                DownAction = LeftAction;
                LeftAction = temp;
            }
            else
            {
                // Shift actions counter-clockwise
                UnityEngine.InputSystem.InputAction temp = UpAction;
                UpAction = LeftAction;
                LeftAction = DownAction;
                DownAction = RightAction;
                RightAction = temp;
            }
        }

        public void SetUICell(AbilityGridUICell uiCell) { UIElement = uiCell; }
    }
}
