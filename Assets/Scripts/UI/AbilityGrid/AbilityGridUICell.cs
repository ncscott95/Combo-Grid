using System.Collections.Generic;
using AbilitySystem;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AbilityGridUICell : MonoBehaviour
{
    [SerializeField] private List<Image> _actionImages = new() { null, null, null, null }; // Up, Left, Down, Right

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

            _actionImages[0].gameObject.SetActive(linkedCell.HasUpAction);
            _actionImages[1].gameObject.SetActive(linkedCell.HasLeftAction);
            _actionImages[2].gameObject.SetActive(linkedCell.HasDownAction);
            _actionImages[3].gameObject.SetActive(linkedCell.HasRightAction);
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
        float rotationAngle = clockwise ? -90f : 90f;
        CellImage.transform.Rotate(0, 0, rotationAngle);
        _actionImages = ListRotator.RotateList(_actionImages, clockwise, 1);

        _manager.UpdateGridUI();
    }
}
