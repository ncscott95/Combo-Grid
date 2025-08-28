using AbilitySystem;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AbilityGridUICell : MonoBehaviour
{
    public Image CellImage { get; private set; }

    private void Awake()
    {
        CellImage = GetComponent<Image>();
        CellImage.color = new(0.5f, 0.5f, 0.5f, 0.25f);
    }

    public void UpdateUICell(AbilityGridCell linkedCell)
    {
        if (linkedCell != null)
        {
            CellImage.sprite = linkedCell.Ability.Icon;
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
        }
        else
        {
            CellImage.transform.Rotate(0, 0, 90);
        }
    }
}
