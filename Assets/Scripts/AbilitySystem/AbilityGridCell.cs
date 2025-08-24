namespace AbilitySystem
{
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Image))]
    public class AbilityGridCell : MonoBehaviour
    {
        public UnityEngine.InputSystem.InputAction LeftAction;
        public UnityEngine.InputSystem.InputAction RightAction;
        public UnityEngine.InputSystem.InputAction UpAction;
        public UnityEngine.InputSystem.InputAction DownAction;

        public string DEBUGName { get; set; }
        public Image CellImage { get; private set; }
        public float Cooldown { get; private set; } = 1f;

        void Awake()
        {
            CellImage = GetComponent<Image>();
            CellImage.color = new(0.5f, 0.5f, 0.5f, 0.25f);
        }

        public void EnterCell()
        {
            Debug.Log($"Activating ability in cell: {DEBUGName}");

            LeftAction.Enable();
            RightAction.Enable();
            UpAction.Enable();
            DownAction.Enable();

            CellImage.color = Color.green;
        }

        public void ExitCell()
        {
            LeftAction.Disable();
            RightAction.Disable();
            UpAction.Disable();
            DownAction.Disable();

            CellImage.color = new(0.5f, 0.5f, 0.5f, 0.25f);
        }
    }
}
