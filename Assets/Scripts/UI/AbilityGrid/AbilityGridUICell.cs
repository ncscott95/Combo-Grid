using AbilitySystem;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AbilityGridUICell : MonoBehaviour
{
    [SerializeField] private Image LeftActionImage;
    [SerializeField] private Image RightActionImage;
    [SerializeField] private Image UpActionImage;
    [SerializeField] private Image DownActionImage;

    public Image CellImage { get; private set; }
    private AbilityGridUIManager _manager;

    private void Awake()
    {
        CellImage = GetComponent<Image>();
        CellImage.color = new(0.5f, 0.5f, 0.5f, 0.25f);
    }

    public void SetManager(AbilityGridUIManager manager) { _manager = manager; }

    public void UpdateUICell(AbilityGridCell linkedCell)
    {
        if (linkedCell != null)
        {
            CellImage.sprite = linkedCell.Ability.Icon;

            LeftActionImage.gameObject.SetActive(linkedCell.HasLeftAction);
            RightActionImage.gameObject.SetActive(linkedCell.HasRightAction);
            UpActionImage.gameObject.SetActive(linkedCell.HasUpAction);
            DownActionImage.gameObject.SetActive(linkedCell.HasDownAction);
        }
        else
        {
            CellImage.sprite = null;
        }
    }

    public void EnterCell()
    {
        CellImage.color = Color.yellow;
    }

    public void ExitCell()
    {
        CellImage.color = new(0.5f, 0.5f, 0.5f, 0.25f);
    }

    public void IdleCell()
    {
        CellImage.color = Color.green;
    }

    public void RotateCell(bool clockwise)
    {
        if (clockwise)
        {
            CellImage.transform.Rotate(0, 0, -90);

            Image temp = UpActionImage;
            UpActionImage = LeftActionImage;
            LeftActionImage = DownActionImage;
            DownActionImage = RightActionImage;
            RightActionImage = temp;
        }
        else
        {
            CellImage.transform.Rotate(0, 0, 90);

            Image temp = UpActionImage;
            UpActionImage = RightActionImage;
            RightActionImage = DownActionImage;
            DownActionImage = LeftActionImage;
            LeftActionImage = temp;
        }

        _manager.UpdateGridUI();
    }
}
